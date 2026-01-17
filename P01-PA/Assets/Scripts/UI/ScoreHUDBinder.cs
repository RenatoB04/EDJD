using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ScoreHUDBinder : NetworkBehaviour
{
    [Header("Refs (arrasta do Canvas)")]
    public TextMeshProUGUI scoreText;   // Texto para mostrar a pontuação
    public TextMeshProUGUI killsText;   // Texto para mostrar o número de kills

    private PlayerScore ps;              // Referência ao script PlayerScore do jogador

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) 
        { 
            enabled = false; 
            return; 
        }

        ps = GetComponentInParent<PlayerScore>();
        if (ps == null)
        {
            Debug.LogError("ScoreHUDBinder: PlayerScore não encontrado no Player.");
            enabled = false;
            return;
        }

        RefreshAll();   // Atualiza UI inicialmente
        ps.Score.OnValueChanged += OnScoreChanged;  // Regista callback para score
        ps.Kills.OnValueChanged += OnKillsChanged;  // Regista callback para kills
    }

    public override void OnNetworkDespawn()
    {
        // Remove callbacks para evitar memory leaks
        if (ps != null)
        {
            ps.Score.OnValueChanged -= OnScoreChanged;
            ps.Kills.OnValueChanged -= OnKillsChanged;
        }
    }

    private void OnScoreChanged(int prev, int curr)
    {
        if (scoreText) scoreText.text = "Score: " + curr;  // Atualiza UI quando muda score
    }

    private void OnKillsChanged(int prev, int curr)
    {
        if (killsText) killsText.text = "Kills: " + curr;  // Atualiza UI quando muda kills
    }

    private void RefreshAll()
    {
        // Atualiza ambos os textos de uma vez
        if (scoreText) scoreText.text = "Score: " + (ps != null ? ps.Score.Value : 0);
        if (killsText) killsText.text = "Kills: " + (ps != null ? ps.Kills.Value : 0);
    }
}
