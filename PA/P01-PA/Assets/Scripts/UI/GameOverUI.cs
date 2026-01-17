using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; } // Singleton da UI de Game Over

    [Header("Painel Principal")]
    [SerializeField] private GameObject panel;           // Painel principal do Game Over
    [SerializeField] private Button leaveButton;         // Botão para sair do jogo

    [Header("Resultados")]
    [SerializeField] private TextMeshProUGUI statusText; // Texto do estado ("Game Over", "Vitória", etc.)
    [SerializeField] private TextMeshProUGUI winnerText; // Texto com o nome do vencedor
    [SerializeField] private TextMeshProUGUI scoreText;  // Texto com a pontuação final

    void Awake()
    {
        // Inicializa singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Desativa painel inicialmente
        if (panel) panel.SetActive(false);

        // Configura botão de saída
        if (leaveButton) leaveButton.onClick.AddListener(OnLeaveClicked);
    }

    // Mostra o painel de Game Over com informação do jogo
    public void ShowGameOver(string message, string winnerName, int finalScore)
    {
        if (panel) 
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling(); // Garante que fica acima de outros elementos
        }

        if (statusText) statusText.text = message;

        if (winnerText) 
        {
            winnerText.text = string.IsNullOrEmpty(winnerName) 
                ? "Sem Vencedor" 
                : $"Vencedor: {winnerName}";
        }

        if (scoreText)
        {
            scoreText.text = $"Pontuação: {finalScore}";
        }

        // Liberta o cursor para o jogador interagir com o menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Evento do botão de sair: termina a rede e carrega cena do Lobby
    void OnLeaveClicked()
    {
        if (NetworkManager.Singleton) NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Lobby");
    }
}
