using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class LinearState : AnimationState
    {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)] 
        private LinearMixerTransitionAsset _linearMixerTransitionAsset;

        public override void OnEnterState()
        {
            base.OnEnterState();

            _stateMachineProcessor.AnimancerComponent.Play(_linearMixerTransitionAsset.Transition);
        }

        public override void Tick()
        {
            if (!IsInitialized || !IsActive) return;
            _linearMixerTransitionAsset.Transition.State.Parameter = _stateMachineProcessor.CharacterLocomotion.MoveDirection.magnitude;
        }
    }
}
