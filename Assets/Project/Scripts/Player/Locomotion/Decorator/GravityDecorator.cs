using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.Locomotion
{
    public class GravityDecorator : LocomotionDecorator
    {
        public GravityModifier GravityModifier => Modifier as GravityModifier;
        
        public override void Modify(ref Vector3 velocity)
        {
            if (Locomotion.IsGrounded) return;
            velocity += Vector3.down * (GravityModifier.GravityFactor * Time.deltaTime);
        }
    }
}
