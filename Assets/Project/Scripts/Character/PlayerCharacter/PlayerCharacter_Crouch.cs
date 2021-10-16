using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Crouch")] 
        [SerializeField] private LinearState _crouchState;
        [SerializeField] private float _crouchSpeed;
        [SerializeField] private CharacterSize _crouchSize;
        [SerializeField] private InputActionProperty _crouchAction;

        public bool IsCrouching { get; private set; }

        private void ProcessCrouch()
        {
            _crouchState.Value = MoveDirection.magnitude;

            if (HasCrouchStarted())
            {
                IsCrouching = true;
                ModifyCharacterSize(_crouchSize);
                TrySetState(_crouchState);
            }
            
            if (HasCrouchEnded())
            {
                IsCrouching = false;
                ModifyCharacterSize(_defaultSize);
                CheckFall();
            }
        }

        private bool HasCrouchStarted()
        {
            return !IsCrouching && 
                   !IsSliding &&
                   !IsRolling &&
                   IsGrounded && 
                   IsCrouchAbovePressPoint();
        }

        private bool HasCrouchEnded()
        {
            return IsCrouching &&
                   (!IsGrounded ||
                    !IsCrouchAbovePressPoint());
        }

        private bool IsCrouchAbovePressPoint()
        {
            return _crouchAction.action.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;
        }
    }
}
