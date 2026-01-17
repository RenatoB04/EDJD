namespace InfimaGames.LowPolyShooterPack
{
    public class GameModeService : IGameModeService
    {
        #region FIELDS
        private CharacterBehaviour playerCharacter;
        #endregion
        #region FUNCTIONS
        public CharacterBehaviour GetPlayerCharacter()
        {
            if (playerCharacter == null)
                playerCharacter = UnityEngine.Object.FindObjectOfType<CharacterBehaviour>();
            return playerCharacter;
        }
        #endregion
    }
}