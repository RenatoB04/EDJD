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
                float3 normal : NORMAL;
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
                o.pos = v.vertex;
                o.dir = v.normal;
                return o;
            }

            // Expand each source point into a camera-facing triangular spark.
            [maxvertexcount(3)]
            void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
            {
                float3 velocity = IN[0].dir;

                // Simulate parabolic motion using _Progress as normalized lifetime.
                float3 localPos = IN[0].pos.xyz + velocity * _Speed * _Progress;
                localPos.y -= 0.5 * 9.81 * _Progress * _Progress * _GravityScale;

                float4 worldPos = mul(unity_ObjectToWorld, float4(localPos, 1.0));

                float3 right = UNITY_MATRIX_V[0].xyz;
                float3 up = UNITY_MATRIX_V[1].xyz;
                float size = _Size * (1.0 - _Progress);

                g2f o;
                o.color = _MainColor;
                o.color.a *= (1.0 - _Progress);

                float3 pos1 = worldPos.xyz + (right * -0.5 - up * 0.288) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos1, 1.0));
                o.uv = float2(0.0, 0.0);
                triStream.Append(o);

                float3 pos2 = worldPos.xyz + (right * 0.5 - up * 0.288) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos2, 1.0));
                o.uv = float2(1.0, 0.0);
                triStream.Append(o);

                float3 pos3 = worldPos.xyz + (up * 0.577) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos3, 1.0));
                o.uv = float2(0.5, 1.0);
                triStream.Append(o);

                triStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // Soft radial falloff hides the hard triangle edges.
                float distToCenter = length(i.uv - float2(0.5, 0.33));
                float alpha = saturate(1.0 - distToCenter * 2.5) * i.color.a;

                return float4(i.color.rgb * 3.0, alpha);
            }
            ENDCG
        }
    }
}
