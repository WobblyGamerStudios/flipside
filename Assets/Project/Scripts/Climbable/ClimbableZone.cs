using System;
using UnityEngine;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(BoxCollider))]
    public class ClimbableZone : MonoBehaviour
    {
        public BoxCollider Collider { get; private set; }

        public Action<ClimbableZone, Collider> OnZoneEnterEvent;
        public Action<ClimbableZone, Collider> OnZoneStayEvent;
        public Action<ClimbableZone, Collider> OnZoneExitEvent;

        private void OnValidate()
        {
            if (Collider) return;
            
            Collider = GetComponent<BoxCollider>();
            Collider.isTrigger = true;
        }

        private void Awake()
        {
            if (Collider) return;
         
            Collider = GetComponent<BoxCollider>();
            Collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnZoneEnterEvent?.Invoke(this, other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnZoneStayEvent?.Invoke(this, other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnZoneExitEvent?.Invoke(this, other);
        }
    }
}
