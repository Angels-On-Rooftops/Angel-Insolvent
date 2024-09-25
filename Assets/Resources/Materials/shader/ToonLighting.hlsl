#ifndef TOON_LIGHTING_COMPILED
#define TOON_LIGHTING_COMPILED

struct ToonLightingParams
{
    float3 albedo;
    float3 normal;
};

#ifndef SHADERGRAPH_PREVIEW
float3 CalculateDiffuse(ToonLightingParams params, Light light)
{
    float diffuseScalar = dot(params.normal, light.direction);
    
    return params.albedo * light.color * diffuseScalar;

}
#endif

float3 CalculateLighting(ToonLightingParams params) 
{
    #ifdef SHADERGRAPH_PREVIEW
    //approximate diffuse for node preview
    return dot(float3(0.5,0.5,0), params.normal) * params.albedo;
    #else
    float3 result = 0;
    
    result += CalculateDiffuse(params, GetMainLight());
    
    return result;
    #endif
}

void ToonLighting_float(float3 Albedo, float3 Normal, out float3 Color)
{
    ToonLightingParams params;
    
    params.albedo = Albedo;
    params.normal = Normal;
    
    Color = CalculateLighting(params);
}
#endif