using System;
using System.Collections;
using System.Collections.Generic;
using Animancer.FSM;
using Sirenix.OdinInspector;
using UnityEngine;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
    public class FlipSideStateMachineProcessor : StateMachineProcessor
    {
        private const string LOG_FORMAT = nameof(FlipSideStateMachineProcessor) + ".{0} :: {1}";
        
        [SerializeField] private FlipSideLinearState _moveState;
        [SerializeField] private FlipSideLinearState _crouchState;
        [SerializeField] private ClipTransitionState _sprintState;

        public new ThirdPersonCharacterLocomotion CharacterLocomotion => base.CharacterLocomotion as ThirdPersonCharacterLocomotion;

        private CrouchDecorator _crouchDecorator;
        private SprintDecorator _sprintDecorator;
        
        protected override void Initialize()
        {
            _moveState.Initialize(this);
            _crouchState.Initialize(this);
            _sprintState.Initialize(this);
        }

        protected override void Begin()
        {
            StateMachine.DefaultState = _moveState;
            
            if (!CharacterLocomotion.TryGetDecorator(out _crouchDecorator)) Debug.LogErrorFormat(LOG_FORMAT, nameof(Begin), $"Could not find {nameof(CrouchDecorator)}");
            if (!CharacterLocomotion.TryGetDecorator(out _sprintDecorator)) Debug.LogErrorFormat(LOG_FORMAT, nameof(Begin), $"Could not find {nameof(SprintDecorator)}");
            
            Set(FlipSideAnimationType.Default);
        }

        protected override void Process()
        {
            _moveState.Tick();
            _crouchState.Tick();
            _sprintState.Tick();

            if (StateMachine.CurrentState != _crouchState && _crouchDecorator.IsCrouching)
            {
                Set(FlipSideAnimationType.Crouch);
            }

            if (StateMachine.CurrentState != _sprintState && _sprintDecorator.IsSprinting)
            {
                Set(FlipSideAnimationType.Sprint);
            }
            
            if (StateMachine.CurrentState != _moveState && 
                !_crouchDecorator.IsCrouching &&
                !_sprintDecorator.IsSprinting)
            {
                Set(FlipSideAnimationType.Default);
            }
        }

        [Button]
        private void Set(FlipSideAnimationType type)
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
                    StateMachine.ForceSetState(_sprintState);
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
