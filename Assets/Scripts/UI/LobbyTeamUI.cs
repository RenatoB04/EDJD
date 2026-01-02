using UnityEngine;
using UnityEngine.UI;

public class LobbyTeamUI : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("0 = Equipa A (Azul), 1 = Equipa B (Laranja)")]
    public int teamToJoin = 0; 

    [Header("Referências")]
    [SerializeField] private Button myButton; 

    void Start()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnBtnClick);
        }
    }

    void OnBtnClick()
    {
        // EM VEZ DE PROCURAR O PLAYER, GUARDAMOS APENAS A ESCOLHA NO "CARTÃO"
        GameInfo.MyChosenTeam = teamToJoin;

        // Feedback visual para saberes que clicaste (opcional)
        Debug.Log($"Escolha guardada: Vou ser da Equipa {teamToJoin} quando o jogo começar!");
    }
}