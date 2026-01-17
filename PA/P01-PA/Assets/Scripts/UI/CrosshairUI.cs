using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public static CrosshairUI Instance { get; private set; } // Instância singleton do CrosshairUI

    [Header("Refs")]
    public RectTransform dot;      // Ponto central da mira
    public RectTransform top;      // Barra superior
    public RectTransform bottom;   // Barra inferior
    public RectTransform left;     // Barra esquerda
    public RectTransform right;    // Barra direita

    [Header("Look")]
    public bool useDotOnly = false; // Usar apenas o ponto central, sem barras
    public float thickness = 3f;    // Espessura das barras/ponto
    public float length = 12f;      // Comprimento das barras
    public float baseGap = 8f;      // Distância base entre centro e barras

    [Header("Dynamics")]
    public float kickPerShot = 8f;     // Quanto a mira "recuará" por cada disparo
    public float maxKick = 30f;        // Máximo recuo da mira
    public float relaxSpeed = 30f;     // Velocidade de relaxamento da mira
    public float moveKick = 6f;        // Kick extra quando se move (não implementado neste snippet)
    public float aimDownSightsScale = 0.7f; // Escala da mira ao mirar

    [Header("Hitmarker")]
    public Image dotImage;             // Imagem do ponto central
    public Color hitColor = Color.green; // Cor ao acertar
    public float hitFlashTime = 0.07f;   // Duração do flash ao acertar

    float currentKick = 0f;           // Recuo atual da mira
    float hitTimer = 0f;              // Timer do flash do hitmarker
    Color dotBaseColor;                // Cor base do ponto central

    void Awake()
    {
        // Inicializa singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (dotImage == null && dot != null) dotImage = dot.GetComponent<Image>();
        if (dotImage != null) dotBaseColor = dotImage.color;

        ApplyGeometry(); // Aplica geometria inicial da mira
    }

    void Update()
    {
        // Relaxa o kick ao longo do tempo
        if (currentKick > 0f)
        {
            currentKick = Mathf.Max(0f, currentKick - relaxSpeed * Time.unscaledDeltaTime);
            ApplyGeometry();
        }

        // Gerir temporizador do hitmarker
        if (hitTimer > 0f)
        {
            hitTimer -= Time.unscaledDeltaTime;
            if (hitTimer <= 0f && dotImage != null) dotImage.color = dotBaseColor;
        }
    }

    // Atualiza posições e dimensões da mira
    void ApplyGeometry()
    {
        float gap = baseGap + currentKick;

        if (dot != null)
        {
            dot.sizeDelta = new Vector2(thickness, thickness);
            dot.anchoredPosition = Vector2.zero;
        }

        if (useDotOnly) { ToggleBars(false); return; }

        ToggleBars(true);

        if (top != null)
        {
            top.sizeDelta = new Vector2(thickness, length);
            top.anchoredPosition = new Vector2(0f, gap + length * 0.5f);
        }
        if (bottom != null)
        {
            bottom.sizeDelta = new Vector2(thickness, length);
            bottom.anchoredPosition = new Vector2(0f, -(gap + length * 0.5f));
        }
        if (left != null)
        {
            left.sizeDelta = new Vector2(length, thickness);
            left.anchoredPosition = new Vector2(-(gap + length * 0.5f), 0f);
        }
        if (right != null)
        {
            right.sizeDelta = new Vector2(length, thickness);
            right.anchoredPosition = new Vector2(gap + length * 0.5f, 0f);
        }
    }

    // Ativa ou desativa as barras da mira
    void ToggleBars(bool on)
    {
        if (top) top.gameObject.SetActive(on);
        if (bottom) bottom.gameObject.SetActive(on);
        if (left) left.gameObject.SetActive(on);
        if (right) right.gameObject.SetActive(on);
    }

    // Aplica "kick" quando se dispara
    public void Kick(float amount = -1f)
    {
        float add = (amount > 0f) ? amount : kickPerShot;
        currentKick = Mathf.Min(maxKick, currentKick + add);
        ApplyGeometry();
    }

    // Define escala da mira ao mirar (ADS)
    public void SetADS(bool adsOn)
    {
        float scale = adsOn ? aimDownSightsScale : 1f;
        transform.localScale = Vector3.one * scale;
    }

    // Mostra hitmarker
    public void ShowHit()
    {
        if (dotImage == null) return;
        dotImage.color = hitColor;
        hitTimer = hitFlashTime;
    }
}
