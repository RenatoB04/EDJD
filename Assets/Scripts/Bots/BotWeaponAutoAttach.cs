using UnityEngine;

public class BotWeaponAutoAttach : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Onde a arma vai ficar presa (mão, WeaponHolder, etc.).")]
    public Transform weaponHolder; // Ponto de fixação da arma no bot

    [Tooltip("Prefab da arma do bot (rifle por defeito).")]
    public GameObject weaponPrefab; // Prefab da arma que será instanciada

    [Tooltip("Nome do transform dentro da arma que será usado como ponta do cano (shoot point).")]
    public string muzzleTransformName = "Muzzle"; // Nome do transform usado como ponto de disparo

    [Header("Opções")]
    [Tooltip("Destruir qualquer arma que já esteja como filho do holder.")]
    public bool clearExistingChildren = true; // Se true, remove armas antigas do holder

    private BotCombat combat; // Referência ao componente BotCombat do bot

    void Awake()
    {
        combat = GetComponent<BotCombat>(); // Obtém o BotCombat

        // Verificações de segurança
        if (!weaponHolder)
        {
            Debug.LogWarning($"[BotWeaponAutoAttach] {name}: weaponHolder não está definido.");
            return;
        }
        if (!weaponPrefab)
        {
            Debug.LogWarning($"[BotWeaponAutoAttach] {name}: weaponPrefab não está definido.");
            return;
        }

        // Limpar armas existentes, se definido
        if (clearExistingChildren)
        {
            for (int i = weaponHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(weaponHolder.GetChild(i).gameObject);
            }
        }

        // Instancia o prefab da arma e define posição, rotação e escala
        GameObject weaponInstance = Instantiate(weaponPrefab, weaponHolder);
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;
        weaponInstance.transform.localScale = Vector3.one;

        // Configura o ponto de disparo no BotCombat
        if (combat != null)
        {
            Transform muzzle = null;

            // Procura pelo transform da ponta do cano na arma
            if (!string.IsNullOrEmpty(muzzleTransformName))
            {
                var allChildren = weaponInstance.GetComponentsInChildren<Transform>();
                foreach (var t in allChildren)
                {
                    if (t.name == muzzleTransformName)
                    {
                        muzzle = t;
                        break;
                    }
                }
            }

            // Se não encontrou, usa o root da arma como fallback
            if (!muzzle) muzzle = weaponInstance.transform;

            // Define shootPoint e eyes no BotCombat
            combat.shootPoint = muzzle;
            if (!combat.eyes)
                combat.eyes = muzzle;
        }
    }
}
