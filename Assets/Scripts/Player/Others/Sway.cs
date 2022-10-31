using UnityEngine;

namespace Game.Player.Others
{
    public class Sway : MonoBehaviour
    {
        [Range(0, 1)] public float accuracy = 1;
        [Space]
        [SerializeField] private float smoothSpeed = 2;
        [SerializeField] private float resetSpeed = 5;
        [SerializeField] private float moveAmount = 1;

        private Quaternion startRot;

        private void Start()
        {
            startRot = this.transform.localRotation;
        }

        private void Update()
        {
            // Apply movement
            Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (mouseAxis != Vector2.zero)
            {
                Quaternion quat = Quaternion.Euler((mouseAxis.y * moveAmount * 2) * accuracy, (-mouseAxis.x * moveAmount) * accuracy, transform.localRotation.z);
                transform.localRotation = Quaternion.Lerp(transform.localRotation,
                transform.localRotation * quat, Time.deltaTime * smoothSpeed);
            }

            // Reset for start rotation
            transform.localRotation = Quaternion.Lerp(transform.localRotation,
                            startRot, Time.deltaTime * resetSpeed);
        }
    }
}