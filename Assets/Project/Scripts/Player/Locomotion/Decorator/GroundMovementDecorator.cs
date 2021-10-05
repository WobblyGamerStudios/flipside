using UnityEngine;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
	public class GroundMovementDecorator : MonoBehaviour, ILocomotionDecorator
	{
		public GroundMovementModifier Modifier { get; private set; }
		public CharacterLocomotion Locomotion { get; private set; }
		public bool IsSprinting { get; private set; }
		
		public void Setup(LocomotionModifier modifier, CharacterLocomotion locomotion)
		{
			Modifier = modifier as GroundMovementModifier;
			Locomotion = locomotion;
		}

		public void Modify(ref Vector3 velocity)
		{
			if (IsSprinting && (Locomotion.MoveDirection.magnitude < 0.9f || !Locomotion.IsGrounded))
			{
				IsSprinting = false;
			}
			
			if (!Locomotion.IsGrounded) return;
            
			if (Modifier.SprintAction.action.triggered)
			{
				IsSprinting = true;
			}
			
			Vector3 targetVel = Locomotion.MoveDirection * (IsSprinting ? Modifier.SprintSpeed : Modifier.MoveSpeed);
			targetVel = GetDirectionTangentToSurface(targetVel.normalized, Locomotion.GroundNormal) * targetVel.magnitude;

			velocity = Vector3.Lerp(velocity, targetVel, Modifier.Friction * Time.deltaTime);
		}
		
		// Gets a reoriented direction that is tangent to a given slope
		private Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
		{
			var directionRight = Vector3.Cross(direction, transform.up);
			return Vector3.Cross(surfaceNormal, directionRight).normalized;
		}
	}
}
