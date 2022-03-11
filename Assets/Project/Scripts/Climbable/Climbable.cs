using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public enum PointResult
    {
        IsWithin,
        ExceedsMin,
        ExceedsMax
    }
    
    public abstract class Climbable : MonoBehaviour
    {
        public virtual Vector3 GetPointOnClimbable(Vector3 point, Vector3 min, Vector3 max, out PointResult result)
        {
            Vector3 pointDirection = point - min;
            Vector3 targetDirection = max - min;
            float projection = Vector3.Dot(pointDirection, targetDirection.normalized);

            if (projection > 0)
            {
                if (projection <= targetDirection.magnitude)
                {
                    result = PointResult.IsWithin;
                    return min + targetDirection.normalized * projection;
                }

                result = PointResult.ExceedsMax;
                return max;
            }

            result = PointResult.ExceedsMin;
            return min;
        }
    }
}
