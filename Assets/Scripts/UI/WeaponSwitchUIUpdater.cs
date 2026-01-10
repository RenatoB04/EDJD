using UnityEngine;
using System. Reflection;

/// <summary>
/// Atualiza a UI de munição sempre que o jogador muda de arma. 
/// Coloca este script no GameObject do PLAYER (root).
/// </summary>
public class WeaponSwitchUIUpdater : MonoBehaviour
{
    [Tooltip("Intervalo de verificação (em segundos)")]
    public float updateInterval = 0.2f;

    private WeaponConfig lastActiveWeapon = null;
    private float nextUpdateTime = 0f;

    void Update()
    {
        // Verifica periodicamente (para não sobrecarregar)
        if (Time.time < nextUpdateTime)
            return;

        nextUpdateTime = Time.time + updateInterval;

        // Procura a arma atualmente ativa
        WeaponConfig[] allWeapons = GetComponentsInChildren<WeaponConfig>(true);
        WeaponConfig activeWeapon = null;

        foreach (var weapon in allWeapons)
        {
            if (weapon. gameObject.activeInHierarchy)
            {
                activeWeapon = weapon;
                break;
            }
        }

        // Se mudou de arma, atualiza a UI
        if (activeWeapon != null && activeWeapon != lastActiveWeapon)
        {
            lastActiveWeapon = activeWeapon;
            UpdateAmmoUI(activeWeapon);
        }
    }

    void UpdateAmmoUI(WeaponConfig weaponConfig)
    {
        // Procura o componente Weapon do Infima Games
        Component[] allComps = weaponConfig.GetComponents<Component>();

        foreach (var comp in allComps)
        {
            if (comp == null) continue;

            var compType = comp.GetType();

            if (compType.Name == "Weapon" && compType.Namespace != null && compType.Namespace.Contains("InfimaGames"))
            {
                try
                {
                    // Lê os campos de munição via reflexão
                    FieldInfo currentField = compType.GetField("ammunitionCurrent", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo reserveField = compType.GetField("ammunitionReserve", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (currentField != null && reserveField != null)
                    {
                        int currentAmmo = (int)currentField.GetValue(comp);
                        int reserveAmmo = (int)reserveField.GetValue(comp);

                        // Atualiza a UI
                        if (AmmoUI. Instance != null)
                        {
                            AmmoUI.Instance.Set(currentAmmo, reserveAmmo);
                            Debug.Log($"[WeaponSwitch] UI atualizada:  {weaponConfig.name} → {currentAmmo}/{reserveAmmo}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[WeaponSwitch] Campos de munição não encontrados em {weaponConfig.name}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[WeaponSwitch] Erro ao ler munição: {ex.Message}");
                }

                break; // Já processou o Weapon
            }
        }
    }

    // Força atualização quando o script é ativado (útil para quando o jogador spawna)
    void OnEnable()
    {
        nextUpdateTime = 0f; // Atualiza imediatamente
    }
}