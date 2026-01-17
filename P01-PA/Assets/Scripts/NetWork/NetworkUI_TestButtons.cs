using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI_TestButtons : MonoBehaviour
{
    [Header("Botões")]
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    [Header("Prefab do Jogador")]
    [Tooltip("Arrasta o teu Prefab do Jogador (com NetworkObject no root).")]
    public GameObject playerPrefabToSpawn;

    void Start()
    {
        // Configura os listeners dos botões para iniciar Host, Client ou Server
        if (hostButton)
        {
            hostButton.onClick.RemoveListener(StartHost);
            hostButton.onClick.AddListener(StartHost);
        }
        if (clientButton)
        {
            clientButton.onClick.RemoveListener(StartClient);
            clientButton.onClick.AddListener(StartClient);
        }
        if (serverButton)
        {
            serverButton.onClick.RemoveListener(StartServer);
            serverButton.onClick.AddListener(StartServer);
        }

        // Adiciona callback para spawn de jogador quando um cliente se conecta
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            Debug.LogError("NetworkManager.Singleton é nulo na cena.");
        }
    }

    private void StartHost()
    {
        if (NetworkManager.Singleton == null) { Debug.LogError("Sem NetworkManager."); return; }
        Debug.Log("Starting Host...");
        bool ok = NetworkManager.Singleton.StartHost();
        if (ok) HideButtons(); // Esconde botões após iniciar
        else Debug.LogError("Falha ao iniciar Host (porta em uso?).");
    }

    private void StartClient()
    {
        if (NetworkManager.Singleton == null) { Debug.LogError("Sem NetworkManager."); return; }
        Debug.Log("Starting Client...");
        bool ok = NetworkManager.Singleton.StartClient();
        if (ok) HideButtons();
        else Debug.LogError("Falha ao iniciar Client.");
    }

    private void StartServer()
    {
        if (NetworkManager.Singleton == null) { Debug.LogError("Sem NetworkManager."); return; }
        Debug.Log("Starting Server...");
        bool ok = NetworkManager.Singleton.StartServer();
        if (ok) HideButtons();
        else Debug.LogError("Falha ao iniciar Server.");
    }

    private void HideButtons()
    {
        // Esconde todos os botões para evitar múltiplos clicks
        if (hostButton) hostButton.gameObject.SetActive(false);
        if (clientButton) clientButton.gameObject.SetActive(false);
        if (serverButton) serverButton.gameObject.SetActive(false);
    }

    private void OnClientConnected(ulong clientId)
    {
        // Apenas o servidor executa spawn de jogadores
        var nm = NetworkManager.Singleton;
        if (nm == null || !nm.IsServer) return;

        // Verifica se o client já tem um PlayerObject (evita spawn duplo)
        if (nm.ConnectedClients.TryGetValue(clientId, out var client) &&
            client != null && client.PlayerObject != null)
        {
            Debug.Log($"[Spawn Skip] Client {clientId} já tem PlayerObject (auto-spawn ou outro script).");
            return;
        }

        // Se não tiver, spawn do jogador
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (playerPrefabToSpawn == null)
        {
            Debug.LogError("Player Prefab To Spawn não definido no Inspector!");
            return;
        }

        var instance = Instantiate(playerPrefabToSpawn);
        var netObj = instance.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("O prefab do jogador NÃO tem NetworkObject no root!");
            Destroy(instance);
            return;
        }

        // Faz spawn do objecto do jogador como PlayerObject do cliente
        netObj.SpawnAsPlayerObject(clientId, true);
        Debug.Log($"Jogador spawnado para Client {clientId}");
    }

    void OnDestroy()
    {
        // Remove o callback ao destruir este objecto
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
}
