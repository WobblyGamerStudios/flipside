using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class ClipTransitionState : AnimationState
    {
        [SerializeField] private ClipTransition _clipTransition;
        
        public new FlipSideStateMachineProcessor StateMachineProcessor => base.StateMachineProcessor as FlipSideStateMachineProcessor;

        public override void OnEnterState()
        {
            base.OnEnterState();
            StateMachineProcessor.AnimancerComponent.Play(_clipTransition);
        }

        public override void Tick() { }
    }
}
