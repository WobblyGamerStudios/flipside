using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string LOG_FORMAT = nameof(PlayerCharacter) + ".{0} :: {1}";

        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _rotateSpeed = 20;
        
        [Title("Fall")]
        [SerializeField] private ClipState _fallState;
        [SerializeField] private float _minFallDistance = 0.5f;
        
        public CollisionFlags CollisionFlags { get; private set; }
        public Vector3 Velocity { get; private set; }
        public float TraversalSpeed { get; private set; }
        
        private void Start()
        {
            _moveState.Initialize(Animancer);
            _fallState.Initialize(Animancer);
            _leftFootJumpState.Initialize(Animancer);
            _rightFootJumpState.Initialize(Animancer);
            _slideState.Initialize(Animancer);
            _crouchState.Initialize(Animancer);
            _sprintState.Initialize(Animancer);
            _rollState.Initialize(Animancer);
            _climbState.Initialize(Animancer);
            
            TrySetState(_moveState);
        }

        protected override void Update()
        {
            base.Update();
            
            CalculateTraversalSpeed();
            
            ProcessMovement();
            ProcessRoll();
            ProcessSlide();
            ProcessCrouch();
            ProcessSprint();
            ProcessJump();
            ProcessClimb();

            CollisionFlags = CharacterController.Move(Velocity * Time.deltaTime);

            if (IsClimbing)
            {
                //Face climbable
            }
            else
            {
                //Rotate character
                if (MoveDirection != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(MoveDirection, transform.up), Time.deltaTime * _rotateSpeed);
            }
        }

        protected override void CheckGround()
        {
            if (Time.time - _lastLostContactTime < 0.2f || IsClimbing) return;
            base.CheckGround();
        }

        private void CalculateTraversalSpeed()
        {
            TraversalSpeed = _moveSpeed;
            
            if (IsCrouching) TraversalSpeed = _crouchSpeed;
            if (IsSprinting) TraversalSpeed = _sprintSpeed;
            if (IsRolling) TraversalSpeed = Mathf.Lerp(_rollSpeed, _moveSpeed, Time.time - _rollStartTime);
            if (IsSliding) TraversalSpeed = Mathf.Lerp(_sprintSpeed, _crouchSpeed, Time.time - _slideStartTime);
        }

        protected override void LostGroundContact()
        {
            CheckFall();
            base.LostGroundContact();
        }

        protected override void RegainedGroundContact()
        {
            IsJumping = false;
            TrySetState(_moveState);
            base.RegainedGroundContact();
        }
        
        public void CheckFall()
        {
            if (IsJumping || IsClimbing) return;
            
            var fallRay = new Ray(transform.position, Vector3.down);
            Debug.DrawRay(fallRay.origin, fallRay.direction * _minFallDistance, Color.red, 3);
            if (Physics.Raycast(fallRay, _minFallDistance, _traversableLayers))
            {
                TrySetState(_moveState);
            }
            else
            {
                TrySetState(_fallState);
                IsSliding = false;
                IsRolling = false;
            }
        }
    }
}
