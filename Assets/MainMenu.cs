using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Botões")]
    [SerializeField] private Button jogarButton;
    [SerializeField] private Button creditosButton;
    [SerializeField] private Button settingsButton;

    [Header("Prefab do painel de Settings")]
    [SerializeField] private GameObject settingsPrefab; // arrasta o prefab do jogo aqui

    private GameObject settingsInstance;

    private void Awake()
    {
        // Botões do menu
        jogarButton.onClick.AddListener(Jogar);
        creditosButton.onClick.AddListener(Creditos);
        settingsButton.onClick.AddListener(AbrirSettings);

        // Instancia o prefab dentro do Canvas do menu
        if (settingsPrefab != null)
        {
            // Procura o Canvas do menu para ser o pai do painel
            Canvas menuCanvas = FindObjectOfType<Canvas>();
            settingsInstance = Instantiate(settingsPrefab, menuCanvas.transform);

            // Opcional: garante que o script SettingsMenu do prefab conhece o painel
            var settingsScript = settingsInstance.GetComponent<SettingsMenu>();
            if (settingsScript != null && settingsScript.settingsPanel == null)
            {
                settingsScript.settingsPanel = settingsInstance;
            }

            // Começa escondido
            settingsInstance.SetActive(false);
        }
    }

    private void Jogar()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void Creditos()
    {
        // Por enquanto vazio
    }

    private void AbrirSettings()
    {
        if (settingsInstance != null)
            settingsInstance.SetActive(true);
    }

    // Para ligar ao botão "Fechar" dentro do painel, se existir
    public void FecharSettings()
    {
        if (settingsInstance != null)
            settingsInstance.SetActive(false);
    }
}