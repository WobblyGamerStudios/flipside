using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public class Sprinting : MonoBehaviour
    {
        [SerializeField] private ClipState _sprintState;
        [SerializeField, Range(0, 1)] private float _sprintThreshold = 0.9f;
        [SerializeField, SuffixLabel("Seconds", true)] private float _sprintDuration = 1;
        [SerializeField, SuffixLabel("Seconds", true)] private float _sprintCooldown = 0.5f;
        [SerializeField] private InputActionProperty _sprintAction;

        private float _lastStartTime;
        private float _lastEndTime;
        
        public bool IsSprinting { get; private set; }

        private FlipSideCharacterController _characterController;
        
        public void Initialize(FlipSideCharacterController characterController)
        {
            _characterController = characterController;

            _sprintState.Initialize(characterController.Animancer);

            _lastEndTime = -_sprintCooldown;
        }

        public void Process()
        {
            if (HasStarted())
            {
                IsSprinting = true;
                _lastStartTime = Time.time;
                _characterController.TrySetState(_sprintState);
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
                   _characterController.MoveDirection.magnitude > _sprintThreshold &&
                   _characterController.IsGrounded;
        }

        private bool HasEnded()
        {
            return IsSprinting && 
                   (Time.time - _lastStartTime >= _sprintDuration ||
                    _sprintAction.action.triggered ||
                    _characterController.MoveDirection.magnitude <= _sprintThreshold ||
                    !_characterController.IsGrounded);
        }
    }
}
