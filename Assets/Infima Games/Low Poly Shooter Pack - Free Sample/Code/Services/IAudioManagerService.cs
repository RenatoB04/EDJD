using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public interface IAudioManagerService : IGameService
    {
        void PlayOneShot(AudioClip clip, AudioSettings settings = default);
        void PlayOneShotDelayed(AudioClip clip, AudioSettings settings = default, float delay = 1.0f);
    }
}