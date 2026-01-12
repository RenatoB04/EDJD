using Unity.Netcode;
using UnityEngine;
using System;
using InfimaGames.LowPolyShooterPack;

public class PlayerWeaponController : NetworkBehaviour
{
    [Header("Network Prefabs (Ligar no Inspector)")]
    [Tooltip("Prefab da bala com NetworkObject e Projectile.cs no ROOT.")]
    [SerializeField]
    private GameObject bulletPrefab;

    [Header("Refs")]
    private Health ownerHealth;          // Referência ao Health do jogador
    private Character playerCharacter;   // Referência ao Character do jogador

    private void Awake()
    {
        // Obtém referências
        ownerHealth = GetComponent<Health>();
        playerCharacter = GetComponent<Character>();

        if (ownerHealth == null)
            Debug.LogError("PlayerWeaponController: Falta componente Health no Player.");
        if (playerCharacter == null)
            Debug.LogError("PlayerWeaponController: Falta componente Character no Player.");
    }

    // Função pública para disparar externamente (usada pelo input, AI, etc.)
    public void FireExternally(Vector3 direction, Vector3 origin, float speed)
    {
        if (!IsOwner) return;  // Apenas o dono do player dispara localmente

        if (ownerHealth == null)
        {
            Debug.LogError("PlayerWeaponController: ownerHealth nulo em FireExternally.");
            return;
        }

        // Chama o ServerRpc para spawnar a bala no servidor
        SpawnBulletServerRpc(
            origin,
            direction,
            speed,
            ownerHealth.team.Value, // Equipa do jogador
            OwnerClientId           // ClientId do jogador
        );
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(
        Vector3 position,
        Vector3 direction,
        float speed,
        int shooterTeam,
        ulong shooterClientId)
    {
        if (!IsServer) return;

        if (bulletPrefab == null)
        {
            Debug.LogError("[PlayerWeaponController] Bullet Prefab não atribuído.");
            return;
        }

        // Dispara efeito de boca da arma nos clientes (exceto owner)
        PlayMuzzleEffectClientRpc();

        // Instancia a bala
        var bulletGO = Instantiate(
            bulletPrefab,
            position,
            Quaternion.LookRotation(direction)
        );

        // Configura o componente Projectile da bala
        if (bulletGO.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.initialVelocity.Value = direction * speed; // Velocidade inicial
            projectile.ownerClientId = shooterClientId;           // OwnerClientId para ignorar self
            projectile.ownerTeam = shooterTeam;                  // Equipa do atirador
            projectile.ownerRoot = transform.root;              // Root do jogador
        }
        else
        {
            Debug.LogError("[PlayerWeaponController] O prefab da bala não tem Projectile.cs.");
        }

        // Verifica e spawn o NetworkObject da bala
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

    [ClientRpc]
    private void PlayMuzzleEffectClientRpc(ClientRpcParams rpcParams = default)
    {
        // Não reproduz no owner (porque ele já vê o efeito localmente)
        if (IsOwner) return;

        if (playerCharacter == null) return;

        var inventory = playerCharacter.GetInventory();
        if (inventory == null) return;

        // Obtém a arma equipada e toca efeito de boca
        var weapon = inventory.GetEquipped() as InfimaGames.LowPolyShooterPack.Weapon;
        if (weapon != null)
        {
            weapon.PlayMuzzleEffect();
        }
    }
}
