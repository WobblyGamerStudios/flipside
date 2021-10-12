using UnityEngine;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
    public class SprintDecorator : LocomotionDecorator
    {
        public new SprintModifier Modifier => base.Modifier as SprintModifier;
        public new ThirdPersonCharacterLocomotion Locomotion => base.Locomotion as ThirdPersonCharacterLocomotion;
        
        public bool IsSprinting { get; private set; }
        
        public override void Modify(ref Vector3 velocity)
        {
            if (!Locomotion.IsGrounded || 
                (IsSprinting && (!Locomotion.IsGrounded || Locomotion.MoveDirection.magnitude < Modifier.CancelThreshold)))
            {
                IsSprinting = false;
                return;
            }

            var isSprintTriggered = Modifier.SprintAction.action.triggered;

            if (!IsSprinting && isSprintTriggered)
            {
                IsSprinting = true;
            }
            
            Vector3 targetVel = Locomotion.MoveDirection * Modifier.Speed;
            velocity = Locomotion.GetDirectionTangentToSurface(targetVel.normalized, Locomotion.GroundNormal) * targetVel.magnitude;
        }
    }
}
