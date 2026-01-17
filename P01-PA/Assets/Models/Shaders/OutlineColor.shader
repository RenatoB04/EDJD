Shader "Custom/OutlineColor"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth("Outline Width", Range(0,0.05)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _BaseColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _BaseColor;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG

        // Pass de outline
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode"="Always" }
            Cull Front
            ZWrite On
            ZTest Less
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float3 offset = norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex + float4(offset, 0));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}