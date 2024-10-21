Shader "Unlit/ToonLightingShader"
{
    Properties
    {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Shininess ("Shininess", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque"
            "LightMode" = "UniversalForward" 
        }

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityStandardBRDF.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float _Smoothness;
                float _Shininess;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.normal = normalize(i.normal);
                // sample the texture
                fixed4 texCol = tex2D(_MainTex, i.uv) * _Tint;

                //get diffuse
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float diffuse = DotClamped(lightDir, i.normal);

                //get specular
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 middle = normalize(viewDir + lightDir);
                float specularDotProduct = pow(DotClamped(middle, i.normal), pow(8 * _Shininess + 1, 2));
                float specular = saturate(specularDotProduct) * diffuse * _Smoothness;


                float3 col = texCol * lightColor * (diffuse+specular);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);


                


                return float4(col, 1);
}
            ENDCG
        }
    }
}
