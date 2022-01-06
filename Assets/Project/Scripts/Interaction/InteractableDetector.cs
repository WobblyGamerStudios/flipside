using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class InteractableDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask = -1;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private int _resultBufferSize = 10;

        public Vector3 Origin => transform.position + _offset;

        private int _resultCount;
        private Collider[] _results;
        private List<Interactable> _interactables;
        private Interactable _currentInteractable;
        private DistanceAngleComparison _distanceAngleComparison;

        private void Awake()
        {
            _interactables = new List<Interactable>();
            _results = new Collider[_resultBufferSize];
            _distanceAngleComparison = new DistanceAngleComparison(transform);
        }

        private void FixedUpdate()
        {
            Array.Clear(_results, 0, _resultBufferSize);
            
            _resultCount = Physics.OverlapSphereNonAlloc(Origin, _radius, _results, _layerMask, QueryTriggerInteraction.Ignore);

            _interactables.Clear();
            for (var i = 0; i < _resultCount; i++)
            {
                var col = _results[i];
                if (!col) continue;
                var detectable = col.GetComponentInParent<Interactable>();
                if (detectable == null) continue;
                _interactables.Add(detectable);
            }

            //If there's no interactable but we have a current interactable, exit the current interactable
            //Early out
            if (_interactables.Count == 0)
            {
                if (!_currentInteractable) return;
                _currentInteractable.OnExit();
                _currentInteractable = null;
                return;
            }
            
            //Sort interactables
            _interactables.Sort(_distanceAngleComparison);

            //Check if current and target interactables are the same
            var targetInteractable = _interactables[0];
            if (targetInteractable == _currentInteractable) return;
            
            //Exit current interactable and enter new interactable
            if (_currentInteractable) _currentInteractable.OnExit();
            _currentInteractable = targetInteractable;
            _currentInteractable.OnEnter();
        }

        private void OnDrawGizmos()
        {
            if (_interactables == null || _interactables.Count == 0) return;

            for (var i = 0; i < _interactables.Count; i++)
            {
                Gizmos.color = i == 0 ? Color.green : Color.white;
                Gizmos.DrawLine(Origin, _interactables[i].transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Origin, _radius);
        }
        
        //Comparer class to sort for the closest interactable to the player
        private class DistanceAngleComparison : IComparer<Interactable>
        {
            private readonly Transform transform;
            private Vector3 position => transform ? transform.position : Vector3.zero;

            public DistanceAngleComparison(Transform transform)
            {
                this.transform = transform;
            }
            
            public int Compare(Interactable i1, Interactable i2)
            {
                if (transform == null) return 0;
                if (i1 == null || i2 == null) return 0;
                if (i1 == i2) return 0;

                var i1Pos = i1.transform.position;
                var i2Pos = i2.transform.position;
                var i1d = Vector3.Distance(position, i1Pos);
                var i2d = Vector3.Distance(position, i2Pos);
                
                if (i1d < i2d) return -1;
                if (i2d < i1d) return 1;
                return 0;
            }
        }
    }
}
