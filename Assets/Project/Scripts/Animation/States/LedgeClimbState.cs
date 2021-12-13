using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class LedgeClimbState : CharacterState
    {
        [SerializeField] private LinearMixerTransition _bracedBlendTree;
        [SerializeField] private LinearMixerTransition _freeHangBlendTree;

        public float BracedToFreeHangBlend
        {
            get => _climbMixer.Parameter;
            set
            {
                if (!IsActive || !_climbMixer.IsValid) return;
                _climbMixer.Parameter = value;
            }
        }

        public float Value
        {
            get => BracedToFreeHangBlend <= 0 ? _bracedBlendTree.State.Parameter : _freeHangBlendTree.State.Parameter;
            set
            {
                if (!IsActive || !_bracedBlendTree.IsValid || !_freeHangBlendTree.IsValid) return;
                _bracedBlendTree.State.Parameter = value;
                _freeHangBlendTree.State.Parameter = value;
            }
        }

        private LinearMixerState _climbMixer;
        
        public override void Initialize(AnimancerComponent animancer)
        {
            _climbMixer = new LinearMixerState();
            // Allocate 2 child states.
            _climbMixer.Initialize(2);
            // Create Child [0] using the Regular Movement transition with a Threshold of 0.
            _climbMixer.CreateChild(0, _bracedBlendTree, 0);
            // Create Child [1] using the Crouching Movement transition with a Threshold of 1.
            _climbMixer.CreateChild(1, _freeHangBlendTree, 1);
            
            base.Initialize(animancer);
        }
        
        public override void OnEnterState()
        {
            _animancer.Play(_climbMixer);
            
            base.OnEnterState();
        }
    }
}
