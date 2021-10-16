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

        [SerializeField] private GameObject _blockers;
        [SerializeField, ListDrawerSettings(CustomAddFunction = "AddZoneReference", CustomRemoveElementFunction = "RemoveZoneReference")]
        private List<ZoneReference> _zoneReferences = new List<ZoneReference>();
        
        public Vector3 StartPoint { get; private set; }
        public bool IsBeingClimbed { get; set; }
        public Vector3 Forward => _currentZone ? _currentZone.transform.forward : transform.forward;
        
        private ClimbableZone _currentZone;
        private ClimbableZone _lastZone;
        private PlayerCharacter _currentCharacter;

        #region MonoBehaviour

        private void Awake()
        {
            if (_blockers) _blockers.SetActive(false);
        }

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
            if ((_currentZone && !Equals(_currentZone, zone)) || !other.tag.Equals("Player") || IsBeingClimbed) return;

            if (!other.gameObject.TryGetComponent(out _currentCharacter))
            {
                //log
                return;
            }

            //StartPoint = _currentCharacter.transform.position.ClampToBounds(zone.Collider.bounds);
            _lastZone = _currentZone;
            _currentZone = zone;
            _currentCharacter.CurrentClimbable = this;
        }

        private void OnZoneStay(ClimbableZone zone, Collider other)
        {
            StartPoint = _currentCharacter.transform.position.ClampToBounds(zone.Collider.bounds);
        }
        
        private void OnZoneExit(ClimbableZone zone, Collider other)
        {
            if (Equals(_lastZone, zone) || !other.tag.Equals("Player") || IsBeingClimbed) return;

            if (_currentCharacter) _currentCharacter.CurrentClimbable = null;
            
            _currentCharacter = null;
            _currentZone = null;
            _lastZone = null;
            IsBeingClimbed = false;
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
