using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    [System.Serializable]
    public struct AudioSettings
    {
        public bool AutomaticCleanup => automaticCleanup;
        public float Volume => volume;
        public float SpatialBlend => spatialBlend;
        [Header("Settings")]
        [Tooltip("If true, any AudioSource created will be removed after it has finished playing its clip.")]
        [SerializeField]
        private bool automaticCleanup;
        [Tooltip("Volume.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float volume;
        [Tooltip("Spatial Blend.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float spatialBlend;
        public AudioSettings(float volume = 1.0f, float spatialBlend = 0.0f, bool automaticCleanup = true)
        {
            this.volume = volume;
            this.spatialBlend = spatialBlend;
            this.automaticCleanup = automaticCleanup;
        }
    }
}