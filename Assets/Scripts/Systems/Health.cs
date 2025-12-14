using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using Unity.Netcode;
using System.Collections;

public class Health : NetworkBehaviour
{
    [Header("Config")]
    public float maxHealth = 100f;

    [Header("Network")]
    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> team = new NetworkVariable<int>(
        -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("Events")]
    public UnityEvent<float, float> OnHealthChanged;
    public UnityEvent OnDied;
    public event Action<float, Transform> OnTookDamage;

    [Header("UI")]
    [HideInInspector] public TextMeshProUGUI healthText;

    PlayerShield playerShield;
    ulong lastInstigatorClientId = ulong.MaxValue;
    Coroutine uiFinderCo;

    float timeOfDeath = -999f;

    void Awake()
    {
        playerShield = GetComponent<PlayerShield>();
        UpdateHealthUI(maxHealth);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (team.Value == -1)
            {
                if (GetComponent<BotAI_Proto>() != null)
                    team.Value = -2; // BOT
                else
                    team.Value = (int)OwnerClientId; // PLAYER
            }

            currentHealth.Value = maxHealth;
            isDead.Value = false;
        }

        currentHealth.OnValueChanged += OnHealthValueChanged;
        isDead.OnValueChanged += OnIsDeadChanged;

        UpdateHealthUI(currentHealth.Value);
        OnHealthChanged?.Invoke(currentHealth.Value, maxHealth);

        if (IsOwner)
            uiFinderCo = StartCoroutine(FindUI());
    }

    IEnumerator FindUI()
    {
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
        currentHealth.OnValueChanged -= OnHealthValueChanged;
        isDead.OnValueChanged -= OnIsDeadChanged;
        if (uiFinderCo != null) StopCoroutine(uiFinderCo);
    }

    void OnHealthValueChanged(float prev, float curr)
    {
        UpdateHealthUI(curr);
        OnHealthChanged?.Invoke(curr, maxHealth);
    }

    void OnIsDeadChanged(bool prev, bool curr)
    {
        if (curr && !prev)
            OnDied?.Invoke();
    }

    // =========================
    //          DANO
    // =========================

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

        // Proteção absoluta contra instigator inválido
        if (instigatorTeam == -1)
            return;

        // Friendly fire OFF
        if (team.Value == instigatorTeam)
            return;

        // Escudo
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

        if (showIndicator)
        {
            var rpc = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } }
            };
            DamageIndicatorClientRpc(hitWorldPos, amount, rpc);
        }

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
            DamageIndicatorUI.Instance.RegisterHit(pos, dmg);
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
                score.AwardKillAndPoints();
        }

        lastInstigatorClientId = ulong.MaxValue;
    }

    // =========================
    //          CURA
    // =========================

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

        currentHealth.Value = Mathf.Min(maxHealth, currentHealth.Value + amount);
    }

    // =========================
    //        RESPAWN
    // =========================

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
        if (Time.time < timeOfDeath + 2f) return;

        currentHealth.Value = maxHealth;
        isDead.Value = false;
    }

    // =========================

    void UpdateHealthUI(float v)
    {
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(v)}";
    }
}
