using UnityEngine;
using System.Reflection;

public class WeaponSwitchUIUpdater : MonoBehaviour
{
    [Tooltip("Intervalo de verificação (em segundos)")]
    public float updateInterval = 0.2f;  // Tempo entre verificações da arma ativa

    private WeaponConfig lastActiveWeapon = null;  // Arma ativa anterior
    private float nextUpdateTime = 0f;             // Tempo da próxima verificação

    void Update()
    {
        // Aguarda até ao próximo intervalo
        if (Time.time < nextUpdateTime)
            return;

        nextUpdateTime = Time.time + updateInterval;

        // Procura todas as armas no jogador
        WeaponConfig[] allWeapons = GetComponentsInChildren<WeaponConfig>(true);
        WeaponConfig activeWeapon = null;

        foreach (var weapon in allWeapons)
        {
            if (weapon.gameObject.activeInHierarchy)
            {
                activeWeapon = weapon;
                break; // Pega na primeira arma ativa
            }
        }

        // Se mudou a arma ativa, atualiza a UI
        if (activeWeapon != null && activeWeapon != lastActiveWeapon)
        {
            lastActiveWeapon = activeWeapon;
            UpdateAmmoUI(activeWeapon);
        }
    }

    void UpdateAmmoUI(WeaponConfig weaponConfig)
    {
        // Procura o script "Weapon" do pacote InfimaGames
        Component[] allComps = weaponConfig.GetComponents<Component>();
        foreach (var comp in allComps)
        {
            if (comp == null) continue;

            var compType = comp.GetType();

            if (compType.Name == "Weapon" && compType.Namespace != null && compType.Namespace.Contains("InfimaGames"))
            {
                try
                {
                    // Usa reflection para aceder a campos privados de munição
                    FieldInfo currentField = compType.GetField("ammunitionCurrent", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo reserveField = compType.GetField("ammunitionReserve", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (currentField != null && reserveField != null)
                    {
                        int currentAmmo = (int)currentField.GetValue(comp);
                        int reserveAmmo = (int)reserveField.GetValue(comp);

                        if (AmmoUI.Instance != null)
                        {
                            AmmoUI.Instance.Set(currentAmmo, reserveAmmo);
                            Debug.Log($"[WeaponSwitch] UI atualizada: {weaponConfig.name} → {currentAmmo}/{reserveAmmo}");
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

                break; // Já encontrou a arma, não precisa continuar
            }
        }
    }

    void OnEnable()
    {
        nextUpdateTime = 0f; // Reseta timer ao ativar o script
    }
}
