using UnityEngine;
using UnityEngine.UI;
public class LobbyTeamUI : MonoBehaviour
{
    [Header("Botões de equipa")]
    public Button buttonTeamA;
    public Button buttonTeamB;
    [Header("Opcional: texto de estado")]
    public Text statusText;
    void Awake()
    {
        if (buttonTeamA) buttonTeamA.onClick.AddListener(() => OnTeamPicked(0));
        if (buttonTeamB) buttonTeamB.onClick.AddListener(() => OnTeamPicked(1));
        UpdateStatus();
    }
    void OnTeamPicked(int team)
    {
        GameInfo.SetTeam(team);
        UpdateStatus();
    }
    void UpdateStatus()
    {
        if (statusText)
            statusText.text = $"Equipa atual: {(GameInfo.MyChosenTeam == 0 ? "A" : "B")}";
    }
    public void SetTeamButtonsInteractable(bool interactable)
    {
        if (buttonTeamA) buttonTeamA.interactable = interactable;
        if (buttonTeamB) buttonTeamB.interactable = interactable;
    }
}