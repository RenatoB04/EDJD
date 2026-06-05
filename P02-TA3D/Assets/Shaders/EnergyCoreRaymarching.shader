Shader "Custom/EnergyCoreRaymarching"
{
    Properties
    {
        [HDR] _CoreColor ("Core Glow Color", Color) = (0.0, 0.8, 1.0, 1.0)
        _CoreRadius ("Core Radius", Range(0.05, 0.45)) = 0.22
        _CoreSoftness ("Core Softness", Range(0.01, 0.3)) = 0.12
        _NoiseScale ("Noise Scale", Float) = 6.0
        _NoiseSpeed ("Noise Speed", Float) = 1.5
        _DensityMultiplier ("Density Multiplier", Range(0.01, 0.5)) = 0.08
        _Steps ("Raymarching Steps", Range(16, 64)) = 32
    }
    SubShader
    {
        Tags { "Queue"="Transparent-10" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 localPos : TEXCOORD0;
                float3 localCamPos : TEXCOORD1;
            };

            float4 _CoreColor;
            float _CoreRadius;
            float _CoreSoftness;
            float _NoiseScale;
            float _NoiseSpeed;
            float _DensityMultiplier;
            int _Steps;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                o.localCamPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz;
                return o;
            }

            // Hash and value noise provide a cheap 3D density field for the plasma volume.
            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + float3(0.1, 0.1, 0.1));
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            float noise(float3 x)
            {
                float3 i = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(lerp(hash(i + float3(0,0,0)), hash(i + float3(1,0,0)), f.x),
                                lerp(hash(i + float3(0,1,0)), hash(i + float3(1,1,0)), f.x), f.y),
                            lerp(lerp(hash(i + float3(0,0,1)), hash(i + float3(1,0,1)), f.x),
                                lerp(hash(i + float3(0,1,1)), hash(i + float3(1,1,1)), f.x), f.y), f.z);
            }

            // Layer multiple noise octaves for finer plasma detail.
            float fbm(float3 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;

                for (int i = 0; i < 3; i++)
                {
                    value += amplitude * noise(p * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // March through the cube in local space instead of shading only its surface.
                float3 rayOrigin = i.localPos;
                float3 rayDir = normalize(i.localPos - i.localCamPos);

                float t = 0.0;
                float stepSize = 1.0 / _Steps;
                float4 accumulatedColor = float4(0, 0, 0, 0);

                for (int stepIdx = 0; stepIdx < _Steps; stepIdx++)
                {
                    float3 p = rayOrigin + rayDir * t;

                    if (any(abs(p) > 0.5)) break;

                    // Sphere SDF controls where the volumetric core exists.
                    float d = length(p) - _CoreRadius;

                    if (d < _CoreSoftness)
                    {
                        float3 noiseOffset = float3(_Time.y, _Time.y * 1.3, _Time.y * 0.7) * _NoiseSpeed;
                        float plasmaNoise = fbm(p * _NoiseScale - noiseOffset);
                        float density = saturate(1.0 - (d / _CoreSoftness)) * plasmaNoise;

                        if (density > 0.0)
                        {
                            float alphaStep = density * _DensityMultiplier;

                            accumulatedColor.rgb += _CoreColor.rgb * alphaStep * 1.5;
                            accumulatedColor.a += alphaStep;

                            // Stop once the volume is effectively opaque.
                            if (accumulatedColor.a >= 0.95)
                            {
                                accumulatedColor.a = 1.0;
                                break;
                            }
                        }
                    }

                    t += stepSize;
                }

                if (accumulatedColor.a <= 0.0) discard;

                return accumulatedColor;
            }
            ENDCG
        }
    }
}
