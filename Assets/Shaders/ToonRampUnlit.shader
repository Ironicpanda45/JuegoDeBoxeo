Shader "Custom/URP/ToonRampUnlit_Distortable_Magic_Outline"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _LightRamp ("Light Ramp (5 colors)", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)

        _DistortionScale ("Distortion Scale", Float) = 3.3
        _MagicDepth ("Magic Depth (Chaos)", Range(1, 8)) = 8

        _LightIntensity ("Light Intensity", Range(0, 3)) = 1
        _RimStrength ("Rim Strength", Range(-2, 2)) = 0.5
        _RimColor ("Rim Color", Color) = (1, 0.9, 0.7, 1)

        // Outline
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.03
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 300

        // ---------- PASO 1: OUTLINE ----------
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                // Expandir vértices a lo largo de la normal
                positionWS += normalWS * _OutlineWidth;

                OUT.positionHCS = TransformWorldToHClip(positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // ---------- PASO 2: SHADER PRINCIPAL ----------
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // === LIBRERÍAS URP ===
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // === TEXTURAS Y PARÁMETROS ===
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_LightRamp);
            SAMPLER(sampler_LightRamp);

            float4 _Tint;
            float _DistortionScale;
            float _MagicDepth;
            float _LightIntensity;
            float _RimStrength;
            float4 _RimColor;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.uv = IN.uv;
                return OUT;
            }

            // === MAGIC NOISE ===
            float magicNoise(float3 p, float depth)
            {
                float n = 0.0;
                float scale = 1.0;
                float sum = 0.0;

                for (int i = 0; i < (int)depth; i++)
                {
                    n += sin(p.x + sin(p.y * scale + sin(p.z * scale))) / scale;
                    scale *= 1.7;
                    p = p.yzx * 1.1;
                    sum += 1.0 / scale;
                }

                return saturate(0.5 + 0.5 * n / sum);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // --- Textura base ---
                float4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Tint;

                // --- Luz principal ---
                Light mainLight = GetMainLight();
                float3 normal = normalize(IN.normalWS);
                float3 lightDir = normalize(mainLight.direction);
                float ndotl = saturate(dot(normal, lightDir));
                float lightVal = saturate(ndotl * _LightIntensity);

                // --- Magic Noise ---
                float3 magicPos = IN.positionWS * _DistortionScale * 0.2;
                float magicVal = magicNoise(magicPos, _MagicDepth);

                // --- Mezcla Toon ---
                float combined = clamp(lightVal + (magicVal - 0.1) * 0.5, 0.0, 1.0);
                const float steps = 8.0;
                float idx = floor(combined * steps);
                float stepped = idx / steps;
                float2 rampUV = float2(stepped, 0.5);
                float4 rampColor = SAMPLE_TEXTURE2D(_LightRamp, sampler_LightRamp, rampUV);

                // --- Rim Light ---
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.positionWS);
                float fresnel = pow(1.0 - saturate(dot(normal, viewDir)), 2.0);
                float4 rimColor = _RimColor * (fresnel * _RimStrength);

                // --- Color Final ---
                float4 finalColor = albedo * rampColor;
                finalColor.rgb += rimColor.rgb;
                finalColor.a = 1.0;

                return finalColor;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
