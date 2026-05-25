Shader "Custom/Shield"
{
    Properties
    {
        // Cores base do escudo e da borda iluminada.
        _MainColor ("Main Color", Color) = (0.1, 0.4, 0.8, 0.2)
        _RimColor ("Rim Color", Color) = (0.3, 0.8, 1.0, 1.0)
        _RimPower ("Rim Power", Range(0.1, 8.0)) = 3.0 
        
        // Intensidade e cor da linha gerada quando o escudo atravessa outra geometria (ex: chao).
        [HDR] _IntersectColor ("Intersection Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _IntersectPower ("Intersection Thickness", Range(0.01, 5.0)) = 1.0
        
        // Definicoes do impacto (onda de choque ativada via C#).
        [HDR] _HitColor ("Hit Color", Color) = (1.0, 0.2, 0.2, 1.0)
        _RippleWidth ("Ripple Width", Range(0.1, 2.0)) = 0.5
        _RippleDisplacement ("Vertex Displacement", Range(0.0, 1.0)) = 0.2
        
        // Textura e forca para a refracao (distorcao visual do fundo).
        _DistortionMap ("Distortion Noise (Normal Map)", 2D) = "bump" {}
        _DistortionStrength ("Distortion Strength", Range(0.0, 0.5)) = 0.05
        
        // Variaveis ocultas, preenchidas exclusivamente pelo ShieldInteract.cs
        [HideInInspector] _HitPos ("Hit Position", Vector) = (0,0,0,0)
        [HideInInspector] _HitRadius ("Hit Radius", Float) = 0
        [HideInInspector] _MaxRadius ("Max Radius", Float) = 3.0
    }
    
    SubShader
    {
        // Tags que informam o Unity para desenhar este objeto apenas depois 
        // dos objetos opacos, permitindo a mistura de cores (transparencia).
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // Pausa temporariamente o pipeline grafico para guardar o que esta a 
        // ser renderizado atras deste objeto na textura _BackgroundTexture.
        GrabPass { "_BackgroundTexture" }

        CGPROGRAM
        // Diretiva surface: define a funcao 'surf' com iluminacao Standard.
        // alpha:fade ativa o blend transparente. vertex:vert liga o calculo de vertices.
        #pragma surface surf Standard alpha:fade vertex:vert
        #pragma target 3.0

        // Variaveis globais do Unity para refracao e intersecao.
        sampler2D _CameraDepthTexture;
        sampler2D _BackgroundTexture;
        sampler2D _DistortionMap;

        // Estrutura Input transporta dados interpolados do Vertex para o Fragment Shader.
        struct Input
        {
            float3 viewDir;          // Vetor direcao da camara (para calculo do Fresnel)
            float4 screenPos;        // Posicao do pixel projetado no ecra
            float3 worldPos;         // Posicao global XYZ do pixel
            float2 uv_DistortionMap; // Coordenadas UV da textura de ruido
        };

        float4 _MainColor;
        float4 _RimColor;
        float _RimPower;
        float4 _IntersectColor;
        float _IntersectPower;
        float4 _HitColor;
        float _RippleWidth;
        float _RippleDisplacement;
        float4 _HitPos;
        float _HitRadius;
        float _MaxRadius;
        float _DistortionStrength;

        // FUNCAO DE VERTICES: Executada na GPU para cada vertice da malha.
        void vert(inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            // 1. Converter a posicao do vertice (local) para coordenadas globais do mundo.
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            
            // 2. Calcular a distancia escalar entre o vertice atual e o ponto central do impacto.
            float dist = distance(worldPos, _HitPos.xyz);

            // 3. Criar uma mascara matematica em forma de anel (ripple) usando smoothstep.
            // O resultado sera 1 (deformacao maxima) na crista da onda, descendo para 0 nas margens.
            float ripple = 1.0 - smoothstep(0.0, _RippleWidth, abs(dist - _HitRadius));

            // 4. Modificar fisicamente o vertice, empurrando-o para fora ao longo do seu
            // proprio vetor normal, multiplicado pela mascara e pela forca de deslocamento.
            v.vertex.xyz += v.normal * ripple * _RippleDisplacement;
        }

        // FUNCAO DE SUPERFICIE: Executada na GPU para cada pixel visivel.
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // --- REFRACAO E DISTORCAO ---
            // Converter o vetor 4D da posicao de ecra numa coordenada UV 2D.
            // A divisao pelo componente W assegura a correta perspetiva. A funcao max() previne divisoes por zero.
            float2 screenUV = IN.screenPos.xy / max(IN.screenPos.w, 0.001);

            // Somar a variavel de tempo (_Time.y) as UVs originais para animar o ruido.
            float2 animatedUV = IN.uv_DistortionMap + float2(_Time.y * 0.1, _Time.y * 0.15);
            
            // UnpackNormal extrai os vetores XYZ gravados na textura bump map.
            float3 distortion = UnpackNormal(tex2D(_DistortionMap, animatedUV));

            // Adicionar o vetor X/Y do ruido as coordenadas limpas do ecra, causando o desvio visual.
            float2 distortedUV = screenUV + (distortion.xy * _DistortionStrength);

            // Capturar o RGB do ecrã (atras do escudo) usando as novas UVs distorcidas.
            float3 refraction = tex2D(_BackgroundTexture, distortedUV).rgb;


            // --- FRESNEL (BORDAS ILUMINADAS) ---
            // O produto escalar (dot) devolve 1 quando o angulo entre a camara e a normal e 0.
            // Subtraindo a 1.0, invertemos o efeito: 0 no centro, 1 nas bordas tangenciais.
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            
            // Potenciar o resultado usando uma exponencial (pow) e multiplicar pela cor escolhida.
            float3 rimEmission = _RimColor.rgb * pow(rim, _RimPower);


            // --- INTERSECAO DE PROFUNDIDADE ---
            // Ler e converter o valor puro do Z-Buffer da camara para uma escala linear de distancia.
            float sceneZ = LinearEyeDepth(tex2D(_CameraDepthTexture, screenUV).r);
            
            // Extrair a distancia linear a que se encontra a superficie do proprio escudo.
            float shieldZ = IN.screenPos.z;

            // Determinar a diferenca fisica entre o cenario e o escudo.
            // Quanto mais perto de zero (saturate), maior o valor de 'intersect' gerado.
            float diff = sceneZ - shieldZ;
            float intersect = 1.0 - saturate(diff / _IntersectPower);


            // --- ONDA DE IMPACTO (VISUAL) ---
            // Repetir a logica do anel aplicada nos vertices, mas agora para calcular a cor do fragmento.
            float dist = distance(IN.worldPos, _HitPos.xyz);
            float rippleRing = 1.0 - smoothstep(0.0, _RippleWidth, abs(dist - _HitRadius));

            // Calcular a percentagem de dispersao: quanto maior for o raio da onda em relacao 
            // ao raio maximo, mais o fadeOut se aproxima de 0, tornando o anel invisivel.
            float fadeOut = 1.0 - saturate(_HitRadius / _MaxRadius);
            rippleRing *= fadeOut;


            // --- RESULTADO FINAL ---
            // Tingir a imagem refratada capturada pelo GrabPass usando a cor principal do escudo.
            float3 baseColor = refraction * _MainColor.rgb * 2.0;

            // Somar todos os componentes isolados e atribuir a emissao do material.
            o.Emission = baseColor + rimEmission + (_IntersectColor.rgb * intersect) + (_HitColor.rgb * rippleRing);

            // Como a refracao (o cenario capturado) ja e desenhada na variavel Emission, 
            // a malha pode manter o seu canal Alpha a 1.0 (opaco) e manter a ilusao de ser de vidro.
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}