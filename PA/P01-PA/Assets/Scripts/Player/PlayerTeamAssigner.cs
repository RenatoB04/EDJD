using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerTeamAssigner : NetworkBehaviour
{
    private Health _health;

    public override void OnNetworkSpawn()
    {
        // Obtém referência ao componente Health
        _health = GetComponent<Health>();

        // Apenas o jogador local solicita a atribuição de equipa
        if (IsOwner)
        {
            int chosen = GameInfo.MyChosenTeam; // Equipa escolhida pelo jogador
            RequestTeamServerRpc(chosen);       // Envia pedido ao servidor
        }
    }

    [ServerRpc]
    private void RequestTeamServerRpc(int chosenTeam)
    {
        // Garante que o valor da equipa é 0 ou 1
        chosenTeam = Mathf.Clamp(chosenTeam, 0, 1);

        if (_health != null)
        {
            _health.team.Value = chosenTeam; // Define a equipa no servidor
            Debug.Log($"[Team] Player {OwnerClientId} set to team {chosenTeam}");
        }
    }
}
