#ifndef TOON_LIGHTING_COMPILED
#define TOON_LIGHTING_COMPILED

struct ToonLightingParams
{
    //surface
    float3 albedo;
    float3 normal;
    float smoothness;
    
    //relational info
    float3 viewDir;
    float3 fragWorldPos;
    float4 shadowCoordinate;
};

float GetSmoothnessPower(float smoothness)
{
    return exp2(8 * smoothness + 1);
}

#ifndef SHADERGRAPH_PREVIEW
float3 CalculateOneLight(ToonLightingParams params, Light light)
{
    //diffuse
    float diffuse = saturate(dot(params.normal, light.direction));
    
    //Blinn-Phong
    float3 middle = normalize(params.viewDir + light.direction);
    float3 specularDotProduct = dot(middle, params.normal);
    float3 smoothedSpecularDot = pow(specularDotProduct, GetSmoothnessPower(params.smoothness));
    float3 specular = smoothedSpecularDot * diffuse;
    
    //contribution from light
    return params.albedo * light.color * light.shadowAttenuation * (diffuse + specular * diffuse);

}
#endif

float3 CalculateLighting(ToonLightingParams params) 
{
    #ifdef SHADERGRAPH_PREVIEW
    //approximate lighting for node preview
        float3 lightDir = float3(0.5,0.5,0);
    
        float3 diffuse = dot(lightDir, params.normal);
    
        float3 middle = normalize(params.viewDir + lightDir);
        float3 specularDotProduct = saturate(dot(middle, params.normal));
        float3 smoothedSpecularDot = pow(specularDotProduct, GetSmoothnessPower(params.smoothness));
        float3 specular = smoothedSpecularDot * diffuse;
    
        return params.albedo * (diffuse+specular);
    #else
    
        return CalculateOneLight(params, GetMainLight(params.shadowCoordinate, params.fragWorldPos, 1));
    #endif
}

float4 GetShadowCoordinate(float3 pos)
{
    //don't inclide shadows in shadergraph preview
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

//This function exists to allow this script to be used as a node in Unity's ShaderGraph
void ToonLighting_float(
    float3 Albedo, 
    float3 Normal, 
    float3 ViewDir, 
    float Smoothness, 
    float3 WorldPos,
    out float3 Color
){ 
    ToonLightingParams params;
    
    params.albedo = Albedo;
    params.normal = normalize(Normal);
    params.viewDir = normalize(ViewDir);
    params.smoothness = Smoothness;
    params.fragWorldPos = WorldPos;
    params.shadowCoordinate = GetShadowCoordinate(WorldPos);
    
    Color = CalculateLighting(params);
}
#endif