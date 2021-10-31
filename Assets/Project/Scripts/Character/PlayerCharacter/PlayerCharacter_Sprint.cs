using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Sprint")] 
        [SerializeField] private ClipState _sprintState;
        [SerializeField] private float _sprintSpeed;
        [SerializeField, Range(0, 1)] private float _sprintThreshold = 0.9f;
        [SerializeField, SuffixLabel("Seconds", true)] private float _sprintDuration = 3;
        [SerializeField, SuffixLabel("Seconds", true)] private float _sprintCooldown = 1.5f;
        [SerializeField] private InputActionProperty _sprintAction;


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
