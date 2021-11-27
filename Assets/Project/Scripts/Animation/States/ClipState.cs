using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class ClipState : CharacterState
    {
        [SerializeField] 
        protected RootMotionClipTransition _clipTransition;
        public RootMotionClipTransition ClipTransition => _clipTransition;

        public override void OnEnterState()
        {
            base.OnEnterState();

            _animancer.Play(_clipTransition);
        }
    }
}
