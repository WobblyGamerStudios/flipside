using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(AnimancerComponent))]
    public abstract class Character : MonoBehaviour
    {
        public AnimancerComponent Animancer { get; protected set; }
        public StateMachine<CharacterState>.WithDefault StateMachine = new StateMachine<CharacterState>.WithDefault();
        
        protected virtual void OnValidate()
        {
            FindDependencies();
        }

        protected virtual void Awake()
        {
            FindDependencies();
        }

        protected virtual void FindDependencies()
        {
	        if (!Animancer) Animancer = GetComponent<AnimancerComponent>();
        }

        public virtual bool TrySetState(CharacterState state) => StateMachine.TrySetState(state);
    }
}
