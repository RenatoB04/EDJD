using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public abstract class MagazineBehaviour : MonoBehaviour
    {
        #region GETTERS
        public abstract int GetAmmunitionTotal();
        public abstract Sprite GetSprite();
        #endregion
    }
}