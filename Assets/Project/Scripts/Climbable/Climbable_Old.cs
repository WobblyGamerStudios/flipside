using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    [Flags]
    public enum ReleasePoints
    {
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2, 
        Right = 1 << 3
    }

    [Flags]
    public enum ClimbingAxis
    {
        Horizontal = 1 << 0,
        Vertical = 1 << 1,
        Top = 1 << 2
    }

    

    public struct PointProjection
    {
        public Vector3 PointDirection;
        public Vector3 TargetDirection;
        public float Projection;
        public Vector3 ClosestPoint;
    }
    
    [Serializable, InlineProperty(LabelWidth = 30)]
    public struct RangeOffset
    {
        [HorizontalGroup] public float Min;
        [HorizontalGroup] public float Max;
    }

    [RequireComponent(typeof(BoxCollider))]
    public class Climbable_Old : MonoBehaviour
    {
        [SerializeField] private ClimbingAxis _climbingAxis;
        public ClimbingAxis ClimbingAxis => _climbingAxis;

        [SerializeField] private RangeOffset _verticalOffset;
        [SerializeField] private RangeOffset _horizontalOffset;
        [SerializeField] private float _depth;
        
        [SerializeField] private ReleasePoints _releasePoints;
        public ReleasePoints ReleasePoints => _releasePoints;
        [SerializeField] private float _topExitDepth;
        [SerializeField] private float _bottomExitDepth;
        [SerializeField] private float _leftExitDepth;
        [SerializeField] private float _rightExitDepth;

        public Vector3 CenterPoint => transform.position + transform.TransformVector(_colliderCenter);
        public Vector3 VerticalMinPoint => transform.position + transform.TransformVector(_colliderBottomOffset);
        public Vector3 VerticalMaxPoint => transform.position + transform.TransformVector(_colliderTopOffset);
        public Vector3 HorizontalMinPoint => transform.position + transform.TransformVector(_colliderLeftOffset);
        public Vector3 HorizontalMaxPoint => transform.position + transform.TransformVector(_colliderRightOffset);

        public Vector3 TopExitPoint => transform.position + transform.TransformVector(_colliderTop + new Vector3 {z = _topExitDepth});
        public Vector3 BottomExitPoint => transform.position + transform.TransformVector(_colliderBottom + new Vector3 {z = _bottomExitDepth});
        public Vector3 LeftExitPoint => transform.position + transform.TransformVector(_colliderLeft + new Vector3 {x = _leftExitDepth, z = _depth});
        public Vector3 RightExitPoint => transform.position + transform.TransformVector(_colliderRight + new Vector3 {x = _rightExitDepth, z = _depth});
        
        private BoxCollider _boxCollider;
        private Vector3 _colliderCenter => _boxCollider ? _boxCollider.center : Vector3.zero;
        private Vector3 _colliderTop => _boxCollider ? _colliderCenter + new Vector3 {y = (_boxCollider.size.y * 0.5f)} : Vector3.zero;
        private Vector3 _colliderBottom => _boxCollider ? _colliderCenter + new Vector3 {y = -(_boxCollider.size.y * 0.5f)} : Vector3.zero;
        private Vector3 _colliderLeft => _boxCollider ? _colliderCenter + new Vector3 {x = -(_boxCollider.size.x * 0.5f)} : Vector3.zero;
        private Vector3 _colliderRight => _boxCollider ? _colliderCenter + new Vector3 {x = _boxCollider.size.x * 0.5f} : Vector3.zero;

        private Vector3 _colliderTopOffset => _colliderTop + new Vector3 {y = -_verticalOffset.Max, z = _depth};
        private Vector3 _colliderBottomOffset => _colliderBottom + new Vector3 {y = _verticalOffset.Min, z = _depth};
        private Vector3 _colliderLeftOffset => _colliderLeft + new Vector3 {x = _horizontalOffset.Min, z = _depth};
        private Vector3 _colliderRightOffset => _colliderRight + new Vector3 {x = -_horizontalOffset.Max, z = _depth};

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        //ToDo create a base abstraction that Ladder, Ledge, Beam, Point climbables inherit from
        public Vector3 ClosestPointOnClimbable(Vector3 point, out PointResult result)
        {
            var closestPoint = Vector3.zero;
            result = default;

            if (ClimbingAxis.HasFlag(ClimbingAxis.Vertical) && ClimbingAxis.HasFlag(ClimbingAxis.Horizontal))
            {
                CalculateProjection(point, VerticalMinPoint, VerticalMaxPoint, out var verticalPointProjection);
                CalculateProjection(point, HorizontalMinPoint, HorizontalMaxPoint, out var horizontalPointProjection);
                
                closestPoint = new Vector3
                {
                    x = Mathf.Clamp(horizontalPointProjection.ClosestPoint.x, HorizontalMinPoint.x, HorizontalMaxPoint.x),
                    y = Mathf.Clamp(verticalPointProjection.ClosestPoint.y, VerticalMinPoint.y, VerticalMaxPoint.y),
                    z = Mathf.Clamp(horizontalPointProjection.ClosestPoint.z, HorizontalMinPoint.z, HorizontalMaxPoint.z)
                };
            }
            else
            {
                if (ClimbingAxis.HasFlag(ClimbingAxis.Vertical))
                {
                    CalculateProjection(point, VerticalMinPoint, VerticalMaxPoint, out var pointProjection);

                    if (pointProjection.Projection > 0)
                    {
                        if (pointProjection.Projection <= pointProjection.TargetDirection.magnitude)
                        {
                            result = PointResult.IsWithin;
                            closestPoint = pointProjection.ClosestPoint;
                        }
                        else
                        {
                            result = PointResult.ExceedsMax;
                            closestPoint = VerticalMaxPoint;
                        }
                    }
                    else
                    {
                        result = PointResult.ExceedsMin;
                        closestPoint = VerticalMinPoint;
                    }
                }
                else if (ClimbingAxis.HasFlag(ClimbingAxis.Horizontal))
                {
                    CalculateProjection(point, HorizontalMinPoint, HorizontalMaxPoint, out var pointProjection);

                    if (pointProjection.Projection > 0)
                    {
                        if (pointProjection.Projection <= pointProjection.TargetDirection.magnitude)
                        {
                            result = PointResult.IsWithin;
                            closestPoint = pointProjection.ClosestPoint;
                        }
                        else
                        {

                            result = PointResult.ExceedsMax;
                            closestPoint = HorizontalMaxPoint;
                        }
                    }
                    else
                    {
                        result = PointResult.ExceedsMin;
                        closestPoint = HorizontalMinPoint;
                    }
                }
            }

            return closestPoint;
        }

        private void CalculateProjection(Vector3 point, Vector3 min, Vector3 max, out PointProjection pointProjection)
        {
            pointProjection.PointDirection = point - min;
            pointProjection.TargetDirection = max - min;
            pointProjection.Projection = Vector3.Dot(pointProjection.PointDirection, pointProjection.TargetDirection.normalized);
            pointProjection.ClosestPoint = min + pointProjection.TargetDirection.normalized * pointProjection.Projection;
        }

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!_boxCollider) _boxCollider = GetComponent<BoxCollider>();
            
            DrawClimbAreaGizmo();
            DrawReleasePointsGizmo();
        }

        private void DrawClimbAreaGizmo()
        {
            var color = Color.cyan;
            color.a = 0.5f;
            Gizmos.color = color;
            
            if (ClimbingAxis.HasFlag(ClimbingAxis.Vertical) && ClimbingAxis.HasFlag(ClimbingAxis.Horizontal))
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                var verticalCenter = Vector3.Lerp(_colliderBottomOffset, _colliderTopOffset, 0.5f);
                var horizontalCenter = Vector3.Lerp(_colliderLeftOffset, _colliderRightOffset, 0.5f);
                var center = new Vector3((horizontalCenter).x, verticalCenter.y, _depth);
                Gizmos.DrawCube(center, new Vector3((_colliderLeftOffset - _colliderRightOffset).magnitude, (_colliderBottomOffset - _colliderTopOffset).magnitude, 0));
                Gizmos.matrix = transform.localToWorldMatrix;
            }
            else
            {
                if (ClimbingAxis.HasFlag(ClimbingAxis.Vertical))
                {
                    Gizmos.DrawLine(VerticalMinPoint, VerticalMaxPoint);
                }
                else if (ClimbingAxis.HasFlag(ClimbingAxis.Horizontal))
                {
                    Gizmos.DrawLine(HorizontalMinPoint, HorizontalMaxPoint);
                }
            }

            if (ClimbingAxis.HasFlag(ClimbingAxis.Top))
            {
                
            }
        }

        private void DrawReleasePointsGizmo()
        {
            Handles.color = Color.blue;

            if (ClimbingAxis.HasFlag(ClimbingAxis.Vertical) && ClimbingAxis.HasFlag(ClimbingAxis.Horizontal))
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = transform.localToWorldMatrix;
            }
            else
            {
                if (ReleasePoints.HasFlag(ReleasePoints.Top))
                {
                    Handles.DrawSolidDisc(TopExitPoint, transform.up, 0.5f);
                }
                
                if (ReleasePoints.HasFlag(ReleasePoints.Bottom))
                {
                    Handles.DrawSolidDisc(BottomExitPoint, transform.up, 0.5f);
                }

                if (ReleasePoints.HasFlag(ReleasePoints.Left))
                {
                    Handles.DrawSolidDisc(LeftExitPoint, transform.up, 0.5f);
                }

                if (ReleasePoints.HasFlag(ReleasePoints.Right))
                {
                    Handles.DrawSolidDisc(RightExitPoint, transform.up, 0.5f);
                }
            }
        }
        
        #endregion Gizmos

    }
}
