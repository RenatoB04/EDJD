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
    private Health ownerHealth;
    private Character playerCharacter;
    private void Awake()
    {
        ownerHealth = GetComponent<Health>();
        playerCharacter = GetComponent<Character>();
        if (ownerHealth == null)
            Debug.LogError("PlayerWeaponController: Falta componente Health no Player.");
        if (playerCharacter == null)
            Debug.LogError("PlayerWeaponController: Falta componente Character no Player.");
    }
    public void FireExternally(Vector3 direction, Vector3 origin, float speed)
    {
        if (!IsOwner)
            return;
        if (ownerHealth == null)
        {
            Debug.LogError("PlayerWeaponController: ownerHealth nulo em FireExternally.");
            return;
        }
        SpawnBulletServerRpc(
            origin,
            direction,
            speed,
            ownerHealth.team.Value,
            OwnerClientId
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
        if (!IsServer)
            return;
        if (bulletPrefab == null)
        {
            Debug.LogError("[PlayerWeaponController] Bullet Prefab não atribuído.");
            return;
        }
        PlayMuzzleEffectClientRpc();
        var bulletGO = Instantiate(
            bulletPrefab,
            position,
            Quaternion.LookRotation(direction)
        );
        if (bulletGO.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.initialVelocity.Value = direction * speed;
            projectile.ownerClientId = shooterClientId;
            projectile.ownerTeam = shooterTeam;
            projectile.ownerRoot = transform.root;
        }
        else
        {
            Debug.LogError("[PlayerWeaponController] O prefab da bala não tem Projectile.cs.");
        }
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