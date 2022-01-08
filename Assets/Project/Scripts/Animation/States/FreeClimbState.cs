using System.Collections;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class FreeClimbState : MixerState
    {
        [SerializeField] private ClipTransition _leanUpwards;
        [SerializeField] private ClipTransition _leanLeft;
        [SerializeField] private ClipTransition _leanRight;
        [SerializeField] private ClipTransition _climbUp;

        [Button]
        private void Return()
        {
            OnEnterState();
        }

        [Button]
        private void LeanUp()
        {
            if (!IsActive) return;
            _animancer.Play(_leanUpwards);
        }
        
        [Button]
        private void LeanLeft()
        {
            if (!IsActive) return;
            _animancer.Play(_leanLeft);
        }
        
        [Button]
        private void LeanRight()
        {
            if (!IsActive) return;
            _animancer.Play(_leanRight);
        }

        [Button]
        public void ClimbUp()
        {
            if (!IsActive) return;
            _animancer.Play(_climbUp);
        }
    }
}
