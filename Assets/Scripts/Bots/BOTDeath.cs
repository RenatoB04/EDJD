using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode; 

public class BOTDeath : MonoBehaviour
{
    [Header("Refs")]
    [Tooltip("Componente que tem a bool isDead (ex: Health script).")]
    public MonoBehaviour health;       // Referência ao script que controla a vida do bot
    [Tooltip("Nome exato da bool no script de vida (case-sensitive).")]
    public string isDeadField = "isDead"; // Nome do campo ou propriedade que indica morte

    [Header("Comportamento")]
    [Tooltip("Atraso antes de desaparecer (segundos).")]
    public float delay = 0f;           // Tempo de espera antes do bot desaparecer
    [Tooltip("Se true: Destroy(gameObject); se false: SetActive(false).")]
    public bool destroyInstead = true; // Define se o objeto é destruído ou apenas desativado
    [Tooltip("Desativar collider ao morrer.")]
    public bool disableColliderOnDeath = true; // Desativa colliders ao morrer
    [Tooltip("Desativar Animator ao morrer.")]
    public bool disableAnimatorOnDeath = true; // Desativa animator ao morrer
    [Tooltip("Desativar NavMeshAgent ao morrer.")]
    public bool disableNavMeshAgentOnDeath = true; // Desativa agente de navegação

    // Eventos para outros scripts saberem que o bot morreu
    public event Action<BOTDeath> OnDied;          
    public static event Action OnAnyBotKilled;      

    private bool hasDied = false; // Flag interna para garantir que a morte é processada uma vez

    void Update()
    {
        // Não processa se já morreu
        if (hasDied) return;

        // Verifica se o bot está morto
        if (IsHealthDead())
        {
            HandleDeath();
        }
    }

    // Verifica se o campo/propriedade 'isDead' indica morte
    bool IsHealthDead()
    {
        if (!health || string.IsNullOrEmpty(isDeadField))
            return false;

        var type = health.GetType();
        var field = type.GetField(isDeadField);

        // Se for um campo booleano normal
        if (field != null)
        {
            if (field.FieldType == typeof(bool))
            {
                return (bool)field.GetValue(health);
            }

            // Se for NetworkVariable<bool> (Netcode)
            if (field.FieldType == typeof(NetworkVariable<bool>))
            {
                var netVar = (NetworkVariable<bool>)field.GetValue(health);
                if (netVar != null)
                    return netVar.Value;
            }
        }

        // Se for uma propriedade do tipo bool
        var prop = type.GetProperty(isDeadField);
        if (prop != null && prop.PropertyType == typeof(bool))
        {
            return (bool)prop.GetValue(health);
        }

        // Caso não encontre, mostra aviso
        Debug.LogWarning($"[BOTDeath] Não foi possível encontrar o campo/propriedade '{isDeadField}' do tipo 'bool' ou 'NetworkVariable<bool>' no script '{health.GetType().Name}'.");
        return false;
    }

    // Processa a morte do bot
    public void HandleDeath()
    {
        if (hasDied) return; // Evita processar mais de uma vez
        hasDied = true;

        // Desativa todos os colliders
        if (disableColliderOnDeath)
        {
            foreach (var col in GetComponentsInChildren<Collider>())
                col.enabled = false;
        }

        // Desativa animator
        if (disableAnimatorOnDeath)
        {
            var anim = GetComponentInChildren<Animator>();
            if (anim) anim.enabled = false;
        }

        // Desativa NavMeshAgent
        if (disableNavMeshAgentOnDeath)
        {
            var agent = GetComponent<NavMeshAgent>();
            if (agent) agent.enabled = false;
        }

        // Dispara eventos de morte
        try { OnDied?.Invoke(this); } catch { }
        try { OnAnyBotKilled?.Invoke(); } catch { }

        // Inicia corrotina para desaparecer
        StartCoroutine(Disappear());
    }

    // Corrotina que destrói ou desativa o bot após o delay
    IEnumerator Disappear()
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (destroyInstead)
            Destroy(gameObject);  // Destrói objeto
        else
            gameObject.SetActive(false); // Apenas desativa objeto
    }
}
