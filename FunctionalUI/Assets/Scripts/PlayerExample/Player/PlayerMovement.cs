using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PlayerExample
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, IPlayerMovement
    {
        //Serializable fields
        [SerializeField] [Tooltip("The normal movement-speed of the player")]
        private float walkSpeed = 5f;

        [SerializeField] [Tooltip("The sensitivity of the rotation of the player")]
        private float rotateSpeed = 0.5f;

        [Space] [SerializeField] [Tooltip("The sprint movement speed of the player (when holding SHIFT)")]
        private float sprintSpeed = 10f;

        [SerializeField] [Tooltip("The rate at the amount of stamina is depleting")]
        private float sprintDepletionRate = 3f;

        [SerializeField] [Tooltip("How fast stamina is regenerating, when not sprinting")]
        private float staminaRegenerationRate = 5f;

        [SerializeField] [Tooltip("The amount of time before stamina starts regenerating after sprinting")]
        private float sprintDelayTime = 1.5f;

        //Events
        public UnityEvent<float> onStaminaChangedEvent = new();

        //Movement
        private CharacterController _controller;

        //Sprinting variables
        private float _currentStamina;
        private Vector3 _gravity;
        private bool _isSprinting;

        // Input
        private Vector2 _move;
        private float _movementSpeed;
        private Vector3 _previous;
        private float _sprintDelayTimer;

        //For calculating velocity of movement
        private float _velocity;

        private float Stamina
        {
            get => _currentStamina;
            set
            {
                _currentStamina = Mathf.Clamp(value, 0, 100);
                onStaminaChangedEvent.Invoke(_currentStamina); //UIManager is listening to this event
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _movementSpeed = walkSpeed;
            Stamina = 100f;
        }

        public void MovePlayer()
        {
            var moveDirection = transform.forward * (_move.y * _movementSpeed);
            moveDirection += _gravity;
            _controller.Move(moveDirection * Time.deltaTime);
        }

        public void RotatePlayer()
        {
            var rotation = _move.x * rotateSpeed;
            transform.Rotate(Vector3.up, rotation * Time.timeScale);
        }

        public void HandleGravity()
        {
            //Need to push the character to the ground in order to stop flipping the isGrounded variable
            if (_controller.isGrounded && _controller.velocity.y < 0.01f)
                _gravity = Vector3.zero + Vector3.down * .05f;
            else
                _gravity += Physics.gravity * Time.deltaTime;
        }

        public void HandleSprint()
        {
            if (_isSprinting && _move.y != 0)
            {
                Stamina -= Time.deltaTime * sprintDepletionRate;
                _sprintDelayTimer = sprintDelayTime;

                //If the stamina is too low, the player will stop sprinting
                if (_currentStamina <= 0)
                {
                    _isSprinting = false;
                    _movementSpeed = walkSpeed;
                }
            }
            else
            {
                _sprintDelayTimer -= Time.deltaTime;
                if (_sprintDelayTimer <= 0f && _currentStamina < 100f)
                    Stamina += Time.deltaTime * staminaRegenerationRate;
            }
        }


        //INPUT HANDLERS
        private void OnMove(InputValue value)
        {
            _move = value.Get<Vector2>();
        }

        private void OnSprint(InputValue value)
        {
            if (value.isPressed && _currentStamina > 0)
            {
                _movementSpeed = sprintSpeed;
                _isSprinting = value.isPressed;
                return;
            }

            _movementSpeed = walkSpeed;
            _isSprinting = value.isPressed;
        }

        public void RespawnOnPosition(Vector3 pos)
        {
            _controller.enabled = false;
            _controller.transform.position = pos;
            _controller.enabled = true;
        }

        public void ResetStamina()
        {
            Stamina = 100;
        }

        public float GetCurrentStamina()
        {
            return _currentStamina;
        }

        public void RestoreStamina(float amount)
        {
            Stamina = Mathf.Clamp(_currentStamina + amount, 0, 100);
        }
    }
}