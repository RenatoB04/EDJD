Shader "Custom/EmergencyPostProcessing"
{
    Properties
    {
        // A textura do ecra (o Unity preenche isto automaticamente com o que a camara esta a ver)
        _MainTex ("Screen Texture", 2D) = "white" {}
        
        // Variavel global controlada pelo C# (variavel de vida do escudo)
        _EmergencyIntensity ("Emergency Intensity (0-1)", Range(0, 1.0)) = 0.0
        
        // Controlos visuais do efeito
        _GlitchScale ("Glitch Scale", Range(0, 0.1)) = 0.03
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.05)) = 0.015
        _EmergencyColor ("Emergency Vignette Color", Color) = (0.8, 0.0, 0.0, 1.0)
    }
    SubShader
    {
        // Como este shader e de ecrã inteiro (Post-Processing), estamos essencialmente a desenhar
        // um retangulo 2D colado a lente da camara. Por isso, desligamos o Culling (Cull Off), 
        // desligamos a escrita de profundidade (ZWrite Off) e obrigamos a desenhar sempre (ZTest Always).
        // Isto foi exatamente o que o professor fez no shader "random2d".
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EmergencyIntensity;
            float _GlitchScale;
            float _ChromaticAberration;
            float4 _EmergencyColor;

            // --- VERTEX SHADER ---
            // Apenas reencaminha os vertices do ecra (quad) para serem pintados.
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // --- FRAGMENT SHADER (O Filtro da Camara) ---
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // ---------------------------------------------------------
                // EFEITO 1: GLITCH (Interferencias Horizontais)
                // ---------------------------------------------------------
                // Multiplicamos o uv.y (eixo vertical) por valores altos dentro de um sin/cos.
                // Como usamos o eixo Y, a variacao acontece de cima para baixo, criando "linhas" horizontais.
                // O _Time.y faz com que essas linhas desçam/subam pelo ecra ao longo do tempo.
                float glitchNoise = sin(uv.y * 40.0 + _Time.y * 70.0) * cos(uv.y * 12.0 - _Time.y * 30.0);
                
                // Uma segunda onda de ruido muito mais fina e rapida (alta frequencia)
                float burstNoise = sin(uv.y * 300.0 + _Time.y * 120.0);
                
                // Mistura as duas ondas para um ruido mais caotico e realista
                float combinedNoise = lerp(glitchNoise, burstNoise, 0.3);

                // Para que o glitch nao esteja sempre a acontecer (o que enjoa), usamos a funcao step().
                // O step retorna 1 (ligado) se o ruido for maior que 0.7, e 0 (desligado) caso contrario.
                // Isto cria "picos" ou "surtos" intermitentes de interferencia.
                float glitchThreshold = step(0.7, sin(_Time.y * 5.0) * cos(_Time.y * 3.1));
                
                // Calculamos o desvio final apenas no eixo X (queremos que a imagem trema para os lados).
                // Multiplicamos pela intensidade global para que so trema se o escudo estiver a sofrer dano.
                float2 glitchOffset = float2(combinedNoise * _GlitchScale * glitchThreshold * _EmergencyIntensity, 0.0);
                
                // Aplicamos este tremor as UVs originais do ecra.
                float2 distortedUV = uv + glitchOffset;


                // ---------------------------------------------------------
                // EFEITO 2: ABERRACAO CROMATICA (Cores Separadas)
                // ---------------------------------------------------------
                // A Aberracao Cromatica simula uma lente partida, separando os canais RGB.
                // Calculamos o desvio animando-o suavemente com um sin() para pulsar ligeiramente.
                float caScale = _ChromaticAberration * _EmergencyIntensity * (sin(_Time.y * 20.0) * 0.3 + 0.7);
                float2 caOffset = float2(caScale, 0.0);

                // Lemos a textura do ecra 3 vezes! 
                // Canal Vermelho (R): Le a imagem um pouco deslocada para a direita (+ caOffset)
                // Canal Verde (G): Le a imagem no sitio exato (distortedUV)
                // Canal Azul (B): Le a imagem um pouco deslocada para a esquerda (- caOffset)
                float r = tex2D(_MainTex, distortedUV + caOffset).r;
                float g = tex2D(_MainTex, distortedUV).g;
                float b = tex2D(_MainTex, distortedUV - caOffset).b;
                
                // Reconstruimos a cor do pixel juntando os 3 canais separados.
                float4 screenColor = float4(r, g, b, 1.0);


                // ---------------------------------------------------------
                // EFEITO 3: VINHETA VERMELHA (Alarme a piscar nas bordas)
                // ---------------------------------------------------------
                // Medimos a distancia desde o pixel atual ate ao centro exato do ecra (0.5, 0.5).
                float distFromCenter = distance(uv, float2(0.5, 0.5));
                
                // Usamos o smoothstep para criar uma mascara circular.
                // Pixeis a menos de 0.35 de distancia do centro retornam 0 (mascara invisivel).
                // Pixeis a mais de 0.85 de distancia do centro retornam 1 (mascara opaca).
                float vignette = smoothstep(0.35, 0.85, distFromCenter);
                
                // Fazemos a vinheta "piscar" como uma sirene usando um sin() associado ao tempo.
                float pulse = sin(_Time.y * 12.0) * 0.4 + 0.6;
                vignette *= pulse * _EmergencyIntensity;

                // Misturamos a cor do ecra (com aberração e glitch) com a cor vermelha de emergencia (_EmergencyColor).
                // O valor 'vignette' atua como balança: no centro pinta a cor do ecra, nas bordas pinta o vermelho.
                fixed4 finalColor = lerp(screenColor, _EmergencyColor, vignette);
                
                return finalColor;
            }
            ENDCG
        }
    }
}