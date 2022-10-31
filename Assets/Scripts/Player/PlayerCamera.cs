using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(PlayerConsole))]
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Settings")]
        public Transform cameraTransform;
        public float sensitivity = 3f;
        public bool invertedCamera;

        [Header("Clamp")]
        public float minClampY = -90f;
        public float maxClampY = 90f;

        private Vector2 _mouseRotation = Vector2.zero;
        private float rotTransformY;
        private float rotCameraX;
        private PlayerConsole playerConsole;

        private Vector2 CameraAxis
        {
            get
            {
                return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
        }

        private void Start()
        {
            // Setting values
            _mouseRotation.y = rotTransformY;
            _mouseRotation.x = rotCameraX;

            // Get components
            playerConsole = GetComponent<PlayerConsole>();
        }

        private void Update()
        {
            bool inputConditions = CameraAxis != Vector2.zero;
            bool lookConditions = !playerConsole.consoleEnabled;

            if (inputConditions && lookConditions)
            {
                // Mouse Look
                _mouseRotation.x += CameraAxis.x * sensitivity;
                _mouseRotation.y += (invertedCamera ? CameraAxis.y : -CameraAxis.y) * sensitivity;
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
            bool lockCursorConditions = !playerConsole.consoleEnabled;

            if (lockCursorConditions)
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