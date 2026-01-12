using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.Netcode;

// Garante que este GameObject tem sempre um NavMeshAgent
[RequireComponent(typeof(NavMeshAgent))]
public class BotAI_Proto : NetworkBehaviour
{
    // Estados possíveis da máquina de estados do bot
    public enum BotState
    {
        Patrol,     // Patrulha entre waypoints
        Chase,      // Persegue o jogador
        Attack,     // Ataca o jogador
        Search,     // Procura o último local conhecido do jogador
        Retreat,    // Recuo para recuperar vida
        GoToAmmo    // Procura munição
    }

    [Header("Debug")]
    [SerializeField] BotState currentState = BotState.Patrol;
    public bool debugLogs = false;

    [Header("Referências")]
    public Transform eyes;                 // Ponto de origem da visão (raycast)
    public Animator animator;              // Animator do bot
    public BotCombat combat;               // Script responsável pelo combate
    public MonoBehaviour healthSource;     // Fonte genérica de vida
    public string healthCurrentField = "currentHealth";
    public string healthMaxField = "maxHealth";

    [Header("Patrulha")]
    public Transform[] patrolPoints;        // Waypoints de patrulha
    public float waypointTolerance = 1.0f;  // Distância mínima para considerar waypoint atingido

    [Header("Pickups")]
    public Transform[] healthPickups;       // Pontos de vida disponíveis no mapa
    public Transform[] ammoPickups;         // Pontos de munição disponíveis no mapa

    [Header("Visão / Target")]
    public string playerTag = "Player";
    public LayerMask obstacleMask = ~0;     // Máscara para obstáculos na visão
    public float viewRadius = 60f;           // Raio máximo de visão
    public float maxSearchTime = 10f;        // Tempo máximo de procura após perder o alvo

    [Header("Otimização (NOVO)")]
    [Tooltip("Intervalo em segundos entre verificações de Raycast de visão.")]
    public float visionCheckInterval = 0.2f;

    [Header("Combate")]
    public float idealCombatDistance = 10f; // Distância ideal de combate
    public float tooCloseDistance = 4f;     // Distância mínima aceitável ao jogador
    public float giveUpDistance = 120f;     // Distância máxima antes de desistir da perseguição

    [Header("Prioridades")]
    [Range(0f, 1f)] public float lowHealthThreshold = 0.2f; // Percentagem de vida considerada baixa
    [Range(0f, 1f)] public float lowAmmoThreshold = 0.2f;   // Percentagem de munição considerada baixa

    [Header("Fuga")]
    public float retreatSpeedMultiplier = 1.5f; // Multiplicador de velocidade ao recuar

    [Header("Comunicação")]
    public static List<BotAI_Proto> allBots = new List<BotAI_Proto>(); // Lista global de bots
    public float alertRadius = 25f; // Raio para alertar bots aliados

    NavMeshAgent agent;              // Agente de navegação
    Transform currentTarget;         // Alvo atual (jogador)
    bool isDead = false;

    float baseSpeed;                 // Velocidade base do bot
    int patrolIndex = -1;            // Índice do waypoint atual
    int patrolDirection = 1;         // Direção da patrulha (ida/volta)

    Vector3 lastKnownPlayerPos;      // Última posição conhecida do jogador
    float timeSinceLastSeen;          // Tempo desde que o jogador foi visto
    float targetSearchTimer = 0f;     // Timer para procura de jogador
    float visionTimer = 0f;           // Timer para otimização do raycast
    bool cachedVisibility = false;    // Resultado de visibilidade em cache

    // Regista o bot na lista global
    void OnEnable()
    {
        allBots.Add(this);
    }

    // Remove o bot da lista global
    void OnDisable()
    {
        allBots.Remove(this);
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent) baseSpeed = agent.speed;

        // Inicialização defensiva de referências
        if (!eyes) eyes = transform;
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!combat) combat = GetComponent<BotCombat>();

        // Tenta encontrar um componente de vida automaticamente
        if (healthSource == null)
        {
            var h = GetComponent("Health");
            if (h != null) healthSource = (MonoBehaviour)h;
        }

        // Direção de patrulha aleatória
        patrolDirection = Random.value < 0.5f ? 1 : -1;
        patrolIndex = -1;
    }

    // Apenas o servidor executa a lógica de IA
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        ChangeState(BotState.Patrol);
    }

    void Update()
    {
        // Segurança: só o servidor e apenas se o agente for válido
        if (!IsServer || !agent || !agent.isOnNavMesh) return;

        // Verificação de morte
        var health = GetComponent<Health>();
        if (health != null && health.isDead.Value)
        {
            if (!isDead)
            {
                isDead = true;
                HandleDeath();
            }
            return;
        }

        // Procura periódica do jogador mais próximo
        targetSearchTimer += Time.deltaTime;
        if (targetSearchTimer > 0.5f)
        {
            FindClosestPlayer();
            targetSearchTimer = 0f;
        }

        // Avaliação de vida
        float health01 = GetHealth01();
        bool lowHealth = health01 > 0f && health01 <= lowHealthThreshold;

        // Avaliação de munição
        float ammo01 = 1f;
        bool lowAmmo = false;
        if (combat)
        {
            ammo01 = combat.AmmoNormalized;
            lowAmmo = ammo01 <= lowAmmoThreshold;
        }

        // Distância ao jogador
        float distToPlayer = Mathf.Infinity;
        if (currentTarget)
        {
            distToPlayer = Vector3.Distance(transform.position, currentTarget.position);

            // Raycast de visão otimizado com intervalo
            visionTimer += Time.deltaTime;
            if (visionTimer >= visionCheckInterval)
            {
                cachedVisibility = CheckVisibilityPhysics(distToPlayer);
                visionTimer = 0f;
            }
        }
        else
        {
            cachedVisibility = false;
        }

        bool playerVisible = cachedVisibility;

        // Atualização da última posição conhecida
        if (playerVisible)
        {
            lastKnownPlayerPos = currentTarget.position;
            timeSinceLastSeen = 0f;
        }
        else
        {
            timeSinceLastSeen += Time.deltaTime;
        }

        // Prioridade máxima: sobrevivência
        if (lowHealth)
        {
            if (currentState != BotState.Retreat)
                ChangeState(BotState.Retreat);
        }
        else if (lowAmmo && currentState != BotState.GoToAmmo)
        {
            ChangeState(BotState.GoToAmmo);
        }
        else
        {
            // Lógica principal de combate e perseguição
            if (playerVisible && distToPlayer <= giveUpDistance)
            {
                if (distToPlayer <= idealCombatDistance * 1.1f)
                    ChangeState(BotState.Attack);
                else
                    ChangeState(BotState.Chase);
            }
            else
            {
                // Estado de procura após perder visão do jogador
                if (timeSinceLastSeen > 0f && timeSinceLastSeen <= maxSearchTime &&
                    (currentState == BotState.Chase || currentState == BotState.Attack))
                {
                    ChangeState(BotState.Search);
                }
                // Retorno à patrulha após desistir
                else if (timeSinceLastSeen > maxSearchTime &&
                         (currentState == BotState.Search || currentState == BotState.Chase || currentState == BotState.Attack))
                {
                    ChangeState(BotState.Patrol);
                }
            }
        }

        // Execução do comportamento do estado atual
        switch (currentState)
        {
            case BotState.Patrol: TickPatrol(); break;
            case BotState.Chase: TickChase(); break;
            case BotState.Attack: TickAttack(); break;
            case BotState.Search: TickSearch(); break;
            case BotState.Retreat: TickRetreat(); break;
            case BotState.GoToAmmo: TickGoToAmmo(); break;
        }

        UpdateAnimator();
    }

    // Trata a morte do bot
    void HandleDeath()
    {
        if (animator)
        {
            animator.SetBool("IsDead", true);
            animator.SetFloat("Speed", 0f);
        }

        if (agent)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (combat) combat.SetInCombat(false);
    }

    // Encontra o jogador vivo mais próximo
    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDst = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (var p in players)
        {
            var h = p.GetComponent<Health>();
            if (h != null && h.isDead.Value) continue;

            float d = Vector3.Distance(p.transform.position, myPos);
            if (d < minDst)
            {
                minDst = d;
                closest = p.transform;
            }
        }

        currentTarget = closest;
        if (combat) combat.SetTarget(currentTarget);
    }

    // --- Estados da FSM ---

    void TickPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.speed = baseSpeed;

        if (patrolIndex < 0 || patrolIndex >= patrolPoints.Length)
        {
            patrolIndex = Random.Range(0, patrolPoints.Length);
            agent.SetDestination(patrolPoints[patrolIndex].position);
            return;
        }

        Transform curWp = patrolPoints[patrolIndex];
        if (!curWp) { AdvancePatrolIndex(); return; }

        float sqrDist = (curWp.position - transform.position).sqrMagnitude;
        if (!agent.hasPath || sqrDist <= waypointTolerance * waypointTolerance)
        {
            AdvancePatrolIndex();
            if (patrolPoints[patrolIndex])
                agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        if (combat) combat.SetInCombat(false);
    }

    void TickChase()
    {
        if (!currentTarget) { ChangeState(BotState.Search); return; }

        agent.isStopped = false;
        agent.speed = baseSpeed;
        agent.SetDestination(currentTarget.position);

        if (combat) combat.SetInCombat(true);
    }

    void TickAttack()
    {
        if (!currentTarget) { ChangeState(BotState.Search); return; }

        Vector3 toPlayer = currentTarget.position - transform.position;
        float dist = toPlayer.magnitude;

        // Ajuste dinâmico da posição em combate
        if (dist > idealCombatDistance + 1f)
        {
            agent.isStopped = false;
            agent.speed = baseSpeed;
            agent.SetDestination(currentTarget.position);
        }
        else if (dist < tooCloseDistance)
        {
            agent.isStopped = false;
            Vector3 away = (transform.position - currentTarget.position).normalized;
            agent.SetDestination(transform.position + away * 3f);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();

            // Rotação suave para enfrentar o jogador
            Vector3 dir = (currentTarget.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * 5f
                );
        }

        if (combat) combat.SetInCombat(true);
    }

    void TickSearch()
    {
        agent.isStopped = false;
        agent.speed = baseSpeed;
        agent.SetDestination(lastKnownPlayerPos);

        if (combat) combat.SetInCombat(false);
    }

    void TickRetreat()
    {
        agent.isStopped = false;
        agent.speed = baseSpeed * retreatSpeedMultiplier;

        // Prioridade: ir buscar vida
        Transform hp = GetClosestTransform(healthPickups, transform.position);
        if (hp != null)
        {
            agent.SetDestination(hp.position);
        }
        else if (currentTarget)
        {
            // Caso não haja vida, afasta-se do jogador
            Vector3 away = (transform.position - currentTarget.position).normalized;
            agent.SetDestination(transform.position + away * 8f);
        }

        if (combat) combat.SetInCombat(true);
    }

    void TickGoToAmmo()
    {
        agent.isStopped = false;
        agent.speed = baseSpeed;

        Transform ammo = GetClosestTransform(ammoPickups, transform.position);
        if (ammo != null)
            agent.SetDestination(ammo.position);
        else
            ChangeState(BotState.Patrol);

        if (combat) combat.SetInCombat(false);
    }

    // Verificação física de visibilidade usando Raycast
    bool CheckVisibilityPhysics(float distToPlayer)
    {
        if (!currentTarget) return false;
        if (distToPlayer > viewRadius) return false;

        Vector3 origin = eyes.position;
        Vector3 targetPos = currentTarget.position + Vector3.up * 1.0f;
        Vector3 dir = (targetPos - origin);
        float dist = dir.magnitude;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform != currentTarget &&
                hit.collider.transform.root != currentTarget)
                return false;
        }
        return true;
    }

    // Avança o índice de patrulha respeitando a direção
    void AdvancePatrolIndex()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (patrolDirection >= 0)
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        else
        {
            patrolIndex--;
            if (patrolIndex < 0) patrolIndex = patrolPoints.Length - 1;
        }
    }

    // Devolve o Transform mais próximo de uma lista
    Transform GetClosestTransform(Transform[] list, Vector3 from)
    {
        if (list == null || list.Length == 0) return null;

        Transform best = null;
        float bestSqr = float.MaxValue;

        foreach (var t in list)
        {
            if (!t) continue;
            float d = (t.position - from).sqrMagnitude;
            if (d < bestSqr)
            {
                bestSqr = d;
                best = t;
            }
        }
        return best;
    }

    // Vida normalizada (0–1)
    float GetHealth01()
    {
        var h = GetComponent<Health>();
        if (h != null)
            return h.currentHealth.Value / h.maxHealth;
        return 1f;
    }

    // Transição de estados da FSM
    void ChangeState(BotState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // Preparação específica para o estado de procura
        if (currentState == BotState.Search && lastKnownPlayerPos != Vector3.zero)
            agent.SetDestination(lastKnownPlayerPos);

        // Atualiza estado de combate
        if (combat)
        {
            bool inCombat =
                currentState == BotState.Chase ||
                currentState == BotState.Attack ||
                currentState == BotState.Retreat;

            combat.SetInCombat(inCombat);
        }

        // Alerta bots aliados se entrar em combate
        if (currentState == BotState.Chase || currentState == BotState.Attack)
            AlertNearbyBots();
    }

    // Alerta bots próximos da presença do jogador
    void AlertNearbyBots()
    {
        if (!currentTarget) return;

        foreach (var bot in allBots)
        {
            if (!bot || bot == this) continue;

            float d = Vector3.Distance(transform.position, bot.transform.position);
            if (d <= alertRadius)
                bot.OnAllySpottedPlayer(currentTarget.position);
        }
    }

    // Recebe informação de um bot aliado
    public void OnAllySpottedPlayer(Vector3 pos)
    {
        lastKnownPlayerPos = pos;
        timeSinceLastSeen = 0f;

        if (currentState == BotState.Patrol || currentState == BotState.Search)
            ChangeState(BotState.Chase);
    }

    // Atualização simples do Animator com base na velocidade
    void UpdateAnimator()
    {
        if (!animator || !agent) return;
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    // Placeholder para futura IA baseada em som
    public void HearSound(Vector3 pos, float loudness) { }
}
