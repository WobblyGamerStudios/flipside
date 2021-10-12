using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Slide")] 
        [SerializeField] private ClipState _slideState;
        [SerializeField] private float _minSlideThreshold;
        
        public bool IsSliding { get; private set; }

        private float _slopeAngle;
        private float _slideStartTime;
        
        private void ProcessSlide()
        {
            _slopeAngle = Vector3.Angle(GroundNormal, transform.forward);
            
            if (HasSlideStarted())
            {
                IsSliding = true;
                _slideStartTime = Time.time;
                TrySetState(_slideState);
            }

            if (IsSliding && _slopeAngle > 90 + _minSlideThreshold)
            {
                SlideComplete();
            }
        }

        private bool HasSlideStarted()
        {
            return !IsSliding &&
                   IsSprinting &&
                   _crouchAction.action.triggered;
        }

        public void SlideComplete()
        {
            IsSliding = false;
            TrySetState(_moveState);
        }
    }
}
