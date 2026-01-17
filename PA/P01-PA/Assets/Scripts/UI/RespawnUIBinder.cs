using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUIManager : MonoBehaviour
{
    [Header("Referências da UI")]
    [SerializeField] private GameObject respawnPanel;         // Painel principal da UI de respawn
    [SerializeField] private Button respawnButton;           // Botão para iniciar o respawn
    [SerializeField] private TextMeshProUGUI timerText;      // Texto do temporizador de respawn
    [SerializeField] private TextMeshProUGUI messageText;    // Texto da mensagem "Morreu" ou instruções

    [Header("Configuração")]
    [SerializeField] private float countdownTime = 3.0f;     // Tempo de contagem regressiva antes de respawn

    private Health localHealth;                               // Referência ao script Health do jogador local
    private PlayerDeathAndRespawn localRespawner;            // Referência ao script de respawn do jogador local
    private Coroutine _uiSequenceCoroutine;                 // Coroutine da sequência de respawn UI

    private void OnEnable()
    {
        Debug.Log("[UI CHECK] O RespawnUIManager começou! À procura do jogador...");

        // Desativa o painel inicialmente
        if (respawnPanel) respawnPanel.SetActive(false);
        else Debug.LogError("[UI ERRO] O 'Respawn Panel' não está arrastado no Inspector!");

        StartCoroutine(FindLocalPlayer());
    }

    private void OnDisable()
    {
        // Remove listener ao desligar o script
        if (localHealth != null)
        {
            localHealth.isDead.OnValueChanged -= OnPlayerDeathChanged;
        }
    }

    private IEnumerator FindLocalPlayer()
    {
        // Espera até que o jogador local esteja pronto na rede
        while (NetworkManager.Singleton == null ||
               NetworkManager.Singleton.LocalClient == null ||
               NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            yield return null;
        }

        Debug.Log("[UI CHECK] Jogador Local ENCONTRADO na rede!");
        var player = NetworkManager.Singleton.LocalClient.PlayerObject;

        // Obtem os scripts de Health e Respawn do jogador
        localHealth = player.GetComponentInChildren<Health>();
        localRespawner = player.GetComponentInChildren<PlayerDeathAndRespawn>();

        if (localHealth != null)
        {
            Debug.Log($"[UI CHECK] Script de Vida encontrado! Está morto agora? {localHealth.isDead.Value}");
            localHealth.isDead.OnValueChanged += OnPlayerDeathChanged;

            // Se o jogador já estiver morto ao iniciar, mostra o painel
            if (localHealth.isDead.Value) ShowDeathScreen();
        }
        else
        {
            Debug.LogError("[UI ERRO] Não encontrei o script 'Health' no jogador!");
        }

        // Configura o botão de respawn
        if (respawnButton != null)
        {
            respawnButton.onClick.RemoveAllListeners();
            respawnButton.onClick.AddListener(OnRespawnClicked);
        }
    }

    private void OnPlayerDeathChanged(bool wasDead, bool isDead)
    {
        Debug.Log($"[UI CHECK] O Jogador morreu? {isDead}");
        if (isDead)
            ShowDeathScreen();   // Mostra UI de morte
        else
            HideDeathScreen();   // Esconde UI de morte
    }

    private void ShowDeathScreen()
    {
        Debug.Log("[UI CHECK] A LIGAR O CANVAS DE MORTE!");

        if (respawnPanel) respawnPanel.SetActive(true);
        if (respawnButton) respawnButton.gameObject.SetActive(true);
        if (timerText) timerText.gameObject.SetActive(false);
        if (messageText) messageText.gameObject.SetActive(true);

        GameplayCursor.Unlock();  // Desbloqueia cursor ao morrer
    }

    private void HideDeathScreen()
    {
        Debug.Log("[UI CHECK] A desligar Canvas.");
        if (respawnPanel) respawnPanel.SetActive(false);
        GameplayCursor.Lock();    // Bloqueia cursor ao voltar a jogar
    }

    private void OnRespawnClicked()
    {
        Debug.Log("[UI CHECK] Botão clicado! Iniciando contagem.");

        // Se já houver contagem a correr, interrompe
        if (_uiSequenceCoroutine != null) StopCoroutine(_uiSequenceCoroutine);

        // Inicia a sequência de respawn
        _uiSequenceCoroutine = StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        // Esconde botão e mensagem durante contagem
        if (respawnButton) respawnButton.gameObject.SetActive(false);
        if (messageText) messageText.gameObject.SetActive(false);

        // Mostra temporizador
        if (timerText)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = countdownTime.ToString();
        }

        float timeLeft = countdownTime;
        while (timeLeft > 0)
        {
            if (timerText) timerText.text = Mathf.CeilToInt(timeLeft).ToString();
            yield return null;
            timeLeft -= Time.deltaTime;
        }

        Debug.Log("[UI CHECK] Contagem terminou. Enviando Respawn ao servidor.");

        // Chama respawn no servidor
        if (localRespawner != null)
        {
            localRespawner.RespawnServerRpc(true);
        }
    }
}
