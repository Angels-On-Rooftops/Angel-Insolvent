#ifndef TOON_LIGHTING_COMPILED
#define TOON_LIGHTING_COMPILED

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
#pragma multi_compile _ ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ SHADOWS_SHADOWMASK

struct ToonLightingParams
{
    //surface
    float3 albedo;
    float3 normal;
    float shininess;
    float smoothness;
    float ambientOcclusion;
    float3 bakedLighting;
    float4 shadowMask;
    
    //relational info
    float3 viewDir;
    float3 fragWorldPos;
    float4 shadowCoordinate;
    
    //toon info
    float diffuseSteps;
    float specularSteps;
    float stepOffset;
};

float GetShininessPower(float shininess)
{
    //arbitrary function to get a nice-looking map from 0-1 to used shininess values
    return exp2(8 * shininess + 1);
}

float Posterize(float value, float steps)
{
    //posterize with offset to account for floor cutting off too much by default
    //saturate(floor((value + (0.5 / steps)) * steps) * (1 / steps));
    return saturate(floor(value * steps) * (1 / steps));
}

float LinearAttenuation(ToonLightingParams params, int lightIndex)
{
    #ifndef SHADERGRAPH_PREVIEW
        #if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
		    float4 lightPosition = _AdditionalLightsBuffer[lightIndex].position;
		    half4 spotDirection = _AdditionalLightsBuffer[lightIndex].spotDirection;
		    half4 attenuationInfo = _AdditionalLightsBuffer[lightIndex].attenuation;
        #else
            float4 lightPosition = _AdditionalLightsPosition[lightIndex];
            half4 spotDirection = _AdditionalLightsSpotDir[lightIndex];
            half4 attenuationInfo = _AdditionalLightsAttenuation[lightIndex];
        #endif
        
        //point attenuation
        float3 lightVec = lightPosition.xyz - params.fragWorldPos * lightPosition.w;
        float distance = length(lightVec);
        float range = rsqrt(attenuationInfo.x);
        float result = saturate(1.0 - (distance / range));
    
    
        [branch]
        if (attenuationInfo.z > 0)
        {
                //spot light, incorporate spot attenuation
            half SdotL = dot(spotDirection.xyz, lightVec);
            result *= saturate(SdotL * attenuationInfo.x + attenuationInfo.y);
        }
    
        return result;
    #endif
    return 1;
}

#ifndef SHADERGRAPH_PREVIEW
float3 CalculateOneLight(ToonLightingParams params, Light light, float attenuation)
{
    //diffuse
    float diffuse = saturate(dot(params.normal, light.direction)) * attenuation;
    
    //specular: Blinn-Phong
    float3 middle = normalize(params.viewDir + light.direction);
    float specularDotProduct = dot(middle, params.normal);
    float smoothedSpecularDot = pow(abs(specularDotProduct), GetShininessPower(params.shininess));
    float specular = saturate(smoothedSpecularDot) * diffuse * params.smoothness;
    
    //posterize lighting contributions
    diffuse = Posterize(diffuse + (params.stepOffset / params.diffuseSteps), params.diffuseSteps);
    specular = Posterize(specular + (params.stepOffset / params.specularSteps), params.specularSteps);

    //calc light total
    float combinedContributions = (diffuse + specular);
    
    return saturate(params.albedo * light.color * light.shadowAttenuation * combinedContributions) * step(0, light.distanceAttenuation);
}

float3 CalculateGlobalIllumination(ToonLightingParams params)
{
    return params.albedo * params.bakedLighting * params.ambientOcclusion;
}
#endif

float3 CalculateLighting(ToonLightingParams params)
{
    #ifdef SHADERGRAPH_PREVIEW
        //approximate lighting for node preview (shadergraph preview doesn't have world info)
        float3 lightDir = float3(0.5,0.5,-0.5);
    
        float3 diffuse = dot(lightDir, params.normal);
    
        float3 middle = normalize(params.viewDir + lightDir);
        float3 specularDotProduct = saturate(dot(middle, params.normal));
        float3 smoothedSpecularDot = pow(specularDotProduct, GetShininessPower(params.smoothness));
        float3 specular = smoothedSpecularDot * diffuse;

            //posterize lighting contributions
            diffuse = Posterize(diffuse, params.diffuseSteps + (params.stepOffset/ params.diffuseSteps));
            specular = Posterize(specular, params.diffuseSteps + (params.stepOffset/ params.diffuseSteps));
    
        return params.albedo * (diffuse+specular);

    #else

        Light mainLight = GetMainLight(params.shadowCoordinate, params.fragWorldPos, params.shadowMask);
    
        //make sure no lights are considered twice, baked vs realtime
        MixRealtimeAndBakedGI(mainLight, params.normal, params.bakedLighting);
        float3 color = CalculateGlobalIllumination(params);

        //calculate main light info
        color += CalculateOneLight(params, mainLight, 1);
    
        uint pixelLightsCount = GetAdditionalLightsCount();
    
        //calculate info for additional directional lights if allowed
        #if USE_FORWARD_PLUS
            for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++) 
            {
                FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK
                Light light = GetAdditionalLight(lightIndex, params.fragWorldPos, params.shadowMask);
                color += CalculateOneLight(params, light, 1);
            }
        #endif
    
        InputData inputData = (InputData) 0;
        float4 screenPos = ComputeScreenPos(TransformWorldToHClip(params.fragWorldPos));
        inputData.normalizedScreenSpaceUV = screenPos.xy / screenPos.w;
        inputData.positionWS = params.fragWorldPos;
    
        LIGHT_LOOP_BEGIN(pixelLightsCount)
            Light light = GetAdditionalLight(lightIndex, params.fragWorldPos, params.shadowMask);
            float attenuation = LinearAttenuation(params, lightIndex);
            color += CalculateOneLight(params, light, attenuation);
        LIGHT_LOOP_END
            
        
        return color;
    #endif
}

float4 GetShadowCoordinate(float3 pos)
{
    //don't include shadows in shadergraph preview
    #ifdef SHADERGRAPH_PREVIEW
        return 0;
    #else
        //get coords based on if using screen space deferred cascaded shadows
        #if SHADOWS_SCREEN
                float4 clipSpacePos = TransformWorldToHClip(pos);
                return ComputeScreenPos(clipSpacePos);
        #else
            return TransformWorldToShadowCoord(pos);
        #endif
    #endif
   
}

float3 GetBakedLighting(float3 normal, float3 lightmapUV, float3 sphericalHarmonics)
{
    //don't consider baked lighting in shadergraph preview
    #ifdef SHADERGRAPH_PREVIEW
        return 0;
    #else
        return SAMPLE_GI(lightmapUV, sphericalHarmonics, normal);
    #endif
}

float4 GetShadowMask(float3 lightmapUV)
{
    //don't consider baked shadows in shadergraph preview
    #ifdef SHADERGRAPH_PREVIEW
            return 0;
    #else
        return SAMPLE_SHADOWMASK(lightmapUV);
    #endif
}

//This function exists to allow this script to be used as a node in Unity's ShaderGraph
void ToonLighting_float(
    float3 Albedo,
    float3 Normal,
    float3 ViewDir,
    float Smoothness,
    float Shininess,
    float3 WorldPos,
    float AmbientOcclusion,
    float3 LightmapUV,
    float3 SphericalHarmonics,
    float DiffuseSteps,
    float SpecularSteps,
    float StepOffset,
    out float3 Color
)
{
    ToonLightingParams params;
    
    params.albedo = Albedo;
    params.normal = normalize(Normal);
    params.viewDir = normalize(ViewDir);
    params.smoothness = Smoothness;
    params.shininess = Shininess;
    params.fragWorldPos = WorldPos;
    params.ambientOcclusion = AmbientOcclusion;
    params.shadowCoordinate = GetShadowCoordinate(WorldPos);
    params.bakedLighting = GetBakedLighting(Normal, LightmapUV, SphericalHarmonics);
    params.shadowMask = GetShadowMask(LightmapUV);
    params.diffuseSteps = DiffuseSteps;
    params.specularSteps = SpecularSteps;
    params.stepOffset = StepOffset;
    
    Color = CalculateLighting(params);
}
#endif