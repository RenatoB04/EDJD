using UnityEngine;
using System. Collections;
using System.Reflection;
public class ResourcePickup :  MonoBehaviour
{
    [Header("Recursos")]
    public float healthAmount = 0f;
    [Tooltip("Quantidade de balas a adicionar (ex: 30 para um carregador)")]
    public int ammoReserveAmount = 30;
    public string targetTag = "Player";
    [Header("Efeitos")]
    public AudioClip pickupSound;
    public GameObject pickupVFX;
    bool pickedUp = false;
    void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (!other.CompareTag(targetTag) && !other.transform.root.CompareTag(targetTag))
            return;
        pickedUp = true;
        Transform playerRoot = other.transform. root;
        Health playerHealth = other.GetComponent<Health>() ?? playerRoot.GetComponent<Health>();
        bool appliedHealth = false;
        bool appliedAmmo = false;
        if (healthAmount > 0f)
        {
            if (playerHealth != null)
            {
                playerHealth. Heal(healthAmount);
                appliedHealth = true;
            }
            else
            {
                Debug.LogWarning("[Pickup] Player sem componente Health.");
            }
        }
        if (ammoReserveAmount > 0)
        {
            appliedAmmo = true;
        }
        if (appliedHealth || appliedAmmo)
        {
            if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform. position);
            if (pickupVFX) Instantiate(pickupVFX, transform. position, Quaternion.identity);
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
            if (appliedAmmo)
            {
                StartCoroutine(TryApplyAmmoAndDestroy(playerRoot));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    IEnumerator TryApplyAmmoAndDestroy(Transform playerRoot)
    {
        WeaponConfig[] configs = playerRoot.GetComponentsInChildren<WeaponConfig>(true);
        if (configs. Length == 0)
        {
            Debug.LogError("[Pickup] Nenhum WeaponConfig encontrado!");
            Destroy(gameObject);
            yield break;
        }
        bool foundActiveWeapon = false;
        Component activeWeaponComp = null;
        FieldInfo activeCurrentField = null;
        FieldInfo activeReserveField = null;
        foreach (var cfg in configs)
        {
            if (! cfg.gameObject.activeInHierarchy)
            {
                Debug.Log($"[Pickup] ⏭️ {cfg.name} está inativa, a ignorar.");
                continue;
            }
            Component[] allComps = cfg.GetComponents<Component>();
            foreach (var comp in allComps)
            {
                if (comp == null) continue;
                var compType = comp.GetType();
                if (compType.Name == "Weapon" && compType. Namespace != null && compType. Namespace.Contains("InfimaGames"))
                {
                    try
                    {
                        FieldInfo currentField = compType.GetField("ammunitionCurrent", BindingFlags.NonPublic | BindingFlags. Instance);
                        FieldInfo reserveField = compType.GetField("ammunitionReserve", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (reserveField != null && currentField != null)
                        {
                            int currentAmmo = (int)currentField.GetValue(comp);
                            int reserveBefore = (int)reserveField.GetValue(comp);
                            Debug. Log($"[Pickup] 📌 {cfg.name} está EQUIPADA! Munição ANTES: {currentAmmo}/{reserveBefore}");
                            int newReserve = reserveBefore + ammoReserveAmount;
                            reserveField. SetValue(comp, newReserve);
                            Debug.Log($"[Pickup] ✅ {cfg.name}:  {reserveBefore} → {newReserve} (+{ammoReserveAmount})");
                            foundActiveWeapon = true;
                            activeWeaponComp = comp;
                            activeCurrentField = currentField;
                            activeReserveField = reserveField;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug. LogError($"[Pickup] ❌ Erro:  {ex.Message}");
                    }
                    break;
                }
            }
            if (foundActiveWeapon)
                break;
        }
        if (foundActiveWeapon && activeWeaponComp != null)
        {
            yield return null;
            try
            {
                int currentAmmoAfter = (int)activeCurrentField.GetValue(activeWeaponComp);
                int reserveAfter = (int)activeReserveField.GetValue(activeWeaponComp);
                if (AmmoUI. Instance != null)
                {
                    AmmoUI.Instance. Set(currentAmmoAfter, reserveAfter);
                    Debug.Log($"[Pickup] ✅ UI atualizada para {currentAmmoAfter}/{reserveAfter}!");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Pickup] ❌ Erro ao atualizar UI: {ex. Message}");
            }
        }
        else
        {
            Debug.LogWarning("[Pickup] ⚠️ Nenhuma arma ATIVA encontrada!");
        }
        Destroy(gameObject);
    }
}