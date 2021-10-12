using UnityEngine;
using UnityEngine.InputSystem;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
    public class CrouchDecorator : LocomotionDecorator
    {
        public new CrouchModifier Modifier => base.Modifier as CrouchModifier;
        public new ThirdPersonCharacterLocomotion Locomotion => base.Locomotion as ThirdPersonCharacterLocomotion;

        public bool IsCrouching { get; private set; }
        
        public override void Modify(ref Vector3 velocity)
        {
            if (!Locomotion.IsGrounded) return;

            IsCrouching = Modifier.CrouchAction.action.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;

            if (!IsCrouching) return;
            
            Vector3 targetVel = Locomotion.MoveDirection * Modifier.Speed;
            velocity = Locomotion.GetDirectionTangentToSurface(targetVel.normalized, Locomotion.GroundNormal) * targetVel.magnitude;
        }
    }
}
