using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class FlipSideLinearState : AnimationState
    {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)] 
        private LinearMixerTransitionAsset _linearMixerTransitionAsset;

        public new FlipSideStateMachineProcessor StateMachineProcessor => base.StateMachineProcessor as FlipSideStateMachineProcessor; 
        
        public override void OnEnterState()
        {
            base.OnEnterState();

            StateMachineProcessor.AnimancerComponent.Play(_linearMixerTransitionAsset.Transition);
        }

        public override void Tick()
        {
            if (!IsInitialized || !IsActive) return;
            _linearMixerTransitionAsset.Transition.State.Parameter = StateMachineProcessor.CharacterLocomotion.MoveDirection.magnitude;
        }
    }
}
