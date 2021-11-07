using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [FoldoutGroup("Jump"), SerializeField] 
        private ClipState _leftFootJumpState;
        [FoldoutGroup("Jump"), SerializeField] 
        private ClipState _rightFootJumpState;
        [FoldoutGroup("Jump"), SerializeField] 
        private float _jumpPower;
        [FoldoutGroup("Jump"), SerializeField] 
        private float _checkFallDelay = 1;
        [FoldoutGroup("Jump"), SerializeField]
        private InputActionProperty _jumpAction;
        
        private float _jumpTime;
        private bool _checkedForFall;
        
        private void ProcessJump()
        {
            if (HasJumpStarted())
            {
                CurrentState = PlayerState.Jump | PlayerState.IgnoreFall;
                _isForceDetach = true;
                Velocity += MoveDirection + (Vector3.up * _jumpPower);
                Debug.DrawRay(transform.position, Velocity.normalized * 1, Color.yellow, 5);
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

            if (!CurrentState.HasFlag(PlayerState.Jump)) return;
            
            if (Time.time - _jumpTime >= _checkFallDelay)
            {
                CurrentState &= ~PlayerState.IgnoreFall;
            }
            
            if (CurrentState.HasFlag(PlayerState.IgnoreFall)) return;
            
            CheckFall();
        }

        private bool HasJumpStarted()
        {
            return IsGrounded && 
                   !CurrentState.HasFlag(PlayerState.Climb) &&
                   !CurrentState.HasFlag(PlayerState.Disabled) &&
                   _jumpAction.action.triggered;
        }
    }
}
