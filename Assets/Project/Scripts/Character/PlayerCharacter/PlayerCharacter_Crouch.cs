using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [FoldoutGroup("Crouch"), SerializeField] private LinearState _crouchState;
        [FoldoutGroup("Crouch"), SerializeField] private float _crouchSpeed;
        [FoldoutGroup("Crouch"), SerializeField] private CharacterSize _crouchSize;
        [FoldoutGroup("Crouch"), SerializeField] private InputActionProperty _crouchAction;

        private void ProcessCrouch()
        {
            _crouchState.Value = MoveDirection.magnitude;

            if (HasCrouchStarted())
            {
                CurrentState = PlayerState.Crouch;
                ModifyCharacterSize(_crouchSize);
                TrySetState(_crouchState);
            }
            
            if (HasCrouchEnded())
            {
                CurrentState = PlayerState.Move;
                ModifyCharacterSize(_defaultSize);
                CheckFall();
            }
        }

        private bool HasCrouchStarted()
        {
            return CurrentState == PlayerState.Move &&
                   IsGrounded && 
                   IsCrouchAbovePressPoint();
        }

        private bool HasCrouchEnded()
        {
            return CurrentState == PlayerState.Crouch &&
                   (!IsGrounded ||
                    !IsCrouchAbovePressPoint());
        }

        private bool IsCrouchAbovePressPoint()
        {
            return _crouchAction.action.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;
        }
    }
}
