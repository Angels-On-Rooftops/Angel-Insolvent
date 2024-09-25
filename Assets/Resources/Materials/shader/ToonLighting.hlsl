#ifndef TOON_LIGHTING_COMPILED
#define TOON_LIGHTING_COMPILED

struct ToonLightingParams
{
    float3 albedo;
    float3 normal;
    float3 viewDir;
    float smoothness;
};

#ifndef SHADERGRAPH_PREVIEW
float3 CalculateDiffuse(ToonLightingParams params, Light light)
{
    float diffuseScalar = dot(params.normal, light.direction);
    
    return light.color * diffuseScalar;

}

float3 CalculateSpecular(ToonLightingParams params, Light light)
{
    float3 reflectionVector = -reflect(light.direction, params.normal);
    float dotProduct = dot(reflectionVector, params.viewDir);
    float3 specular = pow(dotProduct, params.smoothness);
    return specular;
}
#endif

float3 CalculateLighting(ToonLightingParams params) 
{
    #ifdef SHADERGRAPH_PREVIEW
    //approximate lighting for node preview
    return dot(float3(0.5,0.5,0), params.normal) * params.albedo;
    #else
    float3 result = 0;
    
    result += CalculateDiffuse(params, GetMainLight());
    //result += CalculateSpecular(params, GetMainLight());
    result *= params.albedo;
    
    return result;
    #endif
}

void ToonLighting_float(float3 Albedo, float3 Normal, float3 ViewDir, float3 Smoothness, out float3 Color)
{ 
    ToonLightingParams params;
    
    params.albedo = Albedo;
    params.normal = Normal;
    params.viewDir = ViewDir;
    params.smoothness = Smoothness;
    
    Color = CalculateLighting(params);
}
#endif