using System.Collections.Generic;
using UnityEngine;

public class DamageFeedUI : MonoBehaviour
{
    public static DamageFeedUI Instance { get; private set; } // Singleton para fácil acesso global

    [Header("Refs")]
    public RectTransform content;          // Painel onde os itens de dano vão ser adicionados
    public DamageFeedItem itemPrefab;      // Prefab do item de dano
    public int maxItems = 6;               // Número máximo de itens ativos no ecrã

    [Header("Estilo")]
    public Color normalColor = Color.white;                     // Cor para dano normal
    public Color critColor = new Color(1f, 0.3f, 0.3f);        // Cor para dano crítico

    readonly Queue<DamageFeedItem> pool = new();   // Pool de itens inativos para reutilização
    readonly List<DamageFeedItem> active = new();  // Lista de itens atualmente ativos no ecrã

    void Awake()
    {
        // Implementa singleton
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
    }

    // Obtém um item da pool ou instancia um novo
    DamageFeedItem GetItem()
    {
        DamageFeedItem it = pool.Count > 0 ? pool.Dequeue() : Instantiate(itemPrefab, content);
        it.gameObject.SetActive(true);               // Ativa o item
        it.transform.SetAsFirstSibling();            // Coloca no topo da lista visual
        return it;
    }

    // Recolhe itens que deixaram de estar ativos e devolve-os à pool
    void RecycleInactive()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            if (!active[i].gameObject.activeSelf)
            {
                pool.Enqueue(active[i]);
                active.RemoveAt(i);
            }
        }
    }

    // Adiciona um novo item de dano à UI
    public void Push(float amount, bool isCrit = false, string targetName = null)
    {
        RecycleInactive(); // Limpa itens inativos antes de adicionar

        // Se já houver muitos itens ativos, remove os mais antigos
        while (active.Count >= maxItems)
        {
            var oldest = active[active.Count - 1];
            oldest.gameObject.SetActive(false);
            pool.Enqueue(oldest);
            active.RemoveAt(active.Count - 1);
        }

        // Cria o texto a mostrar
        string txt = $"-{Mathf.RoundToInt(amount)}";
        if (!string.IsNullOrEmpty(targetName))
            txt += $"  ({targetName})";

        // Determina a cor (crítico ou normal)
        var color = isCrit ? critColor : normalColor;

        // Obtém um item da pool e inicializa
        var item = GetItem();
        item.Init(txt, color);

        // Adiciona à lista de ativos no topo
        active.Insert(0, item); 
    }
}
