using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [Header("Botões")]
    [SerializeField] private Button jogarButton;
    [SerializeField] private Button creditosButton;
    [SerializeField] private Button settingsButton;
    [Header("Botão Voltar (Créditos)")]
    [SerializeField] private Button voltarCreditsButton;
    [Header("Painel de Créditos")]
    [SerializeField] private GameObject creditsPanel;
    [Header("Prefab do painel de Settings")]
    [SerializeField] private GameObject settingsPrefab;
    private GameObject settingsInstance;
    private void Awake()
    {
        jogarButton.onClick.AddListener(Jogar);
        creditosButton.onClick.AddListener(AbrirCreditos);
        settingsButton.onClick.AddListener(AbrirSettings);
        if (voltarCreditsButton != null)
            voltarCreditsButton.onClick.AddListener(FecharCreditos);
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
        if (settingsPrefab != null)
        {
            Canvas menuCanvas = FindObjectOfType<Canvas>();
            settingsInstance = Instantiate(settingsPrefab, menuCanvas.transform);
            var settingsScript = settingsInstance.GetComponent<SettingsMenu>();
            if (settingsScript != null && settingsScript.settingsPanel == null)
                settingsScript.settingsPanel = settingsInstance;
            settingsInstance.SetActive(false);
        }
    }
    private void Jogar()
    {
        SceneManager.LoadScene("Lobby");
    }
    private void AbrirSettings()
    {
        if (settingsInstance != null)
            settingsInstance.SetActive(true);
    }
    private void AbrirCreditos()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }
    public void FecharSettings()
    {
        if (settingsInstance != null)
            settingsInstance.SetActive(false);
    }
    public void FecharCreditos()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}