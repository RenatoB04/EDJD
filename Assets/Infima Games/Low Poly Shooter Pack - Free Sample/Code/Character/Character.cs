using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Netcode; 
namespace InfimaGames.LowPolyShooterPack
{
    [RequireComponent(typeof(CharacterKinematics))]
    public sealed class Character : NetworkBehaviour, ICameraLook
    {
       #region FIELDS SERIALIZED
       [Header("Visibility")]
       [Tooltip("Renderers que só o dono deve ver (braços FP, arma FP).")]
       [SerializeField] private Renderer[] firstPersonOnlyRenderers;
       [Tooltip("Renderers que só os outros devem ver (modelo TPS).")]
       [SerializeField] private Renderer[] thirdPersonOnlyRenderers;
       [Header("Network (Ligar no Inspector)")] 
       [Tooltip("A câmara principal do jogador (para ativar/desativar).")]
       [SerializeField] private Camera playerCamera;
       [Tooltip("O AudioListener principal (para ativar/desativar).")]
       [SerializeField] private AudioListener audioListener;
       [Tooltip("O script Kinematics (para ativar/desativar).")]
       [SerializeField] private CharacterKinematics characterKinematicsScript;
       [Header("Inventory")]
       [Tooltip("Inventory.")]
       [SerializeField]
       private InventoryBehaviour inventory;
       [Header("Cameras")]
       [Tooltip("Normal Camera.")]
       [SerializeField]
       private Camera cameraWorld;
       [SerializeField, Range(0.1f, 10f)]
       private float mouseSensitivity = 1.0f;
       [Header("Animation")]
       [Tooltip("Determines how smooth the locomotion blendspace é.")]
       [SerializeField]
       private float dampTimeLocomotion = 0.15f;
       [Tooltip("How smoothly we play aiming transitions. Beware that this affects lots of things!")]
       [SerializeField]
       private float dampTimeAiming = 0.3f;
       [Header("Animation Procedural")]
       [Tooltip("Character Animator.")]
       [SerializeField]
       private Animator characterAnimator;
       [Header("Gestão de Estado de Morte")]
       [Tooltip("Referência ao script que gere a morte/respawn.")]
       [SerializeField] private PlayerDeathAndRespawn deathStateController;
        [Header("Animation Third Person")]
        [Tooltip("Arrasta aqui o Animator do corpo (o que os outros veem).")]
        [SerializeField] private Animator tpAnimator;
        private AudioSource weaponAudioSource;
       #endregion
       #region FIELDS
       private bool aiming;
       private bool running;
       private bool holstered;
       private float lastShotTime;
       private int layerOverlay;
       private int layerHolster;
       private int layerActions;
       private CharacterKinematics characterKinematics;
       private WeaponBehaviour equippedWeapon;
       private WeaponAttachmentManagerBehaviour weaponAttachmentManager;
       private ScopeBehaviour equippedWeaponScope;
       private MagazineBehaviour equippedWeaponMagazine;
       private bool reloading;
       private bool inspecting;
       private bool holstering;
       private Vector2 axisLook;
       private Vector2 axisMovement;
       private bool holdingButtonAim;
       private bool holdingButtonRun;
       private bool holdingButtonFire;
       private bool tutorialTextVisible;
       private bool cursorLocked;
       private Camera[] allCameras;
       private AudioListener[] allAudioListeners;
       private PlayerInput cachedPlayerInput;
       #endregion
       #region CONSTANTS
       private static readonly int HashAimingAlpha = Animator.StringToHash("Aiming");
       private static readonly int HashMovement = Animator.StringToHash("Movement");
       #endregion
       public override void OnNetworkSpawn()
       {
           base.OnNetworkSpawn();
           if (!playerCamera) playerCamera = GetComponentInChildren<Camera>(true);
           if (!audioListener) audioListener = GetComponentInChildren<AudioListener>(true);
           if (!characterKinematicsScript) characterKinematicsScript = GetComponent<CharacterKinematics>();
           if (!weaponAudioSource) weaponAudioSource = GetComponentInChildren<AudioSource>(true);
           allCameras = GetComponentsInChildren<Camera>(true);
           allAudioListeners = GetComponentsInChildren<AudioListener>(true);
           if (cachedPlayerInput == null)
               cachedPlayerInput = GetComponent<PlayerInput>();
           bool owner = IsOwner;
           SetRenderersEnabled(firstPersonOnlyRenderers, owner);
           SetRenderersEnabled(thirdPersonOnlyRenderers, !owner);
           if (allCameras != null)
           {
               foreach (var cam in allCameras)
               {
                   if (cam != null)
                       cam.enabled = owner;
               }
           }
           if (playerCamera != null)
           {
              playerCamera.enabled = owner;
              if (owner)
              {
                 playerCamera.nearClipPlane = 0.03f;
                 playerCamera.farClipPlane = 2000f;
              }
           }
           if (allAudioListeners != null)
           {
               foreach (var al in allAudioListeners)
               {
                   if (al != null)
                       al.enabled = owner;
               }
           }
           if (audioListener) audioListener.enabled = owner;
           if (characterKinematicsScript) characterKinematicsScript.enabled = owner;
           if (cachedPlayerInput != null)
               cachedPlayerInput.enabled = owner;
           else if (GetComponent<PlayerInput>() is PlayerInput pi)
               pi.enabled = owner;
           if (owner)
           {
               cursorLocked = true;
               UpdateCursorState(); 
               if (TryGetComponent<NetworkSpawnHandler>(out var spawnHandler))
               {
                   spawnHandler.OnNetworkSpawn(); 
               }
               if(inventory != null) inventory.Init(); 
               if(inventory != null) RefreshWeaponSetup();
           }
           else
           {
               cursorLocked = false; 
               Cursor.lockState = CursorLockMode.None;
               Cursor.visible = true;
           }
       }
       public override void OnNetworkDespawn()
       {
           base.OnNetworkDespawn();
           if (IsOwner)
           {
               Cursor.lockState = CursorLockMode.None;
               Cursor.visible = true;
           }
       }
       #region UNITY
       protected void Awake()
       {
          if (!playerCamera) playerCamera = GetComponentInChildren<Camera>(true);
          if (!audioListener) audioListener = GetComponentInChildren<AudioListener>(true);
          if (!characterKinematicsScript) characterKinematicsScript = GetComponent<CharacterKinematics>();
          if (weaponAudioSource == null)
              weaponAudioSource = GetComponentInChildren<AudioSource>(true);
          allCameras = GetComponentsInChildren<Camera>(true);
          allAudioListeners = GetComponentsInChildren<AudioListener>(true);
          cachedPlayerInput = GetComponent<PlayerInput>();
          #region Lock Cursor
          cursorLocked = true;
          #endregion
          characterKinematics = GetComponent<CharacterKinematics>();
          if (!deathStateController) deathStateController = GetComponent<PlayerDeathAndRespawn>();
       }
       protected void Start()
       {
          layerHolster = characterAnimator.GetLayerIndex("Layer Holster");
          layerActions = characterAnimator.GetLayerIndex("Layer Actions");
          layerOverlay = characterAnimator.GetLayerIndex("Layer Overlay");
          float savedSensitivity = PlayerPrefs.GetFloat("settings_sensitivity", 1.0f);
          SetMouseSensitivity(savedSensitivity);
       }
       protected void Update()
       {
          if (!IsOwner) return; 
          if (!CanProcessInput())
          {
             axisMovement = Vector2.zero;
             holdingButtonFire = false;
             holdingButtonAim = false;
             return; 
          }
          if (reloading)
          {
              var stateInfo = characterAnimator.GetCurrentAnimatorStateInfo(layerActions);
              if (!stateInfo.IsName("Reload") && !stateInfo.IsName("Reload Empty"))
              {
                  reloading = false;
              }
          }
          if (holstering)
          {
              var holsterInfo = characterAnimator.GetCurrentAnimatorStateInfo(layerHolster);
              if (!holsterInfo.IsName("Holster"))
              {
                  holstering = false;
              }
          }
          aiming = holdingButtonAim && CanAim();
          running = holdingButtonRun && CanRun();
          if (holdingButtonFire)
          {
             if (equippedWeapon == null) return; 
             if (CanPlayAnimationFire() && equippedWeapon.HasAmmunition() && equippedWeapon.IsAutomatic())
             {
                if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                   Fire();
             }  
          }
          UpdateAnimator();
       }
       protected void LateUpdate()
       {
          if (!IsOwner) return; 
          if (equippedWeapon == null)
             return;
          if (equippedWeaponScope == null)
             return;
          if(characterKinematics != null)
          {
             characterKinematics.Compute();
          }
       }
       #endregion
       #region GETTERS
       public Camera GetCameraWorld() => cameraWorld;
       public InventoryBehaviour GetInventory() => inventory;
       public bool IsCrosshairVisible() => !aiming && !holstered;
       public bool IsRunning() => running;
       public bool IsAiming() => aiming;
       public bool IsCursorLocked() => cursorLocked;
       public bool IsTutorialTextVisible() => tutorialTextVisible;
       public Vector2 GetInputMovement() => axisMovement;
       public Vector2 GetInputLook() => axisLook;
        #endregion
        #region METHODS
        private void UpdateAnimator()
        {
            characterAnimator.SetFloat(HashMovement, Mathf.Clamp01(Mathf.Abs(axisMovement.x) + Mathf.Abs(axisMovement.y)), dampTimeLocomotion, Time.deltaTime);
            characterAnimator.SetFloat(HashAimingAlpha, Convert.ToSingle(aiming), 0.25f / 1.0f * dampTimeAiming, Time.deltaTime);
            characterAnimator.SetBool("Aim", aiming);
            characterAnimator.SetBool("Running", running);
            if (tpAnimator != null)
            {
                float moveValue = Mathf.Clamp01(Mathf.Abs(axisMovement.x) + Mathf.Abs(axisMovement.y));
                tpAnimator.SetFloat("Movement", moveValue);
                tpAnimator.SetBool("shoot", holdingButtonFire);
                if (deathStateController != null)
                    tpAnimator.SetBool("die", !deathStateController.IsPlayerControlled);
            }
        }
        private void Inspect()
       {
          inspecting = true;
          characterAnimator.CrossFade("Inspect", 0.0f, layerActions, 0);
       }
       private void Fire()
       {
          lastShotTime = Time.time;
          equippedWeapon.Fire();
          const string stateName = "Fire";
          characterAnimator.CrossFade(stateName, 0.05f, layerOverlay, 0);
       }
       private bool CanProcessInput()
       {
          if (!IsOwner) return false;
          if (deathStateController != null && !deathStateController.IsPlayerControlled)
          {
             return !cursorLocked;
          }
          return cursorLocked;
       }
       private void PlayReloadAnimation()
       {
          if (equippedWeapon == null)
              return;
          #region Animation
          string stateName = equippedWeapon.HasAmmunition() ? "Reload" : "Reload Empty";
          characterAnimator.Play(stateName, layerActions, 0.0f);
          reloading = true;
          #endregion
          AudioClip reloadClip = equippedWeapon.HasAmmunition()
              ? equippedWeapon.GetAudioClipReload()
              : equippedWeapon.GetAudioClipReloadEmpty();
          if (reloadClip != null && weaponAudioSource != null)
          {
              weaponAudioSource.PlayOneShot(reloadClip);
          }
          equippedWeapon.Reload();
       }
       private IEnumerator Equip(int index = 0)
       {
          if(!holstered)
          {
             SetHolstered(holstering = true);
             yield return new WaitUntil(() => holstering == false);
          }
          SetHolstered(false);
          characterAnimator.Play("Unholster", layerHolster, 0);
          inventory.Equip(index);
          RefreshWeaponSetup();
       }
       private void RefreshWeaponSetup()
       {
          if ((equippedWeapon = inventory.GetEquipped()) == null)
             return;
          characterAnimator.runtimeAnimatorController = equippedWeapon.GetAnimatorController();
          weaponAttachmentManager = equippedWeapon.GetAttachmentManager();
          if (weaponAttachmentManager == null) 
             return;
          equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();
          equippedWeaponMagazine = weaponAttachmentManager.GetEquippedMagazine();
       }
       private void FireEmpty()
       {
          lastShotTime = Time.time;
          characterAnimator.CrossFade("Fire Empty", 0.05f, layerOverlay, 0);
       }
       private void UpdateCursorState()
       {
          Cursor.visible = !cursorLocked;
          Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
       }
       private void SetHolstered(bool value = true)
       {
          holstered = value;
          const string boolName = "Holstered";
          characterAnimator.SetBool(boolName, holstered);    
       }
       #region ACTION CHECKS
       private bool CanPlayAnimationFire()
       {
          if (holstered || holstering)
             return false;
          if (reloading)
             return false;
          if (inspecting)
             return false;
          return true;
       }
       private bool CanPlayAnimationReload()
       {
          if (reloading)
             return false;
          if (inspecting)
             return false;
          return true;
       }
       private bool CanPlayAnimationHolster()
       {
          if (reloading)
             return false;
          if (inspecting)
             return false;
          return true;
       }
       private bool CanChangeWeapon()
       {
          if (holstering)
             return false;
          if (reloading)
             return false;
          if (inspecting)
             return false;
          return true;
       }
       private bool CanPlayAnimationInspect()
       {
          if (holstered || holstering)
             return false;
          if (reloading)
             return false;
          if (inspecting)
             return false;
          return true;
       }
       private bool CanAim()
       {
          if (holstered || inspecting)
             return false;
          if (reloading || holstering)
             return false;
          return true;
       }
       private bool CanRun()
       {
          if (inspecting)
             return false;
          if (reloading || aiming)
             return false;
          if (holdingButtonFire && equippedWeapon != null && equippedWeapon.HasAmmunition())
             return false;
          if (axisMovement.y <= 0 || Math.Abs(Mathf.Abs(axisMovement.x) - 1) < 0.01f)
             return false;
          return true;
       }
       #endregion
       #region INPUT
       public void OnTryFire(InputAction.CallbackContext context)
       {
          if (!CanProcessInput()) return;
          if (!IsOwner) return; 
          if (equippedWeapon == null) return; 
          if (!cursorLocked)
             return;
          switch (context)
          {
             case {phase: InputActionPhase.Started}:
                holdingButtonFire = true;
                break;
             case {phase: InputActionPhase.Performed}:
                if (!CanPlayAnimationFire())
                   break;
                if (equippedWeapon.HasAmmunition())
                {
                   if (equippedWeapon.IsAutomatic())
                      break;
                   if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                      Fire();
                }
                else
                   FireEmpty();
                break;
             case {phase: InputActionPhase.Canceled}:
                holdingButtonFire = false;
                break;
          }
       }
       public void OnTryPlayReload(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          if (!CanProcessInput()) return;
          if (!cursorLocked)
             return;
          if (!CanPlayAnimationReload())
             return;
          switch (context)
          {
             case {phase: InputActionPhase.Performed}:
                PlayReloadAnimation();
                break;
          }
       }
       public void OnTryInspect(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          if (!CanProcessInput()) return;
          if (!cursorLocked)
             return;
          if (!CanPlayAnimationInspect())
             return;
          switch (context)
          {
             case {phase: InputActionPhase.Performed}:
                Inspect();
                break;
          }
       }
       public void OnTryAiming(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          if (!CanProcessInput()) return;
          if (!cursorLocked)
             return;
          switch (context.phase)
          {
             case InputActionPhase.Started:
                holdingButtonAim = true;
                break;
             case InputActionPhase.Canceled:
                holdingButtonAim = false;
                break;
          }
       }
       public void OnTryHolster(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          if (!CanProcessInput()) return;
          if (!cursorLocked)
             return;
          switch (context.phase)
          {
             case InputActionPhase.Performed:
                if (CanPlayAnimationHolster())
                {
                   SetHolstered(!holstered);
                   holstering = true;
                }
                break;
          }
       }
       public void OnTryRun(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          if (!CanProcessInput()) return;
          if (!cursorLocked)
             return;
          switch (context.phase)
          {
             case InputActionPhase.Started:
                holdingButtonRun = true;
                break;
             case InputActionPhase.Canceled:
                holdingButtonRun = false;
                break;
          }
       }
       public void OnTryInventoryNext(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          if (!CanProcessInput()) return;
          if (!cursorLocked)
             return;
          if (inventory == null)
             return;
          switch (context)
          {
             case {phase: InputActionPhase.Performed}:
                float scrollValue = context.valueType.IsEquivalentTo(typeof(Vector2)) ? Mathf.Sign(context.ReadValue<Vector2>().y) : 1.0f;
                int indexNext = scrollValue > 0 ? inventory.GetNextIndex() : inventory.GetLastIndex();
                int indexCurrent = inventory.GetEquippedIndex();
                if (CanChangeWeapon() && (indexCurrent != indexNext))
                   StartCoroutine(nameof(Equip), indexNext);
                break;
          }
       }
       public void OnLockCursor(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          switch (context)
          {
             case {phase: InputActionPhase.Performed}:
                cursorLocked = !cursorLocked;
                UpdateCursorState();
                break;
          }
       }
       public void OnMove(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          if (!CanProcessInput()) 
          {
             axisMovement = Vector2.zero; 
             return; 
          }
          axisMovement = cursorLocked ? context.ReadValue<Vector2>() : default;
       }
       public void OnLook(InputAction.CallbackContext context)
       {
          if (!IsOwner) return;
          if (!CanProcessInput())
          {
             axisLook = Vector2.zero;
             return;
          }
          axisLook = cursorLocked ? context.ReadValue<Vector2>() * mouseSensitivity : default;
       }
       public void OnUpdateTutorial(InputAction.CallbackContext context)
       {
          if (!IsOwner) return; 
          tutorialTextVisible = context switch
          {
             {phase: InputActionPhase.Started} => true,
             {phase: InputActionPhase.Canceled} => false,
             _ => tutorialTextVisible
          };
       }
       private void SetRenderersEnabled(Renderer[] renderers, bool enabled)
       {
           if (renderers == null) return;
           foreach (var r in renderers)
           {
               if (r != null)
                   r.enabled = enabled;
           }
       }
       public void SetMouseSensitivity(float value)
       {
          mouseSensitivity = value;
       }
       #endregion
       #region ANIMATION EVENTS
       public void EjectCasing()
       {
          if(equippedWeapon != null)
             equippedWeapon.EjectCasing();
       }
       public void FillAmmunition(int amount)
       {
          if(equippedWeapon != null)
             equippedWeapon.FillAmmunition(amount);
       }
       public void SetActiveMagazine(int active)
       {
          if(equippedWeaponMagazine != null) equippedWeaponMagazine.gameObject.SetActive(active != 0);
       }
       public void AnimationEndedReload()
       {
          reloading = false;
       }
       public void AnimationEndedInspect()
       {
          inspecting = false;
       }
       public void AnimationEndedHolster()
       {
          holstering = false;
       }
       #endregion
    }
    #endregion
}