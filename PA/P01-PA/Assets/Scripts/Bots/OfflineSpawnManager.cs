using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class OfflineSpawnManager : MonoBehaviour
{
    [SerializeField] Transform spawnPoint; // Ponto onde o jogador local será posicionado no modo offline

    IEnumerator Start()
    {
        // Só executa se estiver no modo offline (PlayerPrefs "OfflineMode" == 1)
        if (PlayerPrefs.GetInt("OfflineMode", 0) != 1)
            yield break;

        GameObject localPlayer = null;

        // Tenta encontrar o jogador local por até 120 frames
        for (int i = 0; i < 120 && localPlayer == null; i++)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
            {
                var netObj = go.GetComponent<NetworkObject>();
                // Verifica se este objeto é do jogador local (owner)
                if (netObj != null && netObj.IsOwner)
                {
                    localPlayer = go;
                    break;
                }
            }
            yield return null; // Espera um frame antes de tentar novamente
        }

        // Reset do modo offline após encontrar o jogador
        PlayerPrefs.SetInt("OfflineMode", 0);

        // Se encontrou o jogador e o spawnPoint está definido, move o jogador para lá
        if (localPlayer != null && spawnPoint != null)
        {
            localPlayer.transform.SetPositionAndRotation(
                spawnPoint.position,
                spawnPoint.rotation
            );
        }
        else
        {
            // Caso não tenha encontrado o jogador ou o spawnPoint
            Debug.LogWarning("OfflineSpawnManager: não encontrou player ou spawnPoint.");
        }
    }
}
