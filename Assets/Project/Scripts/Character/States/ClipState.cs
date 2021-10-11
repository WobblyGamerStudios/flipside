using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class ClipState : CharacterState
    {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)] 
        protected ClipTransitionAsset _clipTransition;

        public override void OnEnterState()
        {
            base.OnEnterState();

            _animancer.Play(_clipTransition);
        }
    }
}
