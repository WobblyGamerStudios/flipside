using System;
using UnityEngine.InputSystem;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
    public class CrouchModifier : LocomotionModifier
    {
        public float Speed;
        public InputActionProperty CrouchAction;
        
        public override string Title => "Crouch";
        public override Type DecoratorType => typeof(CrouchDecorator);
    }
}
