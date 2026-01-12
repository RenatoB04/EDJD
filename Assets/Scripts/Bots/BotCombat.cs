using UnityEngine;
using Unity.Netcode;

public class BotCombat : NetworkBehaviour
{
    [Header("Referências")]
    public Transform shootPoint;       // Ponto de disparo do bot
    public Transform eyes;             // Ponto de visão / referência para rotação
    public string playerTag = "Player";

    [Header("Física e Layers")]
    public LayerMask playerLayer;      // Layer do jogador
    public LayerMask obstacleLayer;    // Layer de obstáculos para raycast

    [Header("Projétil (Netcode)")]
    public GameObject bulletPrefab;    // Prefab do projétil
    public float bulletSpeed = 60f;    // Velocidade do projétil

    [Header("Dificuldade - Curva de Mira (NOVO)")]
    [Tooltip("Define a imprecisão baseada na distância. Eixo X = Distância (m), Eixo Y = Erro (m).")]
    public AnimationCurve spreadOverDistance = new AnimationCurve(
        new Keyframe(0f, 0.1f),    // Curta distância: erro pequeno
        new Keyframe(20f, 0.5f),   // Médio alcance
        new Keyframe(50f, 2.5f),   // Alcance longo
        new Keyframe(100f, 6.0f)   // Muito longe: erro grande
    );

    [Header("Dificuldade / Nerf")]
    [Tooltip("Multiplicador final da curva. 1 = Normal, 0.5 = Sniper, 2 = Stormtrooper")]
    public float aimInaccuracyMultiplier = 1.0f; // Ajusta dificuldade da mira

    [Header("Arma: Rifle (Variáveis Finais)")]
    public int rifleMagSize = 30;     // Capacidade do magazine do rifle
    public int rifleReserveAmmo = 120;// Munição de reserva do rifle
    public float rifleFireRate = 8f;  // Tiros por segundo
    public float rifleReloadTime = 2.2f; // Tempo de recarga
    public float rifleDamage = 12f;   // Dano por tiro

    [Header("Arma: Pistola (Variáveis Finais)")]
    public int pistolMagSize = 12;
    public int pistolReserveAmmo = 48;
    public float pistolFireRate = 3f;
    public float pistolReloadTime = 1.2f;
    public float pistolDamage = 18f;

    [Header("Geral")]
    public float maxShootDistance = 150f;  // Distância máxima de disparo
    public bool drawDebugRays = false;     // Debug visual dos raycasts

    [Header("Dificuldade - Previsão")]
    [Range(0f, 1f)]
    public float leadAccuracy = 0.9f; // Quão preciso é ao prever movimento do alvo

    [Header("Debug - Diagnóstico ShootPoint")]
    public bool showShootPointGizmos = true;   // Mostrar gizmos do ponto de disparo
    public bool logShootingPosition = false;   // Log para debugging de tiro

    // Calcula a quantidade total de munição normalizada (0–1)
    public float AmmoNormalized
    {
        get
        {
            float curTotal = rifleMag + rifleRes + pistolMag + pistolRes;
            float maxTotal = rifleMagSize + rifleReserveAmmo + pistolMagSize + pistolReserveAmmo;
            if (maxTotal <= 0f) return 0f;
            return Mathf.Clamp01(curTotal / maxTotal);
        }
    }

    // --- Variáveis privadas ---
    private Transform currentTarget;          // Alvo atual
    private Rigidbody targetRbCache;          // Cache do Rigidbody do alvo
    private CharacterController targetCcCache;// Cache do CharacterController do alvo
    private Health myHealth;                  // Referência à vida do bot
    private bool inCombat = false;            // Se o bot está em combate

    private enum WeaponSlot { Rifle, Pistol } // Tipo de arma
    private WeaponSlot currentWeapon = WeaponSlot.Rifle;

    private int rifleMag, rifleRes, pistolMag, pistolRes; // Contadores de munição
    private bool isReloading = false;
    private float reloadTimer = 0f;
    private float fireCooldown = 0f;          // Timer entre disparos

    void Awake()
    {
        // Inicializa vida e munição
        myHealth = GetComponent<Health>();
        rifleMag = rifleMagSize;
        rifleRes = rifleReserveAmmo;
        pistolMag = pistolMagSize;
        pistolRes = pistolReserveAmmo;

        // Definição defensiva do ponto de visão
        if (!eyes) eyes = shootPoint != null ? shootPoint : transform;
    }

    void LateUpdate()
    {
        if (!IsServer) return; // Apenas o servidor controla a IA

        // Não dispara se estiver morto
        if (myHealth != null && myHealth.currentHealth.Value <= 0) return;

        // Reduz cooldown de tiro
        if (fireCooldown > 0f) fireCooldown -= Time.deltaTime;

        // Lógica de recarga
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f) FinishReload();
            return;
        }

        // Comportamento fora de combate: recarga preventiva
        if (!inCombat)
        {
            TryTacticalReload();
        }
        else if (currentTarget != null)
        {
            // Disparo durante combate
            TryShootAtTarget();
        }
    }

    // Define se o bot está em combate
    public void SetInCombat(bool value) => inCombat = value;

    // Define o alvo atual do bot
    public void SetTarget(Transform target)
    {
        if (currentTarget == target) return;
        currentTarget = target;
        targetRbCache = currentTarget ? currentTarget.GetComponent<Rigidbody>() : null;
        targetCcCache = currentTarget ? currentTarget.GetComponent<CharacterController>() : null;
    }

    // Tenta disparar ao alvo
    void TryShootAtTarget()
    {
        if (fireCooldown > 0f) return;

        EnsureUsableWeapon(); // Garante que tem arma com munição

        // Sem munição: nada a fazer
        if (GetCurrentMag() <= 0 && GetCurrentReserve() <= 0) return;

        // Recarregar se magazine vazio
        if (GetCurrentMag() <= 0 && GetCurrentReserve() > 0)
        {
            StartReload();
            return;
        }

        Vector3 origin = shootPoint.position;
        float dist = Vector3.Distance(origin, currentTarget.position);

        // Não dispara se fora do alcance máximo
        if (dist > maxShootDistance) return;

        // Calcula posição do alvo e direção
        Vector3 targetCenter = currentTarget.position + Vector3.up * 1.2f; // Altura média
        Vector3 directionToTarget = (targetCenter - origin).normalized;

        // Verifica obstáculos entre bot e alvo
        if (Physics.Raycast(origin, directionToTarget, out RaycastHit hit, dist, obstacleLayer, QueryTriggerInteraction.Ignore))
        {
            if (drawDebugRays) Debug.DrawLine(origin, hit.point, Color.black, 0.5f);
            return; 
        }

        // Previsão do movimento do alvo
        Vector3 targetVelocity = targetRbCache != null ? targetRbCache.linearVelocity : Vector3.zero;
        float timeToHit = dist / bulletSpeed;
        Vector3 futurePos = currentTarget.position + targetVelocity * timeToHit * leadAccuracy;
        Vector3 perfectTargetPos = futurePos + Vector3.up * 1.3f;

        // Aplicar dispersão (spread) de acordo com a distância e dificuldade
        float spreadRadius = spreadOverDistance.Evaluate(dist) * aimInaccuracyMultiplier;
        Vector3 errorOffset = Random.insideUnitSphere * spreadRadius;
        Vector3 noisyTargetPos = perfectTargetPos + errorOffset;

        // Direção final do tiro
        Vector3 dir = (noisyTargetPos - origin).normalized;
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
        if (flatDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(flatDir), Time.deltaTime * 20f);

        if (logShootingPosition)
            Debug.Log($"[BotCombat] {gameObject.name}: Disparando. Spread: {spreadRadius:F2}m para Distância: {dist:F1}m");

        FireBullet(origin, dir);   // Cria o projétil
        ConsumeAmmo();             // Consome munição

        // Aplica cooldown entre tiros com variação aleatória
        float baseCooldown = 1f / GetCurrentFireRate();
        fireCooldown = baseCooldown * Random.Range(0.92f, 1.08f);
    }

    // Instancia o projétil e configura os parâmetros de rede
    void FireBullet(Vector3 origin, Vector3 direction)
    {
        if (logShootingPosition) Debug.Log($"[DEBUG TIRO] Origem: {origin} | ShootPoint: {shootPoint.position}");
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(direction));

        var bp = bullet.GetComponent<BulletProjectile>();
        var rb = bullet.GetComponent<Rigidbody>();
        var netObj = bullet.GetComponent<NetworkObject>();

        if (bp != null)
        {
            bp.damage = (currentWeapon == WeaponSlot.Rifle) ? rifleDamage : pistolDamage;
            bp.ownerTeam = -2; 
            bp.ownerRoot = transform.root;
            bp.ownerClientId = ulong.MaxValue;
            bp.initialVelocity.Value = direction * bulletSpeed;
        }

        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;

        if (netObj != null)
            netObj.Spawn(true);
    }

    // Troca de arma se a atual estiver sem munição
    void EnsureUsableWeapon()
    {
        if (GetCurrentMag() <= 0 && GetCurrentReserve() <= 0)
        {
            WeaponSlot other = (currentWeapon == WeaponSlot.Rifle) ? WeaponSlot.Pistol : WeaponSlot.Rifle;
            if (GetTotalAmmo(other) > 0)
                currentWeapon = other;
        }
    }

    // Tenta recarregar de forma tática fora de combate
    void TryTacticalReload()
    {
        if (GetCurrentReserve() > 0 && GetCurrentMag() < GetCurrentMagSize())
            StartReload();
    }

    // Inicia recarga
    void StartReload()
    {
        if (isReloading || GetCurrentReserve() <= 0) return;
        isReloading = true;
        reloadTimer = (currentWeapon == WeaponSlot.Rifle) ? rifleReloadTime : pistolReloadTime;
    }

    // Termina recarga e atualiza munição
    void FinishReload()
    {
        isReloading = false;
        int mag = GetCurrentMag();
        int reserve = GetCurrentReserve();
        int needed = GetCurrentMagSize() - mag;
        int toLoad = Mathf.Min(needed, reserve);
        mag += toLoad;
        reserve -= toLoad;
        SetCurrentMag(mag);
        SetCurrentReserve(reserve);
    }

    void ConsumeAmmo() => SetCurrentMag(GetCurrentMag() - 1);
    float GetCurrentFireRate() => (currentWeapon == WeaponSlot.Rifle) ? rifleFireRate : pistolFireRate;
    int GetCurrentMagSize() => (currentWeapon == WeaponSlot.Rifle) ? rifleMagSize : pistolMagSize;
    int GetCurrentMag() => (currentWeapon == WeaponSlot.Rifle) ? rifleMag : pistolMag;
    void SetCurrentMag(int v)
    {
        if (currentWeapon == WeaponSlot.Rifle)
            rifleMag = v;
        else
            pistolMag = v;
    }
    int GetCurrentReserve() => (currentWeapon == WeaponSlot.Rifle) ? rifleRes : pistolRes;
    void SetCurrentReserve(int v)
    {
        if (currentWeapon == WeaponSlot.Rifle)
            rifleRes = v;
        else
            pistolRes = v;
    }
    int GetTotalAmmo(WeaponSlot s) => (s == WeaponSlot.Rifle) ? (rifleMag + rifleRes) : (pistolMag + pistolRes);

    // Gizmos para debugging
    void OnDrawGizmos()
    {
        if (!showShootPointGizmos) return;
        if (shootPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shootPoint.position, 0.1f);
            Gizmos.DrawRay(shootPoint.position, shootPoint.forward * 2f);
        }
        if (eyes != null && eyes != shootPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(eyes.position, 0.08f);
        }
    }
}
