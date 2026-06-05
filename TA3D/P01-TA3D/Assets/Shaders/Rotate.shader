Shader "Custom/Rotate"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base Texture (RGB) Transparency (A)", 2D) = "white" {}
        // Property to control rotation speed from the Inspector
        _RotationSpeed ("Rotation Speed", Range(-10, 10)) = 1.0 
    }
    SubShader
    {
        // 1. Tags adjusted to render transparency correctly
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        // 2. The 'alpha:fade' directive is crucial for transparent materials in the Built-in Pipeline
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        float _RotationSpeed;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Step A: The center of normal UV coordinates is (0.5, 0.5). 
            // To rotate the image around its center, we subtract this value to move the pivot to the origin (0,0).
            float2 pivot = float2(0.5, 0.5);
            float2 centeredUV = IN.uv_MainTex - pivot;

            // Step B: Calculate the continuous rotation angle using _Time.y (time in seconds) and our speed.
            float angle = _Time.y * _RotationSpeed;
            float cosAng = cos(angle);
            float sinAng = sin(angle);

            // Step C: Apply the standard 2D rotation matrix.
            float2 rotatedUV;
            rotatedUV.x = centeredUV.x * cosAng - centeredUV.y * sinAng;
            rotatedUV.y = centeredUV.x * sinAng + centeredUV.y * cosAng;

            // Step D: Return the coordinates to their original position by adding the pivot back.
            rotatedUV += pivot;

            // Sample the texture using the newly calculated UV coordinates
            fixed4 c = tex2D (_MainTex, rotatedUV) * _Color;
            
            // Assign the color to the Albedo and the texture's Alpha channel to the material's transparency
            o.Albedo = c.rgb;
            o.Alpha = c.a; 
        }
        ENDCG
    }
    FallBack "Diffuse"
}