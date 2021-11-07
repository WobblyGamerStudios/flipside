using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
         
        [SerializeField, FoldoutGroup("Sprint")]
        private ClipState _sprintState;
        [SerializeField, FoldoutGroup("Sprint")] 
        private float _sprintSpeed;
        [SerializeField, Range(0, 1), FoldoutGroup("Sprint")] 
        private float _sprintThreshold = 0.9f;
        [SerializeField, FoldoutGroup("Sprint") , SuffixLabel("Seconds", true)] 
        private float _sprintDuration = 3;
        [SerializeField, FoldoutGroup("Sprint"), SuffixLabel("Seconds", true)] 
        private float _sprintCooldown = 1.5f;
        [SerializeField, FoldoutGroup("Sprint")]
        private InputActionProperty _sprintAction;

        private float _sprintStartTime;
        private float _sprintEndTime;
        
        private void ProcessSprint()
        {
            if (HasSprintStarted())
            {
                CurrentState = PlayerState.Sprint;
                _sprintStartTime = Time.time;
                TrySetState(_sprintState);
                return;
            }

            if (HasSprintEnded())
            {
                _sprintEndTime = Time.time;

                if (CurrentState.HasFlag(PlayerState.Jump)) return;
                CurrentState = PlayerState.Move;
                CheckFall();
            }
        }

        private bool HasSprintStarted()
        {
            return CurrentState == PlayerState.Move &&
                   Time.time - _sprintEndTime >= _sprintCooldown &&
                   MoveDirection.magnitude > _sprintThreshold &&
                   IsGrounded && 
                   _sprintAction.action.triggered;
        }

        private bool HasSprintEnded()
        {
            return CurrentState == PlayerState.Sprint && 
                   (Time.time - _sprintStartTime >= _sprintDuration ||
                    MoveDirection.magnitude < _sprintThreshold ||
                    !IsGrounded ||
                    _sprintAction.action.triggered ||
                     _crouchAction.action.triggered);
        }
    }
}
