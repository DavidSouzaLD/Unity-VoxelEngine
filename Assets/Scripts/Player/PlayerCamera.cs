using UnityEngine;

namespace Game.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float sensitivity = 3f;
        [SerializeField] private bool invertedCamera;

        [Header("Clamp")]
        [SerializeField] private float minClampY = -90f;
        [SerializeField] private float maxClampY = 90f;

        bool lockCursor;
        Vector2 _mouseRotation = Vector2.zero;
        float rotTransformY;
        float rotCameraX;

        private void Start()
        {
            _mouseRotation.y = rotTransformY;
            _mouseRotation.x = rotCameraX;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
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

            if (Input.GetKeyDown(PlayerKeys.LockCursor))
            {
                lockCursor = !lockCursor;

                if (lockCursor)
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
}