using UnityEngine;

public class WeaponConfig : MonoBehaviour
{
    [Header("Refs")]
    public Transform firePoint;                 // Ponto de saída das balas
    public GameObject bulletPrefab;             // Prefab da bala
    public ParticleSystem muzzleFlashPrefab;    // Efeito visual do disparo
    public AudioClip fireSfx;                   // Som de disparo
    public AudioClip emptyClickSfx;             // Som quando tenta disparar sem munição
    public AudioClip reloadSfx;                 // Som de recarga

    [Header("Stats")]
    public string displayName = "Pistol";       // Nome do arma mostrado no UI
    public bool automatic = false;              // Se o disparo é automático (segura o botão para disparar continuamente)
    public float bulletSpeed = 40f;             // Velocidade da bala
    public float fireRate = 0.12f;              // Intervalo entre disparos
    public float maxAimDistance = 200f;         // Distância máxima do disparo

    [Header("Ammo")]
    public int magSize = 12;                    // Munição no carregador
    public int startingReserve = 48;            // Munição de reserva inicial
    public float reloadTime = 1.4f;             // Tempo de recarga em segundos
}
