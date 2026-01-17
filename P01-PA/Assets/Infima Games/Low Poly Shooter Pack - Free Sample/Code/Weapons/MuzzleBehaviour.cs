using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public abstract class MuzzleBehaviour : MonoBehaviour
    {
        #region GETTERS
        public abstract Transform GetSocket();
        public abstract Sprite GetSprite();
        public abstract AudioClip GetAudioClipFire();
        public abstract ParticleSystem GetParticlesFire();
        public abstract int GetParticlesFireCount();
        public abstract Light GetFlashLight();
        public abstract float GetFlashLightDuration();
        #endregion
        #region METHODS
        public abstract void Effect(); 
        #endregion
    }
}