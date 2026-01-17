using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Botões")]
    [SerializeField] private Button jogarButton;       // Botão para iniciar o jogo e ir para o Lobby
    [SerializeField] private Button creditosButton;    // Botão para abrir o painel de créditos
    [SerializeField] private Button settingsButton;    // Botão para abrir o painel de settings

    [Header("Botão Voltar (Créditos)")]
    [SerializeField] private Button voltarCreditsButton; // Botão para fechar o painel de créditos

    [Header("Painel de Créditos")]
    [SerializeField] private GameObject creditsPanel;    // Painel que mostra os créditos

    [Header("Prefab do painel de Settings")]
    [SerializeField] private GameObject settingsPrefab;  // Prefab do painel de configurações
    private GameObject settingsInstance;                 // Instância do painel de settings criado

    private void Awake()
    {
        // Adiciona os listeners aos botões
        jogarButton.onClick.AddListener(Jogar);
        creditosButton.onClick.AddListener(AbrirCreditos);
        settingsButton.onClick.AddListener(AbrirSettings);

        if (voltarCreditsButton != null)
            voltarCreditsButton.onClick.AddListener(FecharCreditos);

        // Inicialmente fecha o painel de créditos
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        // Instancia o painel de settings e guarda a referência
        if (settingsPrefab != null)
        {
            Canvas menuCanvas = FindObjectOfType<Canvas>(); // Procura o canvas principal
            settingsInstance = Instantiate(settingsPrefab, menuCanvas.transform);

            // Se o script SettingsMenu estiver presente, garante que a referência ao painel está atribuída
            var settingsScript = settingsInstance.GetComponent<SettingsMenu>();
            if (settingsScript != null && settingsScript.settingsPanel == null)
                settingsScript.settingsPanel = settingsInstance;

            settingsInstance.SetActive(false); // Fecha o painel inicialmente
        }
    }

    // Função chamada ao carregar no botão "Jogar"
    private void Jogar()
    {
        SceneManager.LoadScene("Lobby");
    }

    // Abre o painel de settings
    private void AbrirSettings()
    {
        if (settingsInstance != null)
            settingsInstance.SetActive(true);
    }

    // Abre o painel de créditos
    private void AbrirCreditos()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    // Fecha o painel de settings
    public void FecharSettings()
    {
        if (settingsInstance != null)
            settingsInstance.SetActive(false);
    }

    // Fecha o painel de créditos
    public void FecharCreditos()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}
