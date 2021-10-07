using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
    public class FlipSideStateMachineProcessor : StateMachineProcessor
    {
        [SerializeField] private LinearState _moveState;
        [SerializeField] private LinearState _crouchState;

        protected override void Initialize()
        {
            _moveState.Initialize(this);
            _crouchState.Initialize(this);
        }

        protected override void Begin()
        {
            StateMachine.DefaultState = _moveState;
            
            SetDefault(FlipSideAnimationType.Default);
        }

        protected override void Process()
        {
            _moveState.Tick();
            _crouchState.Tick();
        }

        [Button]
        private void SetDefault(FlipSideAnimationType type)
        {
            switch (type)
            {
                case FlipSideAnimationType.Default:
                    StateMachine.ForceSetState(_moveState);
                    break;
                case FlipSideAnimationType.Crouch:
                    StateMachine.ForceSetState(_crouchState);
                    break;
                case FlipSideAnimationType.Sprint:
                    break;
            }
        }
    }

    public enum FlipSideAnimationType
    {
        Default = 0,
        Crouch,
        Sprint
    }
}
