using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public class PlaySoundBehaviour : StateMachineBehaviour
    {
        #region FIELDS SERIALIZED
        [Header("Setup")]
        [Tooltip("AudioClip to play!")]
        [SerializeField]
        private AudioClip clip;
        [Header("Settings")]
        [Tooltip("Audio Settings.")]
        [SerializeField]
        private AudioSettings settings = new AudioSettings(1.0f, 0.0f, true);
        private IAudioManagerService audioManagerService;
        #endregion
        #region UNITY
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            audioManagerService ??= ServiceLocator.Current.Get<IAudioManagerService>();
            audioManagerService?.PlayOneShot(clip, settings);
        }
        #endregion
    }
}