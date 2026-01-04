using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InfimaGames.LowPolyShooterPack;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private Slider sensitivitySlider;

    [Header("Painel")]
    public GameObject settingsPanel; // Público para ser acessível externamente

    [Header("Botões (opcional)")]
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button closeSettingsButton;

    private Resolution[] resolutions;
    private const string PREF_RESOLUTION = "settings_resolution";
    private const string PREF_SENSITIVITY = "settings_sensitivity";

    private ICameraLook currentCameraLook;

    private void Awake()
    {
        // Se settingsPanel não estiver definido, usa o próprio GameObject
        if (settingsPanel == null)
            settingsPanel = gameObject;

        // Configura os botões open/close (opcional)
        if (openSettingsButton != null)
            openSettingsButton.onClick.AddListener(Abrir);

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(Fechar);
    }

    private void Start()
    {
        // Começa escondido
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Tenta encontrar o player (pode não existir no menu)
        Character playerCharacter = FindObjectOfType<Character>();
        if (playerCharacter != null)
            currentCameraLook = playerCharacter as ICameraLook;

        // Inicializa resoluções
        if (resolutionsDropdown != null)
        {
            CarregarResolucoes();
            resolutionsDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        // Inicializa sensibilidade
        if (sensitivitySlider != null)
        {
            CarregarSensibilidade();
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
    }

    // ======= CONTROLO DO PAINEL =======
    public void Abrir()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void Fechar()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // ======= RESOLUÇÕES =======
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
                indiceActual = i;
            }
        }

        resolutionsDropdown.AddOptions(options);

        int guardada = PlayerPrefs.GetInt(PREF_RESOLUTION, indiceActual);
        guardada = Mathf.Clamp(guardada, 0, resolutions.Length - 1);

        resolutionsDropdown.value = guardada;
        resolutionsDropdown.RefreshShownValue();

        // Aplica resolução imediatamente se estiver no Jogo
        AplicarResolucao(guardada);
    }

    public void OnResolutionChanged(int index)
    {
        // Guarda sempre a escolha no PlayerPrefs
        PlayerPrefs.SetInt(PREF_RESOLUTION, index);
        PlayerPrefs.Save();

        // Aplica a resolução agora se possível
        AplicarResolucao(index);
    }

    private void AplicarResolucao(int index)
    {
        if (resolutions == null || resolutions.Length == 0) return;

        Resolution r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
    }

    // ======= SENSIBILIDADE =======
    private void CarregarSensibilidade()
    {
        float sens = PlayerPrefs.GetFloat(PREF_SENSITIVITY, 1.0f);
        sensitivitySlider.value = sens;

        // Aplica apenas se houver player
        if (currentCameraLook != null)
            AplicarSensibilidade(sens);
    }

    public void OnSensitivityChanged(float value)
    {
        // Guarda sempre no PlayerPrefs
        PlayerPrefs.SetFloat(PREF_SENSITIVITY, value);
        PlayerPrefs.Save();

        // Aplica apenas se houver player activo
        if (currentCameraLook != null)
            AplicarSensibilidade(value);
    }

    private void AplicarSensibilidade(float value)
    {
        if (currentCameraLook != null)
            currentCameraLook.SetMouseSensitivity(value);
    }
}

public interface ICameraLook
{
    void SetMouseSensitivity(float value);
}
