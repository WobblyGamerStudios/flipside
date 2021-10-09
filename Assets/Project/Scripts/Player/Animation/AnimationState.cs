using Animancer.FSM;
using UnityEngine;

namespace Wgs.FlipSide
{
    public abstract class AnimationState : MonoBehaviour, IState
    {
        public bool IsInitialized { get; private set; }
        public bool IsActive { get; protected set; }

        public StateMachineProcessor StateMachineProcessor { get; protected set; }

        public virtual void Initialize(StateMachineProcessor processor)
        {
            StateMachineProcessor = processor;
            IsInitialized = true;
        }

        public abstract void Tick();
        
        #region IState
        
        public virtual bool CanEnterState => true;
        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            if (!StateMachineProcessor) return;
            IsActive = true;
        }

        public virtual void OnExitState()
        {
            if (!StateMachineProcessor) return;
            IsActive = false;
        }
        
        #endregion IState

    }
}
