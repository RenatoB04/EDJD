using Unity.Netcode;
using UnityEngine;
using System;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Responsável por ligar o sistema de armas local (Weapon.cs)
/// ao spawn de projécteis em rede (server-authoritative).
/// </summary>
public class PlayerWeaponController : NetworkBehaviour
{
    [Header("Network Prefabs (Ligar no Inspector)")]
    [Tooltip("Prefab da bala com NetworkObject e Projectile.cs no ROOT.")]
    [SerializeField]
    private GameObject bulletPrefab;

    [Header("Refs")]
    private Health ownerHealth;
    private Character playerCharacter;

    // ----------------------------------------------------
    //                  INICIALIZAÇÃO
    // ----------------------------------------------------
    private void Awake()
    {
        ownerHealth = GetComponent<Health>();
        playerCharacter = GetComponent<Character>();

        if (ownerHealth == null)
            Debug.LogError("PlayerWeaponController: Falta componente Health no Player.");

        if (playerCharacter == null)
            Debug.LogError("PlayerWeaponController: Falta componente Character no Player.");
    }

    // ----------------------------------------------------
    //              DISPARO (CHAMADO PELO Weapon.cs)
    // ----------------------------------------------------

    /// <summary>
    /// Chamado pelo Weapon.Fire().
    /// Executa apenas no Owner e pede ao Servidor para criar a bala.
    /// </summary>
    public void FireExternally(Vector3 direction, Vector3 origin, float speed)
    {
        if (!IsOwner)
            return;

        if (ownerHealth == null)
        {
            Debug.LogError("PlayerWeaponController: ownerHealth nulo em FireExternally.");
            return;
        }

        // Pedido ao servidor para spawnar a bala
        SpawnBulletServerRpc(
            origin,
            direction,
            speed,
            ownerHealth.team.Value,
            OwnerClientId
        );
    }

    // ----------------------------------------------------
    //              SPAWN DA BALA (SERVIDOR)
    // ----------------------------------------------------

    /// <summary>
    /// Executado apenas no Servidor.
    /// Cria a bala, define dono/equipa e faz spawn em rede.
    /// </summary>
    [ServerRpc]
    private void SpawnBulletServerRpc(
        Vector3 position,
        Vector3 direction,
        float speed,
        int shooterTeam,
        ulong shooterClientId)
    {
        if (!IsServer)
            return;

        if (bulletPrefab == null)
        {
            Debug.LogError("[PlayerWeaponController] Bullet Prefab não atribuído.");
            return;
        }

        // 1. Muzzle effect para os proxies (clientes remotos)
        PlayMuzzleEffectClientRpc();

        // 2. Instanciar bala no servidor
        var bulletGO = Instantiate(
            bulletPrefab,
            position,
            Quaternion.LookRotation(direction)
        );

        // 3. Configurar Projectile
        if (bulletGO.TryGetComponent<Projectile>(out var projectile))
        {
            // Velocidade aplicada no OnNetworkSpawn
            projectile.initialVelocity.Value = direction * speed;

            // Identificação correta do dono
            projectile.ownerClientId = shooterClientId;
            projectile.ownerTeam = shooterTeam;

            // 🔴 FIX CRÍTICO
            // Impede que a bala cause dano ao próprio jogador
            projectile.ownerRoot = transform.root;
        }
        else
        {
            Debug.LogError("[PlayerWeaponController] O prefab da bala não tem Projectile.cs.");
        }

        // 4. Spawn em rede
        if (bulletGO.TryGetComponent<NetworkObject>(out var netObj))
        {
            try
            {
                netObj.Spawn(true);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    "[PlayerWeaponController] Erro ao spawnar NetworkObject. " +
                    $"Confirma se o prefab está registado no NetworkManager. Erro: {ex.Message}"
                );
                Destroy(bulletGO);
            }
        }
        else
        {
            Debug.LogError("[PlayerWeaponController] A bala não tem NetworkObject no ROOT.");
            Destroy(bulletGO);
        }
    }

    // ----------------------------------------------------
    //          MULE EFFECT NOS CLIENTES REMOTOS
    // ----------------------------------------------------

    /// <summary>
    /// Executado nos clientes remotos para tocar VFX/SFX.
    /// O Owner já trata disso localmente no Weapon.Fire().
    /// </summary>
    [ClientRpc]
    private void PlayMuzzleEffectClientRpc(ClientRpcParams rpcParams = default)
    {
        if (IsOwner)
            return;

        if (playerCharacter == null)
            return;

        var inventory = playerCharacter.GetInventory();
        if (inventory == null)
            return;

        var weapon = inventory.GetEquipped() as InfimaGames.LowPolyShooterPack.Weapon;
        if (weapon != null)
        {
            weapon.PlayMuzzleEffect();
        }
    }
}
