using UnityEngine;
using System.Collections;
using Unity.Netcode;
using InfimaGames.LowPolyShooterPack;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : NetworkBehaviour
{
    [Range(5, 100)]
    public float destroyAfter = 10f;

    public bool destroyOnImpact = false;

    public float minDestroyTime = 0.05f;
    public float maxDestroyTime = 0.25f;

    [Header("Damage")]
    [SerializeField] private float damage = 20f;

    [Header("Network Data")]
    [HideInInspector] public ulong ownerClientId = ulong.MaxValue;
    [HideInInspector] public int ownerTeam = -1;
    [HideInInspector] public Transform ownerRoot;

    public NetworkVariable<Vector3> initialVelocity =
        new NetworkVariable<Vector3>(Vector3.zero);

    [Header("Impact Effect Prefabs")]
    public Transform[] bloodImpactPrefabs;
    public Transform[] metalImpactPrefabs;
    public Transform[] dirtImpactPrefabs;
    public Transform[] concreteImpactPrefabs;

    private Rigidbody rb;
    private Collider projectileCollider;
    private bool hasHit = false;

    // ============================================================
    //                        NETWORK SPAWN
    // ============================================================
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();

        if (rb == null || projectileCollider == null)
        {
            if (IsSpawned) NetworkObject.Despawn(true);
            else Destroy(gameObject);
            return;
        }

        if (initialVelocity.Value != Vector3.zero)
            rb.linearVelocity = initialVelocity.Value;

        // Ignorar colisão com o próprio jogador (server-side)
        if (IsServer && ownerRoot != null)
        {
            var ownerColliders = ownerRoot.GetComponentsInChildren<Collider>();
            foreach (var col in ownerColliders)
                Physics.IgnoreCollision(col, projectileCollider, true);
        }

        StartCoroutine(DestroyAfter());
    }

    // ============================================================
    //                    PROCESSAMENTO DE IMPACTO
    // ============================================================
    private void ProcessHit(GameObject hitObject, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsServer) return;
        if (hasHit) return;
        hasHit = true;

        // Ignorar outras balas
        if (hitObject.GetComponent<Projectile>() != null)
            return;

        // Evitar auto-dano por root
        if (ownerRoot != null && hitObject.transform.root == ownerRoot)
            return;

        // ------------------ DANO ------------------
        var health = hitObject.GetComponentInParent<Health>();
        if (health != null)
        {
            health.ApplyDamageServer(
                damage,
                ownerTeam,
                ownerClientId,
                hitPoint,
                true
            );
        }

        // ------------------ EFEITOS ------------------
        string tag = hitObject.tag;

        if (tag == "Blood" && bloodImpactPrefabs.Length > 0)
        {
            SpawnImpact(bloodImpactPrefabs, hitPoint, hitNormal);
            DespawnSelf();
            return;
        }

        if (tag == "Metal" && metalImpactPrefabs.Length > 0)
        {
            SpawnImpact(metalImpactPrefabs, hitPoint, hitNormal);
            DespawnSelf();
            return;
        }

        if (tag == "Dirt" && dirtImpactPrefabs.Length > 0)
        {
            SpawnImpact(dirtImpactPrefabs, hitPoint, hitNormal);
            DespawnSelf();
            return;
        }

        if (tag == "Concrete" && concreteImpactPrefabs.Length > 0)
        {
            SpawnImpact(concreteImpactPrefabs, hitPoint, hitNormal);
            DespawnSelf();
            return;
        }

        if (tag == "Target")
        {
            var target = hitObject.GetComponent<TargetScript>();
            if (target != null) target.isHit = true;
            DespawnSelf();
            return;
        }

        if (tag == "ExplosiveBarrel")
        {
            var barrel = hitObject.GetComponent<ExplosiveBarrelScript>();
            if (barrel != null) barrel.explode = true;
            DespawnSelf();
            return;
        }

        if (tag == "GasTank")
        {
            var gas = hitObject.GetComponent<GasTankScript>();
            if (gas != null) gas.isHit = true;
            DespawnSelf();
            return;
        }

        if (destroyOnImpact)
            DespawnSelf();
        else
            StartCoroutine(DestroyTimer());
    }

    // ============================================================
    //                        COLISÕES
    // ============================================================
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        Vector3 point = collision.contactCount > 0
            ? collision.GetContact(0).point
            : transform.position;

        Vector3 normal = collision.contactCount > 0
            ? collision.GetContact(0).normal
            : -rb.linearVelocity.normalized;

        ProcessHit(collision.gameObject, point, normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Vector3 normal = rb != null && rb.linearVelocity != Vector3.zero
            ? -rb.linearVelocity.normalized
            : Vector3.up;

        ProcessHit(other.gameObject, transform.position, normal);
    }

    // ============================================================
    //                    UTILITÁRIOS
    // ============================================================
    private void SpawnImpact(Transform[] prefabs, Vector3 pos, Vector3 normal)
    {
        Instantiate(
            prefabs[Random.Range(0, prefabs.Length)],
            pos,
            Quaternion.LookRotation(normal)
        );
    }

    private void DespawnSelf()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
            NetworkObject.Despawn(true);
        else
            Destroy(gameObject);
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(Random.Range(minDestroyTime, maxDestroyTime));
        DespawnSelf();
    }

    private IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(destroyAfter);
        DespawnSelf();
    }
}
