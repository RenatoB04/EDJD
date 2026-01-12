using UnityEngine;
namespace InfimaGames.LowPolyShooterPack
{
    public class CameraLook : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        [Header("Network Ref")] 
        [Tooltip("Referência ao script Character (Controller de Rede).")]
        public Character characterNetcode; 
        [Header("Settings")]
        [Tooltip("Sensitivity when looking around.")]
        [SerializeField]
        private Vector2 sensitivity = new Vector2(1, 1);
        [Tooltip("Minimum and maximum up/down rotation angle the camera can have.")]
        [SerializeField]
        private Vector2 yClamp = new Vector2(-60, 60);
        [Tooltip("Should the look rotation be interpolated?")]
        [SerializeField]
        private bool smooth;
        [Tooltip("The speed at which the look rotation is interpolated.")]
        [SerializeField]
        private float interpolationSpeed = 25.0f;
        #endregion
        #region FIELDS
        private Character playerCharacter;
        private Rigidbody playerCharacterRigidbody;
        private Quaternion rotationCharacter;
        private Quaternion rotationCamera;
        #endregion
        #region UNITY
        protected void Awake()
        {
            if (characterNetcode == null)
            {
                characterNetcode = GetComponentInParent<Character>(); 
            }
            playerCharacter = characterNetcode;
            if(playerCharacter == null)
            {
                Debug.LogError("CameraLook: O script 'Character' (Controller de Rede) não foi encontrado.");
                return;
            }
            playerCharacterRigidbody = playerCharacter.GetComponent<Rigidbody>();
        }
        protected void Start()
        {
            if (playerCharacterRigidbody == null)
            {
                 Debug.LogError("CameraLook: Rigidbody não encontrado no objeto principal do Player.");
                 return;
            }
            rotationCharacter = playerCharacter.transform.localRotation;
            rotationCamera = transform.localRotation;
        }
        protected void LateUpdate()
        {
            if (playerCharacter == null || !playerCharacter.isActiveAndEnabled || !playerCharacter.IsOwner) return;
            Vector2 frameInput = playerCharacter.IsCursorLocked() ? playerCharacter.GetInputLook() : default;
            frameInput *= sensitivity;
            Quaternion rotationYaw = Quaternion.Euler(0.0f, frameInput.x, 0.0f);
            Quaternion rotationPitch = Quaternion.Euler(-frameInput.y, 0.0f, 0.0f);
            rotationCamera *= rotationPitch;
            rotationCharacter *= rotationYaw;
            Quaternion localRotation = transform.localRotation;
            if (smooth)
            {
                localRotation = Quaternion.Slerp(localRotation, rotationCamera, Time.deltaTime * interpolationSpeed);
                playerCharacterRigidbody.MoveRotation(Quaternion.Slerp(playerCharacterRigidbody.rotation, rotationCharacter, Time.deltaTime * interpolationSpeed));
            }
            else
            {
                localRotation *= rotationPitch;
                localRotation = Clamp(localRotation);
                playerCharacterRigidbody.MoveRotation(playerCharacterRigidbody.rotation * rotationYaw);
            }
            transform.localRotation = localRotation;
        }
        #endregion
        #region FUNCTIONS
        private Quaternion Clamp(Quaternion rotation)
        {
            rotation.x /= rotation.w;
            rotation.y /= rotation.w;
            rotation.z /= rotation.w;
            rotation.w = 1.0f;
            float pitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(rotation.x);
            pitch = Mathf.Clamp(pitch, yClamp.x, yClamp.y);
            rotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * pitch);
            return rotation;
        }
        #endregion
    }
}