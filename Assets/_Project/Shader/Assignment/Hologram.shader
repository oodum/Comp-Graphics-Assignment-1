Shader "Custom/Hologram" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Fresnel ("Fresnel", Range(0,10)) = 0.5
        _Transparency ("Transparency", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        struct appdata
        {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        struct v2f
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : TEXCOORD1;
            float3 viewDirection : TEXCOORD2;
        };

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_ST;
        float4 _Color;
        float  _Fresnel;
        float _Transparency;


        v2f vert(appdata IN)
        {
            v2f OUT;
            OUT.position = TransformObjectToHClip(IN.position.xyz);
            OUT.normal = normalize(TransformObjectToWorldNormal(IN.normal));
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            // calculate view direction
            float3 worldPosition = TransformObjectToWorld(IN.position.xyz);
            OUT.viewDirection = normalize(GetCameraPositionWS() - worldPosition);
            return OUT;
        }

        float4 frag(v2f IN) : SV_Target
        {
            float fresnel = 1 - dot(normalize(IN.viewDirection), normalize(IN.normal));
            fresnel = pow(fresnel, _Fresnel);
            float3 fresnelColor = _Color.rgb * fresnel; 
            float4 fresnelResult = float4(fresnelColor, fresnel);
            float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
            return float4(albedo.rgb + fresnelResult.rgb, min(fresnelResult.a, _Transparency));
        }
        ENDHLSL
        }
    }
    FallBack "Diffuse"
}