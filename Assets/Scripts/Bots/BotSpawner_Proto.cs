using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class BotSpawner_Proto : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("O Prefab do Bot (TEM de estar na lista NetworkPrefabs do NetworkManager!).")]
    public GameObject botPrefab; // Prefab do bot que será instanciado
    [Tooltip("Pontos onde os bots podem nascer.")]
    public Transform[] spawnPoints; // Lista de spawn points possíveis
    [Tooltip("Caminho de patrulha para os bots.")]
    public Transform[] patrolWaypoints; // Waypoints de patrulha atribuídos aos bots

    [Header("Regras da Horda")]
    public int initialBotCount = 2; // Número inicial de bots a spawnar
    public int maxAliveBots = 5;    // Número máximo de bots vivos em simultâneo
    public float respawnDelay = 3f; // Tempo de atraso antes de respawnar um bot

    [Header("Multiplayer")]
    [Tooltip("Se false, os bots só aparecem no modo Offline. Se true, aparecem também no Multiplayer.")]
    public bool enableInMultiplayer = true; // Controla se os bots aparecem no multiplayer

    [Header("Debug")]
    public bool forceSpawnInEditor = true; // Forçar spawn no editor (mesmo sem multiplayer)

    private int currentAliveBots = 0; // Contador de bots vivos
    private bool isSpawningActive = false; // Flag que indica se o spawner está ativo

    void Awake()
    {
        // Aqui poderíamos adicionar lógica inicial dependendo do modo Offline/Online
        if (!forceSpawnInEditor && PlayerPrefs.GetInt("OfflineMode", 0) != 1)
        {
        }
    }

    void Start()
    {
        // Espera até que o NetworkManager esteja pronto e que sejamos o servidor
        StartCoroutine(WaitForServer());
    }

    IEnumerator WaitForServer()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening);

        if (NetworkManager.Singleton.IsServer)
        {
            bool isOfflineMode = PlayerPrefs.GetInt("OfflineMode", 0) == 1;

            // Se estamos online e os bots não estão permitidos, desativa o spawner
            if (!isOfflineMode && !enableInMultiplayer && !forceSpawnInEditor)
            {
                Debug.Log("[BotSpawner] Desativado (Modo Online e enableInMultiplayer=false).");
                enabled = false;
                yield break;
            }

            Debug.Log("[BotSpawner] SOU O HOST. A iniciar ronda de bots...");
            isSpawningActive = true;

            // Associa o evento global de morte de bots para gerir respawns
            BOTDeath.OnAnyBotKilled += HandleBotDeath;

            // Spawn inicial de bots
            for (int i = 0; i < initialBotCount; i++)
            {
                SpawnBot();
                yield return new WaitForSeconds(0.5f); // Pequeno atraso entre spawns
            }
        }
        else
        {
            // Não somos servidor, spawner não ativo
            enabled = false;
        }
    }

    void OnDestroy()
    {
        // Remove listener para evitar referências a objetos destruídos
        BOTDeath.OnAnyBotKilled -= HandleBotDeath;
    }

    // Evento chamado quando um bot morre
    void HandleBotDeath()
    {
        if (!isSpawningActive) return;

        currentAliveBots--;
        if (currentAliveBots < 0) currentAliveBots = 0;

        // Inicia rotina de respawn
        StartCoroutine(SpawnRoutine(2));
    }

    // Rotina que respawna um número de bots com atraso
    IEnumerator SpawnRoutine(int amount)
    {
        yield return new WaitForSeconds(respawnDelay);

        for (int i = 0; i < amount; i++)
        {
            if (currentAliveBots < maxAliveBots)
            {
                SpawnBot();
                yield return new WaitForSeconds(1f); // Delay entre respawns individuais
            }
        }
    }

    // Função principal de spawn de um bot
    void SpawnBot()
    {
        if (botPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;
        if (!NetworkManager.Singleton.IsServer) return; // Apenas o servidor pode spawnar

        // Escolhe um spawn point aleatório
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instancia o prefab do bot na posição escolhida
        GameObject bot = Instantiate(botPrefab, sp.position, sp.rotation);

        // Atribui os waypoints de patrulha
        var ai = bot.GetComponent<BotAI_Proto>();
        if (ai != null) ai.patrolPoints = patrolWaypoints;

        // Spawna o NetworkObject para sincronização multiplayer
        var netObj = bot.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn(true);
            currentAliveBots++;
        }
        else
        {
            Debug.LogError("[BotSpawner] O Bot Prefab não tem NetworkObject!");
            Destroy(bot);
        }
    }

    // Método público chamado pelos bots para agendar respawn
    public void ScheduleRespawn(Transform[] t) { }
}
