using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public abstract class WeaponAttachmentManagerBehaviour : MonoBehaviour
    {
        #region UNITY FUNCTIONS
        protected virtual void Awake(){}
        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}
        #endregion
        #region GETTERS
        public abstract ScopeBehaviour GetEquippedScope();
        public abstract ScopeBehaviour GetEquippedScopeDefault();
        public abstract MagazineBehaviour GetEquippedMagazine();
        public abstract MuzzleBehaviour GetEquippedMuzzle();
        #endregion
    }
}