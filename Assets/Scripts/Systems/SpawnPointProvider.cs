using System;
using UnityEngine;

public class SpawnPointsProvider : MonoBehaviour
{
    public static SpawnPointsProvider Instance { get; private set; } // Singleton para acesso global

    [Header("Arrasta aqui os spawn points da cena")]
    [SerializeField] private Transform spawnA; // Transform do primeiro spawn
    [SerializeField] private Transform spawnB; // Transform do segundo spawn

    [Header("Opcional: auto-descoberta por Tag (se não arrastares)")]
    [SerializeField] private bool autoDiscoverByTag = true; // Permite descobrir spawn points automaticamente
    [SerializeField] private string spawnPointTag = "SpawnPoint"; // Tag usada para descobrir spawn points

    private void Awake()
    {
        // Configura singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Auto-descobre spawn points se não estiverem definidos e autoDiscover ativo
        if (autoDiscoverByTag && (spawnA == null || spawnB == null) && !string.IsNullOrEmpty(spawnPointTag))
        {
            var objs = GameObject.FindGameObjectsWithTag(spawnPointTag);
            if (objs != null && objs.Length > 0)
            {
                // Ordena os objetos por nome para consistência
                Array.Sort(objs, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
                if (objs.Length >= 1 && spawnA == null) spawnA = objs[0].transform;
                if (objs.Length >= 2 && spawnB == null) spawnB = objs[1].transform;
            }
        }
    }

    // Tenta obter a posição e rotação do spawn A
    public bool TryGetSpawnA(out Vector3 pos, out Quaternion rot)
    {
        if (spawnA != null)
        {
            pos = spawnA.position;
            rot = spawnA.rotation;
            return true;
        }
        pos = default;
        rot = Quaternion.identity;
        return false;
    }

    // Tenta obter a posição e rotação do spawn B
    public bool TryGetSpawnB(out Vector3 pos, out Quaternion rot)
    {
        if (spawnB != null)
        {
            pos = spawnB.position;
            rot = spawnB.rotation;
            return true;
        }
        pos = default;
        rot = Quaternion.identity;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // Desenha gizmos para facilitar a visualização dos spawn points na cena
        Gizmos.color = Color.green;
        if (spawnA) Gizmos.DrawWireSphere(spawnA.position, 0.5f);

        Gizmos.color = Color.cyan;
        if (spawnB) Gizmos.DrawWireSphere(spawnB.position, 0.5f);
    }
}
