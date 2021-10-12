using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class LinearState : CharacterState
    {
        [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)] 
        private RootMotionLinearTransitionAsset _linearMixerTransition;
        
        public float Blend
        {
            get => _linearMixerTransition.Transition.State.Parameter;
            set
            {
                if (!IsActive || !_linearMixerTransition.IsValid) return;
                _linearMixerTransition.Transition.State.Parameter = value;
            }
        }
        
        public override void OnEnterState()
        {
            base.OnEnterState();

            _animancer.Play(_linearMixerTransition);
        }

        public void AddCallback(float normalizedTime, Action callback)
        {
            _linearMixerTransition.Transition.Events.Add(normalizedTime, callback);
        }
    }
}
