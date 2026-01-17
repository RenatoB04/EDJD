using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using Unity.Netcode;
using System.Collections;

public class Health : NetworkBehaviour
{
    [Header("Config")]
    public float maxHealth = 100f; // Vida máxima do jogador

    [Header("Network")]
    // Vida atual replicada na rede
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Estado de vida (morto/vivo) replicado
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Equipa do jogador
    public NetworkVariable<int> team = new NetworkVariable<int>(
        -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged; // Evento quando a vida muda
    public UnityEvent OnDied; // Evento quando o jogador morre
    public event Action<float, Transform> OnTookDamage; // Evento de dano recebido

    [Header("UI")]
    [HideInInspector] public TextMeshProUGUI healthText; // Referência ao texto UI de vida

    PlayerShield playerShield; // Referência ao escudo do jogador
    ulong lastInstigatorClientId = ulong.MaxValue; // Último jogador a causar dano
    Coroutine uiFinderCo; // Coroutine para procurar a UI
    float timeOfDeath = -999f; // Tempo da morte para respawn

    void Awake()
    {
        // Obtém referência ao escudo
        playerShield = GetComponent<PlayerShield>();
        UpdateHealthUI(maxHealth); // Inicializa UI com vida máxima
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Define equipa por defeito se ainda não estiver definida
            if (team.Value == -1)
            {
                if (GetComponent<BotAI_Proto>() != null)
                    team.Value = -2; // Bots têm equipa -2
                else
                    team.Value = (int)OwnerClientId; // Jogadores usam ClientId como equipa inicial
            }

            currentHealth.Value = maxHealth;
            isDead.Value = false;
        }

        // Liga callbacks para mudanças de vida e morte
        currentHealth.OnValueChanged += OnHealthValueChanged;
        isDead.OnValueChanged += OnIsDeadChanged;

        UpdateHealthUI(currentHealth.Value);
        OnHealthChanged?.Invoke(currentHealth.Value, maxHealth);

        if (IsOwner)
        {
            uiFinderCo = StartCoroutine(FindUI()); // Procura a UI de vida
            int myChoice = GameInfo.MyChosenTeam;
            ChangeTeamServerRpc(myChoice); // Define a equipa escolhida pelo jogador
        }
    }

    IEnumerator FindUI()
    {
        // Procura a UI durante 600 frames (~10s se 60FPS)
        for (int i = 0; i < 600; i++)
        {
            var obj = GameObject.FindWithTag("HealthText");
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI txt))
            {
                healthText = txt;
                UpdateHealthUI(currentHealth.Value);
                yield break;
            }
            yield return null;
        }
    }

    public override void OnNetworkDespawn()
    {
        // Desliga callbacks quando o objeto é removido
        currentHealth.OnValueChanged -= OnHealthValueChanged;
        isDead.OnValueChanged -= OnIsDeadChanged;
        if (uiFinderCo != null) StopCoroutine(uiFinderCo);
    }

    void OnHealthValueChanged(float prev, float curr)
    {
        UpdateHealthUI(curr); // Atualiza UI
        OnHealthChanged?.Invoke(curr, maxHealth); // Dispara evento
    }

    void OnIsDeadChanged(bool prev, bool curr)
    {
        if (curr && !prev)
            OnDied?.Invoke(); // Dispara evento de morte
    }

    public void ApplyDamageServer(
        float amount,
        int instigatorTeam,
        ulong instigatorClientId,
        Vector3 hitWorldPos,
        bool showIndicator = true)
    {
        if (!IsServer) return;
        if (isDead.Value) return;
        if (amount <= 0f) return;
        if (instigatorTeam == -1) return;
        if (team.Value == instigatorTeam) return; // Não aplica dano à equipa

        // Aplica escudo se ativo
        if (playerShield != null && playerShield.IsShieldActive.Value)
        {
            amount = playerShield.AbsorbDamageServer(amount);
            if (amount <= 0.01f) return;
        }

        float oldHp = currentHealth.Value;
        float newHp = Mathf.Clamp(oldHp - amount, 0f, maxHealth);
        if (Mathf.Approximately(oldHp, newHp)) return;

        currentHealth.Value = newHp;
        lastInstigatorClientId = instigatorClientId;
        OnTookDamage?.Invoke(amount, null);

        // Mostra indicador de dano na UI do jogador
        if (showIndicator)
        {
            var rpc = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } }
            };
            DamageIndicatorClientRpc(hitWorldPos, amount, rpc);
        }

        // Verifica se morreu
        if (newHp <= 0f)
        {
            isDead.Value = true;
            timeOfDeath = Time.time;
            TryAwardKill();
        }
    }

    [ClientRpc]
    void DamageIndicatorClientRpc(Vector3 pos, float dmg, ClientRpcParams rpc = default)
    {
        if (!IsOwner) return;
        if (DamageIndicatorUI.Instance != null)
            DamageIndicatorUI.Instance.RegisterHit(pos, dmg); // Atualiza UI de dano
    }

    void TryAwardKill()
    {
        if (!IsServer) return;
        if (lastInstigatorClientId == ulong.MaxValue) return;
        if (lastInstigatorClientId == OwnerClientId) return;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(lastInstigatorClientId, out var client))
        {
            var score = client.PlayerObject.GetComponent<PlayerScore>();
            if (score != null)
                score.AwardKillAndPoints(); // Dá pontos ao jogador que matou
        }

        lastInstigatorClientId = ulong.MaxValue;
    }

    public void Heal(float amount)
    {
        if (!IsServer)
        {
            HealServerRpc(amount);
            return;
        }
        ApplyHeal(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    void HealServerRpc(float amount)
    {
        ApplyHeal(amount);
    }

    void ApplyHeal(float amount)
    {
        if (isDead.Value) return;
        if (amount <= 0f) return;

        currentHealth.Value = Mathf.Min(maxHealth, currentHealth.Value + amount); // Cura sem ultrapassar o máximo
    }

    public void ResetFullHealth()
    {
        if (!IsServer)
            ResetHealthServerRpc();
        else
            ApplyRespawn();
    }

    [ServerRpc(RequireOwnership = false)]
    void ResetHealthServerRpc()
    {
        ApplyRespawn();
    }

    void ApplyRespawn()
    {
        if (!isDead.Value) return;
        if (Time.time < timeOfDeath + 2f) return; // Delay mínimo antes de respawn

        currentHealth.Value = maxHealth;
        isDead.Value = false;
    }

    void UpdateHealthUI(float v)
    {
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(v)}"; // Mostra vida arredondada
    }

    public void ChangeTeam(int newTeamIndex)
    {
        if (IsOwner) 
        {
            ChangeTeamServerRpc(newTeamIndex); // Envia pedido para o servidor
        }
    }

    [ServerRpc]
    private void ChangeTeamServerRpc(int newTeamIndex)
    {
        team.Value = newTeamIndex; // Altera equipa no servidor
    }
}
