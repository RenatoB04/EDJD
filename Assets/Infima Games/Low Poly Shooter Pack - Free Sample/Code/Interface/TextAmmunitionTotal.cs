using System.Globalization;
namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class TextAmmunitionTotal : ElementText
    {
        #region METHODS
        protected override void Tick()
        {
            float ammunitionTotal = equippedWeapon.GetAmmunitionTotal();
            textMesh.text = ammunitionTotal.ToString(CultureInfo.InvariantCulture);
        }
        #endregion
    }
}