using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LossProbe : NetworkBehaviour
{
    // Instância singleton para acesso global
    public static LossProbe Instance { get; private set; }

    [SerializeField] float interval = 0.5f;   // Intervalo entre envios de probes (em segundos)
    [SerializeField] float window = 10f;      // Janela de tempo para cálculo de perda (em segundos)

    public float CurrentLossPercent { get; private set; } = -1f; // Percentagem de perda de pacotes atual

    ulong _seq = 0; // Sequência incremental de probes

    // Filas para guardar probes enviados e recebidos (eco)
    readonly Queue<(float time, ulong seq)> sent = new();
    readonly Queue<(float time, ulong seq)> echoed = new();

    float _timer; // Contador interno para controlar envio de probes

    void Awake() => Instance = this; // Inicializa singleton

    void OnDestroy()
    {
        if (Instance == this) Instance = null; // Limpa singleton se for a instância atual
    }

    void Update()
    {
        // Só processa se houver NetworkManager ativo
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsListening) return;

        // Cenários server-only ou host (server+client) não têm perda local
        if (IsServer && !IsClient) { CurrentLossPercent = 0f; return; }
        if (IsServer && IsClient) { CurrentLossPercent = 0f; return; }

        // Incrementa timer
        _timer += Time.unscaledDeltaTime;

        // Envia probe quando o intervalo é atingido
        if (_timer >= interval)
        {
            _timer = 0f;
            _seq++; // Incrementa sequência
            SendProbeServerRpc(_seq); // Envia probe para server
            sent.Enqueue((Time.unscaledTime, _seq)); // Guarda timestamp e sequência
        }

        // Remove probes antigos fora da janela de tempo
        float cutoff = Time.unscaledTime - window;
        while (sent.Count > 0 && sent.Peek().time < cutoff) sent.Dequeue();
        while (echoed.Count > 0 && echoed.Peek().time < cutoff) echoed.Dequeue();

        // Calcula percentagem de perda de pacotes
        if (sent.Count > 0)
        {
            int s = sent.Count;   // Total enviado
            int r = echoed.Count; // Total eco recebido
            int loss = Mathf.Clamp(s - r, 0, s); // Número de perdas
            CurrentLossPercent = (loss * 100f) / s;
        }
        else
        {
            CurrentLossPercent = -1f; // Sem dados
        }
    }

    // Server RPC chamado pelo cliente para enviar probe
    [ServerRpc(RequireOwnership = false)]
    void SendProbeServerRpc(ulong seq, ServerRpcParams rpcParams = default)
    {
        // Cria parâmetros para enviar apenas de volta ao cliente que chamou
        var target = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { rpcParams.Receive.SenderClientId } }
        };

        // Eco de volta para o cliente
        EchoClientRpc(seq, target);
    }

    // Cliente recebe eco do server e regista
    [ClientRpc]
    void EchoClientRpc(ulong seq, ClientRpcParams rpcParams = default)
    {
        echoed.Enqueue((Time.unscaledTime, seq));
    }
}
