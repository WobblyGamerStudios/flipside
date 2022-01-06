using System;
using UnityEngine;
using UnityEngine.Events;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class Trigger : MonoBehaviour
    {
        [SerializeField] private TriggerEvent OnTriggerEnterEvent;
        [SerializeField] private TriggerEvent OnTriggerStayEvent;
        [SerializeField] private TriggerEvent OnTriggerExitEvent;
        
        private Rigidbody _rigidbody;
        private BoxCollider _boxCollider;
        
        private void OnEnable()
        {
            ValidateRigidBody();
            ValidateBoxCollider();
        }

        private void ValidateRigidBody()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (!_rigidbody)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            _rigidbody.useGravity = false;
        }

        private void ValidateBoxCollider()
        {
            _boxCollider = GetComponent<BoxCollider>();

            if (!_boxCollider)
            {
                _boxCollider = gameObject.AddComponent<BoxCollider>();
            }

            _boxCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }
        
        [Serializable]
        public class TriggerEvent : UnityEvent<Collider> { }
    }
}
