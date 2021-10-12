using System;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
	public class MoveModifier : LocomotionModifier
	{
		public float Speed = 3;
		public float Friction = 20;
		
		public override string Title => "Move";
		public override Type DecoratorType => typeof(MoveDecorator);
	}
}