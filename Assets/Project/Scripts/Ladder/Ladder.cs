using System;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class Ladder : MonoBehaviour
    {
        private const string LOG_FORMAT = nameof(Ladder) + ".{0} :: {1}";

        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _length;
        [SerializeField] private Transform _bottomReleasePoint;
        public Transform BottomReleasePoint => _bottomReleasePoint;
        [SerializeField] private Transform _topReleasePoint;
        public Transform TopReleasePoint => _topReleasePoint;

        // Gets the position of the bottom point of the ladder segment
        public Vector3 BottomAnchorPoint => transform.position + transform.TransformVector(_offset);

        // Gets the position of the top point of the ladder segment
        public Vector3 TopAnchorPoint => BottomAnchorPoint + transform.up * _length;
        
        public Vector3 ClosestPointOnLadderSegment(Vector3 fromPoint, out float onSegmentState)
        {
            var segment = TopAnchorPoint - BottomAnchorPoint;            
            var segmentPoint1ToPoint = fromPoint - BottomAnchorPoint;
            var pointProjectionLength = Vector3.Dot(segmentPoint1ToPoint, segment.normalized);

            // When higher than bottom point
            if (pointProjectionLength > 0)
            {
                // If we are not higher than top point
                if (pointProjectionLength <= segment.magnitude)
                {
                    onSegmentState = 0;
                    return BottomAnchorPoint + (segment.normalized * pointProjectionLength);
                }

                // If we are higher than top point
                onSegmentState = pointProjectionLength - segment.magnitude;
                return TopAnchorPoint;
            }
            
            // When lower than bottom point
            onSegmentState = pointProjectionLength;
            return BottomAnchorPoint;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(BottomAnchorPoint, TopAnchorPoint);
        }
    }
}
