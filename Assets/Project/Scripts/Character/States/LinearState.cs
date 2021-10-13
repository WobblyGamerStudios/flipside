using System;
using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class LinearState : CharacterState
    {
        [SerializeField] 
        private LinearMixerTransition _linearMixerTransition; 
        
        public float Blend
        {
            get => _linearMixerTransition.State.Parameter;
            set
            {
                if (!IsActive || !_linearMixerTransition.IsValid) return;
                _linearMixerTransition.State.Parameter = value;
            }
        }
        
        public override void OnEnterState()
        {
            base.OnEnterState();

            _animancer.Play(_linearMixerTransition);
        }

        public void AddCallback(float normalizedTime, Action callback)
        {
            _linearMixerTransition.Events.Add(normalizedTime, callback);
        }
    }
}
