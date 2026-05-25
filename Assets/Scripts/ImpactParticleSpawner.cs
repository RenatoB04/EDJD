using UnityEngine;
using System.Collections;

// Este script não está na cena por defeito. É instanciado (criado) dinamicamente 
// pelo ShieldInteract.cs no exato momento e local onde o rato clica no escudo.
public class ImpactParticleSpawner : MonoBehaviour
{
    private Material particleMaterial;
    private Mesh particleMesh;

    [Header("Particle Settings")]
    [Tooltip("The Geometry Shader used to generate and animate sparks on the GPU.")]
    public Shader particleShader;
    
    [Tooltip("Color of the impact sparks.")]
    [ColorUsage(true, true)] // Ativa o modo HDR no Unity para podermos meter intensidades > 1 (Bloom)
    public Color particleColor = new Color(1.0f, 0.4f, 0.0f, 1.0f);
    
    [Tooltip("Number of individual sparks generated per impact.")]
    [Range(5, 50)]
    public int particleCount = 20;
    
    [Tooltip("Initial size of each spark.")]
    public float sparkSize = 0.15f;
    
    [Tooltip("Initial velocity of the sparks.")]
    public float speed = 4.0f;
    
    [Tooltip("Gravity acceleration scale applied to the particles (pulling them down).")]
    public float gravityScale = 0.6f;
    
    [Tooltip("Lifetime of the particle effect in seconds.")]
    public float duration = 1.0f;

    // FUNÇÃO PRINCIPAL: Chamada pelo ShieldInteract para construir a explosão.
    // Recebe o hitPoint (onde a bala bateu) e a normal (para que lado a face do escudo está virada).
    public void Init(Vector3 hitPoint, Vector3 normal)
    {
        // 1. Colocar este objeto invisível exatamente onde o impacto aconteceu.
        transform.position = hitPoint;

        // 2. Adicionar os componentes necessários para desenhar uma malha (Mesh) em tempo real.
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();

        // 3. Preparar o material com o nosso Geometry Shader.
        if (particleShader == null)
            particleShader = Shader.Find("Custom/ImpactParticles");

        // Criar uma instância única do material para que esta explosão não afete outras.
        particleMaterial = new Material(particleShader);
        particleMaterial.SetColor("_MainColor", particleColor);
        particleMaterial.SetFloat("_Size", sparkSize);
        particleMaterial.SetFloat("_Speed", speed);
        particleMaterial.SetFloat("_GravityScale", gravityScale);
        renderer.material = particleMaterial;

        // 4. CONSTRUÇÃO DA MALHA (A Magia):
        particleMesh = new Mesh();
        Vector3[] vertices = new Vector3[particleCount];
        Vector3[] normals = new Vector3[particleCount];
        int[] indices = new int[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            // Todos os pontos nascem exatamente na mesma coordenada: a raiz (0,0,0) em espaço local.
            vertices[i] = Vector3.zero; 

            // Criar uma direção aleatória esférica (uma explosão para todos os lados)
            Vector3 randomDir = Random.onUnitSphere;
            
            // VERIFICAÇÃO DE HEMISFÉRIO (Matemática Pura):
            // O Dot Product (produto escalar) compara a direção aleatória com a direção do escudo (normal).
            // Se for menor que 0, significa que a faísca ia ser disparada para "dentro" do escudo.
            if (Vector3.Dot(randomDir, normal) < 0)
            {
                // Invertemos a direção para garantir que todas as faíscas saltam para "fora" (na direção do jogador).
                randomDir = -randomDir; 
            }

            // O TRUQUE DA NORMAL:
            // Usamos a função Slerp (interpolação esférica) para criar um cone. Misturamos a 
            // direção pura do escudo (normal) com a direção aleatória (randomDir).
            // Guardamos isto no array de Normais para o Geometry Shader ler isto como "Vetor de Velocidade".
            // Multiplicamos no fim por um Random.Range para que umas faíscas saiam mais rápidas que outras.
            normals[i] = Vector3.Slerp(normal, randomDir, Random.Range(0.3f, 0.9f)).normalized * Random.Range(0.6f, 1.4f);
            
            // Atribuir o número (ID) deste vértice.
            indices[i] = i;
        }

        // 5. Injetar as listas matemáticas na Malha (Mesh)
        particleMesh.vertices = vertices;
        particleMesh.normals = normals;
        
        // Topologia de Pontos.
        // O Unity normalmente liga os vértices em grupos de 3 para formar triângulos (MeshTopology.Triangles).
        // Ao usarmos 'Points', dizemos ao Unity para não ligar nada. Envia os pontos soltos para o 
        // Geometry Shader, e ele que construa a geometria real.
        particleMesh.SetIndices(indices, MeshTopology.Points, 0);
        filter.mesh = particleMesh;

        // Iniciar o cronómetro da animação
        StartCoroutine(AnimateParticles());
    }

    // CORROTINA: Uma função que corre em paralelo com o jogo ao longo de vários frames.
    IEnumerator AnimateParticles()
    {
        float elapsed = 0f;
        
        // Enquanto o tempo que passou for menor que a duração máxima da faísca...
        while (elapsed < duration)
        {
            // Somar o tempo que o frame demorou a desenhar.
            elapsed += Time.deltaTime;
            
            // Calcular a percentagem (de 0.0 a 1.0)
            float progress = elapsed / duration;
            
            // Enviar a percentagem diretamente para a variável _Progress do Geometry Shader.
            // É este valor que o shader usa para calcular a queda da gravidade (t da física).
            particleMaterial.SetFloat("_Progress", progress);
            
            // Esperar pelo próximo frame antes de continuar o ciclo (yield return null).
            yield return null;
        }

        // GESTÃO DE MEMÓRIA (Muito Importante):
        // Como criámos o Mesh e o Material por código no Init(), o Unity não os apaga sozinho.
        // Se não os destruirmos aqui, o jogo vai acumular lixo na RAM até encravar (Memory Leak).
        Destroy(particleMesh);
        Destroy(particleMaterial);
        
        // Destruir o próprio objeto de jogo (o spawner)
        Destroy(gameObject);
    }
}