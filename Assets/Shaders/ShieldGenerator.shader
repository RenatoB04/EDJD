Shader "Custom/ShieldGenerator_Pure"
{
    Properties
    {
        _MainColor ("Base Color", Color) = (0.2, 0.2, 0.2, 1.0)
        _EmissionColor ("Active Glow Color", Color) = (0.0, 1.0, 0.8, 1.0)
        
        // Textura a preto e branco onde o branco dita a altura maxima do relevo.
        _DispTex ("Displacement Heightmap", 2D) = "white" {}
        _Displacement ("Max Displacement", Range(0, 1.0)) = 0.2
        
        // Nivel de subdivisao dos poligonos. Quanto maior, mais geometria e gerada.
        _Tess ("Tessellation Factor", Range(1, 32)) = 8
        
        // Variavel controlada pelo C# (ShieldGeneratorController).
        // 0 = Gerador desligado (liso), 1 = Gerador ligado (com relevo e brilho).
        _ActiveState ("Active State (0-1)", Range(0, 1)) = 0.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            // Definicao estrita do pipeline completo de Tessellation
            #pragma vertex vert
            #pragma hull hull
            #pragma domain domain
            #pragma fragment frag
            
            // O target 4.6 e obrigatorio no Unity para usar Tessellation (DirectX 11)
            #pragma target 4.6
            #include "UnityCG.cginc"

            float4 _MainColor;
            float4 _EmissionColor;
            sampler2D _DispTex;
            float _Displacement;
            float _Tess;
            float _ActiveState;

            // --- 1. ESTRUTURAS DE DADOS ---
            
            // Dados originais do modelo 3D
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // Vertex to Hull (Passagem de dados do Vert para o Hull)
            struct v2h {
                float4 vertex : POS;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // Patch Constant Data: Define o quao subdividido o triangulo vai ser.
            struct h2d_main {
                // Declarado como array estrito para evitar erros de compilacao em certos hardwares
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            // Hull to Domain (Passagem dos pontos de controlo para o Domain)
            struct h2d {
                float4 vertex : POS;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // Domain to Fragment (Apos geracao e deformacao, envia para desenhar os pixeis)
            struct d2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            // --- 2. VERTEX SHADER ---
            // Num shader de Tessellation puro, o Vertex Shader funciona apenas como um "Pass-Through".
            // Nao multiplica matrizes aqui, apenas reencaminha os dados puros para o Hull Shader.
            v2h vert(appdata v) {
                v2h o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.uv = v.uv;
                return o;
            }


            // --- 3. PATCH CONSTANT FUNCTION ---
            // Executa uma vez por cada poligono (Patch).
            // Define a quantidade de novos vertices a serem gerados dentro deste poligono.
            h2d_main patchConstantFunction(InputPatch<v2h, 3> patch) {
                h2d_main o;
                // Atribui o fator de subdivisao (_Tess) as 3 arestas externas do triangulo e ao seu interior.
                o.edge[0] = _Tess;
                o.edge[1] = _Tess;
                o.edge[2] = _Tess;
                o.inside = _Tess;
                return o;
            }


            // --- 4. HULL SHADER ---
            // Executa uma vez por cada vertice do triangulo original (3 vezes).
            // Prepara as configuracoes para a fase final.
            [domain("tri")] // Estamos a trabalhar com triangulos
            [partitioning("fractional_odd")] // Como lidar com fatores de subdivisao fracionados (evita saltos bruscos)
            [outputtopology("triangle_cw")] // Sentido dos ponteiros do relogio (ClockWise)
            [outputcontrolpoints(3)]
            [patchconstantfunc("patchConstantFunction")]
            h2d hull(InputPatch<v2h, 3> patch, uint id : SV_OutputControlPointID) {
                h2d o;
                o.vertex = patch[id].vertex;
                o.normal = patch[id].normal;
                o.uv = patch[id].uv;
                return o;
            }


            // --- 5. DOMAIN SHADER (A MAGIA ACONTECE AQUI) ---
            // A placa grafica acabou de gerar milhares de novos vertices. O Domain Shader executa
            // uma vez para CADA UM desses novos vertices para calcular a sua posicao final 3D.
            [domain("tri")]
            d2f domain(h2d_main tessFactors, const OutputPatch<h2d, 3> patch, float3 barycentricCoords : SV_DomainLocation) {
                d2f o;
                
                // COORDENADAS BARICENTRICAS
                // Um novo vertice gerado no meio de um triangulo nao tem dados proprios.
                // A sua posicao real (e normal, e UVs) e calculada como uma media pesada (barycentricCoords)
                // baseada na sua proximidade aos 3 cantos originais do triangulo (patch 0, 1 e 2).
                
                float4 vertex = patch[0].vertex * barycentricCoords.x + patch[1].vertex * barycentricCoords.y + patch[2].vertex * barycentricCoords.z;
                float3 normal = patch[0].normal * barycentricCoords.x + patch[1].normal * barycentricCoords.y + patch[2].normal * barycentricCoords.z;
                float2 uv =     patch[0].uv     * barycentricCoords.x + patch[1].uv     * barycentricCoords.y + patch[2].uv     * barycentricCoords.z;
                
                normal = normalize(normal);

                // LER TEXTURA NO VERTEX PIPELINE
                // Usamos 'tex2Dlod' e nao o 'tex2D' normal porque ainda estamos na fase de manipulacao 
                // de geometria (antes dos pixeis). A placa grafica ainda nao consegue calcular MipMaps 
                // automaticamente, logo temos de forcar a leitura do MipMap 0 (float4(uv, 0, 0)).
                float displacementValue = tex2Dlod(_DispTex, float4(uv, 0, 0)).r;
                
                // Empurra o vertice gerado para fora, seguindo a sua normal, baseado na textura de ruido.
                // A variavel _ActiveState (do C#) funciona como multiplicador. Se for 0, anula o relevo.
                vertex.xyz += normal * displacementValue * _Displacement * _ActiveState;
                
                // Converte a coordenada 3D final para a projecao 2D do ecra.
                o.pos = UnityObjectToClipPos(vertex);
                o.uv = uv;
                return o;
            }


            // --- 6. FRAGMENT SHADER ---
            // Tinta os pixeis do ecra de acordo com o resultado final.
            fixed4 frag(d2f i) : SV_Target {
                
                // Lemos a textura novamente para saber quais as partes altas do relevo
                float displacementValue = tex2D(_DispTex, i.uv).r;
                
                // Partes mais altas (displacementValue proximo de 1) terao mais emissao de luz.
                // Novamente multiplicado por _ActiveState para so acender quando o C# ditar.
                float3 emission = _EmissionColor.rgb * displacementValue * _ActiveState * 2.0;
                
                // Cor final = Cor Base Escura + Emissao Luminosa
                float3 finalColor = _MainColor.rgb + emission;
                
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}