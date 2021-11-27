using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string SPRINT = "Sprint"; 
        
        [SerializeField, FoldoutGroup(SPRINT)]
        private ClipState _sprintState;
        [SerializeField, FoldoutGroup(SPRINT)] 
        private float _sprintSpeed;
        [SerializeField, Range(0, 1), FoldoutGroup(SPRINT)] 
        private float _sprintThreshold = 0.9f;
        [SerializeField, FoldoutGroup(SPRINT) , SuffixLabel("Seconds", true)] 
        private float _sprintDuration = 3;
        [SerializeField, FoldoutGroup(SPRINT), SuffixLabel("Seconds", true)] 
        private float _sprintCooldown = 1.5f;
        [SerializeField, FoldoutGroup(SPRINT)]
        private InputActionProperty _sprintAction;

        private bool _isSprinting;
        private bool _wasSprinting;
        private float _sprintStartTime;
        private float _sprintEndTime;
        
        private void InitializeSprint()
        {
            _sprintState.Initialize(Animancer);
        }
        
        private void ProcessSprint()
        {
            if (HasSprintStarted())
            {
                _isSprinting = true;
                _sprintStartTime = Time.time;
                TrySetState(_sprintState);
                _wasSprinting = false;
                return;
            }

            if (HasSprintEnded())
            {
                _sprintEndTime = Time.time;
                _isSprinting = false;
                _wasSprinting = true;
                if (State == State.Falling) return;
                TrySetState(_moveState);
                return;
            }

            if (_wasSprinting) _wasSprinting = false;
        }

        private bool HasSprintStarted()
        {
            return !_isSprinting &&
                   Time.time - _sprintEndTime >= _sprintCooldown &&
                   MoveDirection.magnitude > _sprintThreshold &&
                   IsGrounded && 
                   _sprintAction.action.triggered;
        }

        private bool HasSprintEnded()
        {
            return _isSprinting && 
                   (Time.time - _sprintStartTime >= _sprintDuration ||
                    MoveDirection.magnitude < _sprintThreshold ||
                    !IsGrounded ||
                    _sprintAction.action.triggered || 
                    _crouchAction.action.triggered || 
                    _rollAction.action.triggered);
        }
    }
}
