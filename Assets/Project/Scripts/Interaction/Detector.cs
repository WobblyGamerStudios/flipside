using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public abstract class Detector : MonoBehaviour
    {
        public bool IsEnabled { get; set; } = true;
        
        protected List<IDetectable> _detectables;
        protected List<IDetectable> _prevDetectables;

        protected virtual void Awake()
        {
            _detectables = new List<IDetectable>();
            _prevDetectables = new List<IDetectable>();
        }

        protected virtual void FixedUpdate()
        {
            if (!IsEnabled) return;
            
            ComputeResults();
            ComparePreviousResults();
            ProcessCurrentResults();
            
            _prevDetectables = new List<IDetectable>(_detectables);
        }
        
        protected abstract void ComputeResults();

        protected virtual void ComparePreviousResults()
        {
            for (var iPrev = 0; iPrev < _prevDetectables.Count; iPrev++)
            {
                var previous = _prevDetectables[iPrev];
                if (previous == null) continue;
                if(!_detectables.Contains(previous)) previous.OnExit();
            }
        }

        protected virtual void ProcessCurrentResults()
        {
            for (var iCurrent = 0; iCurrent < _detectables.Count; iCurrent++)
            {
                var current = _detectables[iCurrent];
                if (current == null) continue;
                if (!_prevDetectables.Contains(current)) current.OnEnter();
            }
        }
    }
}
