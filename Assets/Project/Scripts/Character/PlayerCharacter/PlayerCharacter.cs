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

        [ReadOnly] public PlayerState CurrentState;
        
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _rotateSpeed = 20;
        
        [Title("Fall")]
        [SerializeField] private ClipState _fallState;
        [SerializeField] private float _minFallDistance = 0.5f;
        
        public CollisionFlags CollisionFlags { get; private set; }
        public Vector3 Velocity { get; private set; }
        public float TraversalSpeed { get; private set; }
        
        protected override void Start()
        {
           base.Start();
           TrySetState(_moveState);
           CurrentState = PlayerState.Move;
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

            //Rotate character
            if (IsClimbing)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-Hit.normal, transform.up), Time.deltaTime * _rotateSpeed);
            }
            else
            {
                if (MoveDirection != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MoveDirection, transform.up), Time.deltaTime * _rotateSpeed);
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

            if (CurrentState.HasFlag(PlayerState.Crouch)) TraversalSpeed = _crouchSpeed;
            if (CurrentState.HasFlag(PlayerState.Sprint)) TraversalSpeed = _sprintSpeed;
            if (CurrentState.HasFlag(PlayerState.Roll)) TraversalSpeed = Mathf.Lerp(_rollSpeed, _moveSpeed, Time.time - _rollStartTime);;
            if (CurrentState.HasFlag(PlayerState.Slide)) TraversalSpeed = Mathf.Lerp(_sprintSpeed, _crouchSpeed, Time.time - _slideStartTime);;
        }

        protected override void LostGroundContact()
        {
            CheckFall();
            base.LostGroundContact();
        }

        protected override void RegainedGroundContact()
        {
            CurrentState = PlayerState.Move;
            TrySetState(_moveState);
            base.RegainedGroundContact();
        }
        
        public void CheckFall()
        {
            if (CurrentState.HasFlag(PlayerState.IgnoreFall)) return;
            
            var fallRay = new Ray(transform.position, Vector3.down);
            Debug.DrawRay(fallRay.origin, fallRay.direction * _minFallDistance, Color.red, 3);
            if (Physics.Raycast(fallRay, _minFallDistance, _traversableLayers))
            {
                TrySetState(_moveState);
            }
            else
            {
                TrySetState(_fallState);
                CurrentState = PlayerState.Move;
            }
        }
    }

    [Flags]
    public enum PlayerState
    {
        Move = 1 << 0,
        Crouch = 1 << 1,
        Sprint = 1 << 2,
        Jump = 1 << 4,
        Roll = 1 << 5,
        Slide = 1 << 6,
        Ladder = 1 << 7,
        Free = 1 << 8,
        Ledge = 1 << 9,
        EndClimb = 1 << 10,
        Climb = 1 << 11,
        Disabled = 1 << 12,
        Fall = 1 << 13,
        IgnoreFall = 1 << 14
    }
}
