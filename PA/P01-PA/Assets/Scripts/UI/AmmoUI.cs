using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    public static AmmoUI Instance; // Instância singleton para fácil acesso à UI de munição

    private void Awake()
    {
        // Garante que só existe uma instância da UI
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public TextMeshProUGUI ammoText; // Referência ao texto que mostra a munição

    // Atualiza a UI com a munição atual e a reserva
    public void Set(int inMag, int reserve)
    {
        if (!ammoText) return;
        ammoText.text = $"{inMag}/{reserve}";
    }

    // Atualiza a UI com o nome da arma (atualmente ignora o nome)
    public void Set(string _weaponName, int inMag, int reserve)
    {
        Set(inMag, reserve);
    }

    // Limpa o texto da UI
    public void Clear()
    {
        if (ammoText) ammoText.text = "";
    }
}
