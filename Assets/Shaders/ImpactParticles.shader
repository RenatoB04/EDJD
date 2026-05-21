Shader "Custom/ImpactParticles"
{
    Properties
    {
        _MainColor ("Spark Color", Color) = (1.0, 0.4, 0.0, 1.0)
        _Size ("Spark Size", Float) = 0.15
        _Speed ("Velocity Scale", Float) = 3.0
        _GravityScale ("Gravity Scale", Float) = 0.5
        _Progress ("Progress (0-1)", Range(0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL; // Used to store initial velocity vector
            };

            struct v2g
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float4 _MainColor;
            float _Size;
            float _Speed;
            float _GravityScale;
            float _Progress;

            v2g vert (appdata v)
            {
                v2g o;
                o.pos = v.vertex; // Keep local vertex position (0,0,0)
                o.dir = v.normal; // Pass initial velocity vector
                return o;
            }

            [maxvertexcount(3)]
            void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
            {
                // Initial direction and velocity stored in the vertex normal
                float3 velocity = IN[0].dir;
                
                // Physics calculation: P = P0 + V * t + 0.5 * G * t^2
                float3 localPos = IN[0].pos.xyz + velocity * _Speed * _Progress;
                localPos.y -= 0.5 * 9.81 * _Progress * _Progress * _GravityScale;

                // Transform the simulated local position to world space
                float4 worldPos = mul(unity_ObjectToWorld, float4(localPos, 1.0));

                // Get Camera Billboard axes (Right and Up vectors from View Matrix)
                float3 right = UNITY_MATRIX_V[0].xyz;
                float3 up = UNITY_MATRIX_V[1].xyz;

                // Size decreases over time (fade/shrink out)
                float size = _Size * (1.0 - _Progress);

                g2f o;
                o.color = _MainColor;
                o.color.a *= (1.0 - _Progress); // Fade out transparency

                // Spark vertices layout (forming an upward pointing triangle)
                // Vertex 1: Bottom-Left
                float3 pos1 = worldPos.xyz + (right * -0.5 - up * 0.288) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos1, 1.0));
                o.uv = float2(0.0, 0.0);
                triStream.Append(o);

                // Vertex 2: Bottom-Right
                float3 pos2 = worldPos.xyz + (right * 0.5 - up * 0.288) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos2, 1.0));
                o.uv = float2(1.0, 0.0);
                triStream.Append(o);

                // Vertex 3: Top-Center
                float3 pos3 = worldPos.xyz + (up * 0.577) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos3, 1.0));
                o.uv = float2(0.5, 1.0);
                triStream.Append(o);

                triStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // Soft glow falloff inside the triangle UV space
                float distToCenter = length(i.uv - float2(0.5, 0.33));
                float alpha = saturate(1.0 - distToCenter * 2.5) * i.color.a;
                return float4(i.color.rgb * 3.0, alpha); // HDR Bloom
            }
            ENDCG
        }
    }
}
