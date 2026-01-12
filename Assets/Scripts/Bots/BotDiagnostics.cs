using System;
using UnityEngine;

[DisallowMultipleComponent] // Garante que só existe um componente deste tipo por GameObject
public class BotDiagnostics : MonoBehaviour
{
    [Tooltip("Se true, mostra logs detalhados sobre colisões e mudanças de vida.")]
    public bool verbose = true; // Ativa/desativa logs detalhados

    private Health health;         // Referência ao script de Health do bot
    private Collider anyCollider;   // Collider do bot
    private Rigidbody anyRigidbody; // Rigidbody do bot
    private float lastHealthValue = float.MinValue; // Guarda o último valor de vida para detectar mudanças
    private string id; // Identificador único do bot (nome + ID de instância)

    void Awake()
    {
        // Cria ID único para logs
        id = $"{gameObject.name}#{GetInstanceID()}";

        // Procura componente Health no próprio GameObject ou nos filhos
        health = GetComponentInChildren<Health>() ?? GetComponent<Health>();

        // Procura Collider e Rigidbody (próprio ou filhos)
        anyCollider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        anyRigidbody = GetComponent<Rigidbody>() ?? GetComponentInChildren<Rigidbody>();

        // Log inicial de debug
        Debug.Log($"[BotDiagnostics] ({id}) Awake. health={(health!=null)}, collider={(anyCollider!=null)}, rb={(anyRigidbody!=null)}, layer={gameObject.layer}.");
    }

    void Start()
    {
        if (health != null)
        {
            // Guarda a vida inicial
            lastHealthValue = health != null ? health.currentHealth.Value : float.NaN;
            if (verbose) Debug.Log($"[BotDiagnostics] ({id}) Start: HP inicial = {lastHealthValue}");
        }
        else
        {
            Debug.LogWarning($"[BotDiagnostics] ({id}) Start: Health NÃO encontrado no bot. GetComponentInChildren<Health() retornou null.");
        }
    }

    void Update()
    {
        // Verifica mudanças na vida do bot
        if (health != null)
        {
            float curr = health.currentHealth.Value;
            if (!Mathf.Approximately(curr, lastHealthValue))
            {
                Debug.Log($"[BotDiagnostics] ({id}) HP mudou: {lastHealthValue} -> {curr}");
                lastHealthValue = curr;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!verbose) return;

        var col = collision.collider;
        // Log detalhado da colisão com ponto de contacto
        LogCollision("OnCollisionEnter", col, collision.contacts.Length > 0 ? collision.GetContact(0).point : (Vector3?)null);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!verbose) return;
        // Log de trigger (sem ponto de contacto)
        LogCollision("OnTriggerEnter", other, null);
    }

    // Função central de logging de colisões
    private void LogCollision(string evt, Collider col, Vector3? hitPoint)
    {
        string rootName = col.transform.root ? col.transform.root.name : "null";
        string colliderName = col.name;
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        string s = $"[BotDiagnostics] ({id}) {evt}: collider={colliderName} root={rootName} layer={col.gameObject.layer}({layerName})";
        if (hitPoint.HasValue) s += $" hitPos={hitPoint.Value}";

        Debug.Log(s);

        // Se a colisão foi com um projétil, log detalhado do bullet
        var bullet = col.GetComponentInParent<BulletProjectile>() ?? col.GetComponentInChildren<BulletProjectile>();
        if (bullet != null)
        {
            int ownerTeam = -999;
            try { ownerTeam = bullet.ownerTeam; } catch { }
            var ownerRootName = bullet.ownerRoot ? bullet.ownerRoot.name : "null";

            Debug.Log($"[BotDiagnostics] ({id}) Colidido por BulletProjectile: ownerClientId={bullet.ownerClientId}, ownerTeam={ownerTeam}, ownerRoot={ownerRootName}, damage={bullet.damage}, initialVelocity={bullet.initialVelocity.Value}");
        }
    }

    [ContextMenu("DumpHealthState")]
    public void DumpHealthState()
    {
        if (health == null)
        {
            Debug.Log($"[BotDiagnostics] ({id}) DumpHealthState: Health null.");
            return;
        }

        // Log completo do estado do Health
        Debug.Log($"[BotDiagnostics] ({id}) DumpHealthState: currentHealth={health.currentHealth.Value} maxHealth={health.maxHealth} isDead={health.isDead.Value} team={health.team.Value}");
    }
}
