using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletProjectile : NetworkBehaviour
{
    [Header("Dano")]
    public float damage = 20f; // Quantidade de dano do projéctil

    [Header("Vida útil")]
    public float lifeTime = 5f; // Tempo de vida do projéctil antes de se destruir automaticamente

    [Header("Filtro (opcional)")]
    [Tooltip("Layers que o projéctil pode atingir. Por defeito: todas (~0).")]
    public LayerMask hittableLayers = ~0; // Filtro de layers que o projéctil pode atingir

    [HideInInspector] public int   ownerTeam     = -1; // Equipa do dono do projéctil
    [HideInInspector] public Transform ownerRoot = null; // Transform raiz do dono (para evitar friendly fire)
    [HideInInspector] public ulong ownerClientId = ulong.MaxValue; // ClientId do dono (para scoreboard e logs)

    // Velocidade inicial sincronizada para todos os clientes
    public NetworkVariable<Vector3> initialVelocity = new NetworkVariable<Vector3>(
        Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool hasHit = false; // Marca se já colidiu para evitar múltiplos hits
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Melhor detecção para projécteis rápidos
        }
        var col = GetComponent<Collider>();
        if (col) col.enabled = true; // Garante que o collider está ativo
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Clientes aplicam a velocidade inicial localmente
        if (!IsServer && rb != null)
        {
            if (initialVelocity.Value != Vector3.zero)
                rb.linearVelocity = initialVelocity.Value;
        }

        // Servidor agenda a destruição do projéctil após lifeTime segundos
        if (IsServer)
            Invoke(nameof(ServerLifetimeEnd), lifeTime);
    }

    void ServerLifetimeEnd()
    {
        if (!IsServer) return;
        var no = GetComponent<NetworkObject>();
        if (no && no.IsSpawned) no.Despawn(); // Despawns para Netcode
        else Destroy(gameObject); // Fallback para caso não esteja spawnado
    }

    void OnCollisionEnter(Collision c)
    {
        if (!IsServer) return; // Apenas o servidor processa hits
        if (hasHit) return;    // Evita múltiplas colisões

        // Evita aplicar dano ao próprio atirador comparando a raiz do transform
        if (ownerRoot && c.transform.root == ownerRoot)
        {
            Debug.Log($"[Bullet] Ignorado (collision): colisão com ownerRoot ({ownerRoot.name}). collider={c.collider.name}");
            return;
        }

        // Verifica se a layer está dentro das layers que o projéctil pode atingir
        if (((1 << c.gameObject.layer) & hittableLayers) == 0)
        {
            Debug.Log($"[Bullet] Ignorado (collision): layer {c.gameObject.layer} não está em hittableLayers.");
            ServerCleanup();
            return;
        }

        Vector3 hitPos = transform.position;
        if (c.contactCount > 0) hitPos = c.GetContact(0).point; // Pega o ponto real de contacto
        ProcessHitServer(c.collider, hitPos);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (hasHit) return;

        // Evita aplicar dano ao próprio atirador
        if (ownerRoot && other.transform.root == ownerRoot)
        {
            Debug.Log($"[Bullet] Ignorado (trigger): colisão com ownerRoot ({ownerRoot.name}). collider={other.name}");
            return;
        }

        // Filtro de layers
        if (((1 << other.gameObject.layer) & hittableLayers) == 0)
        {
            Debug.Log($"[Bullet] Ignorado (trigger): layer {other.gameObject.layer} não está em hittableLayers.");
            ServerCleanup();
            return;
        }

        ProcessHitServer(other, transform.position);
    }

    private void ProcessHitServer(Collider col, Vector3 hitPos)
    {
        if (hasHit) return;
        hasHit = true;

        Debug.Log($"[Bullet] ProcessHitServer: collider={col.name}, root={col.transform.root.name}, layer={col.gameObject.layer}, ownerClientId={ownerClientId}, ownerTeam={ownerTeam}, ownerRoot={(ownerRoot? ownerRoot.name : "null")}");

        // 1) Tenta obter Health no parent chain do collider (alvo directo)
        var targetHealth = col.GetComponentInParent<Health>();

        // 2) Se não encontrou, tenta GetComponentInChildren no root
        if (targetHealth == null)
        {
            var root = col.transform.root;
            targetHealth = root.GetComponentInChildren<Health>(true);
            if (targetHealth != null)
            {
                Debug.Log($"[Bullet] Health encontrado via GetComponentInChildren no root '{root.name}' -> health on '{targetHealth.name}'.");
            }
        }

        // 3) Se ainda não encontrou, faz OverlapSphere como fallback
        if (targetHealth == null)
        {
            Collider[] nearby = Physics.OverlapSphere(hitPos, 0.25f, hittableLayers, QueryTriggerInteraction.Ignore);
            Debug.Log($"[Bullet] OverlapSphere fallback: encontrou {nearby.Length} colliders próximos.");
            foreach (var nc in nearby)
            {
                var hh = nc.GetComponentInParent<Health>() ?? nc.GetComponentInChildren<Health>(true);
                if (hh != null)
                {
                    // Preferência para Health do mesmo root do collider
                    if (nc.transform.root == col.transform.root)
                    {
                        targetHealth = hh;
                        Debug.Log($"[Bullet] Escolhido Health preferencial (mesmo root) = {hh.name} (via collider {nc.name}).");
                        break;
                    }
                    // Caso contrário, guarda primeiro candidato
                    if (targetHealth == null)
                    {
                        targetHealth = hh;
                        Debug.Log($"[Bullet] Candidate Health (via OverlapSphere) = {hh.name} (collider {nc.name}).");
                    }
                }
            }
        }

        if (targetHealth == null)
        {
            Debug.Log($"[Bullet] No Health found on collided object {col.name} (root={col.transform.root.name}). Não apliquei dano.");
            ServerCleanup();
            return;
        }

        // Aplica dano com checagens de segurança (owner/friendly)
        bool applied = TryApplyDamageTo(targetHealth, hitPos);
        if (!applied)
        {
            Debug.Log($"[Bullet] Dano NÃO aplicado a {targetHealth.name}. Razão nos logs acima.");
        }

        ServerCleanup();
    }

    private bool TryApplyDamageTo(Health h, Vector3 hitPos)
    {
        if (h == null) return false;

        // Evita aplicar ao próprio atirador
        if (ownerRoot != null && h.transform.root == ownerRoot)
        {
            Debug.Log($"[Bullet] ApplyDamage skipped: target root == ownerRoot ({ownerRoot.name})");
            return false;
        }

        // Friendly fire check
        int targetTeam = h != null ? h.team.Value : -1;
        int instigatorTeam = ownerTeam;
        if (targetTeam != -1 && instigatorTeam != -1 && targetTeam == instigatorTeam)
        {
            Debug.Log($"[Bullet] ApplyDamage skipped por Friendly Fire: targetTeam={targetTeam}, instigatorTeam={instigatorTeam}");
            return false;
        }

        // Aplica dano no servidor
        try
        {
            h.ApplyDamageServer(damage, instigatorTeam, ownerClientId, hitPos, true);
            Debug.Log($"[Bullet] Applied {damage} to {h.name} (team target={h.team.Value}, team owner={instigatorTeam}, ownerClientId={ownerClientId})");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Bullet] Exception applying damage to {h.name}: {ex}");
            return false;
        }
    }

    [ClientRpc]
    void HitmarkerClientRpc(float dealt, string victimName, ClientRpcParams rpcParams = default)
    {
        // Mostra hitmarker no UI
        if (DamageFeedUI.Instance)
            DamageFeedUI.Instance.Push(dealt, false, victimName);
        CrosshairUI.Instance?.ShowHit();
    }

    private void ServerCleanup()
    {
        // Limpeza do projéctil no servidor
        if (!IsServer) return;
        var no = GetComponent<NetworkObject>();
        if (no && no.IsSpawned) no.Despawn();
        else Destroy(gameObject);
    }
}
