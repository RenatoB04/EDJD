using UnityEngine;
using UnityEngine.UI;
namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class ImageWeapon : Element
    {
        #region FIELDS SERIALIZED
        [Header("Settings")]
        [Tooltip("Weapon Body Image.")]
        [SerializeField]
        private Image imageWeaponBody;
        [Tooltip("Weapon Magazine Image.")]
        [SerializeField]
        private Image imageWeaponMagazine;
        [Tooltip("Weapon Scope Default Image.")]
        [SerializeField]
        private Image imageWeaponScopeDefault;
        #endregion
        #region FIELDS
        private WeaponAttachmentManagerBehaviour attachmentManagerBehaviour;
        #endregion
        #region METHODS
        protected override void Tick()
        {
            attachmentManagerBehaviour = equippedWeapon.GetAttachmentManager();
            imageWeaponBody.sprite = equippedWeapon.GetSpriteBody();
            Sprite sprite = default;
            ScopeBehaviour scopeDefaultBehaviour = attachmentManagerBehaviour.GetEquippedScopeDefault();
            if (scopeDefaultBehaviour != null)
                sprite = scopeDefaultBehaviour.GetSprite();
            AssignSprite(imageWeaponScopeDefault, sprite, scopeDefaultBehaviour == null);
            MagazineBehaviour magazineBehaviour = attachmentManagerBehaviour.GetEquippedMagazine();
            if (magazineBehaviour != null)
                sprite = magazineBehaviour.GetSprite();
            AssignSprite(imageWeaponMagazine, sprite, magazineBehaviour == null);
        }
        private static void AssignSprite(Image image, Sprite sprite, bool forceHide = false)
        {
            image.sprite = sprite;
            image.enabled = sprite != null && !forceHide;
        }
        #endregion
    }
}