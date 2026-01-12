using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons in order: 1 = Rifle, 2 = Pistol, 3 = Knife")]
    public GameObject[] weapons; // Lista de armas na ordem do UI ou slots

    [Header("Input Actions")]
    public InputActionReference weapon1; // Tecla para selecionar arma 1
    public InputActionReference weapon2; // Tecla para selecionar arma 2
    public InputActionReference weapon3; // Tecla para selecionar arma 3

    int currentWeaponIndex = 0; // Índice da arma atualmente selecionada

    void OnEnable()
    {
        // Ativa os inputs
        if (weapon1 != null) weapon1.action.Enable();
        if (weapon2 != null) weapon2.action.Enable();
        if (weapon3 != null) weapon3.action.Enable();
    }

    void OnDisable()
    {
        // Desativa os inputs
        if (weapon1 != null) weapon1.action.Disable();
        if (weapon2 != null) weapon2.action.Disable();
        if (weapon3 != null) weapon3.action.Disable();
    }

    void Start()
    {
        // Seleciona a arma inicial
        SelectWeapon(currentWeaponIndex);
    }

    void Update()
    {
        if (PauseMenuManager.IsPaused) return; // Não troca de arma se estiver pausado

        // Troca de arma com os inputs correspondentes
        if (weapon1 != null && weapon1.action.triggered) SelectWeapon(0); 
        if (weapon2 != null && weapon2.action.triggered) SelectWeapon(1);
        if (weapon3 != null && weapon3.action.triggered) SelectWeapon(2); 
    }

    void SelectWeapon(int index)
    {
        if (weapons == null || index < 0 || index >= weapons.Length) return;

        // Ativa apenas a arma escolhida e desativa as restantes
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);

        currentWeaponIndex = index;
    }

    public GameObject GetActiveWeapon()
    {
        if (weapons == null || weapons.Length == 0) return null;

        // Retorna a arma atualmente ativa
        return weapons[currentWeaponIndex];
    }
}
