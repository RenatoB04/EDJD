using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [Header("Configuração Visual")]
    public float maxRadius = 8f;       // Raio máximo do efeito
    public float expansionSpeed = 15f; // Velocidade de expansão da esfera
    public float fadeSpeed = 2f;       // Velocidade de fade (não usado neste código, mas definido)
    
    private float currentRadius = 0.1f; // Raio inicial
    private Material mat;                // Material do renderer
    private Color baseColor;             // Cor original do material

    void Start()
    {
        // Obtém o material do Renderer do GameObject
        var renderer = GetComponent<Renderer>();
        if (renderer)
        {
            mat = renderer.material; // Cria instância do material para manipulação
            baseColor = mat.color;   // Guarda a cor original
        }
    }

    void Update()
    {
        // Expande o raio com o tempo
        currentRadius += expansionSpeed * Time.deltaTime;

        // Escala o objeto com base no raio (diâmetro)
        transform.localScale = Vector3.one * currentRadius * 2f;

        // Destroi o objeto quando atinge o raio máximo
        if (currentRadius >= maxRadius)
        {
            Destroy(gameObject);
        }

        // Faz fade da cor do material com base no raio
        if (mat)
        {
            float alpha = Mathf.Clamp01(1f - (currentRadius / maxRadius));
            Color c = baseColor;
            c.a = alpha; // Aplica transparência proporcional ao raio
            mat.color = c;
        }
    }
}
