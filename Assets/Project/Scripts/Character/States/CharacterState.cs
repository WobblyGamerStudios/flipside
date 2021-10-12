using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Wgs.FlipSide
{
    public abstract class CharacterState : MonoBehaviour, IState
    {
        public bool IsInitialized { get; private set; }
        public bool IsActive { get; protected set; }

        protected AnimancerComponent _animancer;
        
        public virtual void Initialize(AnimancerComponent animancer)
        {
            _animancer = animancer;
            IsInitialized = true;
        }

        #region IState
        
        public virtual bool CanEnterState => true;
        public virtual bool CanExitState => true;

        public virtual void OnEnterState()
        {
            IsActive = true;
        }

        public virtual void OnExitState()
        {
            IsActive = false;
        }
        
        #endregion IState

    }
}
