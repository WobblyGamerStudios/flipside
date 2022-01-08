using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string JUMP = "Jump";
        
        [FoldoutGroup(JUMP), SerializeField] 
        private ClipState _leftFootJumpState;
        [FoldoutGroup(JUMP), SerializeField] 
        private ClipState _rightFootJumpState;
        [FoldoutGroup(JUMP), SerializeField] 
        private float _jumpPower;
        [FoldoutGroup(JUMP), SerializeField] 
        private float _checkFallDelay = 1;
        [FoldoutGroup(JUMP), SerializeField]
        private InputActionProperty _jumpAction;

        private bool _isJumping;
        private float _jumpTime;
        private bool _checkedForFall;
        
        private void InitializeJump()
        {
            _leftFootJumpState.Initialize(Animancer);
            _rightFootJumpState.Initialize(Animancer);
        }
        
        private void ProcessJump()
        {
            if (HasJumpStarted())
            {
                _isJumping = true;
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

            if (!_isJumping) return;
            
            if (Time.time - _jumpTime >= _checkFallDelay)
            {
                _isJumping = false;
                CheckFall();
            }
        }

        private bool HasJumpStarted()
        {
            return IsGrounded &&
                   _jumpAction.action.triggered;
        }
    }
}
