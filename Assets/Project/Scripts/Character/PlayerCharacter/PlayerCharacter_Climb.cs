using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Climb")] 
        [SerializeField] private LayerMask _climbableLayers = -1;
        [SerializeField] private MixerState _climbState;
        [SerializeField, Range(0, 180)] private float _validClimbAngle;
        [SerializeField] private float _attachThreshold = 0.05f;
        [SerializeField] private float _reattachDelay = 0.5f;
        [SerializeField] private float _climbSpeed;
        [SerializeField] private CharacterSize _climbSize;
        [SerializeField] private InputActionProperty _cancelClimbAction;
        
        public bool IsClimbing { get; private set; }
        public Climbable CurrentClimbable { get; set; }

        private bool _isDetached;
        private float _timeSinceLastDetach;
        
        private void ProcessClimb()
        {
            ClimbMove();
            
            if (HasClimbStarted())
            {
                IsClimbing = true;
            
                CurrentClimbable.IsBeingClimbed = true;
                TrySetState(_climbState);

                //CharacterController.enabled = false;

                //Look at the climbable
                // Vector3 lookVector = _currentClimbable.transform.position - transform.position;
                // transform.rotation = Quaternion.LookRotation(lookVector, transform.up);
            
                //Set starting position
                ModifyCharacterSize(_climbSize);
                CharacterController.Warp(CurrentClimbable.StartPoint);
            
                //Reset velocity
                Velocity = Vector3.zero;
                return;
            }
            
            if (HasClimbEnded())
            {
                IsClimbing = false;
            
                CurrentClimbable.IsBeingClimbed = false;
            
                ModifyCharacterSize(_defaultSize);
                TrySetState(_moveState);

                _isDetached = true;
                _timeSinceLastDetach = Time.time;
            }
        }

        private void ClimbMove()
        {
            
        }

        private bool HasClimbStarted()
        {
            if (!CurrentClimbable) return false;
            if (_isDetached && Time.time - _timeSinceLastDetach < _reattachDelay) return false;
            
            var angle = Vector3.Angle(-transform.forward, CurrentClimbable.Forward);
            
            return !IsClimbing &&
                   MoveDirection.magnitude > InputSystem.settings.defaultDeadzoneMin &&
                   angle <= _validClimbAngle &&
                   (CurrentClimbable.CanAttachFromGround || (!IsGrounded && Mathf.Abs(transform.position.y - CurrentClimbable.StartPoint.y) <= _attachThreshold));
        }
        
        private bool HasClimbEnded()
        {
            return IsClimbing && 
                   _cancelClimbAction.action.triggered;
        }
    }
}
