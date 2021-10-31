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

        private float _rollStartTime;
        
        private void ProcessRoll()
        {
            if (HasRollStarted())
            {
                CurrentState = PlayerState.Roll | PlayerState.Disabled | PlayerState.IgnoreFall;
                _rollStartTime = Time.time;
                ModifyCharacterSize(_rollSize);
                TrySetState(_rollState);
            }
        }

        private bool HasRollStarted()
        {
            return (CurrentState == PlayerState.Move || CurrentState == PlayerState.Sprint) &&
                   IsGrounded &&
                   MoveDirection.magnitude > InputSystem.settings.defaultDeadzoneMin &&
                   _rollAction.action.triggered;
        }

        public void RollComplete()
        {
            CurrentState = PlayerState.Move;
            ModifyCharacterSize(_defaultSize);
            TrySetState(_moveState);
        }
    }
}
