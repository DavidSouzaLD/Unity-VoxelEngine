using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Move")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float jumpForce = 5f;
        [Space]
        [SerializeField] private float gravityScale = 1f;

        private bool jumpRequested;
        private Vector3 velocity;
        private CharacterController controller;

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
            float currentSpeed = (!Input.GetKey(PlayerKeys.Run) ? moveSpeed : runSpeed) / 100f;
            Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 direction = transform.forward * moveAxis.y + transform.right * moveAxis.x;

            controller.Move(direction * currentSpeed);

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
    }
}