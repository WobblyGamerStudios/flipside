using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Jump")] 
        [SerializeField] private ClipState _leftFootJumpState;
        [SerializeField] private ClipState _rightFootJumpState;
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _checkFallDelay = 1;
        [SerializeField] private InputActionProperty _jumpAction;
        
        public bool IsJumping { get; private set; }

        private float _jumpTime;
        private bool _checkedForFall;
        
        private void ProcessJump()
        {
            if (HasJumpStarted())
            {
                _isForceDetach = true;
                IsJumping = true;
                Velocity += Vector3.up * _jumpPower;
                _jumpTime = Time.time;
                _checkedForFall = false;

                switch (_groundedFoot)
                {
                    case GroundedFoot.Left:
                        TrySetState(_leftFootJumpState);
                        break;
                    case GroundedFoot.Right:
                        TrySetState(_rightFootJumpState);
                        break;
                }
            }

            if (!IsJumping || _checkedForFall) return;
            
            if (Time.time - _jumpTime >= _checkFallDelay)
            {
                _checkedForFall = true;
                CheckFall();
            }
        }

        private bool HasJumpStarted()
        {
            return IsGrounded && 
                   !IsSliding &&
                   _jumpAction.action.triggered;
        }
    }
}
