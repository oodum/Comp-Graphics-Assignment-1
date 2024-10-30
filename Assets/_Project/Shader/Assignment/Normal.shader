Shader "Custom/Normal" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 10)) = 1
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 tangent : TEXCOORD2;
                float3 bitangent : TEXCOORD3;
                float3 viewDirection : TEXCOORD4;
                float4 shadowCoord : TEXCOORD5;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float  _NormalStrength;
            float4 _Color;

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.position = TransformObjectToHClip(IN.position);
                OUT.normal = normalize(TransformObjectToWorldNormal(IN.normal));
                OUT.tangent = normalize(TransformObjectToWorldNormal(IN.tangent.xyz));
                OUT.bitangent = cross(OUT.normal, OUT.tangent) * IN.tangent.w;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                float3 worldPosition = TransformObjectToWorld(IN.position.xyz);
                OUT.viewDirection = normalize(GetCameraPositionWS() - worldPosition);
                OUT.shadowCoord = GetShadowCoord(GetVertexPositionInputs(IN.position.xyz));
                return OUT;
            }

            float4 frag(v2f IN) : SV_TARGET
            {
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float3 normal = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                normal.xy *= _NormalStrength;
                float3x3 TBN = float3x3(IN.tangent, IN.bitangent, IN.normal);
                float3   worldNormal = normalize(mul(normal, TBN));
                Light    light = GetMainLight(IN.shadowCoord);
                float3   lightDirection = normalize(light.direction);
                float    NdotL = saturate(dot(worldNormal, lightDirection));
                float3   diffuse = texColor.rgb * NdotL;
                half shadowAmount = max(light.shadowAttenuation, 0.1);
                return float4(diffuse * _Color  * shadowAmount, texColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}