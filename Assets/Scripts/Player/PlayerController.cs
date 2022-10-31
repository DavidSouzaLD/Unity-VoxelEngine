using UnityEngine;
using QFSW.QC;

namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerCamera))]
    [RequireComponent(typeof(PlayerBuilder))]
    [RequireComponent(typeof(PlayerConsole))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Speeds")]
        [Command("Player.walkSpeed")] public float walkSpeed = 5f;
        [Command("Player.runSpeed")] public float runSpeed = 8f;
        [Command("Player.crouchSpeed")] public float crouchSpeed = 2f;

        [Header("Jump")]
        [Command("Player.jumpForce")] public float jumpForce = 5f;
        [Command("Player.gravityScale")] public float gravityScale = 1f;

        [Header("Habilities")]
        [Command("Player.canFly")] public bool flyMode;

        // Bools
        public bool isWalking { get; private set; }
        public bool isRunning { get; private set; }
        public bool isCrouching { get; private set; }
        public bool isGrounded { get; private set; }

        // Private
        private bool jumpRequested;
        private float verticalVelocity;

        // Components
        private CharacterController characterController;
        private PlayerCamera playerCamera;
        private PlayerBuilder playerBuilder;
        private PlayerConsole playerConsole;

        private Vector2 InputAxis
        {
            get
            {
                return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            }
        }

        private Vector3 Direction
        {
            get
            {
                return transform.forward * InputAxis.y + transform.right * InputAxis.x;
            }
        }

        private float CurrentSpeed
        {
            get
            {
                if (isWalking && !isRunning)
                {
                    return walkSpeed;
                }

                if (isRunning)
                {
                    return runSpeed;
                }

                if (isCrouching)
                {
                    return crouchSpeed;
                }

                return 0;
            }
        }

        private void Start()
        {
            // Get components
            characterController = GetComponent<CharacterController>();
            playerCamera = GetComponent<PlayerCamera>();
            playerBuilder = GetComponent<PlayerBuilder>();
            playerConsole = GetComponent<PlayerConsole>();
        }

        private void FixedUpdate()
        {
            // Reset jump
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = 0;
            }

            // Update move
            if (!flyMode)
            {
                MoveUpdate();
            }
            else
            {
                FlyMove();
            }

            // Jump
            if (jumpRequested)
            {
                verticalVelocity = Mathf.Sqrt(jumpForce * -3.0f * Physics.gravity.y);
                jumpRequested = false;
            }

            // Apply gravity
            if (!flyMode)
            {
                verticalVelocity += Physics.gravity.y * gravityScale * Time.deltaTime;
            }

            // Apply jump
            characterController.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
        }

        private void Update()
        {
            JumpUpdate();
        }

        private void MoveUpdate()
        {
            bool runInputConditions = Input.GetKey(PlayerKeys.Run);
            bool crouchInputConditions = Input.GetKey(PlayerKeys.Crouch);
            bool inputConditions = InputAxis != Vector2.zero;
            bool walkConditions = !playerConsole.consoleEnabled && !flyMode;

            isWalking = (!crouchInputConditions && inputConditions && walkConditions);
            isRunning = isWalking && runInputConditions;

            if (!crouchInputConditions && inputConditions && walkConditions)
            {
                characterController.Move(Direction * CurrentSpeed * Time.deltaTime);
            }
            else if (crouchInputConditions)
            {
                CrouchUpdate();
            }
        }

        private void FlyMove()
        {
            bool runInputConditions = Input.GetKey(PlayerKeys.Run);
            bool inputConditions = InputAxis != Vector2.zero || Input.GetKey(PlayerKeys.Jump) || Input.GetKey(PlayerKeys.Down);
            bool walkConditions = !playerConsole.consoleEnabled;
            Vector3 currentVelocity = Direction * CurrentSpeed * 2f * Time.deltaTime;

            isWalking = (inputConditions && walkConditions);
            isRunning = isWalking && runInputConditions;

            if (Input.GetKey(PlayerKeys.Jump))
            {
                currentVelocity.y += CurrentSpeed * 2f * Time.deltaTime;
            }

            if (Input.GetKey(PlayerKeys.Down))
            {
                currentVelocity.y -= CurrentSpeed * 2f * Time.deltaTime;
            }

            if (inputConditions && walkConditions)
            {
                characterController.Move(currentVelocity);
            }
        }

        private void CrouchUpdate()
        {
            bool inputConditions = InputAxis != Vector2.zero;
            bool walkConditions = !flyMode;

            isCrouching = (inputConditions && walkConditions);

            if (inputConditions && walkConditions)
            {
                characterController.Move(Direction * CurrentSpeed * Time.deltaTime);
            }
        }

        private void JumpUpdate()
        {
            // Jump
            bool inputConditions = Input.GetKeyDown(PlayerKeys.Jump);
            bool jumpConditions = !playerConsole.consoleEnabled && !flyMode && isGrounded;

            isGrounded = characterController.isGrounded;

            if (inputConditions && jumpConditions)
            {
                jumpRequested = true;
            }
        }
    }
}