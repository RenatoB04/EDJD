using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq; 

public class RoundTimer : NetworkBehaviour
{
    [Header("Configuração")]
    public float roundDuration = 300f; // Duração da ronda em segundos (5 min por defeito)
    public bool startOnSpawn = true;   // Se true, inicia a ronda automaticamente ao spawn

    [Header("UI Timer")]
    [SerializeField] private TMP_Text timerText; // Referência ao texto do temporizador na UI

    // NetworkVariable que guarda o tempo restante da ronda
    private NetworkVariable<float> timeRemaining = new NetworkVariable<float>(
        300f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // NetworkVariable que indica se a ronda está ativa
    private NetworkVariable<bool> isRoundActive = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Chamado quando o objeto de rede é iniciado
    public override void OnNetworkSpawn()
    {
        // Subscrição do evento quando o tempo muda
        timeRemaining.OnValueChanged += OnTimeChanged;

        // Se for servidor e startOnSpawn estiver true, inicia a ronda
        if (IsServer && startOnSpawn) StartRound();
    }

    // Remove subscrição ao destruir
    public override void OnNetworkDespawn()
    {
        timeRemaining.OnValueChanged -= OnTimeChanged;
    }

    // Atualização do temporizador (apenas no servidor)
    void Update()
    {
        if (IsServer && isRoundActive.Value)
        {
            float newVal = timeRemaining.Value - Time.deltaTime;

            if (newVal <= 0f)
            {
                newVal = 0f;
                EndRound(); // Quando o tempo chega a zero, termina a ronda
            }

            timeRemaining.Value = newVal; // Atualiza NetworkVariable
        }
    }

    // Inicia a ronda (apenas servidor)
    public void StartRound()
    {
        if (!IsServer) return;

        timeRemaining.Value = roundDuration;
        isRoundActive.Value = true;
    }

    // Termina a ronda e determina vencedor
    public void EndRound()
    {
        if (!IsServer) return;

        isRoundActive.Value = false;

        string winnerName = "Ninguém";
        int highScore = -1;

        // Procura todos os PlayerScore na cena
        PlayerScore[] allScores = FindObjectsOfType<PlayerScore>();
        if (allScores.Length > 0)
        {
            // Ordena descendentemente e seleciona o melhor jogador
            var bestPlayer = allScores.OrderByDescending(p => p.Score.Value).First();
            if (bestPlayer != null)
            {
                highScore = bestPlayer.Score.Value;

                // Tenta obter o nome do jogador
                var nameScript = bestPlayer.GetComponent<PlayerName>();
                if (nameScript != null)
                    winnerName = nameScript.Name;
                else
                    winnerName = $"Player {bestPlayer.OwnerClientId}";
            }
        }

        // Chama ClientRpc para todos os clientes notificarem fim da ronda
        RoundEndedClientRpc(winnerName, highScore);
    }

    // Chamado quando o tempo muda para atualizar a UI
    private void OnTimeChanged(float prev, float curr) => UpdateTimerUI(curr);

    // ClientRpc para indicar que a ronda terminou
    [ClientRpc]
    private void RoundEndedClientRpc(string winner, int score)
    {
        UpdateTimerUI(0f); // Força a UI a mostrar 00:00
        LockPlayerInputs(); // Bloqueia entradas do jogador

        // Mostra UI de Game Over
        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowGameOver("FIM DA RONDA", winner, score);
        }
    }

    // Atualiza o temporizador na UI
    private void UpdateTimerUI(float seconds)
    {
        if (!timerText) return;

        int s = Mathf.CeilToInt(seconds);
        int mm = s / 60;
        int ss = s % 60;

        timerText.text = $"{mm:00}:{ss:00}";
        timerText.color = seconds <= 10f ? Color.red : Color.white; // Pisca vermelho nos últimos 10s
    }

    // Bloqueia inputs do jogador e mostra cursor
    private void LockPlayerInputs()
    {
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.LocalClient == null || NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            GameplayCursor.Unlock(); // Liberta cursor se player não existe
            return;
        }

        var player = NetworkManager.Singleton.LocalClient.PlayerObject;

        // Desativa o sistema de input
        var inputSystem = player.GetComponent<PlayerInput>();
        if (inputSystem != null) { inputSystem.DeactivateInput(); inputSystem.enabled = false; }

        // Desativa movimento
        var move = player.GetComponent<FP_Controller_IS>();
        if (move) move.enabled = false;

        // Desativa arma
        var weapon = player.GetComponentInChildren<Weapon>();
        if (weapon) weapon.enabled = false;

        // Liberta e mostra cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
