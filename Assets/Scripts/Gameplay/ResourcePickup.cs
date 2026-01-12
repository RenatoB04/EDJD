using UnityEngine;
using System.Collections;
using System.Reflection;

public class ResourcePickup : MonoBehaviour
{
    [Header("Recursos")]
    public float healthAmount = 0f;  // Quantidade de vida a restaurar
    [Tooltip("Quantidade de balas a adicionar (ex: 30 para um carregador)")]
    public int ammoReserveAmount = 30;  // Quantidade de munição a adicionar
    public string targetTag = "Player"; // Tag do alvo que pode apanhar o pickup

    [Header("Efeitos")]
    public AudioClip pickupSound; // Som a tocar ao apanhar
    public GameObject pickupVFX; // Efeito visual ao apanhar

    bool pickedUp = false; // Evita múltiplas ativações

    // Detetado colisão com trigger
    void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return; // Ignora se já foi apanhado
        if (!other.CompareTag(targetTag) && !other.transform.root.CompareTag(targetTag))
            return; // Apenas ativa para o tag correto

        pickedUp = true;

        Transform playerRoot = other.transform.root;

        // Tenta obter componente Health do player
        Health playerHealth = other.GetComponent<Health>() ?? playerRoot.GetComponent<Health>();
        bool appliedHealth = false;
        bool appliedAmmo = false;

        // Aplica vida se necessário
        if (healthAmount > 0f)
        {
            if (playerHealth != null)
            {
                playerHealth.Heal(healthAmount);
                appliedHealth = true;
            }
            else
            {
                Debug.LogWarning("[Pickup] Player sem componente Health.");
            }
        }

        // Aplica munição se necessário
        if (ammoReserveAmount > 0)
        {
            appliedAmmo = true;
        }

        // Se aplicou qualquer coisa, toca som e VFX
        if (appliedHealth || appliedAmmo)
        {
            if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (pickupVFX) Instantiate(pickupVFX, transform.position, Quaternion.identity);

            var col = GetComponent<Collider>();
            if (col) col.enabled = false; // Desativa collider para não recolher duas vezes

            // Se é munição, tenta aplicar depois de 1 frame
            if (appliedAmmo)
            {
                StartCoroutine(TryApplyAmmoAndDestroy(playerRoot));
            }
            else
            {
                Destroy(gameObject); // Apenas vida, destrói imediatamente
            }
        }
    }

    // Aplica munição à arma ativa do jogador e destrói o pickup
    IEnumerator TryApplyAmmoAndDestroy(Transform playerRoot)
    {
        // Obtém todas as configs de arma do player
        WeaponConfig[] configs = playerRoot.GetComponentsInChildren<WeaponConfig>(true);
        if (configs.Length == 0)
        {
            Debug.LogError("[Pickup] Nenhum WeaponConfig encontrado!");
            Destroy(gameObject);
            yield break;
        }

        bool foundActiveWeapon = false;
        Component activeWeaponComp = null;
        FieldInfo activeCurrentField = null;
        FieldInfo activeReserveField = null;

        // Procura pela arma ativa
        foreach (var cfg in configs)
        {
            if (!cfg.gameObject.activeInHierarchy)
            {
                Debug.Log($"[Pickup] ⏭️ {cfg.name} está inativa, a ignorar.");
                continue;
            }

            Component[] allComps = cfg.GetComponents<Component>();
            foreach (var comp in allComps)
            {
                if (comp == null) continue;

                var compType = comp.GetType();

                // Verifica se é uma arma do namespace InfimaGames
                if (compType.Name == "Weapon" && compType.Namespace != null && compType.Namespace.Contains("InfimaGames"))
                {
                    try
                    {
                        // Usa reflection para obter campos privados de munição
                        FieldInfo currentField = compType.GetField("ammunitionCurrent", BindingFlags.NonPublic | BindingFlags.Instance);
                        FieldInfo reserveField = compType.GetField("ammunitionReserve", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (reserveField != null && currentField != null)
                        {
                            int currentAmmo = (int)currentField.GetValue(comp);
                            int reserveBefore = (int)reserveField.GetValue(comp);

                            Debug.Log($"[Pickup] 📌 {cfg.name} está EQUIPADA! Munição ANTES: {currentAmmo}/{reserveBefore}");

                            int newReserve = reserveBefore + ammoReserveAmount;
                            reserveField.SetValue(comp, newReserve);

                            Debug.Log($"[Pickup] ✅ {cfg.name}:  {reserveBefore} → {newReserve} (+{ammoReserveAmount})");

                            foundActiveWeapon = true;
                            activeWeaponComp = comp;
                            activeCurrentField = currentField;
                            activeReserveField = reserveField;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[Pickup] ❌ Erro:  {ex.Message}");
                    }

                    break; // Só precisa de encontrar uma arma
                }
            }

            if (foundActiveWeapon)
                break;
        }

        // Atualiza UI se possível
        if (foundActiveWeapon && activeWeaponComp != null)
        {
            yield return null; // espera 1 frame

            try
            {
                int currentAmmoAfter = (int)activeCurrentField.GetValue(activeWeaponComp);
                int reserveAfter = (int)activeReserveField.GetValue(activeWeaponComp);

                if (AmmoUI.Instance != null)
                {
                    AmmoUI.Instance.Set(currentAmmoAfter, reserveAfter);
                    Debug.Log($"[Pickup] ✅ UI atualizada para {currentAmmoAfter}/{reserveAfter}!");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Pickup] ❌ Erro ao atualizar UI: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("[Pickup] ⚠️ Nenhuma arma ATIVA encontrada!");
        }

        Destroy(gameObject); // Destrói o pickup no final
    }
}
