Shader "Custom/Grow"
{
    Properties
    {
        _Color ("Trunk Base Color", Color) = (1,1,1,1)
        _Color2 ("Roots Color", Color) = (0,1,0,1)
        _MainTex ("Base Texture (Albedo)", 2D) = "white" {}
        _G ("Growth Variable (G)", Range(0, 1)) = 0.0 
    }
    SubShader
    {
        // Configured as TransparentCutout and AlphaTest queue for correct Z-Buffer writing
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200

        CGPROGRAM
        // addshadow allows cutout areas (like leaves) to cast accurate shadows
        #pragma surface surf Standard addshadow
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        fixed4 _Color2;
        float _G;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 1. Fetch the original texture color
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

            // 2. Set the initial color (Trunk base)
            fixed4 finalColor = tex * _Color;

            // 3. Growth Logic:
            // If the V (Y) coordinate is above 0.5, we are in the roots/branches zone
            if (IN.uv_MainTex.y >= 0.5)
            {
                // If the U (X) coordinate is less than or equal to G, paint the root color
                if (IN.uv_MainTex.x <= _G)
                {
                    finalColor = tex * _Color2;
                }
            }

            // 4. Output the final data to the rendering engine
            o.Albedo = finalColor.rgb;
            o.Alpha = tex.a * _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}