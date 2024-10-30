Shader "Custom/Paint" {
    Properties {
        [MainColor] _AmbientColor ("Ambient Color", Color) = (1,1,1,1)
        [MainTexture ]_MainTex ("Main Texture", 2D) = "white" {}

        _PaintBrushTex ("Paint Brush Texture", 2D) = "white" {}
        _ToonRamp ("Toon Ramp", 2D) = "white" {}

        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0.01, 100)) = 5

        _Offset ("Offset", Range(0,10)) = 0

        _HologramColor ("Hologram Color", Color) = (1,1,1,1)

        _Index ("Index", Range(0,6)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
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
                float2 paintuv : TEXCOORD3;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 viewDirection : TEXCOORD2;
                float2 paintuv : TEXCOORD3; // separate uv for paint brush texture
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST; // texture scale and offset

            TEXTURE2D(_PaintBrushTex);
            SAMPLER(sampler_PaintBrushTex);
            float4 _PaintBrushTex_ST;

            TEXTURE2D(_ToonRamp);
            SAMPLER(sampler_ToonRamp);

            float4 _AmbientColor;

            float4 _SpecularColor;
            float  _Shininess;

            float _Offset;

            float _Index;

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.position = TransformObjectToHClip(IN.position.xyz);
                OUT.normal = normalize(TransformObjectToWorldNormal(IN.normal));
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex); // use transform tex to apply scale and offset
                OUT.paintuv = TRANSFORM_TEX(IN.paintuv, _PaintBrushTex);
                // calculate view direction
                float3 worldPosition = TransformObjectToWorld(IN.position.xyz);
                OUT.viewDirection = normalize(GetCameraPositionWS() - worldPosition);

                return OUT;
            }

            float4 frag(v2f IN) : SV_Target
            {
                // Texture
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Lighting
                Light  light = GetMainLight();
                float3 lightDirection = normalize(light.direction);

                // Diffuse
                float3 normal = normalize(IN.normal);
                float  NdotL = saturate(dot(normal, lightDirection));
                float3 diffuse = texColor.rgb * NdotL;

                // Ambient
                float3 ambient = SampleSH(normal) * texColor.rgb * _AmbientColor.xyz;

                // Specular
                float3 reflectionDirection = reflect(-lightDirection, normal);
                float3 viewDirection = normalize(IN.viewDirection);
                float  specularFactor = pow(saturate(dot(reflectionDirection, viewDirection)), _Shininess);
                float3 specular = _SpecularColor.rgb * specularFactor;

                // Toon
                float primaryToonRing = step(NdotL, 0.5);
                float secondaryToonRing = step(NdotL, 0.3);
                float toonRing = lerp(primaryToonRing, secondaryToonRing, 0.4);
                IN.paintuv += _Time.y - 0.5;
                float3 paintColor = SAMPLE_TEXTURE2D(_PaintBrushTex, sampler_PaintBrushTex, IN.paintuv).rgb;
                float3 paintToonRing = paintColor * toonRing;
                // use the r and g values of the toon ring to sample the toon ramp texture
                float3 rampedPaintToonRing = SAMPLE_TEXTURE2D(_ToonRamp, sampler_ToonRamp, paintToonRing.rg).rgb;
                // finalised toon colour
                float3 rampedPaintToonRingTex = rampedPaintToonRing * (texColor.rgb + step(rampedPaintToonRing, 0));

                // Rim
                float fresnel = pow(1 - saturate(dot(normal * _Offset, viewDirection)), 0.01);
                //https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/Fresnel-Effect-Node.html
                float3 rim = fresnel * paintColor * NdotL;

                float3 toon = rim + rampedPaintToonRingTex;

                float3 final;
                // display the different lighting models based on the index
                if(_Index < 1)
                {
                    final = diffuse;
                }
                else if(_Index < 2)
                {
                    final = diffuse + ambient;
                }
                else if(_Index < 3)
                {
                    final = diffuse + specular;
                }
                else if(_Index < 4)
                {
                    final = diffuse + ambient + specular;
                }
                else if(_Index < 5)
                {
                    final = toon;
                }
                else
                {
                    final = toon + diffuse + ambient + specular;
                }

                return float4(final, 1);
            }
            ENDHLSL

        }
    }
}