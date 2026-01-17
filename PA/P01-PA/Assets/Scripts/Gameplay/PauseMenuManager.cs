using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } // Estado global de pausa

    [Header("UI")]
    public GameObject pausePanel;   // Painel de pausa
    public Button btnResume;        // Botão de retomar
    public Button btnDisconnect;    // Botão de desconectar
    public Button btnQuit;          // Botão de sair do jogo
    public TMP_Text txtStatus;      // Texto de status (ex: a desconectar)

    bool isMenuOpen = false;        // Estado interno do menu
    PlayerInput localInput;         // Referência ao PlayerInput do jogador local

    void Start()
    {
        IsPaused   = false;
        isMenuOpen = false;

        // Trava o cursor e oculta-o inicialmente
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        // Desativa painel de pausa
        if (pausePanel) pausePanel.SetActive(false);

        // Adiciona listeners aos botões
        if (btnResume)     btnResume.onClick.AddListener(OnClickResume);
        if (btnDisconnect) btnDisconnect.onClick.AddListener(OnClickDisconnect);
        if (btnQuit)       btnQuit.onClick.AddListener(OnClickQuit);

        // Callbacks de rede
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStopped           += OnServerStopped;
        }

        // Garante que existe um EventSystem
        EnsureEventSystem();
    }

    void Update()
    {
        // Alterna menu quando se pressiona ESC
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ToggleMenu();
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        IsPaused   = isMenuOpen;

        // Mostra/oculta painel de pausa
        if (pausePanel) pausePanel.SetActive(isMenuOpen);

        // Atualiza estado do cursor
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible   = isMenuOpen;

        // Obtém PlayerInput do jogador local
        if (localInput == null)
        {
            var localPlayer = FindLocalPlayer();
            if (localPlayer)
                localInput = localPlayer.GetComponentInChildren<PlayerInput>();
        }

        // Ativa/desativa PlayerInput
        if (localInput)
            localInput.enabled = !isMenuOpen;

        // Seleciona botão "Resume" automaticamente ao abrir o menu
        if (isMenuOpen && btnResume && EventSystem.current)
            EventSystem.current.SetSelectedGameObject(btnResume.gameObject);
    }

    // Procura o jogador local
    GameObject FindLocalPlayer()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var netObj = player.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsOwner)
                return player;
        }
        return null;
    }

    // Eventos dos botões
    void OnClickResume()     => ToggleMenu();
    void OnClickQuit()       => Application.Quit();
    void OnClickDisconnect() => StartCoroutine(DisconnectAndReturnToLobby());

    // Desconecta do servidor e retorna ao lobby
    System.Collections.IEnumerator DisconnectAndReturnToLobby()
    {
        if (txtStatus) txtStatus.text = "A desconectar...";

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        yield return new WaitForSeconds(0.5f);

        if (txtStatus) txtStatus.text = "A voltar ao menu...";
        SceneManager.LoadScene("Lobby");
    }

    // Callback quando um cliente se desconecta
    void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer && clientId == 0)
        {
            Debug.Log("[PauseMenu] Host caiu. A voltar ao lobby...");
            SceneManager.LoadScene("Lobby");
        }
    }

    // Callback quando o servidor é parado
    void OnServerStopped(bool _)
    {
        Debug.Log("[PauseMenu] Servidor parado. A voltar ao lobby...");
        SceneManager.LoadScene("Lobby");
    }

    void OnDestroy()
    {
        // Remove listeners
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStopped           -= OnServerStopped;
        }

        if (btnResume)     btnResume.onClick.RemoveAllListeners();
        if (btnDisconnect) btnDisconnect.onClick.RemoveAllListeners();
        if (btnQuit)       btnQuit.onClick.RemoveAllListeners();
    }

    // Garante que existe um EventSystem para input UI
    void EnsureEventSystem()
    {
        var es = FindObjectOfType<EventSystem>();
        if (es == null)
        {
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }
        else if (es.GetComponent<InputSystemUIInputModule>() == null)
        {
            es.gameObject.AddComponent<InputSystemUIInputModule>();
            var old = es.GetComponent<StandaloneInputModule>();
            if (old) Destroy(old);
        }
    }
}
