using UnityEngine;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
	public class MoveDecorator : LocomotionDecorator
	{
		public new MoveModifier Modifier => base.Modifier as MoveModifier;
		public new ThirdPersonCharacterLocomotion Locomotion => base.Locomotion as ThirdPersonCharacterLocomotion;

		public override void Modify(ref Vector3 velocity)
		{
			if (!Locomotion.IsGrounded) return;
            
			Vector3 targetVel = Locomotion.MoveDirection * Modifier.Speed;
			targetVel = Locomotion.GetDirectionTangentToSurface(targetVel.normalized, Locomotion.GroundNormal) * targetVel.magnitude;

			velocity = Vector3.Lerp(velocity, targetVel, Modifier.Friction * Time.deltaTime);
		}
	}
}
