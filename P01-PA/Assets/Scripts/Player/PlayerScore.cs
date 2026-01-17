using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : NetworkBehaviour
{
    [Header("Config")]
    public int pointsPerKill = 100; // Pontos ganhos por cada kill

    [Header("Replicado")]
    // Número de kills, replicado a todos os clientes mas só o servidor escreve
    public NetworkVariable<int> Kills = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Pontuação total, replicada a todos os clientes mas só o servidor escreve
    public NetworkVariable<int> Score = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Eventos Unity que podem ser ligados no inspector para UI ou outros sistemas
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<int> OnKillsChanged;

    public override void OnNetworkSpawn()
    {
        // Subscrição aos eventos de alteração de valor
        Kills.OnValueChanged += HandleKillsChanged;
        Score.OnValueChanged += HandleScoreChanged;
    }

    public override void OnNetworkDespawn()
    {
        // Remover subscrições para evitar memory leaks
        Kills.OnValueChanged -= HandleKillsChanged;
        Score.OnValueChanged -= HandleScoreChanged;
    }

    void HandleKillsChanged(int prev, int curr) => OnKillsChanged?.Invoke(curr); // Invoca evento de UI ou outros sistemas
    void HandleScoreChanged(int prev, int curr) => OnScoreChanged?.Invoke(curr); // Invoca evento de UI ou outros sistemas

    // Método chamado pelo servidor quando um jogador faz kill
    public void AwardKillAndPoints()
    {
        if (!IsServer) return; // Apenas o servidor altera os valores

        Kills.Value += 1; // Incrementa kills
        Score.Value += pointsPerKill; // Incrementa pontuação
    }
}
