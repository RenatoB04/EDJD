Shader "Custom/ShieldGenerator_Pure"
{
    Properties
    {
        _MainColor ("Base Color", Color) = (0.2, 0.2, 0.2, 1.0)
        _EmissionColor ("Active Glow Color", Color) = (0.0, 1.0, 0.8, 1.0)
        _DispTex ("Displacement Heightmap", 2D) = "white" {}
        _Displacement ("Max Displacement", Range(0, 1.0)) = 0.2
        _Tess ("Tessellation Factor", Range(1, 32)) = 8
        _ActiveState ("Active State (0-1)", Range(0, 1)) = 0.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma hull hull
            #pragma domain domain
            #pragma fragment frag
            #pragma target 4.6
            #include "UnityCG.cginc"

            float4 _MainColor;
            float4 _EmissionColor;
            sampler2D _DispTex;
            float _Displacement;
            float _Tess;
            float _ActiveState;

            // 1. Data Structures
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2h {
                float4 vertex : POS;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // FIX: Use the strict array syntax required by some HLSL compilers
            struct h2d_main {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            struct h2d {
                float4 vertex : POS;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct d2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // 2. Vertex Shader
            v2h vert(appdata v) {
                v2h o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.uv = v.uv;
                return o;
            }

            // 3. Patch Constant Function
            h2d_main patchConstantFunction(InputPatch<v2h, 3> patch) {
                h2d_main o;
                // Assign the detail level to each triangle edge
                o.edge[0] = _Tess;
                o.edge[1] = _Tess;
                o.edge[2] = _Tess;
                o.inside = _Tess;
                return o;
            }

            // 4. Hull Shader
            [domain("tri")]
            [partitioning("fractional_odd")]
            [outputtopology("triangle_cw")]
            [outputcontrolpoints(3)]
            [patchconstantfunc("patchConstantFunction")]
            h2d hull(InputPatch<v2h, 3> patch, uint id : SV_OutputControlPointID) {
                h2d o;
                o.vertex = patch[id].vertex;
                o.normal = patch[id].normal;
                o.uv = patch[id].uv;
                return o;
            }

            // 5. Domain Shader
            [domain("tri")]
            d2f domain(h2d_main tessFactors, const OutputPatch<h2d, 3> patch, float3 barycentricCoords : SV_DomainLocation) {
                d2f o;
                
                // Interpolate the newly created vertices
                float4 vertex = patch[0].vertex * barycentricCoords.x + patch[1].vertex * barycentricCoords.y + patch[2].vertex * barycentricCoords.z;
                float3 normal = patch[0].normal * barycentricCoords.x + patch[1].normal * barycentricCoords.y + patch[2].normal * barycentricCoords.z;
                float2 uv = patch[0].uv * barycentricCoords.x + patch[1].uv * barycentricCoords.y + patch[2].uv * barycentricCoords.z;
                
                normal = normalize(normal);

                // Apply deformation by reading the Voronoi map
                float d = tex2Dlod(_DispTex, float4(uv, 0, 0)).r;
                vertex.xyz += normal * d * _Displacement * _ActiveState;
                
                o.pos = UnityObjectToClipPos(vertex);
                o.uv = uv;
                return o;
            }

            // 6. Fragment Shader
            fixed4 frag(d2f i) : SV_Target {
                float d = tex2D(_DispTex, i.uv).r;
                float3 emission = _EmissionColor.rgb * d * _ActiveState * 2.0;
                
                float3 finalColor = _MainColor.rgb + emission;
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
