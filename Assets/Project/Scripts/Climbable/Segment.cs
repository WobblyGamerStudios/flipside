using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class Segment : MonoBehaviour
    {
        public Transform[] points;

        public void CalculateClosestPoint(Vector3 point, out Vector3 closestPoint, out Vector3 direction)
        {
            closestPoint = default;
            direction = default;
            float minSqrDst = float.MaxValue;
            int segmentIndexA = 0;
            int segmentIndexB = 0;

            for (int i = 0; i < points.Length; i++)
            {
                int next = i + 1;
                if (next >= points.Length) break;
                
                Vector3 closestPointOnSegment = ClosestPointOnSegment (point, points[i].position, points[next].position);
                float sqrDst = (point - closestPointOnSegment).sqrMagnitude;
                if (sqrDst < minSqrDst) {
                    minSqrDst = sqrDst;
                    closestPoint = closestPointOnSegment;
                    segmentIndexA = i;
                    segmentIndexB = next;
                }
            }

            direction = (points[segmentIndexA].position - points[segmentIndexB].position).normalized;
        }

        private Vector3 ClosestPointOnSegment(Vector3 point, Vector3 p1, Vector3 p2)
        {
            Vector3 pointDirection = point - p1;
            Vector3 targetDirection = p2 - p1;
            float projection = Vector3.Dot(pointDirection, targetDirection.normalized);

            if (projection > 0)
            {
                if (projection <= targetDirection.magnitude)
                {
                    return p1 + targetDirection.normalized * projection;
                }

                return p2;
            }

            return p1;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            for (int i = 0; i < points.Length; i++)
            {
                int next = i + 1;
                if (next >= points.Length) break;
                
                Gizmos.DrawLine(points[i].position, points[next].position);
            }
        }
    }
}
