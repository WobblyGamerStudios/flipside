using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Roll")] 
        [SerializeField] private ClipState _rollState;
        [SerializeField] private float _rollSpeed;
        [SerializeField] private CharacterSize _rollSize;
        [SerializeField] private InputActionProperty _rollAction;

        public bool IsRolling { get; private set; }

        private float _rollStartTime;
        
        private void ProcessRoll()
        {
            if (HasRollStarted())
            {
                IsRolling = true;
                _rollStartTime = Time.time;
                IsSprinting = false;
                ModifyCharacterSize(_rollSize);
                TrySetState(_rollState);
            }
        }

        private bool HasRollStarted()
        {
            return !IsRolling &&
                   !IsSliding &&
                   !IsClimbing &&
                   IsGrounded &&
                   MoveDirection.magnitude > InputSystem.settings.defaultDeadzoneMin &&
                   _rollAction.action.triggered;
        }

        public void RollComplete()
        {
            IsRolling = false;
            ModifyCharacterSize(_defaultSize);
            TrySetState(_moveState);
        }
    }
}
