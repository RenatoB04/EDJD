Shader "Custom/EnergyCoreRaymarching"
{
    Properties
    {
        // Propriedades visuais do plasma e do nucleo
        [HDR] _CoreColor ("Core Glow Color", Color) = (0.0, 0.8, 1.0, 1.0)
        _CoreRadius ("Core Radius", Range(0.05, 0.45)) = 0.22
        _CoreSoftness ("Core Softness", Range(0.01, 0.3)) = 0.12
        
        // Controlos matematicos do ruido procedural
        _NoiseScale ("Noise Scale", Float) = 6.0
        _NoiseSpeed ("Noise Speed", Float) = 1.5
        _DensityMultiplier ("Density Multiplier", Range(0.01, 0.5)) = 0.08
        
        // Quantos passos o raio vai dar. Mais passos = melhor qualidade, mas mais pesado.
        _Steps ("Raymarching Steps", Range(16, 64)) = 32
    }
    SubShader
    {
        // IgnoreProjector e ZWrite Off garantem que este volume funciona como fumo/luz 
        // transparente, sem bloquear outros objetos opacos no Z-Buffer.
        Tags { "Queue"="Transparent-10" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 localPos : TEXCOORD0;
                float3 localCamPos : TEXCOORD1;
            };

            float4 _CoreColor;
            float _CoreRadius;
            float _CoreSoftness;
            float _NoiseScale;
            float _NoiseSpeed;
            float _DensityMultiplier;
            int _Steps;

            // --- 1. VERTEX SHADER ---
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                
                // Em vez do World Space, guardamos as posicoes no Local Space (relativas ao cubo).
                // Isto garante que se movermos ou rodarmos o gerador no Unity, o nucleo de plasma
                // roda e move-se perfeitamente com ele.
                o.localPos = v.vertex.xyz; 
                
                // Convertamos a posicao global da camara para o espaco local do cubo.
                o.localCamPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz; 
                return o;
            }

            // --- 2. GERACAO DE RUIDO 3D (VALUE NOISE) ---

            // Funcao pseudo-aleatoria (Hash):
            // Pega numa coordenada 3D e, atraves de multiplicacoes e fracoes matemáticas, 
            // cospe um valor pseudo-aleatorio caotico (entre 0 e 1).
            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + float3(0.1, 0.1, 0.1));
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            // Funcao Noise (Interpolacao Trilinear):
            // O Hash sozinho faz "estatica" de televisao. Esta funcao pega num ponto X,
            // descobre os 8 cantos inteiros (cubo virtual) a volta desse ponto, gera o Hash 
            // para cada canto e faz a media (lerp) entre eles. O resultado sao "nuvens" suaves 3D.
            float noise(float3 x)
            {
                float3 i = floor(x); // Parte inteira (a grelha)
                float3 f = frac(x);  // Parte fracionaria (posicao dentro da celula da grelha)
                
                // Suaviza a transicao (Hermite curve)
                f = f * f * (3.0 - 2.0 * f);
                
                // Interpola as 3 dimensoes (X, Y e Z)
                return lerp(lerp(lerp(hash(i + float3(0,0,0)), hash(i + float3(1,0,0)), f.x),
                                lerp(hash(i + float3(0,1,0)), hash(i + float3(1,1,0)), f.x), f.y),
                            lerp(lerp(hash(i + float3(0,0,1)), hash(i + float3(1,0,1)), f.x),
                                lerp(hash(i + float3(0,1,1)), hash(i + float3(1,1,1)), f.x), f.y), f.z);
            }

            // --- 3. FBM (Fractal Brownian Motion) ---
            // O Value Noise basico e demasiado "esfumaçado". O FBM e um loop que soma o ruido 
            // consigo proprio em varias camadas (octaves). A cada camada (i), o ruido fica 
            // mais pequeno (frequency * 2.0) e mais fraco (amplitude * 0.5), criando os 
            // "veios" e o detalhe fino de um verdadeiro plasma energetico.
            float fbm(float3 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                // 3 Camadas de detalhe (Octaves)
                for (int i = 0; i < 3; i++)
                {
                    value += amplitude * noise(p * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            // --- 4. FRAGMENT SHADER (O CICLO DE RAYMARCHING) ---
            fixed4 frag (v2f i) : SV_Target
            {
                // Em vez de desenhar a superficie do cubo, calculamos um "raio" de visao
                // que parte da Camara (localCamPos) e passa por cada pixel do cubo (localPos).
                float3 rayOrigin = i.localPos;
                float3 rayDir = normalize(i.localPos - i.localCamPos);

                // Preparar o percurso do raio
                float t = 0.0; // Distancia percorrida pelo raio
                float stepSize = 1.0 / _Steps; // Tamanho de cada "salto" do raio
                float4 accumulatedColor = float4(0, 0, 0, 0); // Variavel onde vamos somar a luz/fumo

                // O Loop de Raymarching: Avanca no espaco 3D passo a passo.
                for (int stepIdx = 0; stepIdx < _Steps; stepIdx++)
                {
                    // Descobrir onde o raio esta neste exato passo
                    float3 p = rayOrigin + rayDir * t;

                    // Deteção de Limites: O cubo padrao do Unity vai de -0.5 a 0.5.
                    // Se o raio sair fora do cubo, paramos de calcular para poupar processamento (Break).
                    if (any(abs(p) > 0.5)) break;

                    // "SDF" (Signed Distance Field) de uma esfera: 
                    // Calcula a distancia do ponto atual ate ao centro (0,0,0), menos o raio da esfera.
                    float d = length(p) - _CoreRadius;

                    // Se a distancia for menor que a margem de suavidade, o raio "entrou" no nucleo!
                    if (d < _CoreSoftness)
                    {
                        // Deslocar o ruido no tempo para dar a ilusao de que o plasma esta a rodopiar e a fluir.
                        float3 noiseOffset = float3(_Time.y, _Time.y * 1.3, _Time.y * 0.7) * _NoiseSpeed;
                        
                        // Gerar a textura tridimensional do plasma naquele ponto exato usando FBM.
                        float plasmaNoise = fbm(p * _NoiseScale - noiseOffset);

                        // Calcular a densidade: Mais opaco no centro (d aproxima-se de 0) e mais 
                        // transparente nas bordas (d aproxima-se de _CoreSoftness).
                        float density = saturate(1.0 - (d / _CoreSoftness)) * plasmaNoise;

                        // Se houver material neste ponto do espaco, adicionamos a cor
                        if (density > 0.0)
                        {
                            float alphaStep = density * _DensityMultiplier;
                            
                            // Acumular brilho e opacidade. Multiplicamos por 1.5 para efeito HDR/Bloom.
                            accumulatedColor.rgb += _CoreColor.rgb * alphaStep * 1.5;
                            accumulatedColor.a += alphaStep;

                            // OTIMIZACAO (Early Termination): 
                            // Se a nevoa de plasma ja esta 95% opaca, o raio de luz nao consegue 
                            // atravessar mais fundo. Paramos o ciclo para poupar a placa grafica.
                            if (accumulatedColor.a >= 0.95)
                            {
                                accumulatedColor.a = 1.0;
                                break;
                            }
                        }
                    }

                    // Avancar o raio para o proximo passo
                    t += stepSize;
                }

                // Se o raio atravessou o cubo inteiro sem bater em plasma, descartamos 
                // o pixel (discard) para que fique 100% invisivel.
                if (accumulatedColor.a <= 0.0) discard;

                return accumulatedColor;
            }
            ENDCG
        }
    }
}