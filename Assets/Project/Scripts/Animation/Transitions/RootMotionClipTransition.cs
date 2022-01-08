using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    [Serializable]
    public class RootMotionClipTransition : ClipTransition
    {
        [SerializeField] private bool _applyRootMotion;
        
        public override void Apply(AnimancerState state)
        {
            base.Apply(state);
            state.Root.Component.Animator.applyRootMotion = _applyRootMotion;
        }
    }
}
