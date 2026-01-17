using UnityEngine;
using Unity.Netcode;

public class NetcodeBootstrap : MonoBehaviour
{
    void Awake()
    {
        // Garante que só existe uma instância deste objecto
        var others = FindObjectsOfType<NetcodeBootstrap>();
        if (others.Length > 1) 
        { 
            Destroy(gameObject); // Se já existir outro, destrói este
            return; 
        }

        // Mantém este objecto entre mudanças de cena
        DontDestroyOnLoad(gameObject);
    }
}
