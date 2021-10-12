using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class FlipSideCharacter : Character
    {
        [Title("Crouching")] 
        [SerializeField] private LinearState _crouchState;
        [SerializeField] private InputActionProperty _crouchAction;
        
        public bool IsCrouching { get; private set; }
        
        private void ProcessCrouch()
        {
            _crouchState.Blend = MoveDirection.magnitude;
            
            if (HasCrouchStarted())
            {
                IsCrouching = true;
                TrySetState(_crouchState);
                return;
            }

            if (HasCrouchEnded())
            {
                IsCrouching = false;
                TrySetState(_moveState);
            }
        }

        private bool HasCrouchStarted()
        {
            return !IsCrouching && IsAboveCrouchThreshold();
        }
        
        private bool HasCrouchEnded()
        {
            return IsCrouching && !IsAboveCrouchThreshold();
        }

        private bool IsAboveCrouchThreshold()
        {
            return _crouchAction.action.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;
        }
    }
}
