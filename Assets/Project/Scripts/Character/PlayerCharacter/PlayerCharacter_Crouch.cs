using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string CROUCH = "Crouch";
        private const string SLIDE = "Slide";
        
        [FoldoutGroup(CROUCH), SerializeField] private LinearState _crouchState;
        [FoldoutGroup(CROUCH), SerializeField] private float _crouchSpeed;
        [FoldoutGroup(CROUCH), SerializeField] private CharacterSize _crouchSize;
        [FoldoutGroup(CROUCH), SerializeField] private InputActionProperty _crouchAction;
        
        [FoldoutGroup(SLIDE), SerializeField] private ClipState _slideState;
        [FoldoutGroup(SLIDE), SerializeField] private float _minSlideThreshold;
        [FoldoutGroup(SLIDE), SerializeField] private CharacterSize _slideSize;

        private bool _isCrouching => _crouchType == CrouchType.Crouch;
        private bool _isSliding => _crouchType == CrouchType.Slide;
        private CrouchType _crouchType;
        private float _slopeAngle;
        private float _slideStartTime;
        
        private void InitializeCrouch()
        {
            _crouchState.Initialize(Animancer);
            _slideState.Initialize(Animancer);
        }
        
        private void ProcessCrouch()
        {
            _crouchState.Value = MoveDirection.magnitude;
            
            if (HasCrouchStarted())
            {
                ApplyCrouch(_isSprinting || _wasSprinting ? CrouchType.Slide : CrouchType.Crouch);
                return;
            }
            
            if (HasCrouchEnded())
            {
                ApplyCrouch(CrouchType.None);
                return;
            }

            if (_crouchType != CrouchType.Slide) return;
            
            _slopeAngle = Vector3.Angle(GroundNormal, transform.forward);
                
            if (_slopeAngle > 90 + _minSlideThreshold)
            {
                SlideComplete();
            }
        }

        private bool HasCrouchStarted()
        {
            return _crouchType == CrouchType.None &&
                   IsGrounded && 
                   IsCrouchAbovePressPoint();
        }

        private bool HasCrouchEnded()
        {
            return _crouchType != CrouchType.None &&
                   (!IsGrounded ||
                    !IsCrouchAbovePressPoint());
        }

        private bool IsCrouchAbovePressPoint()
        {
            return _crouchAction.action.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;
        }

        private void ApplyCrouch(CrouchType type)
        {
            _crouchType = type;
            
            switch (_crouchType)
            {
                case CrouchType.Crouch:
                    ModifyCharacterSize(_crouchSize);
                    TrySetState(_crouchState);
                    break;
                case CrouchType.Slide:
                    _slideStartTime = Time.time;
                    ModifyCharacterSize(_slideSize);
                    TrySetState(_slideState);
                    break;
                case CrouchType.None:
                    ModifyCharacterSize(_defaultSize);
                    if (State == State.Falling) return;
                    TrySetState(_moveState);
                    break;
            }
        }

        public void SlideComplete()
        {
            ApplyCrouch(IsCrouchAbovePressPoint() ? CrouchType.Crouch : CrouchType.None);
        }

        public enum CrouchType
        {
            None = 0,
            Crouch, 
            Slide
        }
    }
}
