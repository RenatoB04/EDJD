using UnityEngine;
public static class GameInfo
{
    public static int MyChosenTeam { get; private set; } = 0;
    private const string PrefKey = "MyChosenTeam";
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadSavedTeam()
    {
        if (PlayerPrefs.HasKey(PrefKey))
            MyChosenTeam = PlayerPrefs.GetInt(PrefKey, 0);
    }
    public static void SetTeam(int team)
    {
        MyChosenTeam = Mathf.Clamp(team, 0, 1);
        PlayerPrefs.SetInt(PrefKey, MyChosenTeam);
        PlayerPrefs.Save();
        Debug.Log($"[GameInfo] Equipa escolhida: {MyChosenTeam}");
    }
}