using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public class PlaySoundCharacterBehaviour : StateMachineBehaviour
    {
        private enum SoundType
        {
            Holster, Unholster,
            Reload, ReloadEmpty,
            Fire, FireEmpty,
        }
        #region FIELDS SERIALIZED
        [Header("Setup")]
        [Tooltip("Delay at which the audio is played.")]
        [SerializeField]
        private float delay;
        [Tooltip("Type of weapon sound to play.")]
        [SerializeField]
        private SoundType soundType;
        [Header("Audio Settings")]
        [Tooltip("Audio Settings.")]
        [SerializeField]
        private AudioSettings audioSettings = new AudioSettings(1.0f, 0.0f, true);
        #endregion
        #region FIELDS
        private Character playerCharacter; 
        private InventoryBehaviour playerInventory;
        private IAudioManagerService audioManagerService; 
        #endregion
        #region UNITY
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerCharacter == null)
            {
                playerCharacter = animator.GetComponentInParent<Character>();
            }
            if (playerCharacter == null || !playerCharacter.IsOwner)
                return;
            playerInventory ??= playerCharacter.GetInventory();
            if (playerInventory == null || !(playerInventory.GetEquipped() is { } weaponBehaviour))
                return;
            if (!playerCharacter.TryGetComponent<AudioSource>(out var audioSource))
            {
                 return;
            }
            #region Select Correct Clip To Play
            AudioClip clip = soundType switch
            {
                SoundType.Holster => weaponBehaviour.GetAudioClipHolster(),
                SoundType.Unholster => weaponBehaviour.GetAudioClipUnholster(),
                SoundType.Reload => weaponBehaviour.GetAudioClipReload(),
                SoundType.ReloadEmpty => weaponBehaviour.GetAudioClipReloadEmpty(),
                SoundType.Fire => weaponBehaviour.GetAudioClipFire(),
                SoundType.FireEmpty => weaponBehaviour.GetAudioClipFireEmpty(),
                _ => default
            };
            #endregion
            if (clip != null)
            {
                float finalVolume = 1.0f; 
                if (delay > 0.001f)
                    audioSource.PlayDelayed(delay);
                else
                    audioSource.PlayOneShot(clip, finalVolume); 
            }
        }
        #endregion
    }
}