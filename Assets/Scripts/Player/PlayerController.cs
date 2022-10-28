using System.Collections.Generic;
using UnityEngine;
using Game.Utilities;

namespace Game.Player
{
    [System.Serializable]
    public class PlayerStates : States
    {
        public PlayerStates()
        {
            states = new List<State>()
            {
               new State("Enabled"),
               new State("Grounded"),
               new State("Walking"),
               new State("Running"),
               new State("Crouching")
            };
        }
    }

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

        [Header("Debug")]
        public GameObject debugPanel;

        // Private
        private bool jumpRequested;
        private bool debugEnabled;
        private Vector3 currentDirection;
        private Vector3 velocity;

        // Components
        private CharacterController controller;
        private PlayerCamera playerCamera;
        private PlayerBuilder playerBuilder;
        private PlayerStates states;

        public void SetState(string _name, bool _value)
        => states.SetState(_name, _value);

        public bool GetState(string _name)
        => states.GetState(_name);

        private void Start()
        {
            // Setting states
            states = new PlayerStates();
            SetState("Enabled", true);

            // Get components
            controller = GetComponent<CharacterController>();
            playerCamera = GetComponent<PlayerCamera>();
            playerBuilder = GetComponent<PlayerBuilder>();
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

            // Set states
            SetState("Walking", moveAxis != Vector2.zero && controller.velocity.magnitude > 0);
            SetState("Running", Input.GetKey(PlayerKeys.Run) && moveAxis.y > 0);
            SetState("Crouching", Input.GetKey(PlayerKeys.Crouch));
            SetState("Grounded", controller.isGrounded);

            // Generate speed
            float currentSpeed = (!GetState("Running") ? GetState("Crouching") ? crouchSpeed : moveSpeed : runSpeed) / 100f;
            currentDirection = direction;

            if (GetState("Enabled"))
            {
                if (Input.GetKey(PlayerKeys.Crouch) && controller.isGrounded && CrouchCheck(direction))
                {
                    controller.Move(direction * currentSpeed);
                }
                else if (!Input.GetKey(PlayerKeys.Crouch))
                {
                    controller.Move(direction * currentSpeed);
                }
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
            bool inputConditions = Input.GetKeyDown(PlayerKeys.Jump);
            bool stateConditions = GetState("Grounded") && GetState("Enabled");

            if (inputConditions && stateConditions)
            {
                jumpRequested = true;
            }

            if (Input.GetKeyDown(PlayerKeys.OpenDebug))
            {
                debugEnabled = !debugEnabled;

                if (debugEnabled)
                {
                    debugPanel.SetActive(true);
                    SetState("Enabled", false);
                    playerCamera.SetState("Enabled", false);
                    playerCamera.SetState("CursorLocked", false);
                    playerBuilder.canBuild = false;
                }
                else
                {
                    debugPanel.SetActive(false);
                    SetState("Enabled", true);
                    playerCamera.SetState("Enabled", true);
                    playerCamera.SetState("CursorLocked", true);
                    playerBuilder.canBuild = true;
                }
            }

            // Debug
            DebugSystem.AddInfo("Player Position", "Position in World (" + transform.position.x.ToString("0") + ", " + transform.position.y.ToString("0") + ", " + transform.position.z.ToString("0") + ")");
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