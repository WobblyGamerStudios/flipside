using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class Climbable : MonoBehaviour
    {
        [SerializeField] private bool _canAttachFromGround;
        public bool CanAttachFromGround => _canAttachFromGround;

        [SerializeField] private ClimbableDirections _directions;
        public ClimbableDirections Directions => _directions;
        [SerializeField, ListDrawerSettings(CustomAddFunction = "AddZoneReference", CustomRemoveElementFunction = "RemoveZoneReference")]
        private List<ZoneReference> _zoneReferences = new List<ZoneReference>();

        public bool IsBeingClimbed { get; set; }
        
        public Vector3 StartPoint { get; private set; }
        public Vector3 Forward => _currentZone ? _currentZone.transform.forward : transform.forward;
        
        private ClimbableZone _currentZone;
        private ClimbableZone _lastZone;
        private PlayerCharacter _currentCharacter;

        #region MonoBehaviour

        private void OnEnable()
        {
            foreach (var zoneRef in _zoneReferences.Where(zoneRef => zoneRef.ClimbableZone))
            {
                zoneRef.ClimbableZone.OnZoneEnterEvent += OnZoneEnter;
                zoneRef.ClimbableZone.OnZoneStayEvent += OnZoneStay;
                zoneRef.ClimbableZone.OnZoneExitEvent += OnZoneExit;
            }
        }

        private void OnDisable()
        {
            foreach (var zoneRef in _zoneReferences.Where(zoneRef => zoneRef.ClimbableZone))
            {
                zoneRef.ClimbableZone.OnZoneEnterEvent -= OnZoneEnter;
                zoneRef.ClimbableZone.OnZoneStayEvent -= OnZoneStay;
                zoneRef.ClimbableZone.OnZoneExitEvent -= OnZoneExit;
            }
        }
        
        #endregion MonoBehaviour

        private void OnZoneEnter(ClimbableZone zone, Collider other)
        {
            if ((_currentZone && !Equals(_currentZone, zone)) || !other.tag.Equals("Player")) return;

            _lastZone = _currentZone;
            _currentZone = zone;

            if (IsBeingClimbed) return;
            
            if (!other.gameObject.TryGetComponent(out _currentCharacter))
            {
                //log
                return;
            }
            //_currentCharacter.CurrentClimbable = this;
            
            CalculatePoint();
        }

        private void OnZoneStay(ClimbableZone zone, Collider other)
        {
            if (!_currentCharacter) return;
            
            CalculatePoint();
        }
        
        private void OnZoneExit(ClimbableZone zone, Collider other)
        {
            if (zone != _currentZone && Equals(_lastZone, zone) || !other.tag.Equals("Player") || IsBeingClimbed) return;

            if (_currentCharacter) _currentCharacter.CurrentClimbable = null;
            
            _currentCharacter = null;
            _currentZone = null;
            _lastZone = null;
            IsBeingClimbed = false;
        }

        private void CalculatePoint()
        {
            var clampedPosition = _currentCharacter.transform.position.ClampToBounds(_currentZone.Collider.bounds);

            if (!Directions.HasFlag(ClimbableDirections.Horizontal))
            {
                clampedPosition.x = _currentZone.Collider.bounds.center.x;
                clampedPosition.z = _currentZone.Collider.bounds.center.z;
            }
            
            if (!Directions.HasFlag(ClimbableDirections.Vertical)) clampedPosition.y = _currentZone.Collider.bounds.center.y;
            
            StartPoint = clampedPosition;
        }

        private bool Equals(ClimbableZone zone1, ClimbableZone zone2)
        {
            if (!zone1 || !zone2) return false;
            
            var zone1IsInList = false;
            foreach (var zoneRef in _zoneReferences.Where(zoneRef => zoneRef.ClimbableZone == zone1))
            {
                zone1IsInList = true;
            }
            
            var zone2IsInList = false;
            foreach (var zoneRef in _zoneReferences.Where(zoneRef => zoneRef.ClimbableZone == zone2))
            {
                zone2IsInList = true;
            }

            return zone1IsInList && zone2IsInList;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(StartPoint, 0.05f);
        }

#if UNITY_EDITOR
        
        #region Odin

        private void AddZoneReference()
        {
            if (Application.isPlaying) return;
            
            GameObject zoneRefObject = new GameObject("ZoneReference");
            zoneRefObject.transform.SetParent(transform, false);

            ClimbableZone zone = zoneRefObject.AddComponent<ClimbableZone>();
            
            _zoneReferences.Add(new ZoneReference{ClimbableZone = zone});
        }

        private void RemoveZoneReference(ZoneReference reference)
        {
            if (Application.isPlaying) return;
            
            DestroyImmediate(reference.ClimbableZone.gameObject);
            _zoneReferences.Remove(reference);
        }
        
        #endregion Odin
        
#endif
        
    }

    [Flags]
    public enum ClimbableDirections
    {
        Vertical = 1 << 0, 
        Horizontal = 1 << 1
    }

    [Serializable]
    public class ZoneReference
    {
        [HideLabel]
        public ClimbableZone ClimbableZone;
    }
}
