using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))] // Garante que o GameObject tem um CanvasGroup para controlar a transparência
public class DamageFeedItem : MonoBehaviour
{
    public TextMeshProUGUI label; // Texto que vai mostrar o valor do dano
    public float life = 1.4f;     // Tempo total de vida do item no ecrã (em segundos)
    public float fade = 0.4f;     // Tempo de transição de fade out antes de desaparecer

    CanvasGroup cg; // Referência ao CanvasGroup para controlar alpha
    float t;        // Temporizador interno para acompanhar a vida do item

    void Awake()
    {
        cg = GetComponent<CanvasGroup>(); // Obtém o CanvasGroup do GameObject
        if (!label) label = GetComponent<TextMeshProUGUI>(); // Se não estiver definido, tenta obter o TMP do próprio GameObject
    }

    // Inicializa o texto e cor do dano
    public void Init(string text, Color color)
    {
        if (!label) label = GetComponent<TextMeshProUGUI>(); // Certifica que o label está definido
        label.text = text;    // Define o texto a mostrar
        label.color = color;  // Define a cor do texto
        cg.alpha = 1f;        // Define a transparência como totalmente visível
        t = 0f;               // Reinicia o temporizador
    }

    void Update()
    {
        t += Time.unscaledDeltaTime; // Atualiza o temporizador, usando tempo não afetado por Time.timeScale

        // Inicia fade out quando estiver perto do final da vida
        if (t > life - fade)
        {
            float a = Mathf.InverseLerp(life, life - fade, t); // Calcula alpha normalizado para fade
            cg.alpha = Mathf.Clamp01(1f - a);                 // Atualiza a transparência
        }

        // Desativa o GameObject quando o tempo de vida terminar
        if (t >= life)
            gameObject.SetActive(false);
    }
}
