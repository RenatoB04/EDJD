using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        #region UNITY
        protected virtual void Awake(){}
        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}
        #endregion
        #region GETTERS
        public abstract Sprite GetSpriteBody();
        public abstract AudioClip GetAudioClipHolster();
        public abstract AudioClip GetAudioClipUnholster();
        public abstract AudioClip GetAudioClipReload();
        public abstract AudioClip GetAudioClipReloadEmpty();
        public abstract AudioClip GetAudioClipFireEmpty();
        public abstract AudioClip GetAudioClipFire();
        public abstract int GetAmmunitionCurrent();
        public abstract int GetAmmunitionTotal();
        public abstract Animator GetAnimator();
        public abstract bool IsAutomatic();
        public abstract bool HasAmmunition();
        public abstract bool IsFull();
        public abstract float GetRateOfFire();
        public abstract RuntimeAnimatorController GetAnimatorController();
        public abstract WeaponAttachmentManagerBehaviour GetAttachmentManager();
        #endregion
        #region METHODS
        public abstract void Fire(float spreadMultiplier = 1.0f);
        public abstract void Reload();
        public abstract void FillAmmunition(int amount);
        public abstract void EjectCasing();
        #endregion
    }
}