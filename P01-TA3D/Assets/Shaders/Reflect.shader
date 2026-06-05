Shader "Custom/Reflect"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base Texture", 2D) = "white" {}
        // The _Cube property stores the 360 HDRI or the real-time reflection texture
        _Cube ("HDRI / Cubemap", Cube) = "" {}
        _ReflectLevel ("Reflection Level", Range(0.0, 1.0)) = 0.8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0

        sampler2D _MainTex;
        samplerCUBE _Cube; // Data type for cubic reflection textures
        
        struct Input
        {
            float2 uv_MainTex;
            float3 worldRefl; // Unity automatically calculates and stores the reflection vector here
        };

        fixed4 _Color;
        float _ReflectLevel;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 1. Apply base texture and color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // 2. Sample the reflection color using the worldRefl vector
            fixed4 reflection = texCUBE(_Cube, IN.worldRefl);

            // 3. Add the reflection to the Emission channel to make it glow with the environment
            o.Emission = reflection.rgb * _ReflectLevel;
            
            // 4. Increase Metallic and Smoothness to mimic a mirror/metal surface
            o.Metallic = 0.9;
            o.Smoothness = 0.9;
        }
        ENDCG
    }
    FallBack "Diffuse"
}