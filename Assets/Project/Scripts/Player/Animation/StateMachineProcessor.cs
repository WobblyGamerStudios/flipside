using Animancer;
using Animancer.FSM;
using UnityEngine;
using Wgs.Locomotion;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(AnimancerComponent))]
    public abstract class StateMachineProcessor : MonoBehaviour
    {
        public readonly StateMachine<AnimationState>.WithDefault StateMachine = new StateMachine<AnimationState>.WithDefault();
        
        [SerializeField] private CharacterLocomotion _characterLocomotion;
        public CharacterLocomotion CharacterLocomotion => _characterLocomotion;
        
        public AnimancerComponent AnimancerComponent { get; private set; }
        
        protected virtual void OnValidate()
        {
            if (!AnimancerComponent) AnimancerComponent = GetComponent<AnimancerComponent>();
        }

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            //AnimancerComponent.Layers[0].Play(_defaultClip);
            
            Begin();
        }

        private void Update()
        {
            Process();
        }

        protected abstract void Initialize();

        protected abstract void Begin();

        protected abstract void Process();
    }
}
