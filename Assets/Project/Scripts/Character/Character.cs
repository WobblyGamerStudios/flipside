using System;
using Animancer;
using Animancer.FSM;
using UnityEngine;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(AnimancerComponent), typeof(CharacterController))]
    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected LayerMask _traversableLayers = -1;
        [SerializeField] protected float _checkDistance = 0.1f;
        
        public AnimancerComponent Animancer { get; protected set; }
        public StateMachine<CharacterState>.WithDefault StateMachine { get; } = new StateMachine<CharacterState>.WithDefault();
        public CharacterController CharacterController { get; protected set; }
        public bool IsGrounded { get; protected set; }
        public Vector3 GroundNormal { get; protected set; }
        public Vector3 PreviousPosition { get; protected set; }

        protected CharacterSize _defaultSize;
        protected float _lastLostContactTime;
        protected float _lastRegainedContactTime;
        protected bool _isForceDetach;

        #region MonoBehaviour

        protected virtual void OnValidate()
        {
            FindDependencies();
        }

        protected virtual void Awake()
        {
            FindDependencies();

            _defaultSize.Height = CharacterController.height;
        }

        protected virtual void Update()
        {
            var wasGrounded = IsGrounded;
            if (_isForceDetach) IsGrounded = false;
            else CheckGround();
            
            switch (wasGrounded)
            {
                case true when !IsGrounded:
                    LostGroundContact();
                    break;
                case false when IsGrounded:
                    RegainedGroundContact();
                    break;
            }

            _isForceDetach = false;
        }
        
        protected virtual void LateUpdate()
        {
            PreviousPosition = transform.position;
        }
        
        #endregion MonoBehaviour

        protected virtual void FindDependencies()
        {
	        if (!Animancer) Animancer = GetComponent<AnimancerComponent>();
            if (!CharacterController) CharacterController = GetComponent<CharacterController>();
        }

        protected virtual void ModifyCharacterSize(CharacterSize size)
        {
            CharacterController.height = size.Height;
            CharacterController.center = new Vector3 {y = (size.Height * 0.5f) + size.YOffset};
        }
        
        protected virtual void CheckGround()
        {
            if (!CharacterController) return;
			
            float checkDistance = CharacterController.skinWidth + _checkDistance;

            IsGrounded = false;
            GroundNormal = Vector3.up;

            if (!Physics.CapsuleCast(CharacterBottomHemisphere(),
                CharacterTopHemisphere(CharacterController.height),
                CharacterController.radius, Vector3.down, out RaycastHit hit, checkDistance, _traversableLayers,
                QueryTriggerInteraction.Ignore)) return;

            GroundNormal = hit.normal;

            if (Vector3.Dot(hit.normal, transform.up) <= 0 ||
                !IsOnSlope(GroundNormal)) return;

            IsGrounded = true;

            if (hit.distance > CharacterController.skinWidth)
            {
                CharacterController.Move(Vector3.down * hit.distance);
            }
        }
        
        protected virtual void LostGroundContact()
        {
            _lastLostContactTime = Time.time;
            OnLostGroundContactEvent?.Invoke();
        }

        protected virtual void RegainedGroundContact()
        {
            _lastRegainedContactTime = Time.time;
            OnRegainedGroundContactEvent?.Invoke();
        }

        protected void ForceDetach()
        {
            _isForceDetach = true;
        }
        
        #region Helpers

        public bool IsOnSlope(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= CharacterController.slopeLimit;
        }
        
        public Vector3 CharacterBottomHemisphere()
        {
            return transform.position + transform.up * CharacterController.radius;
        }

        public Vector3 CharacterTopHemisphere(float atHeight)
        {
            return transform.position + transform.up * (atHeight - CharacterController.radius);
        }

        public Vector3 CharacterTop()
        {
            return transform.position + transform.up * CharacterController.height;
        }

        public Vector3 CharacterCenter()
        {
            return transform.position + transform.up * (CharacterController.height * 0.5f);
        }

        #endregion Helpers
        
        #region Events

        public event Action OnLostGroundContactEvent;
        public event Action OnRegainedGroundContactEvent;

        #endregion Events

        public virtual bool TrySetState(CharacterState state) => StateMachine.TrySetState(state);
    }

    [Serializable]
    public struct CharacterSize
    {
        public float Height;
        public float YOffset;
    }
}
