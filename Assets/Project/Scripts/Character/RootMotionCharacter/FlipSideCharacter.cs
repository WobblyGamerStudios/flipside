using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class FlipSideCharacter : Character
    {
        [SerializeField] protected float _gravityFactor = 15;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _rotateSpeed = 20;
        [SerializeField] private float _speedMultiplier;
        [SerializeField] private float _airAcceleration = 20;
        [SerializeField] private float _maxAirSpeed = 3;
        [SerializeField] private ClipState _fallState;
        [SerializeField] private float _minFallDistance = 1;
        
        public Vector3 Velocity { get; private set; }
        
        private Vector3 _deltaPosition;
        private Vector3 _verticalVelocity;
        private Vector3 _horizontalVelocity;

        #region MonoBehaviour

        private void Start()
        {
            _moveState.Initialize(Animancer);
            _crouchState.Initialize(Animancer);
            _fallState.Initialize(Animancer);
            _sprintState.Initialize(Animancer);
            _jumpState.Initialize(Animancer);
            
            TrySetState(_moveState);
        }

        protected override void Update()
        {
            ProcessMovement();
            ProcessSprint();
            ProcessCrouch();
            ProcessJump();

            if (IsGrounded)
            {
                Velocity = CharacterController.velocity;
            }
            else
            {
                Velocity += Vector3.down * (_gravityFactor * Time.deltaTime);
            }

            Velocity = new Vector3{y = Velocity.y};
            CharacterController.Move(Velocity * Time.deltaTime);
            
            //Rotate character
            if (MoveDirection != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MoveDirection, transform.up), Time.deltaTime * _rotateSpeed);
            
            base.Update();
        }
        
        #endregion MonoBeahviour

        protected override void CheckGround()
        {
            if (IsJumping) return;
            base.CheckGround();
        }

        private void OnAnimatorMove()
        {
            var movement = Animancer.Animator.deltaPosition;
            _deltaPosition = movement;
            movement *= 1 + _speedMultiplier;
            
            // if (IsGrounded)
            // {
            //     _verticalVelocity = Vector3.zero;
            // }
            // else
            // {
            //     if (IsJumping)
            //     {
            //         _verticalVelocity += Vector3.up * (_jumpPower * Time.deltaTime);
            //     }
            //     else
            //     {
            //         _verticalVelocity += Vector3.down * (_gravityFactor * Time.deltaTime);
            //     }
            //
            //     //Apply air control
            //     _horizontalVelocity += MoveDirection * (Time.deltaTime * _airAcceleration);
            //     _horizontalVelocity = Vector3.ClampMagnitude(_horizontalVelocity, _maxAirSpeed);
            // }
            //
            // movement += (_verticalVelocity + _horizontalVelocity) * Time.deltaTime;
            //
            CharacterController.Move(movement);
        }
        
        protected override void LostGroundContact()
        {
            _horizontalVelocity = _deltaPosition;
            _horizontalVelocity.y = 0;

            if (!IsJumping) CheckFall();
            
            base.LostGroundContact();
        }

        protected override void RegainedGroundContact()
        {
            _horizontalVelocity = Vector3.zero;
            IsJumping = false;
            TrySetState(_moveState);
            base.RegainedGroundContact();
        }

        private void CheckFall()
        {
            if (Physics.Raycast(transform.position, Vector3.down, _minFallDistance, _traversableLayers)) return;
            TrySetState(_fallState);
        }
    }
}
