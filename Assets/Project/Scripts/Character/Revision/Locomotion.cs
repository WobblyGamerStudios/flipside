using System;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public enum LocomotionState
    {
        Default,
        Ladder,
        Ledge,
        Climbing
    }

    public partial class Locomotion : MonoBehaviour, ICharacterController
    {
        [SerializeField] private KinematicCharacterMotor _motor;
        [SerializeField] private float _gravityFactor = 15;
        
        [Title("Input")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _jumpAction;
        
        public Vector2 MoveInput { get; private set; }
        public Vector3 ClampedMoveInput { get; private set; }
        public Vector3 MoveDirection { get; private set; }
        public bool IsGrounded => _motor && _motor.GroundingStatus.IsStableOnGround;
        public bool IsAnyGrounded => _motor && _motor.GroundingStatus.FoundAnyGround;
        public bool WasGrounded => _motor && _motor.LastGroundingStatus.IsStableOnGround;

        private LocomotionState _locomotionState;
        private bool _jumpRequested;
        private bool _isJumping;
        private bool _jumpedThisFrame;
        private bool _isOnWall;
        private Vector3 _wallJumpNormal;
        private float _lastWallJumpTime;
        private Collider _wallJumpCollider;
        private Collider _lastWallJumpCollider;
        
        private void Awake()
        {
            _motor.CharacterController = this;
        }

        private void Update()
        {
            CalculateMoveDirection();

            if (_jumpAction.action.triggered)
            {
                _jumpRequested = true;
            }
        }

        private void CalculateMoveDirection()
        {
            MoveInput = _moveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            ClampedMoveInput = Vector3.ClampMagnitude(new Vector3(MoveInput.x, 0, MoveInput.y), 1);

            var projection = Vector3.ProjectOnPlane(CameraManager.MainTransform.rotation * Vector3.forward, transform.up).normalized;

            var targetDirection = Quaternion.LookRotation(projection, transform.up) * ClampedMoveInput;
            MoveDirection = targetDirection;
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            _motor.SetPositionAndRotation(position, rotation);
        }
        
        #region ICharacterController

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (_locomotionState)
            {
                case LocomotionState.Default:
                    UpdateDefaultRotation(ref currentRotation, deltaTime);
                    break;
                case LocomotionState.Climbing:
                    UpdateLadderRotation(ref currentRotation, deltaTime);
                    break;
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (_locomotionState)
            {
                case LocomotionState.Default:
                    UpdateDefaultVelocity(ref currentVelocity, deltaTime);
                    break;
                case LocomotionState.Climbing:
                    UpdateLadderVelocity(ref currentVelocity, deltaTime);
                    break;
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            switch (_locomotionState)
            {
                case LocomotionState.Default:
                    break;
                case LocomotionState.Climbing:
                    break;
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            if (!IsGrounded && WasGrounded)
            {
                LeaveGround();
            }
            else if (IsGrounded && !WasGrounded)
            {
                Landed();
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (_locomotionState)
            {
                case LocomotionState.Default:
                    PostUpdateDefault(deltaTime);
                    break;
                case LocomotionState.Climbing:
                    PostUpdateClimbing(deltaTime);
                    break;
            }
            
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }
        
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            if (_wallJumpLayer == (_wallJumpLayer | 1 << hitCollider.gameObject.layer))
            {
                _wallJumpCollider = hitCollider;
                if (!IsGrounded && !hitStabilityReport.IsStable)
                {
                    _isOnWall = true;
                    _wallJumpNormal = hitNormal;
                }
            }

            CheckForLadder(hitCollider);
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
        
        #endregion ICharacterController

        protected virtual void LeaveGround()
        {
            OnLeaveGroundEvent?.Invoke();
        }
        
        protected virtual void Landed()
        {
            _lastWallJumpCollider = null;
            OnLandEvent?.Invoke();
        }

        #region Events

        public event Action OnLandEvent;
        public event Action OnLeaveGroundEvent;

        #endregion Events
    }
}
