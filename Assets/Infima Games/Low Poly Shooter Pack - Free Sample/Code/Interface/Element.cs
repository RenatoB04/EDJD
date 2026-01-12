using UnityEngine;
namespace InfimaGames.LowPolyShooterPack.Interface
{
    public abstract class Element : MonoBehaviour
    {
        #region FIELDS
        protected IGameModeService gameModeService;
        protected CharacterBehaviour playerCharacter;
        protected InventoryBehaviour playerCharacterInventory;
        protected WeaponBehaviour equippedWeapon;
        #endregion
        #region UNITY
        protected virtual void Awake()
        {
            gameModeService = ServiceLocator.Current.Get<IGameModeService>();
            playerCharacter = gameModeService.GetPlayerCharacter();
            playerCharacterInventory = playerCharacter.GetInventory();
        }
        private void Update()
        {
            if (Equals(playerCharacterInventory, null))
                return;
            equippedWeapon = playerCharacterInventory.GetEquipped();
            Tick();
        }
        #endregion
        #region METHODS
        protected virtual void Tick() {}
        #endregion
    }
}