using System.Collections.Generic;
using UnityEngine;
using Game.Utilities;

namespace Game.Player
{
    [System.Serializable]
    public class CameraStates : States
    {
        public CameraStates()
        {
            states = new List<State>()
            {
               new State("Enabled"),
               new State("CursorLocked")
            };
        }
    }

    public class PlayerCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float sensitivity = 3f;
        [SerializeField] private bool invertedCamera;

        [Header("Clamp")]
        [SerializeField] private float minClampY = -90f;
        [SerializeField] private float maxClampY = 90f;

        CameraStates states;
        Vector2 _mouseRotation = Vector2.zero;
        float rotTransformY;
        float rotCameraX;

        public void SetState(string _name, bool _value)
        => states.SetState(_name, _value);

        public bool GetState(string _name)
        => states.GetState(_name);

        private void Start()
        {
            // Setting states
            states = new CameraStates();
            SetState("Enabled", true);
            SetState("CursorLocked", true);

            // Setting values
            _mouseRotation.y = rotTransformY;
            _mouseRotation.x = rotCameraX;
        }

        private void Update()
        {
            if (GetState("Enabled"))
            {
                // Mouse Look
                Vector2 _cameraAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // Easy to implement new input system

                _mouseRotation.x += _cameraAxis.x * sensitivity;
                _mouseRotation.y += (invertedCamera ? _cameraAxis.y : -_cameraAxis.y) * sensitivity;
                _mouseRotation.y = Mathf.Clamp(_mouseRotation.y, minClampY, maxClampY); // Limit vertical angle

                // Rotate camera to vertical
                cameraTransform.localRotation = Quaternion.Euler(
                    _mouseRotation.y,
                    cameraTransform.localRotation.y,
                    cameraTransform.localRotation.z);

                // Rotate body to horizontal
                transform.localRotation = Quaternion.Euler(
                    transform.localRotation.x,
                    _mouseRotation.x,
                    transform.localRotation.z);
            }
        }

        private void LateUpdate()
        {
            if (GetState("CursorLocked"))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}