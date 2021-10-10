using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(CharacterController))]
    public class FlipSideCharacterController : Character
    {
        [SerializeField] private LayerMask _traversableLayers = -1;
        [SerializeField] private float _checkDistance = 0.1f;
        [SerializeField] private float _gravityFactor = 15;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _rotateSpeed = 20;
        [SerializeField] private float _speedMultiplier;
        [SerializeField] private float _airAcceleration = 20;
        [SerializeField] private float _maxAirSpeed = 3;
        [SerializeField] private Sprinting _sprinting;
        
        [Title("Animation States")] 
        [SerializeField] private LinearState _moveState;
        [SerializeField] private LinearState _crouchState;
        [SerializeField] private ClipState _fallState;
        
        [Title("Input")] 
        [SerializeField] private InputActionProperty _moveAction;
        [SerializeField] private InputActionProperty _crouchAction;
        
        public CharacterController CharacterController { get; private set; }
        public bool IsGrounded { get; private set; }
        public Vector2 MoveInput { get; private set; }
        public Vector3 MoveDirection { get; private set; }
        public Vector3 GroundNormal { get; private set; }
        public Vector3 PreviousPosition { get; private set; }
        public bool IsCrouching { get; private set; }
        
        private Vector3 _moveInputClamped;
        private float _lastLostContactTime;
        private float _lastRegainedContactTime;
        private Vector3 _deltaPosition;
        private Vector3 _verticalVelocity;
        private Vector3 _horizontalVelocity;

        #region MonoBehaviour

        private void Start()
        {
            _moveState.Initialize(Animancer);
            _crouchState.Initialize(Animancer);
            _fallState.Initialize(Animancer);
            _sprinting.Initialize(this);
            
            TrySetState(_moveState);
        }

        private void Update()
        {
            CalculateMoveDirection();

            var wasGrounded = IsGrounded;
            CheckGround();
            
            switch (wasGrounded)
            {
                case true when !IsGrounded:
                    LostGroundContact();
                    break;
                case false when IsGrounded:
                    RegainedGroundContact();
                    break;
            }
            
            ProcessInput();
            ProcessState();
            
            _moveState.Blend = MoveDirection.magnitude;
            _crouchState.Blend = MoveDirection.magnitude;
            
            if (MoveDirection != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MoveDirection, transform.up), Time.deltaTime * _rotateSpeed);
        }
        
        protected virtual void LateUpdate()
        {
            PreviousPosition = transform.position;
        }

        #endregion MonoBeahviour

        protected override void FindDependencies()
        {
            if (!CharacterController) CharacterController = GetComponent<CharacterController>();
            
            base.FindDependencies();
        }
        
        private void CalculateMoveDirection()
        {
            if (!CharacterController) return;
            
            MoveInput = _moveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            _moveInputClamped = Vector3.ClampMagnitude(new Vector3(MoveInput.x, 0, MoveInput.y), 1);

            var projection = Vector3.ProjectOnPlane(_cameraTransform.rotation * Vector3.forward, transform.up).normalized;
            MoveDirection = Quaternion.LookRotation(projection, transform.up) * _moveInputClamped;
        }

        private void CheckGround()
        {
            if (!CharacterController) return;
			
            float checkDistance = CharacterController.skinWidth + _checkDistance;

            IsGrounded = false;
            GroundNormal = Vector3.up;

            if (!Physics.CapsuleCast(GetCapsuleBottomHemisphere(),
                GetCapsuleTopHemisphere(CharacterController.height),
                CharacterController.radius, Vector3.down, out RaycastHit hit, checkDistance, _traversableLayers,
                QueryTriggerInteraction.Ignore)) return;

            GroundNormal = hit.normal;

            if (Vector3.Dot(hit.normal, transform.up) <= 0 ||
                !IsOnSlope(GroundNormal)) return;

            IsGrounded = true;

            if (hit.distance > CharacterController.skinWidth)
            {
                CharacterController.Move(Vector3.down * hit.distance);
            }
        }

        private void ProcessInput()
        {
            IsCrouching = _crouchAction.action.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;
            if (_sprinting) _sprinting.Process();
        }

        private void ProcessState()
        {
            if (!IsGrounded)
            {
                //set fall state
                TrySetState(_fallState);
                return;
            }
            
            if (IsCrouching)
            {
                TrySetState(_crouchState);
            }

            if (_sprinting.IsSprinting || IsCrouching) return;
            
            TrySetState(_moveState);
        }

        private void OnAnimatorMove()
        {
            var movement = Animancer.Animator.deltaPosition;
            _deltaPosition = movement;
            movement *= 1 + _speedMultiplier;

            if (IsGrounded)
            {
                _verticalVelocity = Vector3.zero;
            }
            else
            {
                _verticalVelocity += Vector3.down * Time.deltaTime * _gravityFactor;
                
                //Apply air control
                _horizontalVelocity += MoveDirection * (Time.deltaTime * _airAcceleration);
                _horizontalVelocity = Vector3.ClampMagnitude(_horizontalVelocity, _maxAirSpeed);
            }

            movement += (_verticalVelocity + _horizontalVelocity) * Time.deltaTime;
            
            CharacterController.Move(movement);
        }
        
        protected virtual void LostGroundContact()
        {
            _lastLostContactTime = Time.time;
            _horizontalVelocity = _deltaPosition;
            _horizontalVelocity.y = 0;
            OnLostGroundContactEvent?.Invoke();
        }

        protected virtual void RegainedGroundContact()
        {
            _lastRegainedContactTime = Time.time;
            _horizontalVelocity = Vector3.zero;
            OnRegainedGroundContactEvent?.Invoke();
        }

        #region Helpers

        public bool IsOnSlope(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= CharacterController.slopeLimit;
        }
        
        public Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + transform.up * CharacterController.radius;
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        public Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + transform.up * (atHeight - CharacterController.radius);
        }
        
        public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
        {
            var directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(surfaceNormal, directionRight).normalized;
        }

        #endregion Helpers
        
        #region Events

        public event Action OnLostGroundContactEvent;
        public event Action OnRegainedGroundContactEvent;

        #endregion Events
    }
}
