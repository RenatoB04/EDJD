using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCam : MonoBehaviour
{
    [Header("Velocidades")]
    public float moveSpeed = 6f;
    public float turboMultiplier = 8f;
    public float lookSensitivity = 0.1f; 

    [Header("Pitch Clamp (evitar dar a volta)")]
    public float minPitch = -85f;
    public float maxPitch =  85f;

    [Header("Cursor")]
    public bool lockCursorWhileAiming = true;
    
    float yaw;
    float pitch;
    
    InputAction lookAction;
    InputAction moveAction;
    InputAction upDownAction;
    InputAction rotateHoldAction;
    InputAction turboHoldAction;

    void Awake()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        var map = new InputActionMap("SimpleFreeCam");

        lookAction = map.AddAction("Look", binding: "<Mouse>/delta");

        moveAction = map.AddAction("Move");
        var move = moveAction.AddCompositeBinding("2DVector");
        move.With("Up", "<Keyboard>/w").With("Up", "<Keyboard>/upArrow");
        move.With("Down", "<Keyboard>/s").With("Down", "<Keyboard>/downArrow");
        move.With("Left", "<Keyboard>/a").With("Left", "<Keyboard>/leftArrow");
        move.With("Right","<Keyboard>/d").With("Right","<Keyboard>/rightArrow");

        upDownAction = map.AddAction("UpDown");
        var yd = upDownAction.AddCompositeBinding("1DAxis");
        yd.With("Negative", "<Keyboard>/q");
        yd.With("Positive", "<Keyboard>/e");

        rotateHoldAction = map.AddAction("RotateHold", binding: "<Mouse>/rightButton");
        
        turboHoldAction = map.AddAction("TurboHold", binding: "<Keyboard>/leftShift");

        map.Enable();
    }

    void OnDisable()
    {
        lookAction?.Disable();
        moveAction?.Disable();
        upDownAction?.Disable();
        rotateHoldAction?.Disable();
        turboHoldAction?.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        bool rotating = rotateHoldAction.IsPressed();
        if (rotating)
        {
            if (lockCursorWhileAiming)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Vector2 delta = lookAction.ReadValue<Vector2>();
            yaw   += delta.x * lookSensitivity;
            pitch -= delta.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else if (lockCursorWhileAiming)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        Vector2 move2D = moveAction.ReadValue<Vector2>();
        float upDown = upDownAction.ReadValue<float>();

        Vector3 dir = new Vector3(move2D.x, upDown, move2D.y);
        if (dir.sqrMagnitude > 0f)
        {
            float speed = moveSpeed * Time.deltaTime;
            
            if (turboHoldAction.IsPressed())
                speed *= turboMultiplier;

            transform.position +=
                transform.right   * (dir.x * speed) +
                transform.up      * (dir.y * speed) +
                transform.forward * (dir.z * speed);
        }
    }
}
