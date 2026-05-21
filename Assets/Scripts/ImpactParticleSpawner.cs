using UnityEngine;
using System.Collections;

public class ImpactParticleSpawner : MonoBehaviour
{
    private Material particleMaterial;
    private Mesh particleMesh;

    [Header("Particle Settings")]
    [Tooltip("The Geometry Shader used to generate and animate sparks on the GPU.")]
    public Shader particleShader;
    
    [Tooltip("Color of the impact sparks.")]
    [ColorUsage(true, true)] // Enables HDR color picker
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

    public void Init(Vector3 hitPoint, Vector3 normal)
    {
        // Place the spawner object at the hit location
        transform.position = hitPoint;

        // Add MeshFilter and MeshRenderer dynamically
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();

        // Set up the Material with our custom Geometry Shader
        if (particleShader == null)
            particleShader = Shader.Find("Custom/ImpactParticles");

        particleMaterial = new Material(particleShader);
        particleMaterial.SetColor("_MainColor", particleColor);
        particleMaterial.SetFloat("_Size", sparkSize);
        particleMaterial.SetFloat("_Speed", speed);
        particleMaterial.SetFloat("_GravityScale", gravityScale);
        renderer.material = particleMaterial;

        // Generate the point mesh
        particleMesh = new Mesh();
        Vector3[] vertices = new Vector3[particleCount];
        Vector3[] normals = new Vector3[particleCount];
        int[] indices = new int[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            vertices[i] = Vector3.zero; // Start at the origin (hit point local space)

            // Distribute vectors in a hemisphere facing away from the impact normal
            Vector3 randomDir = Random.onUnitSphere;
            if (Vector3.Dot(randomDir, normal) < 0)
            {
                randomDir = -randomDir; // Flip direction to match the impact surface normal
            }

            // Slerp to blend normal direction with spread noise
            normals[i] = Vector3.Slerp(normal, randomDir, Random.Range(0.3f, 0.9f)).normalized * Random.Range(0.6f, 1.4f);
            indices[i] = i;
        }

        particleMesh.vertices = vertices;
        particleMesh.normals = normals;
        
        // MeshTopology.Points so the Geometry Shader gets each vertex as a Point input
        particleMesh.SetIndices(indices, MeshTopology.Points, 0);
        filter.mesh = particleMesh;

        // Start animating
        StartCoroutine(AnimateParticles());
    }

    IEnumerator AnimateParticles()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            particleMaterial.SetFloat("_Progress", progress);
            yield return null;
        }

        // Clean up resources to prevent memory leaks
        Destroy(particleMesh);
        Destroy(particleMaterial);
        Destroy(gameObject);
    }
}
