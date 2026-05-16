Shader "Custom/SciFi_Shield"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (0.1, 0.4, 0.8, 0.2)
        _RimColor ("Rim Color", Color) = (0.3, 0.8, 1.0, 1.0)
        _RimPower ("Rim Power", Range(0.1, 8.0)) = 3.0
        
        [HDR] _IntersectColor ("Intersection Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _IntersectPower ("Intersection Thickness", Range(0.01, 5.0)) = 1.0
        
        [HDR] _HitColor ("Hit Color", Color) = (1.0, 0.2, 0.2, 1.0)
        _RippleWidth ("Ripple Width", Range(0.1, 2.0)) = 0.5
        _RippleDisplacement ("Vertex Displacement", Range(0.0, 1.0)) = 0.2
        
        // Distortion / refraction
        _DistortionMap ("Distortion Noise (Normal Map)", 2D) = "bump" {}
        _DistortionStrength ("Distortion Strength", Range(0.0, 0.5)) = 0.05
        
        [HideInInspector] _HitPos ("Hit Position", Vector) = (0,0,0,0)
        [HideInInspector] _HitRadius ("Hit Radius", Float) = 0
        [HideInInspector] _MaxRadius ("Max Radius", Float) = 3.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // Captures background for screen-space refraction
        GrabPass { "_BackgroundTexture" }

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert
        #pragma target 3.0

        sampler2D _CameraDepthTexture;
        sampler2D _BackgroundTexture;
        sampler2D _DistortionMap;

        struct Input
        {
            float3 viewDir;
            float4 screenPos;
            float3 worldPos;
            float2 uv_DistortionMap;
        };

        float4 _MainColor;
        float4 _RimColor;
        float _RimPower;
        float4 _IntersectColor;
        float _IntersectPower;

        float4 _HitColor;
        float _RippleWidth;
        float _RippleDisplacement;
        float4 _HitPos;
        float _HitRadius;
        float _MaxRadius;

        float _DistortionStrength;

        void vert(inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float dist = distance(worldPos, _HitPos.xyz);

            float ripple = 1.0 - smoothstep(0.0, _RippleWidth, abs(dist - _HitRadius));

            // Local vertex displacement for impact ripple
            v.vertex.xyz += v.normal * ripple * _RippleDisplacement;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Screen-space UVs (for depth + refraction sampling)
            float2 screenUV = IN.screenPos.xy / max(IN.screenPos.w, 0.001);

            // Distortion (animated noise)
            float2 animatedUV = IN.uv_DistortionMap + float2(_Time.y * 0.1, _Time.y * 0.15);
            float3 distortion = UnpackNormal(tex2D(_DistortionMap, animatedUV));

            float2 distortedUV = screenUV + (distortion.xy * _DistortionStrength);

            // Screen refraction
            float3 refraction = tex2D(_BackgroundTexture, distortedUV).rgb;

            // Fresnel rim
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            float3 rimEmission = _RimColor.rgb * pow(rim, _RimPower);

            // Depth intersection mask (kept in screen space to avoid distortion artifacts)
            float sceneZ = LinearEyeDepth(tex2D(_CameraDepthTexture, screenUV).r);
            float shieldZ = IN.screenPos.z;

            float diff = sceneZ - shieldZ;
            float intersect = 1.0 - saturate(diff / _IntersectPower);

            // Impact ripple mask
            float dist = distance(IN.worldPos, _HitPos.xyz);
            float rippleRing = 1.0 - smoothstep(0.0, _RippleWidth, abs(dist - _HitRadius));

            float fadeOut = 1.0 - saturate(_HitRadius / _MaxRadius);
            rippleRing *= fadeOut;

            // Base shading (refraction tinted by shield color)
            float3 baseColor = refraction * _MainColor.rgb * 2.0;

            o.Emission =
                baseColor +
                rimEmission +
                (_IntersectColor.rgb * intersect) +
                (_HitColor.rgb * rippleRing);

            // Fully opaque surface; transparency is faked via emission-based refraction
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}