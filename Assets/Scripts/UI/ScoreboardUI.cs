using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ScoreboardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;           // Painel do scoreboard
    [SerializeField] private TMP_Text listText;          // Texto onde a lista será mostrada

    [Header("Input")]
    [Tooltip("Ação para mostrar o scoreboard (ex.: Tab).")]
    [SerializeField] private InputActionReference showScoreboardAction;  // Ação de input para abrir/fechar

    [Header("Opções")]
    [SerializeField] private float refreshRate = 10f;    // Quantas vezes por segundo atualizar
    float nextRefreshTime;

    void OnEnable()
    {
        // Ativa a ação de input caso exista
        if (showScoreboardAction && !showScoreboardAction.action.enabled)
            showScoreboardAction.action.Enable();

        if (panel) panel.SetActive(false);   // Esconde painel inicialmente
        nextRefreshTime = 0f;
    }

    void OnDisable()
    {
        // Desativa a ação de input
        if (showScoreboardAction && showScoreboardAction.action.enabled)
            showScoreboardAction.action.Disable();
    }

    void Update()
    {
        bool wantShow = showScoreboardAction != null && showScoreboardAction.action.IsPressed();

        // Mostra ou esconde o painel conforme input
        if (panel && panel.activeSelf != wantShow)
        {
            panel.SetActive(wantShow);
            if (wantShow) RefreshNow();  // Atualiza imediatamente ao abrir
        }

        // Atualização periódica enquanto estiver aberto
        if (wantShow && Time.unscaledTime >= nextRefreshTime)
        {
            RefreshNow();
            nextRefreshTime = Time.unscaledTime + (refreshRate > 0f ? 1f / refreshRate : 0.2f);
        }
    }

    void RefreshNow()
    {
        if (!listText) return;

        var scores = FindObjectsOfType<PlayerScore>();
        if (scores == null || scores.Length == 0)
        {
            listText.text = "À espera de jogadores...";
            return;
        }

        // Cria lista de jogadores com nome, kills e score
        var sorted = new List<(string name, int kills, int score)>(scores.Length);
        foreach (var ps in scores)
        {
            if (ps == null) continue;
            string pname = GetCorrectPlayerName(ps.gameObject);
            int kills = ps.Kills.Value;
            int score = ps.Score.Value;
            sorted.Add((pname, kills, score));
        }

        // Ordena por score descendente, depois por kills descendente
        var ordered = sorted
            .OrderByDescending(e => e.score)
            .ThenByDescending(e => e.kills)
            .ToList();

        // Constroi o texto da UI
        var sb = new StringBuilder();
        sb.AppendLine("JOGADOR               Kills   Score");
        sb.AppendLine("-----------------------------------");
        foreach (var e in ordered)
        {
            sb.AppendLine($"{e.name,-20}  {e.kills,5}   {e.score,5}");
        }

        listText.text = sb.ToString();
    }

    string GetCorrectPlayerName(GameObject playerObj)
    {
        // Tenta obter o nome através do script PlayerName
        var nameScript = playerObj.GetComponent<PlayerName>();
        if (nameScript != null)
        {
            return nameScript.Name;
        }

        // Se for bot, usa o nome do objeto
        if (playerObj.name.StartsWith("Bot"))
        {
            return playerObj.name;
        }

        // Se for jogador de rede, mostra ClientId
        var netObj = playerObj.GetComponent<NetworkObject>();
        if (netObj != null) 
        {
            return $"Player {netObj.OwnerClientId}";
        }

        return "Desconhecido";
    }
}
