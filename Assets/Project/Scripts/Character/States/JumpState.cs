using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class JumpState : CharacterState
    {
        [SerializeField] private ClipTransition _clipTransition;

        public override void OnEnterState()
        {
            base.OnEnterState();

            _animancer.Play(_clipTransition);
        }
    }
}
