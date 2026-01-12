using TMPro;
using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public class DisplayMaterialName : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        [Header("Settings")]
        [Tooltip("Mesh.")]
        [SerializeField]
        private Renderer mesh;
        [Tooltip("Text.")]
        [SerializeField]
        private TextMeshProUGUI materialText;
        #endregion
        #region FIELDS
        private Material meshMaterial;
        #endregion
        #region UNITY
        private void Start()
        {
            string sharedMaterialName = mesh.sharedMaterial.name;
            materialText.text = sharedMaterialName;
        }
        #endregion
    }
}