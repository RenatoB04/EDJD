using UnityEngine;
namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class TextTimescale : ElementText
    {
        #region METHODS
        protected override void Tick()
        {
            textMesh.text = "Timescale : " + Time.timeScale;
        }        
        #endregion
    }
}