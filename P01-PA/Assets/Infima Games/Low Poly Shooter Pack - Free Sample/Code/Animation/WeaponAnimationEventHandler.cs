using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public class WeaponAnimationEventHandler : MonoBehaviour
    {
        #region FIELDS
        private WeaponBehaviour weapon;
        #endregion
        #region UNITY
        private void Awake()
        {
            weapon = GetComponent<WeaponBehaviour>();
        }
        #endregion
        #region ANIMATION
        private void OnEjectCasing()
        {
            if(weapon != null)
                weapon.EjectCasing();
        }
        #endregion
    }
}