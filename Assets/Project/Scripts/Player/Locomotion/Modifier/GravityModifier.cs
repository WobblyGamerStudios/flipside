using System;

namespace Wgs.Locomotion
{
    [Serializable]
    public class GravityModifier : LocomotionModifier
    {
        public float GravityFactor = 15;

        public override string Title => "Gravity";
        public override Type DecoratorType => typeof(GravityDecorator);
    }
}
