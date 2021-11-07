using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [FoldoutGroup("Slide"), SerializeField] private ClipState _slideState;
        [FoldoutGroup("Slide"), SerializeField] private float _minSlideThreshold;
        [FoldoutGroup("Slide"), SerializeField] private CharacterSize _slideSize;
        
        private float _slopeAngle;
        private float _slideStartTime;
        
        private void ProcessSlide()
        {
            _slopeAngle = Vector3.Angle(GroundNormal, transform.forward);
            
            if (HasSlideStarted())
            {
                CurrentState = PlayerState.Slide | PlayerState.Disabled | PlayerState.IgnoreFall;
                _slideStartTime = Time.time;
                ModifyCharacterSize(_slideSize);
                TrySetState(_slideState);
            }

            if (!CurrentState.HasFlag(PlayerState.Slide)) return;

            if (_slopeAngle > 90 + _minSlideThreshold)
            {
                SlideComplete();
            }
        }

        private bool HasSlideStarted()
        {
            if (CurrentState.HasFlag(PlayerState.Slide)) return false;
            return CurrentState == PlayerState.Sprint && _crouchAction.action.triggered;
        }

        public void SlideComplete()
        {
            CurrentState = PlayerState.Move;
            ModifyCharacterSize(_defaultSize);
            TrySetState(_moveState);
        }
    }
}
