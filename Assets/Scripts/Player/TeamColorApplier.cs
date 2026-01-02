using UnityEngine;
using Unity.Netcode;

public class TeamColorApplier : NetworkBehaviour
{
    [Header("Onde aplicar a tinta?")]
    [Tooltip("Arrasta o objeto 'Body' ou 'Mesh' do boneco para aqui.")]
    public Renderer targetRenderer;

    [Header("Paleta de Cores")]
    public Color botColor = Color.red;       // Inimigos (Bots)
    public Color teamAColor = Color.blue;    // Equipa 0
    public Color teamBColor = new Color(1f, 0.5f, 0f); // Laranja (Equipa 1)

    private Health myHealth;

    public override void OnNetworkSpawn()
    {
        myHealth = GetComponent<Health>();
        
        // Espera 0.1s para garantir que a variável de equipa (Team) chegou da rede
        Invoke(nameof(UpdateColor), 0.1f);

        // Se a equipa mudar a meio do jogo, muda a cor também
        if (myHealth != null)
        {
            myHealth.team.OnValueChanged += OnTeamChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (myHealth != null)
            myHealth.team.OnValueChanged -= OnTeamChanged;
    }

    void OnTeamChanged(int oldTeam, int newTeam)
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        if (targetRenderer == null || myHealth == null) return;

        // Cria uma cópia do material para não estragar o original do projeto
        Material newMat = new Material(targetRenderer.material);
        int currentTeam = myHealth.team.Value;

        // --- Lógica de Seleção de Cor ---
        if (currentTeam == -2) 
        {
            // É um BOT
            newMat.color = botColor;
            newMat.SetColor("_EmissionColor", botColor * 0.5f); // Brilho ligeiro
        }
        else if (currentTeam == 0)
        {
            // TEAM A
            newMat.color = teamAColor;
            newMat.SetColor("_EmissionColor", teamAColor * 0.5f);
        }
        else
        {
            // TEAM B (ou qualquer outra)
            newMat.color = teamBColor;
            newMat.SetColor("_EmissionColor", teamBColor * 0.5f);
        }

        // Aplica a cor final
        targetRenderer.material = newMat;
    }
}