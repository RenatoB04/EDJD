using UnityEngine;
namespace InfimaGames.LowPolyShooterPack.Interface
{
    public class CanvasSpawner : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        [Header("Settings")]
        [Tooltip("Canvas prefab spawned at start. Displays the player's user interface.")]
        [SerializeField]
        private GameObject canvasPrefab;
        #endregion
        private GameObject canvas;
        #region UNITY FUNCTIONS
        private void Awake()
        {
        }
        #endregion
        #region PUBLIC METHODS 
        public void SpawnCanvas()
        {
            if (canvas == null)
            {
                canvas = Instantiate(canvasPrefab);
            }
        }
        #endregion
    }
}