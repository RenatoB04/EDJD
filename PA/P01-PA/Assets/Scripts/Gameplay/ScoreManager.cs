using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Singleton para fácil acesso a partir de outros scripts
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] TMP_Text scoreText; // Texto da UI para mostrar o score

    [Header("Valores")]
    public int pointsPerKill = 10; // Pontos ganhos por cada kill
    public int Score { get; private set; } // Pontuação atual

    void Awake()
    {
        // Singleton pattern: garante que só exista uma instância
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;

        // Atualiza UI com o valor inicial
        UpdateUI();
    }

    void OnEnable()
    {
        // Subscrição ao evento global quando qualquer bot morre
        BOTDeath.OnAnyBotKilled += AddKillPoints;
    }

    void OnDisable()
    {
        // Remove subscrição ao evento para evitar leaks
        BOTDeath.OnAnyBotKilled -= AddKillPoints;
    }

    // Incrementa a pontuação quando um bot é morto
    void AddKillPoints()
    {
        Score += pointsPerKill;
        UpdateUI();
    }

    // Atualiza a UI do score
    void UpdateUI()
    {
        if (scoreText) 
            scoreText.text = $"Score: {Score}";
    }

    // Reseta a pontuação para zero
    public void ResetScore()
    {
        Score = 0;
        UpdateUI();
    }
}
