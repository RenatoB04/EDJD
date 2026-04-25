Shader "Custom/WorldReflex"
{
    Properties
    {
        _Color ("Cor Principal", Color) = (1,1,1,1)
        _MainTex ("Textura Base", 2D) = "white" {}
        // A propriedade _Cube vai armazenar a nossa imagem 360º (HDRI) ou a reflexão em tempo real
        _Cube ("HDRI / Cubemap", Cube) = "" {}
        _ReflectLevel ("Nível de Reflexão", Range(0.0, 1.0)) = 0.8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard

        #pragma target 3.0

        sampler2D _MainTex;
        samplerCUBE _Cube; // Tipo de dados para texturas de reflexão (Cúbicas)
        
        struct Input
        {
            float2 uv_MainTex;
            float3 worldRefl; // O Unity calcula automaticamente o vetor de reflexão e guarda-o aqui
        };

        fixed4 _Color;
        float _ReflectLevel;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 1. Aplicar a textura base e cor
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // 2. Obter a cor da reflexão usando o vetor worldRefl
            fixed4 reflexao = texCUBE(_Cube, IN.worldRefl);

            // 3. Adicionamos a reflexão à Emissão para que brilhe com o ambiente
            o.Emission = reflexao.rgb * _ReflectLevel;
            
            // 4. Fechar os poros do material para ele parecer um espelho/metal
            o.Metallic = 0.9;
            o.Smoothness = 0.9;
        }
        ENDCG
    }
    FallBack "Diffuse"
}