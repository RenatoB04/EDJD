Shader "Custom/TexturaRotativa"
{
    Properties
    {
        _Color ("Cor Principal", Color) = (1,1,1,1)
        _MainTex ("Textura Base (RGB) Transparência (A)", 2D) = "white" {}
        // Adicionamos uma propriedade para controlar a velocidade no Inspector
        _RotationSpeed ("Velocidade de Rotação", Range(-10, 10)) = 1.0 
    }
    SubShader
    {
        // 1. Tags ajustadas para renderizar transparência corretamente
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        // 2. A diretiva 'alpha:fade' é crucial no Built-in Pipeline para que o shader suporte transparência
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
            
            // Passo A: O centro das coordenadas UV normais é (0.5, 0.5). 
            // Para rodar a imagem sobre si mesma, temos de subtrair este valor para mover o "eixo de rotação" para a origem (0,0).
            float2 pivot = float2(0.5, 0.5);
            float2 uvCentralizado = IN.uv_MainTex - pivot;

            // Passo B: Calcular o ângulo de rotação contínua utilizando a variável _Time.y (tempo em segundos) e a nossa velocidade.
            float angulo = _Time.y * _RotationSpeed;
            float cosAng = cos(angulo);
            float sinAng = sin(angulo);

            // Passo C: Aplicar a matriz de rotação 2D clássica.
            float2 uvRotacionado;
            uvRotacionado.x = uvCentralizado.x * cosAng - uvCentralizado.y * sinAng;
            uvRotacionado.y = uvCentralizado.x * sinAng + uvCentralizado.y * cosAng;

            // Passo D: Devolver as coordenadas ao seu local original somando novamente o pivot (0.5, 0.5).
            uvRotacionado += pivot;

            // Lemos a textura usando as novas coordenadas UV que acabámos de calcular
            fixed4 c = tex2D (_MainTex, uvRotacionado) * _Color;
            
            // Atribuímos a cor ao Albedo e o canal Alpha da textura à transparência do material
            o.Albedo = c.rgb;
            o.Alpha = c.a; 
        }
        ENDCG
    }
    FallBack "Diffuse"
}