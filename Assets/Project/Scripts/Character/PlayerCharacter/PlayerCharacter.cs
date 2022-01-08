using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string LOG_FORMAT = nameof(PlayerCharacter) + ".{0} :: {1}";
        
        [ReadOnly] public State State;

        [SerializeField] private CinemachineVirtualCamera _characterCamera;
        public CinemachineVirtualCamera CharacterCamera => _characterCamera;

        public CollisionFlags CollisionFlags { get; private set; }
        public Vector3 Velocity { get; private set; }
        public float TraversalSpeed { get; private set; }

        protected override void Start()
        {
            PlayerManager.RegisterPlayer(this);
            CameraManager.RegisterCamera(_characterCamera, true);
            
            base.Start();

            InitializeFall();
            InitializeMove();
            InitializeSprint();
            InitializeRoll();
            InitializeCrouch();
            InitializeJump();
            InitializeClimb();
            InitializeStraddle();

            TrySetState(_moveState);
        }

        private void OnDestroy()
        {
            CameraManager.UnRegisterCamera(_characterCamera);
        }

        protected override void Update()
        {
            base.Update();

            CalculateTraversalSpeed();

            ProcessMovement();
            ProcessSprint();
            ProcessRoll();
            ProcessCrouch();
            ProcessJump();
            //ProcessLadderClimb();
            ProcessClimb();
            ProcessStraddle();

            //Move player
            if (!Animancer.Animator.applyRootMotion) MoveCharacter(Velocity * Time.deltaTime);
           
            //Rotate player
            ProcessRotation();
        }

        protected virtual void MoveCharacter(Vector3 motion)
        {
            if (CharacterController.enabled)
            {
                CollisionFlags = CharacterController.Move(motion);
            }
            else
            {
                transform.position += motion;
            }
        }

        protected override void CheckGround()
        {
            if (Time.time - _lastLostContactTime < 0.2f || State == State.Climbing) return;
            base.CheckGround();
        }

        private void CalculateTraversalSpeed()
        {
            TraversalSpeed = _moveSpeed;

            if (_isCrouching) TraversalSpeed = _crouchSpeed;
            if (_isSprinting) TraversalSpeed = _sprintSpeed;
            if (_isRolling)
                TraversalSpeed = Mathf.Lerp(_rollSpeed, _moveSpeed, Time.time - _rollStartTime);
            if (_isSliding)
                TraversalSpeed = Mathf.Lerp(_sprintSpeed, _crouchSpeed, Time.time - _slideStartTime);
        }

        protected override void LostGroundContact()
        {
            State = State.InAir;
            CheckFall();
            base.LostGroundContact();
        }

        protected override void RegainedGroundContact()
        {
            State = State.Ground;
            TrySetState(_moveState);
            base.RegainedGroundContact();
        }
        
        private void OnAnimatorMove()
        {
            if (!Animancer.Animator.applyRootMotion) return;

            Debug.LogFormat(LOG_FORMAT, nameof(OnAnimatorMove), "Moving character with RM");
            MoveCharacter(Animancer.Animator.deltaPosition);
        }

        private void OnDrawGizmos()
        {
            DrawClimbGizmos();
        }
    }
    
    public enum State
    {
        Ground, 
        InAir,
        Falling,
        Climbing,
        Hopping
    }
}
