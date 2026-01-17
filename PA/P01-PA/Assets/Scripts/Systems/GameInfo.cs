using UnityEngine;

public static class GameInfo
{
    // Guarda a equipa escolhida pelo jogador
    public static int MyChosenTeam { get; private set; } = 0;

    private const string PrefKey = "MyChosenTeam"; // Chave usada no PlayerPrefs

    // Este método é chamado automaticamente antes de qualquer cena carregar
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadSavedTeam()
    {
        // Se já existir uma equipa guardada, carrega-a
        if (PlayerPrefs.HasKey(PrefKey))
            MyChosenTeam = PlayerPrefs.GetInt(PrefKey, 0);
    }

    // Define a equipa escolhida pelo jogador
    public static void SetTeam(int team)
    {
        MyChosenTeam = Mathf.Clamp(team, 0, 1); // Garante que só é 0 ou 1
        PlayerPrefs.SetInt(PrefKey, MyChosenTeam); // Guarda no disco
        PlayerPrefs.Save();
        Debug.Log($"[GameInfo] Equipa escolhida: {MyChosenTeam}");
    }
}
