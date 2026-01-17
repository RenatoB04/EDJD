using UnityEngine;

[DisallowMultipleComponent] // Garante que não haja múltiplos deste script no mesmo GameObject
public class TeamOutlineController : MonoBehaviour
{
    [Header("Renderers-alvo (se vazio, busca nos filhos)")]
    public Renderer[] renderers; // Lista de renderers que terão cores/outlines aplicados

    [Header("Cores de base")]
    public Color teamAColor = Color.blue;           // Cor para a equipa A
    public Color teamBColor = new Color(1f, 0.5f, 0f); // Cor para a equipa B
    public Color botColor  = Color.red;            // Cor para bots
    public Color neutralColor = Color.gray;        // Cor para neutros (ou indefinidos)

    [Header("Outline")]
    public Color enemyOutlineColor = Color.red;     // Cor do contorno para inimigos
    public Color allyOutlineColor  = Color.cyan;   // Cor do contorno para aliados
    [Range(0f, 0.05f)] public float enemyOutlineWidth = 0.02f; // Largura do contorno inimigo
    [Range(0f, 0.05f)] public float allyOutlineWidth  = 0.0f;  // Largura do contorno aliado

    private Health _health; // Referência ao componente Health
    private int _lastTeam = 999; // Guarda o último valor de equipa para não atualizar sempre

    void Awake()
    {
        _health = GetComponentInParent<Health>(); // Procura Health no próprio GameObject ou nos pais
        // Se não houver renderers definidos, busca todos nos filhos
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);
    }

    void Update()
    {
        int team = _health != null ? _health.team.Value : -3; // Pega a equipa do Health
        if (team != _lastTeam)
        {
            ApplyVisuals(team); // Atualiza cores e outline quando a equipa muda
            _lastTeam = team;
        }
    }

    void ApplyVisuals(int targetTeam)
    {
        int localTeam = GameInfo.MyChosenTeam; // Equipa do jogador local
        bool isBot = (targetTeam == -2);        // Bots têm a equipa -2
        bool isEnemy = isBot || (targetTeam != -1 && targetTeam != localTeam); // Determina inimigo
        Color baseColor = neutralColor;

        if (isBot) baseColor = botColor;
        else if (targetTeam == 0) baseColor = teamAColor;
        else if (targetTeam == 1) baseColor = teamBColor;

        Color outlineColor = isEnemy ? enemyOutlineColor : allyOutlineColor;
        float outlineWidth = isEnemy ? enemyOutlineWidth : allyOutlineWidth;

        var mpb = new MaterialPropertyBlock();
        foreach (var r in renderers)
        {
            if (r == null) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_BaseColor", baseColor);
            mpb.SetColor("_OutlineColor", outlineColor);
            mpb.SetFloat("_OutlineWidth", outlineWidth);
            r.SetPropertyBlock(mpb);
        }
    }
}
