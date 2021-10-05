using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.Locomotion
{
    public class GravityDecorator : MonoBehaviour, ILocomotionDecorator
    {
        public GravityModifier Modifier { get; private set; }
        public CharacterLocomotion Locomotion { get; private set; }
        
        public void Setup(LocomotionModifier modifier, CharacterLocomotion locomotion)
        {
            Modifier = modifier as GravityModifier;
            Locomotion = locomotion;
        }

        public void Modify(ref Vector3 velocity)
        {
            if (Locomotion.IsGrounded) return;
            velocity += Vector3.down * (Modifier.GravityFactor * Time.deltaTime);
        }
    }
}
