using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InfimaGames.LowPolyShooterPack;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;  // Dropdown para escolher resolução
    [SerializeField] private Slider sensitivitySlider;          // Slider para ajustar sensibilidade do rato

    [Header("Painel")]
    public GameObject settingsPanel;  // Painel de settings

    [Header("Botões (opcional)")]
    [SerializeField] private Button openSettingsButton;   // Botão para abrir settings
    [SerializeField] private Button closeSettingsButton;  // Botão para fechar settings

    private Resolution[] resolutions;  // Lista de resoluções disponíveis
    private const string PREF_RESOLUTION = "settings_resolution";  // Chave PlayerPrefs resolução
    private const string PREF_SENSITIVITY = "settings_sensitivity"; // Chave PlayerPrefs sensibilidade

    private ICameraLook currentCameraLook;  // Referência para aplicar sensibilidade

    private void Awake()
    {
        if (settingsPanel == null)
            settingsPanel = gameObject;

        if (openSettingsButton != null)
            openSettingsButton.onClick.AddListener(Abrir);  // Associa botão abrir

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(Fechar); // Associa botão fechar
    }

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);  // Fecha painel no início

        // Procura o Character do jogador para aplicar sensibilidade
        Character playerCharacter = FindObjectOfType<Character>();
        if (playerCharacter != null)
            currentCameraLook = playerCharacter as ICameraLook;

        // Configura resoluções
        if (resolutionsDropdown != null)
        {
            CarregarResolucoes();
            resolutionsDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        // Configura sensibilidade
        if (sensitivitySlider != null)
        {
            CarregarSensibilidade();
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
    }

    public void Abrir()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);  // Mostra painel
    }

    public void Fechar()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false); // Esconde painel
    }

    private void CarregarResolucoes()
    {
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        List<string> options = new List<string>();
        int indiceActual = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                indiceActual = i; // Guarda a resolução atual
            }
        }

        resolutionsDropdown.AddOptions(options);

        int guardada = PlayerPrefs.GetInt(PREF_RESOLUTION, indiceActual);
        guardada = Mathf.Clamp(guardada, 0, resolutions.Length - 1);
        resolutionsDropdown.value = guardada;
        resolutionsDropdown.RefreshShownValue();

        AplicarResolucao(guardada);  // Aplica resolução guardada
    }

    public void OnResolutionChanged(int index)
    {
        PlayerPrefs.SetInt(PREF_RESOLUTION, index);
        PlayerPrefs.Save();
        AplicarResolucao(index);  // Aplica nova resolução
    }

    private void AplicarResolucao(int index)
    {
        if (resolutions == null || resolutions.Length == 0) return;
        Resolution r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode); // Aplica resolução
    }

    private void CarregarSensibilidade()
    {
        float sens = PlayerPrefs.GetFloat(PREF_SENSITIVITY, 1.0f);
        sensitivitySlider.value = sens;
        if (currentCameraLook != null)
            AplicarSensibilidade(sens);  // Aplica sensibilidade guardada
    }

    public void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat(PREF_SENSITIVITY, value);
        PlayerPrefs.Save();
        if (currentCameraLook != null)
            AplicarSensibilidade(value);  // Aplica nova sensibilidade
    }

    private void AplicarSensibilidade(float value)
    {
        if (currentCameraLook != null)
            currentCameraLook.SetMouseSensitivity(value);
    }
}

// Interface para qualquer script que possa receber sensibilidade do rato
public interface ICameraLook
{
    void SetMouseSensitivity(float value);
}
