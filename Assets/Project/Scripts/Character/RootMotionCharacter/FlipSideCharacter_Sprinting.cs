using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class FlipSideCharacter
    {
        [Title("Sprinting")]
        [SerializeField] private ClipState _sprintState;
        [SerializeField, Range(0, 1)] private float _sprintThreshold = 0.9f;
        [SerializeField, SuffixLabel("Seconds", true)] private float _sprintDuration = 1;
        [SerializeField, SuffixLabel("Seconds", true)] private float _sprintCooldown = 0.5f;
        [SerializeField] private InputActionProperty _sprintAction;

        private float _sprintStartTime;
        private float _sprintEndTime;
        
        public bool IsSprinting { get; private set; }

        private void ProcessSprint()
        {
            if (HasSprintStarted())
            {
                IsSprinting = true;
                _sprintStartTime = Time.time;
                TrySetState(_sprintState);
                return;
            }

            if (HasSprintEnded())
            {
                IsSprinting = false;
                _sprintEndTime = Time.time;
                TrySetState(_moveState);
            }
        }

        private bool HasSprintStarted()
        {
            return !IsSprinting &&
                   _sprintAction.action.triggered &&
                   Time.time - _sprintEndTime >= _sprintCooldown &&
                   MoveDirection.magnitude > _sprintThreshold &&
                   IsGrounded;
        }

        private bool HasSprintEnded()
        {
            return IsSprinting && 
                   (Time.time - _sprintStartTime >= _sprintDuration ||
                    _sprintAction.action.triggered ||
                    MoveDirection.magnitude <= _sprintThreshold ||
                    !IsGrounded);
        }
    }
}
