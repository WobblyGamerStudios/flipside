using System;
using UnityEngine.InputSystem;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
	public class GroundMovementModifier : LocomotionModifier
	{
		public float MoveSpeed = 3;
		public float Friction = 20;
		public float SprintSpeed = 5;
		public InputActionProperty SprintAction;
		
		public override string Title => "GroundMovement";
		public override Type DecoratorType => typeof(GroundMovementDecorator);
	}
}