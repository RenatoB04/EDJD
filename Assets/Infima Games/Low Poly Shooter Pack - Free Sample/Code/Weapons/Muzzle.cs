using UnityEngine;
using System.Collections;
namespace InfimaGames.LowPolyShooterPack
{
    public class Muzzle : MuzzleBehaviour
    {
        #region FIELDS SERIALIZED
        [Header("Settings")]
        [Tooltip("Socket at the tip of the Muzzle. Commonly used as a firing point.")]
        [SerializeField]
        private Transform socket;
        [Tooltip("Sprite. Displayed on the player's interface.")]
        [SerializeField]
        private Sprite sprite;
        [Tooltip("Audio clip played when firing through this muzzle.")]
        [SerializeField]
        private AudioClip audioClipFire;
        [Header("Audio")]
        [Tooltip("AudioSource usado para reproduzir o som do tiro. Idealmente colocado no mesmo objecto do muzzle/socket.")]
        [SerializeField]
        private AudioSource audioSource;
        [Header("Particles")]
        [Tooltip("Firing Particles.")]
        [SerializeField]
        private GameObject prefabFlashParticles;
        [Tooltip("Number of particles to emit when firing.")]
        [SerializeField]
        private int flashParticlesCount = 5;
        [Header("Flash Light")]
        [Tooltip("Muzzle Flash Prefab. A small light we use when firing.")]
        [SerializeField]
        private GameObject prefabFlashLight;
        [Tooltip("Time that the light flashed stays active. After this time, it is disabled.")]
        [SerializeField]
        private float flashLightDuration;
        [Tooltip("Local offset applied to the light.")]
        [SerializeField]
        private Vector3 flashLightOffset;
        #endregion
        #region FIELDS
        private ParticleSystem particles;
        private Light flashLight;
        #endregion
        #region UNITY FUNCTIONS
        private void Awake()
        {
            if(prefabFlashParticles != null)
            {
                GameObject spawnedParticlesPrefab = Instantiate(prefabFlashParticles, socket);
                spawnedParticlesPrefab.transform.localPosition = default;
                spawnedParticlesPrefab.transform.localEulerAngles = default;
                particles = spawnedParticlesPrefab.GetComponent<ParticleSystem>();
            }
            if (prefabFlashLight)
            {
                GameObject spawnedFlashLightPrefab = Instantiate(prefabFlashLight, socket);
                spawnedFlashLightPrefab.transform.localPosition = flashLightOffset;
                spawnedFlashLightPrefab.transform.localEulerAngles = default;
                flashLight = spawnedFlashLightPrefab.GetComponent<Light>();
                flashLight.enabled = false;
            }
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null && socket != null)
                    audioSource = socket.GetComponent<AudioSource>();
            }
        }
        #endregion
        #region GETTERS
        public override void Effect()
        {
            if(particles != null)
                particles.Emit(flashParticlesCount);
            if (flashLight != null)
            {
                flashLight.enabled = true;
                StartCoroutine(nameof(DisableLight));
            }
            if (audioClipFire != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(audioClipFire);
                }
                else
                {
                    Vector3 pos = socket != null ? socket.position : transform.position;
                    AudioSource.PlayClipAtPoint(audioClipFire, pos);
                }
            }
        }
        public override Transform GetSocket() => socket;
        public override Sprite GetSprite() => sprite;
        public override AudioClip GetAudioClipFire() => audioClipFire;
        public override ParticleSystem GetParticlesFire() => particles;
        public override int GetParticlesFireCount() => flashParticlesCount;
        public override Light GetFlashLight() => flashLight;
        public override float GetFlashLightDuration() => flashLightDuration;
        #endregion
        #region METHODS
        private IEnumerator DisableLight()
        {
            yield return new WaitForSeconds(flashLightDuration);
            flashLight.enabled = false;
        }
        #endregion
    }
}