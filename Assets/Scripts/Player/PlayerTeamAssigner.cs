using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerTeamAssigner : NetworkBehaviour
{
    private Health _health;

    public override void OnNetworkSpawn()
    {
        _health = GetComponent<Health>();

        if (IsOwner)
        {
            // Dono informa o servidor qual equipa escolheu
            int chosen = GameInfo.MyChosenTeam;
            RequestTeamServerRpc(chosen);
        }
    }

    [ServerRpc]
    private void RequestTeamServerRpc(int chosenTeam)
    {
        chosenTeam = Mathf.Clamp(chosenTeam, 0, 1);
        if (_health != null)
        {
            _health.team.Value = chosenTeam;
            Debug.Log($"[Team] Player {OwnerClientId} set to team {chosenTeam}");
        }
    }
}