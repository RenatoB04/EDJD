using UnityEngine;
using System.Collections;

public class ShieldInteract : MonoBehaviour
{
    private Material shieldMaterial;
    private Renderer shieldRenderer;
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
        shieldRenderer = GetComponent<Renderer>();
        shieldMaterial = shieldRenderer.material;

        // Initialise ripple state as inactive
        shieldMaterial.SetFloat("_HitRadius", -1000f);
        shieldMaterial.SetFloat("_MaxRadius", maxRadius);

        currentHealth = maxHealth;

        // Auto-find components in the scene if not assigned
        if (generatorController == null)
            generatorController = FindObjectOfType<ShieldGeneratorController>();
        if (emergencyController == null)
            emergencyController = FindObjectOfType<EmergencyPostProcessController>();
    }

    void Update()
    {
        // Don't register clicks if the shield is down/recharging
        if (currentHealth <= 0f) return;

        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                // Trigger the visual impact wave and pass the hit normal for particles
                TriggerHit(hit.point, hit.normal);
            }
        }
    }

    public void TriggerHit(Vector3 hitPoint, Vector3 normal)
    {
        // 1. Trigger the ripple shader effect
        shieldMaterial.SetVector("_HitPos", hitPoint);

        if (rippleCoroutine != null)
            StopCoroutine(rippleCoroutine);

        rippleCoroutine = StartCoroutine(AnimateRipple());

        // 2. Spawn Geometry Shader sparks at the impact point
        GameObject sparkObj = new GameObject("ImpactSparks_GPU");
        ImpactParticleSpawner spawner = sparkObj.AddComponent<ImpactParticleSpawner>();
        spawner.particleShader = particleShader;
        spawner.particleColor = sparkColor;
        spawner.particleCount = sparksPerHit;
        spawner.sparkSize = sparkSize;
        spawner.Init(hitPoint, normal);

        // 3. Update Health System
        TakeDamage(damagePerHit);
    }

    void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damage);

        // 4. Update camera warning post-process based on low health threshold (<= 50%)
        if (emergencyController != null)
        {
            if (currentHealth <= maxHealth * 0.5f)
            {
                // Intensity increases as health drops below 50%
                float range = maxHealth * 0.5f;
                float pct = 1.0f - (currentHealth / range);
                emergencyController.emergencyIntensity = Mathf.Clamp(pct, 0.1f, 1.0f);
            }
            else
            {
                emergencyController.emergencyIntensity = 0f;
            }
        }

        // 5. Shield break check
        if (currentHealth <= 0f)
        {
            BreakShield();
        }
    }

    void BreakShield()
    {
        // Play extreme post-process glitch burst
        if (emergencyController != null)
        {
            emergencyController.emergencyIntensity = 1.0f;
        }

        // Hide shield geometry
        shieldRenderer.enabled = false;

        // Tell generator base to return to flat inactive state
        if (generatorController != null)
        {
            generatorController.SetShieldActive(false);
        }

        // Start recharge cycle
        if (rechargeCoroutine != null)
            StopCoroutine(rechargeCoroutine);
        rechargeCoroutine = StartCoroutine(RechargeShield());
    }

    IEnumerator RechargeShield()
    {
        // Wait 3.0 seconds before starting recharge
        yield return new WaitForSeconds(3.0f);

        float elapsed = 0f;
        float rechargeDuration = 3.0f;

        while (elapsed < rechargeDuration)
        {
            elapsed += Time.deltaTime;
            float pct = elapsed / rechargeDuration;
            
            currentHealth = pct * maxHealth;

            // Slowly fade out emergency post-process as shield powers back up
            if (emergencyController != null)
            {
                emergencyController.emergencyIntensity = 1.0f - pct;
            }

            yield return null;
        }

        // Shield fully recharged
        currentHealth = maxHealth;
        shieldRenderer.enabled = true;

        if (emergencyController != null)
        {
            emergencyController.emergencyIntensity = 0f;
        }

        // Power generator base back up
        if (generatorController != null)
        {
            generatorController.SetShieldActive(true);
        }
    }

    IEnumerator AnimateRipple()
    {
        float currentRadius = 0f;

        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * rippleSpeed;
            shieldMaterial.SetFloat("_HitRadius", currentRadius);
            yield return null;
        }

        // Reset state
        shieldMaterial.SetFloat("_HitRadius", -1000f);
    }
}