using System;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI (TMP)")]
    [SerializeField] TMP_InputField ifPlayerName;    // Campo para nome do jogador
    [SerializeField] Button btnConnect;              // Botão para ligar ao Photon
    [SerializeField] Button btnCreate;               // Botão para criar lobby
    [SerializeField] TMP_Text txtCreatedCode;        // Texto para mostrar código do lobby criado
    [SerializeField] TMP_InputField ifJoinCode;     // Campo para código de lobby a entrar
    [SerializeField] Button btnJoin;                 // Botão para entrar no lobby
    [SerializeField] Button btnLeave;                // Botão para sair do lobby
    [SerializeField] TMP_Text txtStatus;             // Texto para mostrar estado/status
    [SerializeField] Button btnStartGame;            // Botão para iniciar jogo (só host)
    [SerializeField] TMP_Text txtCountdown;          // Contador regressivo de início
    [SerializeField] Button btnPlayBots;             // Botão para modo offline com bots

    [Header("UI (Equipas)")]
    [Tooltip("Painel que contém os botões de equipa (TeamSelectPanel)")]
    [SerializeField] GameObject teamSelectionPanel;  // Painel com botões de escolha de equipa
    [Tooltip("Script LobbyTeamUI (com referências aos botões A/B)")]
    [SerializeField] LobbyTeamUI lobbyTeamUI;        // Script para gerir UI das equipas

    [Header("Config")]
    [SerializeField] string gameSceneName = "Prototype"; // Cena do jogo
    [SerializeField] int roomCodeLength = 6;             // Comprimento do código do lobby
    [SerializeField] int maxPlayers = 2;                // Máximo de jogadores
    [SerializeField] int countdownSeconds = 3;          // Segundos do contador regressivo

    const string ROOM_PROP_RELAY = "relay"; // Nome da propriedade do Relay no Photon
    bool _matchStarted = false;             // Se o jogo começou
    bool _isCountingDown = false;           // Se o contador regressivo está activo

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false; // Não sincroniza automaticamente a cena
        Application.runInBackground = true;           // Continua a correr em background

        // Inicializar UI
        SetUIConnected(false);
        SetUILobbyActions(false);
        if (btnStartGame) btnStartGame.gameObject.SetActive(false);
        if (txtCountdown) txtCountdown.gameObject.SetActive(false);

        Log("Pronto. Define nome e carrega Conectar.");

        // Associar botões a funções
        btnConnect.onClick.AddListener(OnClickConnect);
        btnCreate.onClick.AddListener(OnClickCreate);
        btnJoin.onClick.AddListener(OnClickJoin);
        btnLeave.onClick.AddListener(OnClickLeave);
        if (btnStartGame) btnStartGame.onClick.AddListener(OnClickStartGame);
        if (btnPlayBots) btnPlayBots.onClick.AddListener(OnClickPlayWithBots);
    }

    void OnDestroy()
    {
        // Limpar listeners dos botões
        btnConnect.onClick.RemoveAllListeners();
        btnCreate.onClick.RemoveAllListeners();
        btnJoin.onClick.RemoveAllListeners();
        btnLeave.onClick.RemoveAllListeners();
        if (btnStartGame) btnStartGame.onClick.RemoveAllListeners();
        if (btnPlayBots) btnPlayBots.onClick.RemoveAllListeners();
    }

    // Configura UI antes de estar em lobby (ligado ou não)
    void SetUIConnected(bool connected)
    {
        if (btnConnect) btnConnect.interactable = !connected;
        if (ifPlayerName) ifPlayerName.interactable = !connected;
        if (btnCreate) btnCreate.interactable = connected;
        if (btnJoin) btnJoin.interactable = connected && !string.IsNullOrEmpty(ifJoinCode?.text);
        if (ifJoinCode) ifJoinCode.interactable = connected;
        if (btnLeave) btnLeave.interactable = false;

        if (txtCreatedCode) { txtCreatedCode.gameObject.SetActive(false); txtCreatedCode.text = ""; }
        if (btnStartGame) btnStartGame.gameObject.SetActive(false);
        if (teamSelectionPanel) teamSelectionPanel.SetActive(false);
        if (lobbyTeamUI) lobbyTeamUI.SetTeamButtonsInteractable(false);
    }

    // Configura UI quando está em lobby
    void SetUILobbyActions(bool inRoom)
    {
        if (btnLeave) btnLeave.interactable = inRoom;
        if (btnCreate) btnCreate.interactable = !inRoom && PhotonNetwork.IsConnectedAndReady;
        if (btnJoin) btnJoin.interactable = !inRoom && PhotonNetwork.IsConnectedAndReady && !string.IsNullOrEmpty(ifJoinCode?.text);
        if (ifJoinCode) ifJoinCode.interactable = !inRoom && PhotonNetwork.IsConnectedAndReady;
        if (txtCreatedCode) txtCreatedCode.gameObject.SetActive(inRoom);
        if (teamSelectionPanel) teamSelectionPanel.SetActive(inRoom);
        if (lobbyTeamUI) lobbyTeamUI.SetTeamButtonsInteractable(inRoom);

        Debug.Log($"[Lobby] teamSelectionPanel: {(teamSelectionPanel ? teamSelectionPanel.name : "NULL")} | inRoom={inRoom}");
    }

    // Mostra log no UI e console
    void Log(string msg)
    {
        if (txtStatus) txtStatus.text = msg;
        Debug.Log("[Lobby] " + msg);
    }

    // Ligação ao Photon
    async void OnClickConnect()
    {
        // Define nickname
        var nick = string.IsNullOrWhiteSpace(ifPlayerName?.text)
            ? ("Player" + UnityEngine.Random.Range(1000, 9999))
            : ifPlayerName.text.Trim();
        PhotonNetwork.NickName = nick;

        Log($"A ligar ao Photon como {PhotonNetwork.NickName}...");

        // Conecta ao Photon
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            Log("Já estás ligado.");

        // Garantir Unity Services e autenticação
        await EnsureUnityServicesAsync();
    }

    // Criar lobby
    void OnClickCreate()
    {
        string code = GenerateRoomCode(roomCodeLength);
        var options = new RoomOptions
        {
            MaxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 16),
            IsVisible = false,
            IsOpen = true,
            CustomRoomProperties = new Hashtable { { ROOM_PROP_RELAY, "" } },
            CustomRoomPropertiesForLobby = new[] { ROOM_PROP_RELAY }
        };
        Log($"A criar lobby com código {code}...");
        PhotonNetwork.CreateRoom(code, options, TypedLobby.Default);
    }

    // Entrar em lobby existente
    void OnClickJoin()
    {
        string code = ifJoinCode?.text?.Trim().ToUpper();
        if (string.IsNullOrEmpty(code)) { Log("Escreve um código para entrar."); return; }
        Log($"A entrar no lobby {code}...");
        PhotonNetwork.JoinRoom(code);
    }

    // Sair do lobby
    void OnClickLeave()
    {
        if (PhotonNetwork.InRoom)
        {
            Log("A sair do lobby...");
            PhotonNetwork.LeaveRoom();
        }
    }

    // Iniciar jogo (host)
    void OnClickStartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_isCountingDown || _matchStarted) return;

        // Define propriedade para começar o countdown
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "startCountdown", true } });
    }

    // Callbacks do Photon
    public override void OnConnectedToMaster()
    {
        Log("Ligado ao Master. A entrar no lobby...");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Log("Estás no lobby. Podes criar ou entrar por código.");
        SetUIConnected(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Log($"Desligado: {cause}");
        SetUIConnected(false);
        SetUILobbyActions(false);
    }

    public override void OnCreatedRoom()
    {
        Log($"Lobby criado. Código: {PhotonNetwork.CurrentRoom?.Name}");
        SetUILobbyActions(true);
        if (lobbyTeamUI) lobbyTeamUI.SetTeamButtonsInteractable(true);
        if (txtCreatedCode)
        {
            txtCreatedCode.gameObject.SetActive(true);
            txtCreatedCode.text = $"Código: {PhotonNetwork.CurrentRoom?.Name}";
        }
        if (PhotonNetwork.IsMasterClient && btnStartGame)
            btnStartGame.gameObject.SetActive(true);
    }

    public override async void OnJoinedRoom()
    {
        string code = PhotonNetwork.CurrentRoom.Name;
        Log($"Entraste no lobby ({code}). Espera o início do jogo ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})");

        if (txtCreatedCode)
        {
            txtCreatedCode.gameObject.SetActive(true);
            txtCreatedCode.text = $"Código: {code}";
        }

        SetUILobbyActions(true);
        if (lobbyTeamUI) lobbyTeamUI.SetTeamButtonsInteractable(true);
        if (PhotonNetwork.IsMasterClient && btnStartGame)
            btnStartGame.gameObject.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Log($"Entrou: {newPlayer.NickName} ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Log($"{otherPlayer.NickName} saiu. ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})");
    }

    // Atualizações de propriedades do lobby
    public override async void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("startCountdown"))
        {
            await StartCountdownAndLaunch();
            return;
        }

        if (propertiesThatChanged.ContainsKey(ROOM_PROP_RELAY))
        {
            string joinCode = propertiesThatChanged[ROOM_PROP_RELAY] as string;
            if (!string.IsNullOrEmpty(joinCode) && !IsNgoConnected())
            {
                Log($"Código Relay recebido: {joinCode}. A ligar ao jogo...");
                await StartClientWithRelayAsync(joinCode);
            }
        }
    }

    // Countdown regressivo antes do jogo
    async Task StartCountdownAndLaunch()
    {
        if (_isCountingDown) return;
        _isCountingDown = true;

        if (btnStartGame) btnStartGame.interactable = false;
        if (txtCountdown) txtCountdown.gameObject.SetActive(true);
        if (teamSelectionPanel) teamSelectionPanel.SetActive(false);
        if (lobbyTeamUI) lobbyTeamUI.SetTeamButtonsInteractable(false);

        for (int i = countdownSeconds; i > 0; i--)
        {
            if (txtCountdown) txtCountdown.text = $"Começa em {i}...";
            await Task.Delay(1000);
        }

        if (txtCountdown) txtCountdown.text = "A começar!";
        if (PhotonNetwork.IsMasterClient)
        {
            _matchStarted = true;
            await StartHostWithRelayAndLoadAsync();
        }
    }

    // Iniciar host com Relay
    async Task StartHostWithRelayAndLoadAsync()
    {
        await EnsureUnityServicesAsync();
        int maxConnections = Mathf.Max(1, maxPlayers - 1);
        Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

        Log($"Relay criado. JoinCode: {joinCode}");

        var props = new Hashtable { { ROOM_PROP_RELAY, joinCode } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        var serverData = AllocationUtils.ToRelayServerData(alloc, "dtls");
        transport.SetRelayServerData(serverData);

        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            if (!NetworkManager.Singleton.StartHost())
            {
                Debug.LogError("Falha ao iniciar Host NGO.");
                return;
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    // Cliente a ligar com Relay
    async Task StartClientWithRelayAsync(string joinCode)
    {
        await EnsureUnityServicesAsync();
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
        JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var serverData = AllocationUtils.ToRelayServerData(joinAlloc, "dtls");
        transport.SetRelayServerData(serverData);

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.StartClient();
    }

    bool IsNgoConnected()
    {
        var nm = NetworkManager.Singleton;
        return nm && (nm.IsClient || nm.IsServer);
    }

    // Inicializar Unity Services + autenticação anónima
    async Task EnsureUnityServicesAsync()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
            await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Gerar código aleatório de lobby
    string GenerateRoomCode(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var rnd = new System.Random();
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++) sb.Append(chars[rnd.Next(chars.Length)]);
        return sb.ToString();
    }

    // Modo offline com bots
    public void OnClickPlayWithBots()
    {
        Debug.Log("[Lobby] Modo offline com bots (host local).");

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("[Lobby] NetworkManager.Singleton == null.");
            return;
        }

        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            bool ok = NetworkManager.Singleton.StartHost();
            if (!ok)
            {
                Debug.LogError("[Lobby] Falha ao iniciar Host local.");
                return;
            }
        }

        PlayerPrefs.SetInt("OfflineMode", 1);
        NetworkManager.Singleton.SceneManager.LoadScene("Prototype", LoadSceneMode.Single);
    }
}
