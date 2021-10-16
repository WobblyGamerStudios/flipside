using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class MixerState : CharacterState
    {
        [SerializeField] private MixerTransition2D _mixerTransition;

        public Vector2 Value
        {
            get => _mixerTransition.State.Parameter;
            set
            {
                if (!IsActive || !_mixerTransition.IsValid) return;
                _mixerTransition.State.Parameter = value;
            }
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            _animancer.Play(_mixerTransition);
        }
    }
}
