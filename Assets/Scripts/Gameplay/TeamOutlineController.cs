using UnityEngine;
[DisallowMultipleComponent]
public class TeamOutlineController : MonoBehaviour
{
    [Header("Renderers-alvo (se vazio, busca nos filhos)")]
    public Renderer[] renderers;
    [Header("Cores de base")]
    public Color teamAColor = Color.blue;
    public Color teamBColor = new Color(1f, 0.5f, 0f); 
    public Color botColor  = Color.red;
    public Color neutralColor = Color.gray;
    [Header("Outline")]
    public Color enemyOutlineColor = Color.red;
    public Color allyOutlineColor  = Color.cyan;
    [Range(0f, 0.05f)] public float enemyOutlineWidth = 0.02f;
    [Range(0f, 0.05f)] public float allyOutlineWidth  = 0.0f; 
    private Health _health;
    private int _lastTeam = 999;
    void Awake()
    {
        _health = GetComponentInParent<Health>();
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);
    }
    void Update()
    {
        int team = _health != null ? _health.team.Value : -3;
        if (team != _lastTeam)
        {
            ApplyVisuals(team);
            _lastTeam = team;
        }
    }
    void ApplyVisuals(int targetTeam)
    {
        int localTeam = GameInfo.MyChosenTeam; 
        bool isBot = (targetTeam == -2);
        bool isEnemy = isBot || (targetTeam != -1 && targetTeam != localTeam);
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