using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(BoxCollider))]
    public class EventTrigger : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private TriggerEvent _onTriggerEnter;
        [SerializeField] private TriggerEvent _onTriggerExit;
        [SerializeField] private TriggerEvent _onTriggerStay;

        public bool IsEnabled
        {
            get => _boxCollider && _boxCollider.enabled && enabled;
            set
            {
                if (!_boxCollider) return;
                _boxCollider.enabled = value;
            }
        }

        private BoxCollider _boxCollider;

        private void OnValidate()
        {
            if (!_boxCollider) _boxCollider = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_layerMask != (1 << _layerMask | other.gameObject.layer)) return;
            _onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (_layerMask != (1 << _layerMask | other.gameObject.layer)) return;
            _onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_layerMask != (1 << _layerMask | other.gameObject.layer)) return;
            _onTriggerExit?.Invoke(other);
        }

        private void Reset()
        {
            _boxCollider = GetComponent<BoxCollider>();
            if (!_boxCollider) return;
            _boxCollider.isTrigger = true;
        }

        [Serializable]
        public class TriggerEvent : UnityEvent<Collider> { }
    }
}
