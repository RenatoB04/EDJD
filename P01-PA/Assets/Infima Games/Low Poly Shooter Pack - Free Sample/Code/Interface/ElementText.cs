using TMPro;
using UnityEngine;
namespace InfimaGames.LowPolyShooterPack.Interface
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class ElementText : Element
    {
        #region FIELDS
        protected TextMeshProUGUI textMesh;
        #endregion
        #region UNITY
        protected override void Awake()
        {
            base.Awake();
            textMesh = GetComponent<TextMeshProUGUI>();
        }
        #endregion
    }
}