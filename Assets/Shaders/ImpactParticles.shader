Shader "Custom/ImpactParticles"
{
    Properties
    {
        _MainColor ("Spark Color", Color) = (1.0, 0.4, 0.0, 1.0)
        _Size ("Spark Size", Float) = 0.15
        _Speed ("Velocity Scale", Float) = 3.0
        _GravityScale ("Gravity Scale", Float) = 0.5
        
        // Variavel fundamental animada pelo C# (ImpactParticleSpawner.cs).
        // Vai de 0 (nascimento) a 1 (morte) e serve como a nossa variavel de "Tempo" (t) na equacao da fisica.
        _Progress ("Progress (0-1)", Range(0, 1.0)) = 0.0
    }
    SubShader
    {
        // IgnoreProjector previne que as particulas recebam sombras de projetores.
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        // Blend Aditivo: Soma a cor da particula a cor do fundo. Ideal para fogo, luz e faiscas.
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            // Definicao das 3 fases do pipeline
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            
            // Target 4.0 e o minimo exigido para suportar Geometry Shaders no Unity.
            #pragma target 4.0
            #include "UnityCG.cginc"

            // --- 1. ESTRUTURAS DE DADOS ---

            struct appdata
            {
                float4 vertex : POSITION;
                // TRUQUE DE OTIMIZACAO: Como um ponto nao tem "normal" (nao tem superficie), 
                // usamos a variavel normal para armazenar e transportar o vetor de direcao/velocidade inicial da faisca.
                float3 normal : NORMAL; 
            };

            // Vertex to Geometry
            struct v2g
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            // Geometry to Fragment
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


            // --- 2. VERTEX SHADER ---
            // Nao aplicamos transformacoes de matriz aqui. Passamos a posicao crua (local) 
            // e a direcao para o Geometry Shader tratar de tudo.
            v2g vert (appdata v)
            {
                v2g o;
                o.pos = v.vertex; 
                o.dir = v.normal; 
                return o;
            }


            // --- 3. GEOMETRY SHADER ---
            // Recebe um unico ponto (a raiz da faisca) e gera ate 3 novos vertices (um triangulo).
            [maxvertexcount(3)]
            void geom(point v2g IN[1], inout TriangleStream<g2f> triStream)
            {
                // Extrair a direcao/velocidade inicial que estava guardada na variavel normal
                float3 velocity = IN[0].dir;
                
                // CALCULO DE FISICA (Movimento Parabolico / Newton):
                // Formula: Posicao Final = Posicao Inicial + (Velocidade * Tempo) + (0.5 * Gravidade * Tempo^2)
                // Aqui o "Tempo" e representado pela variavel _Progress.
                float3 localPos = IN[0].pos.xyz + velocity * _Speed * _Progress;
                
                // Aplica a aceleracao da gravidade puxando o eixo Y para baixo com o passar do tempo.
                // 9.81 e a constante gravitacional da Terra.
                localPos.y -= 0.5 * 9.81 * _Progress * _Progress * _GravityScale;

                // Transforma a posicao simulada (espaco local) na posicao final no mundo (espaco global)
                float4 worldPos = mul(unity_ObjectToWorld, float4(localPos, 1.0));

                // BILLBOARDING (Fazer a particula olhar para a camara):
                // A matriz de visualizacao da camara (UNITY_MATRIX_V) guarda a orientacao da mesma.
                // Extraimos o vetor "Direita" (indice 0) e o vetor "Cima" (indice 1) da camara.
                // Vamos usar estes vetores para desenhar o triangulo sempre alinhado com o ecra.
                float3 right = UNITY_MATRIX_V[0].xyz;
                float3 up = UNITY_MATRIX_V[1].xyz;

                // Anima o tamanho: a particula encolhe a medida que o _Progress chega a 1 (morte).
                float size = _Size * (1.0 - _Progress);

                // Preparar as variaveis que vao ser partilhadas por todos os 3 novos vertices.
                g2f o;
                o.color = _MainColor;
                o.color.a *= (1.0 - _Progress); // O canal Alpha desvanece suavemente ao longo do tempo.

                // CONSTRUCAO DA GEOMETRIA (Gerar o Triangulo):
                // Vertice 1: Canto Inferior Esquerdo
                // Movemos o ponto para a esquerda (-right) e para baixo (-up) multiplicando pelo tamanho.
                float3 pos1 = worldPos.xyz + (right * -0.5 - up * 0.288) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos1, 1.0)); // Projetar no ecra
                o.uv = float2(0.0, 0.0); // Coordenada UV inferior esquerda
                triStream.Append(o); // Adicionar o vertice a malha final

                // Vertice 2: Canto Inferior Direito
                float3 pos2 = worldPos.xyz + (right * 0.5 - up * 0.288) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos2, 1.0));
                o.uv = float2(1.0, 0.0);
                triStream.Append(o);

                // Vertice 3: Canto Superior Centro
                float3 pos3 = worldPos.xyz + (up * 0.577) * size;
                o.pos = mul(UNITY_MATRIX_VP, float4(pos3, 1.0));
                o.uv = float2(0.5, 1.0);
                triStream.Append(o);

                // Finaliza e corta o fluxo, indicando que o triangulo esta completo.
                triStream.RestartStrip();
            }


            // --- 4. FRAGMENT SHADER ---
            // Colorir os pixeis no interior do triangulo recem-criado.
            fixed4 frag (g2f i) : SV_Target
            {
                // Em vez de desenhar um triangulo afiado, calculamos a distancia do pixel 
                // atual ate ao centro dinamico das UVs (float2(0.5, 0.33)).
                float distToCenter = length(i.uv - float2(0.5, 0.33));
                
                // Mascara de desvanecimento suave (Soft glow): pixeis nos limites ficam com Alpha 0.
                float alpha = saturate(1.0 - distToCenter * 2.5) * i.color.a;
                
                // Multiplicamos a cor RGB por 3.0 para forcar valores HDR (brilho extremo / Bloom).
                return float4(i.color.rgb * 3.0, alpha); 
            }
            ENDCG
        }
    }
}