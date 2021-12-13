using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class PhysicsExtensions
    {
        public static bool RaycastArray(Vector3 start, Vector3 end, Vector3 direction, int count, out RaycastHit hit, LayerMask layerMask, QueryTriggerInteraction interaction)
        {
            hit = default;
            return false;
        }
    }
}
