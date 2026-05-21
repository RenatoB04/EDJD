Shader "Custom/EmergencyPostProcessing"
{
    Properties
    {
        _MainTex ("Screen Texture", 2D) = "white" {}
        _EmergencyIntensity ("Emergency Intensity (0-1)", Range(0, 1.0)) = 0.0
        _GlitchScale ("Glitch Scale", Range(0, 0.1)) = 0.03
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.05)) = 0.015
        _EmergencyColor ("Emergency Vignette Color", Color) = (0.8, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        // No culling or depth writing for full-screen post-processing blit
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EmergencyIntensity;
            float _GlitchScale;
            float _ChromaticAberration;
            float4 _EmergencyColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 1. Calculate Glitch displacement (horizontal lines displacing screen UVs)
                // We create a scanning noise wave based on Y coordinate and Time
                float glitchNoise = sin(uv.y * 40.0 + _Time.y * 70.0) * cos(uv.y * 12.0 - _Time.y * 30.0);
                
                // Add high frequency burst noise
                float burstNoise = sin(uv.y * 300.0 + _Time.y * 120.0);
                float combinedNoise = lerp(glitchNoise, burstNoise, 0.3);

                // We only glitch in periodic "bursts" based on time
                float glitchThreshold = step(0.7, sin(_Time.y * 5.0) * cos(_Time.y * 3.1));
                float2 glitchOffset = float2(combinedNoise * _GlitchScale * glitchThreshold * _EmergencyIntensity, 0.0);
                float2 distortedUV = uv + glitchOffset;

                // 2. Chromatic Aberration (separating R, G, B channels offset on the X axis)
                float caScale = _ChromaticAberration * _EmergencyIntensity * (sin(_Time.y * 20.0) * 0.3 + 0.7);
                float2 caOffset = float2(caScale, 0.0);

                float r = tex2D(_MainTex, distortedUV + caOffset).r;
                float g = tex2D(_MainTex, distortedUV).g;
                float b = tex2D(_MainTex, distortedUV - caOffset).b;
                float4 screenColor = float4(r, g, b, 1.0);

                // 3. Flashing Red Vignette (around screen borders)
                float distFromCenter = distance(uv, float2(0.5, 0.5));
                
                // Create a smooth vignette circle masking from center outwards
                float vignette = smoothstep(0.35, 0.85, distFromCenter);
                
                // Pulse vignette intensity using Time
                float pulse = sin(_Time.y * 12.0) * 0.4 + 0.6;
                vignette *= pulse * _EmergencyIntensity;

                // Blend chromatic aberration screen color with flashing emergency color
                fixed4 finalColor = lerp(screenColor, _EmergencyColor, vignette);
                
                return finalColor;
            }
            ENDCG
        }
    }
}
