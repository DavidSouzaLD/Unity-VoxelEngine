using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Move")]
        public float moveSpeed = 5f;
        public float runSpeed = 8f;
        public float crouchSpeed = 2f;

        [Header("Jump")]
        public float jumpForce = 5f;
        public float gravityScale = 1f;

        [Header("Crouch Extents")]
        public float angleToCheck;

        private bool jumpRequested;
        private Vector3 velocity;
        private CharacterController controller;
        Vector3 currentDirection;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            // Reset jump
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = 0;
            }

            // Move
            Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 direction = (transform.forward * moveAxis.y + transform.right * moveAxis.x).normalized;
            float currentSpeed = (!(Input.GetKey(PlayerKeys.Run) && moveAxis.y > 0) ? (Input.GetKey(PlayerKeys.Crouch) ? crouchSpeed : moveSpeed) : runSpeed) / 100f;
            currentDirection = direction;

            if (Input.GetKey(PlayerKeys.Crouch) && controller.isGrounded && CrouchCheck(direction))
            {
                controller.Move(direction * currentSpeed);
            }
            else if (!Input.GetKey(PlayerKeys.Crouch))
            {
                controller.Move(direction * currentSpeed);
            }

            // Jump
            if (jumpRequested)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -3.0f * Physics.gravity.y);
                jumpRequested = false;
            }

            velocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void Update()
        {
            // Jump
            if (Input.GetKeyDown(PlayerKeys.Jump) && controller.isGrounded)
            {
                jumpRequested = true;
            }
        }

        private bool CrouchCheck(Vector3 _direction)
        {
            Vector3 pos = new Vector3(
                    transform.position.x + controller.center.x,
                    transform.position.y + controller.center.y - controller.height / 2,
                    transform.position.z + controller.center.z) - currentDirection * 0.1f;

            return Physics.Raycast(pos, new Vector3(_direction.x, _direction.y - angleToCheck, _direction.z), .5f);
        }

        private void OnDrawGizmosSelected()
        {
            if (controller != null)
            {
                Gizmos.color = Color.red;

                Vector3 pos = new Vector3(
                    transform.position.x + controller.center.x,
                    transform.position.y + controller.center.y - controller.height / 2,
                    transform.position.z + controller.center.z);

                Gizmos.DrawRay(pos, new Vector3(currentDirection.x, currentDirection.y - angleToCheck, currentDirection.z));
            }
            else
            {
                controller = GetComponent<CharacterController>();
            }
        }
    }
}