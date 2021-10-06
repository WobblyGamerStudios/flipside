using UnityEngine;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
	public class GroundMovementDecorator : LocomotionDecorator
	{
		public GroundMovementModifier GroundMovementModifier => Modifier as GroundMovementModifier;
		
		public bool IsSprinting { get; private set; }

		public override void Modify(ref Vector3 velocity)
		{
			if (IsSprinting && (Locomotion.MoveDirection.magnitude < 0.9f || !Locomotion.IsGrounded))
			{
				IsSprinting = false;
			}
			
			if (!Locomotion.IsGrounded) return;
            
			if (GroundMovementModifier.SprintAction.action.triggered)
			{
				IsSprinting = true;
			}
			
			Vector3 targetVel = Locomotion.MoveDirection * (IsSprinting ? GroundMovementModifier.SprintSpeed : GroundMovementModifier.MoveSpeed);
			targetVel = GetDirectionTangentToSurface(targetVel.normalized, Locomotion.GroundNormal) * targetVel.magnitude;

			velocity = Vector3.Lerp(velocity, targetVel, GroundMovementModifier.Friction * Time.deltaTime);
		}
		
		// Gets a reoriented direction that is tangent to a given slope
		private Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
		{
			var directionRight = Vector3.Cross(direction, transform.up);
			return Vector3.Cross(surfaceNormal, directionRight).normalized;
		}
	}
}
