using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(AnimancerComponent))]
    public class AnimationProcessor : MonoBehaviour
    {
        private AnimancerComponent _animancerComponent;
        
        private void OnValidate()
        {
            if (!_animancerComponent) _animancerComponent = GetComponent<AnimancerComponent>();
        }
    }
}
