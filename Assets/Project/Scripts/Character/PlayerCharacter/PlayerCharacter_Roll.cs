using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string ROLL = "Roll";
        
        [FoldoutGroup(ROLL), SerializeField] private ClipState _rollState;
        [FoldoutGroup(ROLL), SerializeField] private float _rollSpeed;
        [FoldoutGroup(ROLL), SerializeField] private float _rollDurationOffset;
        [FoldoutGroup(ROLL), SerializeField] private CharacterSize _rollSize;
        [FoldoutGroup(ROLL), SerializeField] private InputActionProperty _rollAction;

        private bool _isRolling;
        private float _rollStartTime;
        
        private void InitializeRoll()
        {
            _rollState.Initialize(Animancer);
        }
        
        private void ProcessRoll()
        {
            if (HasRollStarted())
            {
                _isRolling = true;
                _rollStartTime = Time.time;
                ModifyCharacterSize(_rollSize);
                TrySetState(_rollState);
                return;
            }

            if (HasRollEnded())
            {
                _isRolling = false;
                ModifyCharacterSize(_defaultSize);
                TrySetState(_moveState);
            }
        }

        private bool HasRollStarted()
        {
            return !_isRolling &&
                   IsGrounded &&
                   MoveDirection.magnitude > InputSystem.settings.defaultDeadzoneMin &&
                   _rollAction.action.triggered;
        }

        private bool HasRollEnded()
        {
            return _isRolling &&
                   Time.time - _rollStartTime >= (_rollState.ClipTransition.Clip.length - _rollDurationOffset);
        }
    }
}
