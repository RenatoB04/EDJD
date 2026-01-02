using UnityEngine;

public static class GameInfo
{
    // Equipa local escolhida: 0 = Equipa A, 1 = Equipa B
    public static int MyChosenTeam { get; private set; } = 0;

    private const string PrefKey = "MyChosenTeam";

    // Carrega a última equipa gravada antes de qualquer cena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadSavedTeam()
    {
        if (PlayerPrefs.HasKey(PrefKey))
            MyChosenTeam = PlayerPrefs.GetInt(PrefKey, 0);
    }

    // Chama quando o jogador escolhe a equipa (UI do lobby)
    public static void SetTeam(int team)
    {
        MyChosenTeam = Mathf.Clamp(team, 0, 1);
        PlayerPrefs.SetInt(PrefKey, MyChosenTeam);
        PlayerPrefs.Save();
        Debug.Log($"[GameInfo] Equipa escolhida: {MyChosenTeam}");
    }
}