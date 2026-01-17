using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Estrutura que representa o estado do cooldown de uma habilidade.
/// </summary>
public struct AbilityCooldownState : INetworkSerializable, IEquatable<AbilityCooldownState>
{
    public FixedString32Bytes Id;   // Identificador único da habilidade
    public double EndTime;          // Momento (NetworkTime) em que o cooldown termina

    // Serialização para Netcode
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref EndTime);
    }

    // Comparação de igualdade aproximada (considera tolerância para doubles)
    public bool Equals(AbilityCooldownState other) => Id.Equals(other.Id) && Math.Abs(EndTime - other.EndTime) < 0.0001;
}

/// <summary>
/// Gestão de cooldowns de habilidades em rede.
/// </summary>
public class AbilityCooldowns : NetworkBehaviour
{
    public NetworkList<AbilityCooldownState> Cooldowns; // Lista de cooldowns sincronizada em rede
    private readonly Dictionary<string, int> indexById = new(); // Mapeamento rápido Id -> índice na NetworkList

    void Awake()
    {
        // Inicializa a NetworkList
        Cooldowns = new NetworkList<AbilityCooldownState>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Assina evento quando a lista muda
        Cooldowns.OnListChanged += OnCooldownsChanged;

        // Apenas servidor: poderia inicializar aqui habilidades se necessário
        if (IsServer && Cooldowns.Count == 0)
        {
        }
    }

    public override void OnNetworkDespawn()
    {
        // Remove listener para evitar leaks
        Cooldowns.OnListChanged -= OnCooldownsChanged;
        base.OnNetworkDespawn();
    }

    // Atualiza o dicionário sempre que a lista muda
    private void OnCooldownsChanged(NetworkListEvent<AbilityCooldownState> change)
    {
        indexById.Clear();
        for (int i = 0; i < Cooldowns.Count; i++)
            indexById[Cooldowns[i].Id.ToString()] = i;
    }

    /// <summary>
    /// Regista uma nova habilidade no servidor.
    /// </summary>
    public void RegisterAbilityServer(string id)
    {
        if (!IsServer) return; // Apenas servidor manipula NetworkList
        if (indexById.ContainsKey(id)) return; // Já existe

        var state = new AbilityCooldownState
        {
            Id = new FixedString32Bytes(id),
            EndTime = 0
        };
        Cooldowns.Add(state);
        indexById[id] = Cooldowns.Count - 1;
    }

    /// <summary>
    /// Tenta usar a habilidade no servidor. Retorna true se sucesso (cooldown expirado).
    /// </summary>
    public bool TryUseAbilityServer(string id, float cooldownSeconds)
    {
        if (!IsServer) return false;
        int idx = EnsureIndexServer(id);
        double now = NetworkManager.LocalTime.Time;
        var st = Cooldowns[idx];
        if (st.EndTime <= now)
        {
            st.EndTime = now + Mathf.Max(0.01f, cooldownSeconds); // Garante cooldown mínimo
            Cooldowns[idx] = st; 
            return true;
        }
        return false;
    }

    /// <summary>
    /// Define o cooldown de uma habilidade a partir do tempo atual.
    /// </summary>
    public void SetCooldownServer(string id, float secondsFromNow)
    {
        if (!IsServer) return;
        int idx = EnsureIndexServer(id);
        double now = NetworkManager.LocalTime.Time;
        var st = Cooldowns[idx];
        st.EndTime = now + Mathf.Max(0f, secondsFromNow);
        Cooldowns[idx] = st;
    }

    /// <summary>
    /// Obtém o tempo restante do cooldown (em segundos) para uma habilidade.
    /// </summary>
    public float GetRemaining(string id)
    {
        if (!indexById.TryGetValue(id, out int idx)) return 0f;
        double now = NetworkManager ? NetworkManager.LocalTime.Time : Time.unscaledTimeAsDouble;
        double remain = Cooldowns[idx].EndTime - now;
        return (float)Math.Max(0.0, remain);
    }

    /// <summary>
    /// Indica se a habilidade está pronta a ser usada.
    /// </summary>
    public bool IsReady(string id) => GetRemaining(id) <= 0.0001f;

    // Garante que a habilidade existe na lista e retorna o índice
    private int EnsureIndexServer(string id)
    {
        if (indexById.TryGetValue(id, out int idx)) return idx;

        var state = new AbilityCooldownState
        {
            Id = new FixedString32Bytes(id),
            EndTime = 0
        };
        Cooldowns.Add(state);
        idx = Cooldowns.Count - 1;
        indexById[id] = idx;
        return idx;
    }

    /// <summary>
    /// Pedido de cliente para usar uma habilidade (servidor valida e aplica cooldown)
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void RequestUseAbilityServerRpc(string id, float cooldownSeconds)
    {
        TryUseAbilityServer(id, cooldownSeconds);
    }
}
