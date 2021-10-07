using Animancer.FSM;
using UnityEngine;

namespace Wgs.FlipSide
{
    public abstract class AnimationState : MonoBehaviour, IState
    {
        public bool IsInitialized { get; private set; }
        public bool IsActive { get; protected set; }

        protected StateMachineProcessor _stateMachineProcessor;

        public virtual void Initialize(StateMachineProcessor processor)
        {
            _stateMachineProcessor = processor;
            IsInitialized = true;
        }

        public abstract void Tick();
        
        #region IState
        
        public virtual bool CanEnterState => true;
        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            if (!_stateMachineProcessor) return;
            IsActive = true;
        }

        public virtual void OnExitState()
        {
            if (!_stateMachineProcessor) return;
            IsActive = false;
        }
        
        #endregion IState

    }
}
