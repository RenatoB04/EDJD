using UnityEngine;

[DisallowMultipleComponent] // Garante que só existe um componente deste tipo por GameObject
public class BotRespawnLink : MonoBehaviour
{
    [Header("Ligação ao Spawner (opcional)")]
    public BotSpawner_Proto spawner; // Referência ao spawner responsável pelo respawn do bot

    [Tooltip("Waypoints preferidos para este bot em respawns futuros (opcional).")]
    public Transform[] patrolWaypoints; // Waypoints preferenciais para respawn

    BOTDeath death; // Referência ao script de gestão de morte do bot

    void Awake()
    {
        // Obtém o componente BOTDeath associado ao bot
        death = GetComponent<BOTDeath>();

        if (death != null)
        {
            // Remove a ligação anterior (para evitar múltiplos listeners)
            death.OnDied -= OnBotDied;

            // Regista a função OnBotDied para ser chamada quando o bot morrer
            death.OnDied += OnBotDied;
        }
    }

    void OnDestroy()
    {
        // Remove o listener para evitar chamadas a objetos destruídos
        if (death != null)
            death.OnDied -= OnBotDied;
    }

    // Função chamada quando o bot morre
    void OnBotDied(BOTDeath d)
    {
        // Se existir um spawner e waypoints definidos, agenda respawn com os waypoints preferidos
        if (spawner != null && patrolWaypoints != null && patrolWaypoints.Length > 0)
        {
            spawner.ScheduleRespawn(patrolWaypoints);
        }
    }
}
