using Unity.Netcode;
using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(PlayerDeathAndRespawn))]
public class NetworkSpawnHandler : NetworkBehaviour
{
    private PlayerDeathAndRespawn respawnController;

    void Awake()
    {
        // Obtém o componente PlayerDeathAndRespawn anexado ao jogador
        respawnController = GetComponent<PlayerDeathAndRespawn>();
        if (respawnController == null)
        {
            Debug.LogError("NetworkSpawnHandler: Falha ao encontrar PlayerDeathAndRespawn. Verifique o Prefab.");
        }
    }

    public override void OnNetworkSpawn() 
    {
        base.OnNetworkSpawn();

        // Apenas o proprietário do objecto (player local) executa o respawn inicial
        if (IsOwner && respawnController != null)
        {
            StartCoroutine(SafeRespawnCoroutine());
        }
    }

    private IEnumerator SafeRespawnCoroutine()
    {
        // Espera um frame para garantir que tudo está inicializado
        yield return null; 

        if (IsSpawned && respawnController != null)
        {
            Debug.Log("[SpawnHandler] A chamar RespawnServerRpc(ignoreAliveCheck: true) para spawn inicial...");
            
            // Chama o RPC do servidor para forçar o spawn inicial do jogador, ignorando checks de vida
            respawnController.RespawnServerRpc(true);
        }
    }
}
