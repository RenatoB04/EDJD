using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Health))] // Garante que o GameObject tem um componente Health
public class BotTeamSetter : NetworkBehaviour
{
    private Health _health; // Referência ao componente Health do bot

    // Chamado quando o objecto é spawnado na rede
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // Apenas o servidor define a equipa do bot

        _health = GetComponent<Health>(); // Obtém o componente Health

        if (_health != null)
        {
            _health.team.Value = -2; // Define o bot como pertencente à equipa "inimiga/neutra" (-2)
            Debug.Log($"[Team] Bot {OwnerClientId} set to team -2"); // Log para debug
        }
    }
}
