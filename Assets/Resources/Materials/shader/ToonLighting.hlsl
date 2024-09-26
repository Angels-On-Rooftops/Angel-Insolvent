#ifndef TOON_LIGHTING_COMPILED
#define TOON_LIGHTING_COMPILED

struct ToonLightingParams
{
    //surface
    float3 albedo;
    float3 normal;
    float smoothness;
    float ambientOcclusion;
    float3 bakedLighting;
    
    //relational info
    float3 viewDir;
    float3 fragWorldPos;
    float4 shadowCoordinate;
};

float GetSmoothnessPower(float smoothness)
{
    //arbitrary function to get a nice-looking map from 0-1 to used smoothness values
    return exp2(8 * smoothness + 1);
}

#ifndef SHADERGRAPH_PREVIEW
float3 CalculateOneLight(ToonLightingParams params, Light light)
{
    //diffuse
    float diffuse = saturate(dot(params.normal, light.direction));
    
    //specular: Blinn-Phong
    float3 middle = normalize(params.viewDir + light.direction);
    float3 specularDotProduct = dot(middle, params.normal);
    float3 smoothedSpecularDot = pow(specularDotProduct, GetSmoothnessPower(params.smoothness));
    float3 specular = saturate(smoothedSpecularDot) * diffuse;
    
    //contribution from light
    return params.albedo * light.color * light.shadowAttenuation * light.distanceAttenuation * (diffuse + specular);
}

float3 CalculateGlobalIllumination(ToonLightingParams params)
{
    return params.albedo * params.bakedLighting * params.ambientOcclusion;
}
#endif

float3 CalculateLighting(ToonLightingParams params) 
{
    #ifdef SHADERGRAPH_PREVIEW
    //approximate lighting for node preview
        float3 lightDir = float3(0.5,0.5,-0.5);
    
        float3 diffuse = dot(lightDir, params.normal);
    
        float3 middle = normalize(params.viewDir + lightDir);
        float3 specularDotProduct = saturate(dot(middle, params.normal));
        float3 smoothedSpecularDot = pow(specularDotProduct, GetSmoothnessPower(params.smoothness));
        float3 specular = smoothedSpecularDot * diffuse;
    
        return params.albedo * (diffuse+specular);
    #else
    
        Light mainLight = GetMainLight(params.shadowCoordinate, params.fragWorldPos, 1);
    
        //make sure no lights are considered twice, baked vs realtime
        MixRealtimeAndBakedGI(mainLight, params.normal, params.bakedLighting);
        float3 color = CalculateGlobalIllumination(params);
        //calculate main light info
        color += CalculateOneLight(params, mainLight);
    
        //calculate info for additional lights if allowed
        #ifdef _ADDITIONAL_LIGHTS
            uint additionalLightsCount = GetAdditionalLightsCount();
            for (uint i = 0; i < additionalLightsCount; i++)
            {
                Light light = GetAdditionalLight(i, params.fragWorldPos, 1);
                color += CalculateOneLight(params, light);
            }
            
        #endif
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

//This function exists to allow this script to be used as a node in Unity's ShaderGraph
void ToonLighting_float(
    float3 Albedo, 
    float3 Normal, 
    float3 ViewDir, 
    float Smoothness, 
    float3 WorldPos,
    float AmbientOcclusion,
    float3 LightmapUV,
    float3 SphericalHarmonics,
    out float3 Color
){ 
    ToonLightingParams params;
    
    params.albedo = Albedo;
    params.normal = normalize(Normal);
    params.viewDir = normalize(ViewDir);
    params.smoothness = Smoothness;
    params.fragWorldPos = WorldPos;
    params.ambientOcclusion = AmbientOcclusion;
    params.shadowCoordinate = GetShadowCoordinate(WorldPos);
    params.bakedLighting = GetBakedLighting(Normal, LightmapUV, SphericalHarmonics);
    
    Color = CalculateLighting(params);
}
#endif