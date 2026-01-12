using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public class Magazine : MagazineBehaviour
    {
        #region FIELDS SERIALIZED
        [Header("Settings")]
        [Tooltip("Total Ammunition.")]
        [SerializeField]
        private int ammunitionTotal = 10;
        [Header("Interface")]
        [Tooltip("Interface Sprite.")]
        [SerializeField]
        private Sprite sprite;
        #endregion
        #region GETTERS
        public override int GetAmmunitionTotal() => ammunitionTotal;
        public override Sprite GetSprite() => sprite;
        #endregion
    }
}