using TMPro;
using UnityEngine;
namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class TextTutorial : ElementText
    {
        #region FIELDS SERIALIZED
        [Header("References")]
        [Tooltip("Tutorial prompt text.")]
        [SerializeField]
        private TextMeshProUGUI prompt;
        [Tooltip("Tutorial text.")]
        [SerializeField]
        private TextMeshProUGUI tutorial;
        #endregion
        #region UNITY
        protected override void Awake()
        {
            base.Awake();
            prompt.enabled = true;
            tutorial.enabled = false;
        }
        #endregion
        #region METHODS
        protected override void Tick()
        {
            bool isVisible = playerCharacter.IsTutorialTextVisible();
            prompt.enabled = !isVisible;
            tutorial.enabled = isVisible;
        }
        #endregion
    }
}