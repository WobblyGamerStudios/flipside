using System;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class InteractableDetector : Detector
    {
        [SerializeField] private LayerMask _layerMask = -1;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private int _resultBufferSize = 10;

        public Vector3 Origin => transform.position + _offset;

        private int _resultCount;
        private Collider[] _results;

        protected override void Awake()
        {
            _results = new Collider[_resultBufferSize];
            
            base.Awake();
        }

        protected override void ComputeResults()
        {
            Array.Clear(_results, 0, _resultBufferSize);
            
            _resultCount = Physics.OverlapSphereNonAlloc(Origin, _radius, _results, _layerMask, QueryTriggerInteraction.Ignore);

            _detectables.Clear();
            for (var i = 0; i < _resultCount; i++)
            {
                var col = _results[i];
                if (!col) continue;
                var detectable = col.GetComponent<IDetectable>();
                if (detectable == null) continue;
                _detectables.Add(detectable);
            }
        }

        protected override void ComparePreviousResults()
        {
            base.ComparePreviousResults();
        }

        protected override void ProcessCurrentResults()
        {
            base.ProcessCurrentResults();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Origin, _radius);
        }
    }
}
