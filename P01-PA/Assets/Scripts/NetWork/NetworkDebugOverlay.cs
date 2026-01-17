using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; 
#endif

public class NetworkDebugOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;   // Texto da UI onde mostramos debug (ping, FPS, loss)
    [SerializeField] private KeyCode toggleKey = KeyCode.F3; // Tecla para ligar/desligar overlay
    private bool visible = true;       // Estado atual de visibilidade da overlay
    private float fpsTimer;            // Contador de tempo para cálculo de FPS
    private int frames;                // Contador de frames no intervalo de 1 segundo
    private float fps;                 // FPS calculado

    void Awake()
    {
        // Tenta obter TextMeshProUGUI se não estiver atribuído
        if (!debugText) debugText = GetComponent<TextMeshProUGUI>();
        if (!debugText) debugText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    void Start()
    {
        // Define visibilidade inicial da overlay
        if (debugText) debugText.enabled = visible;
        ForceRefreshNow(); // Mostra valores iniciais
    }

    void Update()
    {
        bool pressed = false;

        // Suporte para novo Input System
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.f3Key.wasPressedThisFrame)
            pressed = true;
#endif
        // Suporte para Input legacy
        if (Input.GetKeyDown(toggleKey))
            pressed = true;

        // Alterna visibilidade da overlay se tecla premida
        if (pressed)
        {
            visible = !visible;
            if (debugText) debugText.enabled = visible;
            Debug.Log($"[Overlay] Toggle -> {(visible ? "ON" : "OFF")}");
        }

        // Se overlay invisível, não processa FPS/ping/loss
        if (!visible || !debugText) return;

        // --- Cálculo de FPS ---
        frames++;
        fpsTimer += Time.unscaledDeltaTime;
        if (fpsTimer >= 1f)
        {
            fps = frames / fpsTimer; // FPS = frames / tempo decorrido
            frames = 0;
            fpsTimer = 0f;
        }

        // --- Ping da rede ---
        ulong ping = 0;
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsClient)
        {
            var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            ping = transport.GetCurrentRtt(NetworkManager.ServerClientId); // RTT em ms
        }

        // --- Loss de pacotes ---
        string loss = "-";
        if (LossProbe.Instance)
        {
            float v = LossProbe.Instance.CurrentLossPercent;
            if (v >= 0f) loss = v.ToString("F1") + " %";
        }

        // Atualiza texto da UI
        debugText.text = $"PING: {ping} ms\nLOSS: {loss}\nFPS: {fps:F0}";
    }

    // Mostra valores iniciais padrão no start
    private void ForceRefreshNow()
    {
        if (!debugText) return;
        debugText.text = "PING: 0 ms\nLOSS: -\nFPS: 0";
    }
}
