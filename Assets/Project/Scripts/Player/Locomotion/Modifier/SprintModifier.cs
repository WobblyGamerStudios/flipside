using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
    public class SprintModifier : LocomotionModifier
    {
        public float Speed = 5;
        [Range(0.1f, 1)] public float CancelThreshold = 0.9f;
        public InputActionProperty SprintAction;

        public override string Title => "Sprint";
        public override Type DecoratorType => typeof(SprintDecorator);
    }
}
