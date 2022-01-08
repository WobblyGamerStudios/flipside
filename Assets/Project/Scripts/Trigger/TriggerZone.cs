using UnityEngine;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class TriggerZone : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ((_layerMask.value & (1 << other.gameObject.layer)) <= 0) return;
            OnEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if ((_layerMask.value & (1 << other.gameObject.layer)) <= 0) return;
            OnStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if ((_layerMask.value & (1 << other.gameObject.layer)) <= 0) return;
            OnExit(other);
        }

        protected abstract void OnEnter(Collider other);

        protected abstract void OnStay(Collider other);

        protected abstract void OnExit(Collider other);

        protected virtual void Reset()
        {
            var rb = GetComponent<Rigidbody>();

            rb.useGravity = false;
        }
    }
}
