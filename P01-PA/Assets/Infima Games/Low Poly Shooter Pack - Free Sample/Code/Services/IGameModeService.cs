namespace InfimaGames.LowPolyShooterPack
{
    public interface IGameModeService : IGameService
    {
        CharacterBehaviour GetPlayerCharacter();
    }
}