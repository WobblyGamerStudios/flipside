using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class JumpState : ClipState
    {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)] 
        private ClipTransitionAsset _fallTransition;

        public override void OnEnterState()
        {
            base.OnEnterState();
            
            if (!_clipTransition.Transition.State.IsLooping)
                _clipTransition.Transition.State.Events.OnEnd += JumpEnded;
        }

        public override void OnExitState()
        {
            base.OnExitState();

            _clipTransition.Transition.State.Events.OnEnd -= JumpEnded;
        }

        private void JumpEnded()
        {
            _animancer.Play(_fallTransition);
        }
    }
}
