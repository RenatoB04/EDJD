using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class BotTeamSetter : NetworkBehaviour
{
    private Health _health;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _health = GetComponent<Health>();
        if (_health != null)
        {
            _health.team.Value = -2; // bots = inimigos
            Debug.Log($"[Team] Bot {OwnerClientId} set to team -2");
        }
    }
}