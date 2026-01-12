using Unity.Netcode.Components;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    [DisallowMultipleComponent] // Garante que só pode haver 1 deste componente por GameObject
    public class ClientNetworkAnimator : NetworkAnimator
    {
        // Define se o servidor é autoritativo para a animação
        // Neste caso, false significa que o cliente controla as animações
        protected override bool OnIsServerAuthoritative()
        {
            return false; 
        }
    }
}
