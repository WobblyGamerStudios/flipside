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

        private float _lastStartTime;
        private float _lastEndTime;
        
        public bool IsSprinting { get; private set; }

        partial void Initialize()
        {
            _sprintState.Initialize(Animancer);

            _lastEndTime = -_sprintCooldown;
        }

        partial void Process()
        {
            if (HasStarted())
            {
                IsSprinting = true;
                _lastStartTime = Time.time;
                TrySetState(_sprintState);
                return;
            }

            if (HasEnded())
            {
                IsSprinting = false;

                _lastEndTime = Time.time;
            }
        }

        private bool HasStarted()
        {
            return !IsSprinting &&
                   _sprintAction.action.triggered &&
                   Time.time - _lastEndTime >= _sprintCooldown &&
                   MoveDirection.magnitude > _sprintThreshold &&
                   IsGrounded;
        }

        private bool HasEnded()
        {
            return IsSprinting && 
                   (Time.time - _lastStartTime >= _sprintDuration ||
                    _sprintAction.action.triggered ||
                    MoveDirection.magnitude <= _sprintThreshold ||
                    !IsGrounded);
        }
    }
}
