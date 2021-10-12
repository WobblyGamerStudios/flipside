using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class FlipSideCharacter : Character
    {
        [Title("Jumping")] 
        [SerializeField] private ClipState _jumpState;
        [SerializeField] private float _jumpPower = 2;
        [SerializeField] private float _groundCheckDelay = 1;
        [SerializeField] private InputActionProperty _jumpAction;

        public bool IsJumping { get; private set; }

        private void ProcessJump()
        {
            if (HasJumpStarted())
            {
                IsJumping = true;
                ForceDetach();
                TrySetState(_jumpState);
                return;
            }

            if (HasJumpEnded())
            {
                IsJumping = false;
                _verticalVelocity = Vector3.zero;
                //CheckFall();
            }
        }

        private bool HasJumpStarted()
        {
            return !IsJumping && 
                   IsGrounded &&
                   _jumpAction.action.triggered;
        }

        private bool HasJumpEnded()
        {
            return IsJumping && 
                   Time.time - _lastLostContactTime >= _groundCheckDelay;
        }
    }
}
