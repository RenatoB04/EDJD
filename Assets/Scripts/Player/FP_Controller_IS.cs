using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class FP_Controller_IS : NetworkBehaviour
{
    public static Transform PlayerCameraRoot { get; private set; }

    [Header("Refs")]
    [SerializeField] Transform cameraRoot; // Root da câmara (geralmente braços/arma)
    CharacterController cc;
    Animator animator;
    PlayerInput playerInput;

    [Header("Componentes de Rede (Ligar no Inspector)")]
    [SerializeField] private Camera playerCamera; 
    [SerializeField] private AudioListener audioListener;

    [Header("Input Actions")]
    [SerializeField] InputActionReference move;   // Movimento WASD / joystick
    [SerializeField] InputActionReference look;   // Mouse / stick da câmara
    [SerializeField] InputActionReference jump;   
    [SerializeField] InputActionReference sprint; 
    [SerializeField] InputActionReference crouch;

    [Header("Velocidades")]
    public float walkSpeed = 6.5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 3f;

    [Header("Aceleração (suavidade)")]
    public float accelGround = 14f;
    public float accelAir = 6f;

    [Header("Salto / Gravidade")]
    public float gravity = -28f;
    public float jumpHeight = 1.8f;
    public float maxFallSpeed = -50f;

    [Header("Câmara")]
    public float sens = 0.2f;
    float xRot; // Rotação vertical da câmara
    Vector3 velocity; // Velocidade actual do jogador
    bool canJump = true; // Flag para permitir salto
    bool groundedPrev = true; // Estado anterior de grounded

    [Header("Crouch (Toggle)")]
    public float crouchHeight = 1.0f;
    public float crouchCamYOffset = -0.4f; // Deslocamento vertical da câmara ao agachar
    public float crouchSmooth = 12f;
    float originalHeight;
    float cameraRootBaseY;
    float stepOffsetOriginal;
    bool isCrouching;

    [Header("Habilidades")]
    [SerializeField] InputActionReference shieldAction; 
    [SerializeField] InputActionReference pulseAction; 
    private PlayerShield playerShield; 
    private Health playerHealth;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Inicializa referências
        if (!playerInput) playerInput = GetComponent<PlayerInput>();
        if (!cc) cc = GetComponent<CharacterController>();
        if (!playerCamera) playerCamera = GetComponentInChildren<Camera>(true);
        if (!audioListener) audioListener = GetComponentInChildren<AudioListener>(true);

        // Posiciona jogador no spawn do servidor
        if (IsServer && SpawnsManager.I != null)
        {
            SpawnsManager.I.GetNext(out var pos, out var rot);

            // Desativa temporariamente CharacterController para mover objecto
            if (cc) cc.enabled = false;
            transform.SetPositionAndRotation(pos, rot);
            if (cc) cc.enabled = true;

            // Envia posição para o cliente
            var target = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { OwnerClientId }
                }
            };
            SetSpawnClientRpc(pos, rot, target);
        }

        ApplyOwnershipState(IsOwner);

        // Configuração de câmara local e cursor para o dono
        if (IsOwner && cameraRoot != null)
        {
            PlayerCameraRoot = cameraRoot;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void OnGainedOwnership()
    {
        ApplyOwnershipState(true);
        if (cameraRoot) PlayerCameraRoot = cameraRoot;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Alterna mapa de Input para o jogador
        if (playerInput && playerInput.actions != null)
        {
            var map = playerInput.actions.FindActionMap("Player", true);
            if (map != null && playerInput.currentActionMap != map)
                playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public override void OnLostOwnership()
    {
        ApplyOwnershipState(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ApplyOwnershipState(bool owner)
    {
        // Activa/desactiva componentes de rede apenas para o dono
        if (playerCamera) playerCamera.enabled = owner;
        if (audioListener) audioListener.enabled = owner;
        if (playerInput) playerInput.enabled = owner;
        if (cc && owner && !cc.enabled) cc.enabled = true;
        if (cc && !owner) cc.enabled = false;
    }

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerShield = GetComponent<PlayerShield>(); 
        playerHealth = GetComponent<Health>();
        if (!playerCamera) playerCamera = GetComponentInChildren<Camera>(true);
        if (!audioListener) audioListener = GetComponentInChildren<AudioListener>(true);

        // Guardar valores originais do CharacterController
        originalHeight = cc.height;
        stepOffsetOriginal = cc.stepOffset;

        // Posição base da câmara
        if (cameraRoot) cameraRootBaseY = cameraRoot.localPosition.y;
        else Debug.LogWarning("FP_Controller_IS: Arrasta o CameraRoot no Inspector.");

        // Ajustes de CC
        cc.minMoveDistance = 0f;
        cc.slopeLimit = Mathf.Max(cc.slopeLimit, 45f);
        cc.stepOffset = Mathf.Max(cc.stepOffset, 0.3f);

        isCrouching = false;
        cc.height = originalHeight;
        cc.center = new Vector3(0f, originalHeight * 0.5f, 0f);

        if (cameraRoot)
        {
            var p = cameraRoot.localPosition; p.y = cameraRootBaseY;
            cameraRoot.localPosition = p;
        }
    }

    void OnEnable()
    {
        if (!IsOwner) return;

        // Activa inputs apenas para o dono
        if (move) move.action.Enable();
        if (look) look.action.Enable();
        if (jump) jump.action.Enable();
        if (sprint) sprint.action.Enable();
        if (crouch) crouch.action.Enable();
        if (shieldAction) shieldAction.action.Enable();
        if (pulseAction) pulseAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        if (move) move.action.Disable();
        if (look) look.action.Disable();
        if (jump) jump.action.Disable();
        if (sprint) sprint.action.Disable();
        if (crouch) crouch.action.Disable();
        if (shieldAction) shieldAction.action.Disable();
        if (pulseAction) pulseAction.action.Disable();

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Apenas para jogador dono
        if (PauseMenuManager.IsPaused) return; // Pausa

        if (cc && !cc.enabled) cc.enabled = true;

        // Gestão de look/câmara
        bool shieldActive = (playerShield != null && playerShield.IsShieldActive.Value);
        Vector2 lookDelta = look ? look.action.ReadValue<Vector2>() : Vector2.zero;

        xRot = Mathf.Clamp(xRot - lookDelta.y * sens, -85f, 85f);
        if (cameraRoot) cameraRoot.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * (lookDelta.x * sens));

        // Activação de habilidades
        if (shieldAction != null && shieldAction.action.WasPressedThisFrame())
        {
            if (playerHealth == null || !playerHealth.isDead.Value)
                playerShield?.RequestShieldServerRpc();
        }

        if (pulseAction != null && pulseAction.action.WasPressedThisFrame())
        {
            if (playerHealth == null || !playerHealth.isDead.Value)
                playerShield?.RequestPulseServerRpc();
        }

        // Toggle crouch
        if (crouch && crouch.action.WasPressedThisFrame()) isCrouching = !isCrouching;

        // Ajusta altura e centro do CharacterController
        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        float targetCenterY = targetHeight * 0.5f;
        cc.height = Mathf.Lerp(cc.height, targetHeight, Time.deltaTime * crouchSmooth);
        cc.center = Vector3.Lerp(cc.center, new Vector3(0f, targetCenterY, 0f), Time.deltaTime * crouchSmooth);
        cc.stepOffset = isCrouching ? 0.1f : stepOffsetOriginal;

        // Ajusta posição da câmara para crouch
        if (cameraRoot)
        {
            float targetCamY = cameraRootBaseY + (isCrouching ? crouchCamYOffset : 0f);
            Vector3 camLocal = cameraRoot.localPosition;
            camLocal.y = Mathf.Lerp(camLocal.y, targetCamY, Time.deltaTime * crouchSmooth);
            cameraRoot.localPosition = camLocal;
        }

        // Movimento horizontal
        Vector2 m = move ? move.action.ReadValue<Vector2>() : Vector2.zero;
        Vector3 inputDir = (transform.right * m.x + transform.forward * m.y);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        bool sprinting = (sprint && sprint.action.IsPressed()) && !isCrouching;
        float targetSpeed = isCrouching ? crouchSpeed : (sprinting ? sprintSpeed : walkSpeed);
        Vector3 targetHorizVel = inputDir * targetSpeed;

        float accel = cc.isGrounded ? accelGround : accelAir;
        Vector3 horiz = new Vector3(velocity.x, 0f, velocity.z);
        horiz = Vector3.MoveTowards(horiz, targetHorizVel, accel * Time.deltaTime);
        velocity.x = horiz.x;
        velocity.z = horiz.z;

        // Actualiza Animator
        float speedPercent = new Vector3(velocity.x, 0f, velocity.z).magnitude / sprintSpeed;
        if (speedPercent < 0.05f) speedPercent = 0f;
        speedPercent = Mathf.Clamp01(speedPercent);
        if (animator)
        {
            animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
            animator.SetBool("isCrouching", isCrouching);
        }

        // Salto
        if (canJump && jump != null && jump.action.WasPressedThisFrame() && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            canJump = false;
        }

        // Gravidade
        velocity.y += gravity * Time.deltaTime;
        if (velocity.y < maxFallSpeed) velocity.y = maxFallSpeed;

        // Movimento e colisões
        Vector3 motion = velocity * Time.deltaTime;
        CollisionFlags flags = cc.Move(motion);

        bool groundedNow = (flags & CollisionFlags.Below) != 0;
        if (groundedNow)
        {
            if (velocity.y < 0f) velocity.y = -2f;
            if (!groundedPrev) canJump = true;
        }
        groundedPrev = groundedNow;
    }

    [ClientRpc]
    public void SetSpawnClientRpc(Vector3 pos, Quaternion rot, ClientRpcParams rpcParams = default)
    {
        if (!IsOwner) return;
        if (cc == null) cc = GetComponent<CharacterController>();

        // Desativa temporariamente CharacterController para mover jogador
        bool prev = cc && cc.enabled;
        if (cc) cc.enabled = false;
        transform.SetPositionAndRotation(pos, rot);
        velocity = Vector3.zero;
        if (cc) cc.enabled = prev;
    }
}
