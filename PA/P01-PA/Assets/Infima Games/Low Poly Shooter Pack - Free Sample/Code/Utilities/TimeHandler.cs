using UnityEngine;
using UnityEngine.InputSystem;
namespace InfimaGames.LowPolyShooterPack
{
    public class TimeHandler : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Value the time scale gets updated by every time.")]
        [SerializeField]
        private float increment = 0.1f;
        private bool paused;
        private float current = 1.0f;
        private void Scale()
        {
            Time.timeScale = current;
        }
        private void Change(float value = 1.0f)
        {
            current = value;
            Scale();
        }
        private void Increase(float value = 1.0f)
        {
            Change(Mathf.Clamp01(current + value));
        }
        private void Pause()
        {
            paused = true;
            Time.timeScale = 0.0f;
        }
        private void Toggle()
        {
            if (paused)
                Unpause();
            else
                Pause();
        }
        private void Unpause()
        {
            paused = false;
            Change(current);
        }
        public virtual void OnIncrease(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Increase(increment);
                    break;
            }
        }
        public virtual void OnDecrease(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Increase(-increment);
                    break;
            }
        }
        public virtual void OnToggle(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    Toggle();
                    break;
            }      
        }
    }
}