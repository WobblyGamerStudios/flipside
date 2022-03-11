using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] private Segment segment;

        public PointResult result;
        
        private void OnDrawGizmos()
        {
            if (!segment) return;
            Gizmos.color = Color.yellow;
            segment.CalculateClosestPoint(transform.position, out Vector3 position, out Vector3 direction);
            Gizmos.DrawSphere(position, 0.2f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(position, direction);
        }
    }
}
