using UnityEngine;
using Unity.Netcode;

public class FirstPersonViewFix : NetworkBehaviour
{
    [Header("Refs")]
    [Tooltip("Root dos braços/arma de 1.ª pessoa (viewmodel).")]
    public GameObject firstPersonRoot;

    [Tooltip("Câmara principal do jogador (Main Camera).")]
    public Camera mainCamera;

    [Tooltip("Câmara de armas/braços (se o kit usar uma), opcional.")]
    public Camera weaponCamera;

    [Header("Layer do Viewmodel")]
    [Tooltip("Layer dedicada aos braços/arma. Cria em Project Settings → Tags and Layers.")]
    public string firstPersonLayerName = "FirstPerson";

    [Header("Áudio (opcional)")]
    public AudioListener audioListener; 

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Tentativa de encontrar câmaras e audioListener se não foram atribuídos no Inspector
        if (mainCamera == null) mainCamera = GetComponentInChildren<Camera>(true);
        if (audioListener == null) audioListener = GetComponentInChildren<AudioListener>(true);

        // Habilita câmaras e áudio apenas para o jogador dono (IsOwner)
        if (mainCamera) mainCamera.enabled = IsOwner;
        if (weaponCamera) weaponCamera.enabled = IsOwner;
        if (audioListener) audioListener.enabled = IsOwner;

        if (!IsOwner) return; // Só aplica configuração local para o dono do objecto

        if (firstPersonRoot == null || mainCamera == null)
        {
            Debug.LogWarning("[FirstPersonViewFix] Faltam refs (firstPersonRoot/mainCamera).");
            return;
        }

        // Tenta obter a layer do viewmodel
        int fpLayer = LayerMask.NameToLayer(firstPersonLayerName);
        if (fpLayer < 0)
        {
            Debug.LogWarning($"[FirstPersonViewFix] A layer '{firstPersonLayerName}' não existe. " +
                             "Cria-a em Project Settings → Tags and Layers. " +
                             "Vou continuar sem mexer em layers (pode continuar duplicado se tiveres duas câmaras).");
        }
        else
        {
            // Aplica a layer recursivamente a todos os filhos do firstPersonRoot
            SetLayerRecursively(firstPersonRoot, fpLayer);
        }

        // Configuração de duas câmaras (Main + Weapon)
        if (weaponCamera != null && fpLayer >= 0)
        {
            // MainCamera não vê o layer do viewmodel
            int maskMain = mainCamera.cullingMask;
            maskMain &= ~(1 << fpLayer);
            mainCamera.cullingMask = maskMain;

            // WeaponCamera só vê o layer do viewmodel
            weaponCamera.cullingMask = (1 << fpLayer);
            weaponCamera.clearFlags = CameraClearFlags.Depth;
            weaponCamera.depth = Mathf.Max(mainCamera.depth + 1f, mainCamera.depth + 1f);

            // Desativa AudioListener da WeaponCamera se existir
            var wl = weaponCamera.GetComponent<AudioListener>();
            if (wl) wl.enabled = false;

            // Ajusta clipping planes para evitar clipping próximo
            weaponCamera.nearClipPlane = 0.01f;
            weaponCamera.farClipPlane = 500f;

            Debug.Log("[FirstPersonViewFix] Configuração TWO-CAM aplicada (Main exclui FirstPerson; WeaponCamera só FirstPerson).");
        }
        else
        {
            // Caso não haja WeaponCamera, usa apenas a MainCamera
            Debug.Log("[FirstPersonViewFix] Configuração SINGLE-CAM (sem WeaponCamera).");
        }
    }

    // Função auxiliar para definir layer recursivamente em todos os filhos
    private static void SetLayerRecursively(GameObject go, int layer)
    {
        if (!go) return;
        go.layer = layer;
        foreach (Transform t in go.transform)
            if (t) SetLayerRecursively(t.gameObject, layer);
    }
}
