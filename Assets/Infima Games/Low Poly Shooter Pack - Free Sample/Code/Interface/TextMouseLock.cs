namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class TextMouseLock : ElementText
    {
        #region METHODS
        protected override void Tick()
        {
            textMesh.text = "Cursor " + (playerCharacter.IsCursorLocked() ? "Locked" : "Unlocked");
        }
        #endregion
    }
}