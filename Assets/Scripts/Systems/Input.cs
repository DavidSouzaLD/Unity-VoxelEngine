using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Systems
{
    /// <summary>
    /// Manages and returns the values ​​of pressed buttons and keys;
    /// </summary>
    [DisallowMultipleComponent]
    public class Input : MonoBehaviour
    {
        private static InputMap Map;

        private void Awake()
        {
            Map = new InputMap();
        }

        private void OnEnable()
        {
            Map.Enable();
        }

        private void OnDisable()
        {
            Map.Disable();
        }

        public static bool GetBool(string _keyName)
        {
            InputAction action = Map.FindAction(_keyName);

            if (action == null)
            {
                ErrorMessage(_keyName);
                return false;
            }

            return action.ReadValue<float>() != 0;
        }

        public static float GetFloat(string _keyName)
        {
            InputAction action = Map.FindAction(_keyName);

            if (action == null)
            {
                ErrorMessage(_keyName);
                return 0;
            }

            return action.ReadValue<float>();
        }

        public static Vector2 GetVector2(string _keyName)
        {
            InputAction action = Map.FindAction(_keyName);

            if (action == null)
            {
                ErrorMessage(_keyName);
                return Vector2.zero;
            }

            return action.ReadValue<Vector2>();
        }

        private static void ErrorMessage(string _message)
        {
            Debug.LogError("(" + _message + ") not found in InputSystem!");
        }
    }
}