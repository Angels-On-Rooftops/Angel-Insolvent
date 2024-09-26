#ifndef GLOBAL_ILLUM_VERTEX_COMPILED
#define GLOBAL_ILLUM_VERTEX_COMPILED

void GlobalIlluminationVertexCalcs_float(
    float3 Normal,
    float2 UVForLightmap,
    out float3 LightmapUV,
    out float3 SphericalHarmonics
){
    LightmapUV = float3(0, 0, 0);
    SphericalHarmonics = float3(0, 0, 0);
    #ifndef SHADERGRAPH_PREVIEW
    
        //perform all calculations better suited for vertex shader and send them on
        OUTPUT_LIGHTMAP_UV(UVForLightmap, unity_LightmapST, LightmapUV);
        OUTPUT_SH(Normal, SphericalHarmonics);
     
    #endif
}

#endif