using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class GameInput : NetworkBehaviour
{
    // Instância local do input para fácil acesso
    public static GameInput LocalInput { get; private set; }

    [Header("--- Habilidades ---")]
    [SerializeField] private InputActionReference shieldAction; // Input para o escudo
    [SerializeField] private InputActionReference pulseAction;  // Input para a pulse

    public override void OnNetworkSpawn()
    {
        // Apenas o jogador dono mantém este script activo
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        // Guarda referência local para acesso rápido
        LocalInput = this;

        // Activa todos os inputs relevantes
        EnableAllInputs();
    }

    public override void OnNetworkDespawn()
    {
        // Quando o objecto é despawned, desactiva inputs apenas para o dono
        if (IsOwner)
        {
            DisableAllInputs();
        }
    }

    // Activa todas as actions de input
    private void EnableAllInputs()
    {
        shieldAction?.action.Enable();
        pulseAction?.action.Enable();
    }

    // Desactiva todas as actions de input
    private void DisableAllInputs()
    {
        shieldAction?.action.Disable();
        pulseAction?.action.Disable();
    }

    // Verifica se o input do escudo foi activado neste frame
    public bool ShieldTriggered() => shieldAction != null && shieldAction.action.WasPressedThisFrame();

    // Verifica se o input da pulse foi activado neste frame
    public bool PulseTriggered() => pulseAction != null && pulseAction.action.WasPressedThisFrame();
}
