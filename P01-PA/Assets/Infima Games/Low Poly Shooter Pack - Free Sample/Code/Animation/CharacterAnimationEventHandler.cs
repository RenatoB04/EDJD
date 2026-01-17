using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
	public class CharacterAnimationEventHandler : MonoBehaviour
	{
		#region FIELDS
        private CharacterBehaviour playerCharacter;
		#endregion
		#region UNITY
		private void Awake()
		{
			playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
		}
		#endregion
		#region ANIMATION
		private void OnEjectCasing()
		{
			if(playerCharacter != null)
				playerCharacter.EjectCasing();
		}
		private void OnAmmunitionFill(int amount = 0)
		{
			if(playerCharacter != null)
				playerCharacter.FillAmmunition(amount);
		}
		private void OnSetActiveKnife(int active)
		{
		}
		private void OnGrenade()
		{
		}
		private void OnSetActiveMagazine(int active)
		{
			if(playerCharacter != null)
				playerCharacter.SetActiveMagazine(active);
		}
		private void OnAnimationEndedBolt()
		{
		}
		private void OnAnimationEndedReload()
		{
			if(playerCharacter != null)
				playerCharacter.AnimationEndedReload();
		}
		private void OnAnimationEndedGrenadeThrow()
		{
		}
		private void OnAnimationEndedMelee()
		{
		}
		private void OnAnimationEndedInspect()
		{
			if(playerCharacter != null)
				playerCharacter.AnimationEndedInspect();
		}
		private void OnAnimationEndedHolster()
		{
			if(playerCharacter != null)
				playerCharacter.AnimationEndedHolster();
		}
		private void OnSlideBack(int back)
		{
		}
		#endregion
	}   
}