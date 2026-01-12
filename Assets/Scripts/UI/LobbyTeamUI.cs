using UnityEngine;
using UnityEngine.UI;

public class LobbyTeamUI : MonoBehaviour
{
    [Header("Botões de equipa")]
    public Button buttonTeamA; // Botão para escolher a equipa A
    public Button buttonTeamB; // Botão para escolher a equipa B

    [Header("Opcional: texto de estado")]
    public Text statusText; // Texto que mostra a equipa atualmente selecionada

    void Awake()
    {
        // Adiciona listeners aos botões para reagir à escolha de equipa
        if (buttonTeamA) buttonTeamA.onClick.AddListener(() => OnTeamPicked(0));
        if (buttonTeamB) buttonTeamB.onClick.AddListener(() => OnTeamPicked(1));

        // Atualiza o estado inicial do UI
        UpdateStatus();
    }

    // Chamado quando o jogador escolhe uma equipa
    void OnTeamPicked(int team)
    {
        GameInfo.SetTeam(team); // Atualiza a equipa escolhida no GameInfo
        UpdateStatus();         // Atualiza o UI para refletir a escolha
    }

    // Atualiza o texto do estado com a equipa atual
    void UpdateStatus()
    {
        if (statusText)
            statusText.text = $"Equipa atual: {(GameInfo.MyChosenTeam == 0 ? "A" : "B")}";
    }

    // Permite ativar ou desativar os botões de equipa
    public void SetTeamButtonsInteractable(bool interactable)
    {
        if (buttonTeamA) buttonTeamA.interactable = interactable;
        if (buttonTeamB) buttonTeamB.interactable = interactable;
    }
}
