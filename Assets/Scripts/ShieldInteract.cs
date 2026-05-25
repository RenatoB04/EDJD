using UnityEngine;
using System.Collections;

public class ShieldInteract : MonoBehaviour
{
    // Referencias internas para o escudo em si
    private Material shieldMaterial;
    private Renderer shieldRenderer;
    
    // Corrotinas permitem-nos executar funcoes ao longo do tempo (animacoes)
    // Guardamos a referencia para podermos parar a animacao a meio se for preciso.
    private Coroutine rippleCoroutine;
    private Coroutine rechargeCoroutine;

    [Header("Ripple Settings")]
    public float maxRadius = 3.0f;
    public float rippleSpeed = 5.0f;

    [Header("Shield Health System")]
    public float maxHealth = 100f;
    public float currentHealth;
    [Tooltip("Health lost per mouse click impact.")]
    public float damagePerHit = 25f;
    
    [Header("System References")]
    [Tooltip("Reference to the Shield Generator base controller.")]
    public ShieldGeneratorController generatorController;
    
    [Tooltip("Reference to the Camera Emergency Post-Process controller.")]
    public EmergencyPostProcessController emergencyController;

    [Header("Impact Sparks Settings")]
    public Shader particleShader;
    public Color sparkColor = new Color(1.0f, 0.45f, 0.0f, 1.0f);
    public int sparksPerHit = 15;
    public float sparkSize = 0.12f;

    void Start()
    {
        // 1. Obter o material do escudo para podermos enviar dados para o Shader
        shieldRenderer = GetComponent<Renderer>();
        shieldMaterial = shieldRenderer.material;

        // O truque do -1000:
        // Se o raio de impacto for 0, o Vertex Shader vai aplicar a forca maxima no vertice 
        // 0, criando um "bico" estatico no escudo. Ao metermos -1000f, atiramos a onda de 
        // choque para fora do ecra e anulamos a deformacao visual quando o escudo esta calmo.
        shieldMaterial.SetFloat("_HitRadius", -1000f);
        shieldMaterial.SetFloat("_MaxRadius", maxRadius);

        currentHealth = maxHealth;

        // 2. OTIMIZACAO DE WORKFLOW:
        // Se a equipa se esquecer de arrastar os controladores da Base e da Camara 
        // no Inspector, o script procura ativamente por eles na cena para evitar erros (NullReference).
        if (generatorController == null)
            generatorController = FindObjectOfType<ShieldGeneratorController>();
        if (emergencyController == null)
            emergencyController = FindObjectOfType<EmergencyPostProcessController>();
    }

    void Update()
    {
        // Se o escudo estiver destruido (a recarregar), ignoramos os cliques do jogador.
        if (currentHealth <= 0f) return;

        // Verifica se o botao esquerdo do rato (0) foi pressionado neste frame.
        if (!Input.GetMouseButtonDown(0)) return;

        // Raycasting:
        // Transforma a posicao 2D do rato no ecra num "Raio" (uma linha reta 3D) 
        // que e disparado da camara em direcao ao cenario.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Dispara o raio fisico. Se bater nalguma coisa, guarda os dados do impacto na variavel 'hit'.
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Verifica se o objeto em que batemos e este proprio escudo.
            if (hit.transform == transform)
            {
                // Envia a coordenada global do impacto (hit.point) e a normal da face (hit.normal)
                TriggerHit(hit.point, hit.normal);
            }
        }
    }

    // Funcao central que orquestra a reacao de todos os sistemas ao tiro.
    public void TriggerHit(Vector3 hitPoint, Vector3 normal)
    {
        // 1. SHADER DO ESCUDO: Envia a coordenada exata para deformar a malha.
        shieldMaterial.SetVector("_HitPos", hitPoint);

        // Se ja houver uma onda a acontecer, cancelamos a antiga para comecar uma nova limpa.
        if (rippleCoroutine != null)
            StopCoroutine(rippleCoroutine);

        rippleCoroutine = StartCoroutine(AnimateRipple());

        // 2. GEOMETRY SHADER: Criar fisicamente as faiscas na GPU.
        // Criamos um objeto vazio e colamos-lhe o script que constroi a malha de particulas.
        GameObject sparkObj = new GameObject("ImpactSparks_GPU");
        ImpactParticleSpawner spawner = sparkObj.AddComponent<ImpactParticleSpawner>();
        spawner.particleShader = particleShader;
        spawner.particleColor = sparkColor;
        spawner.particleCount = sparksPerHit;
        spawner.sparkSize = sparkSize;
        spawner.Init(hitPoint, normal);

        // 3. SISTEMA DE VIDA: Retirar vida ao escudo.
        TakeDamage(damagePerHit);
    }

    void TakeDamage(float damage)
    {
        // Garante que a vida nunca desce abaixo de zero (Mathf.Max escolhe o maior valor).
        currentHealth = Mathf.Max(0f, currentHealth - damage);

        // 4. POS-PROCESSAMENTO: Atualizar a intensidade do alarme visual.
        if (emergencyController != null)
        {
            // So comecamos a piscar a vermelho se a vida for igual ou inferior a 50%.
            if (currentHealth <= maxHealth * 0.5f)
            {
                // Se a vida atual e 25, e a range de emergencia vai do 0 ao 50...
                // (25 / 50) = 0.5. Subtraindo a 1.0f, obtemos 0.5 (50% de intensidade).
                // A medida que a vida desce, a intensidade aproxima-se de 1.0f.
                float range = maxHealth * 0.5f;
                float pct = 1.0f - (currentHealth / range);
                
                // Mathf.Clamp garante que o valor do efeito fica estrictamente entre 0.1 e 1.0.
                emergencyController.emergencyIntensity = Mathf.Clamp(pct, 0.1f, 1.0f);
            }
            else
            {
                // Se o escudo tiver mais de 50% da vida, o filtro de ecra e desligado.
                emergencyController.emergencyIntensity = 0f;
            }
        }

        // 5. Verificar se o tiro destruiu o escudo.
        if (currentHealth <= 0f)
        {
            BreakShield();
        }
    }

    void BreakShield()
    {
        // Forca o Glitch na camara ao maximo (100%) no instante em que quebra.
        if (emergencyController != null)
        {
            emergencyController.emergencyIntensity = 1.0f;
        }

        // Desliga o renderer: a malha do escudo fica invisivel (mas o objeto continua na cena).
        shieldRenderer.enabled = false;

        // Diz a base de Tessellation para perder o brilho e o relevo (fica lisa).
        if (generatorController != null)
        {
            generatorController.SetShieldActive(false);
        }

        // Inicia o ciclo de recarga.
        if (rechargeCoroutine != null)
            StopCoroutine(rechargeCoroutine);
        rechargeCoroutine = StartCoroutine(RechargeShield());
    }

    // CORROTINA: Recarregar o escudo ao longo do tempo.
    IEnumerator RechargeShield()
    {
        // Espera 3 segundos no escuro antes de comecar a recarregar.
        yield return new WaitForSeconds(3.0f);

        float elapsed = 0f;
        float rechargeDuration = 3.0f;

        // Durante os proximos 3 segundos, vai atualizando aos poucos.
        while (elapsed < rechargeDuration)
        {
            elapsed += Time.deltaTime;
            
            // pct vai de 0.0 a 1.0 dependendo de quanto tempo ja passou.
            float pct = elapsed / rechargeDuration;
            
            // A vida do escudo sobe gradualmente de volta aos 100.
            currentHealth = pct * maxHealth;

            // O filtro vermelho e o glitch diminuem suavemente ate desaparecerem.
            if (emergencyController != null)
            {
                emergencyController.emergencyIntensity = 1.0f - pct;
            }

            // Espera pelo proximo frame do jogo para continuar o ciclo while.
            yield return null;
        }

        // --- Recarga Concluida ---
        currentHealth = maxHealth;
        shieldRenderer.enabled = true; // Volta a mostrar a bolha do escudo

        if (emergencyController != null)
        {
            emergencyController.emergencyIntensity = 0f;
        }

        // Diz a base de Tessellation para voltar a ativar o relevo.
        if (generatorController != null)
        {
            generatorController.SetShieldActive(true);
        }
    }

    // CORROTINA: Animar a onda de impacto crescer.
    IEnumerator AnimateRipple()
    {
        float currentRadius = 0f;

        // Enquanto o raio atual for menor que o raio maximo (3.0)...
        while (currentRadius < maxRadius)
        {
            // Aumenta o raio multiplicando pela velocidade e pelo tempo do frame.
            currentRadius += Time.deltaTime * rippleSpeed;
            
            // Envia o novo valor de raio imediatamente para o Shader.
            shieldMaterial.SetFloat("_HitRadius", currentRadius);
            
            yield return null;
        }

        // Assim que a onda atinge o tamanho maximo, escondemos o raio em -1000 para
        // apagar qualquer residuo de deformacao.
        shieldMaterial.SetFloat("_HitRadius", -1000f);
    }
}