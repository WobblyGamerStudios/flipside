using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Climb")] 
        [SerializeField] private LayerMask _climbableLayers = -1;
        [SerializeField] private ClimbState _climbState;
        [SerializeField, Range(0, 180)] private float _validClimbAngle;
        [SerializeField] private float _attachThreshold = 0.05f;
        [SerializeField] private float _reattachDelay = 0.5f;
        [SerializeField] private float _climbSpeed;
        [SerializeField] private float _wallCheckDistance = 0.1f;
        [SerializeField] private float _distanceFromWall = 0.25f;
        [SerializeField] private CharacterSize _climbSize;
        [SerializeField] private InputActionProperty _cancelClimbAction;
        
        public bool IsClimbing { get; private set; }
        public ClimbableV2 CurrentClimbable { get; set; }

        private bool _isDetached;
        private float _timeSinceLastDetach;
        
        private void ProcessClimb()
        {
            ClimbMove();

            if (HasClimbStarted(out RaycastHit hit))
            {
                IsClimbing = true;
            
                TrySetState(_climbState);
            
                //Set starting position
                ModifyCharacterSize(_climbSize);
                CharacterController.Warp(hit.point + (hit.normal * _distanceFromWall));
            
                //Reset velocity
                Velocity = Vector3.zero;
                return;
            }
            
            if (HasClimbEnded())
            {
                IsClimbing = false;
            
                ModifyCharacterSize(_defaultSize);
                TrySetState(_moveState);

                Vector3 rot = transform.eulerAngles;
                rot.x = 0;
                rot.z = 0;
                transform.eulerAngles = rot;

                _isDetached = true;
                _timeSinceLastDetach = Time.time;
            }
        }

        public RaycastHit Hit;

        public MoveInfoData moveInfo;
        private void ClimbMove()
        {
            if (!IsClimbing) return;
            
            Physics.Raycast(CharacterTopHemisphere(CharacterController.height), transform.forward, out Hit, _wallCheckDistance * 2,
                _climbableLayers);

            moveInfo = GetMoveInfo();

            if (CanClimbUp())
            {
                _climbState.ClimbUp();
            }

            var wallMoveDirection = Quaternion.LookRotation(-Hit.normal, Vector3.up) * MoveInput;
            var direction = new Vector3
            {
                x = moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Left) || moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Right) ? wallMoveDirection.x : 0,
                y = moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Up) || moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Down) ? MoveInput.y : 0,
                z = moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Left) || moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Right) ? wallMoveDirection.z : 0
            };

            _climbState.Value = Vector2.Lerp(_climbState.Value, MoveInput, Time.deltaTime * 10);
            
            Velocity = direction * _climbSpeed;
        }

        private bool HasClimbStarted(out RaycastHit hit)
        {
            hit = default;
            
            if (_isDetached && Time.time - _timeSinceLastDetach < _reattachDelay) return false;
            
            bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, _wallCheckDistance,
                _climbableLayers);
            
            // if (isHit) Debug.DrawRay(transform.position, transform.forward * _wallCheckDistance, Color.green);
            // else Debug.DrawRay(transform.position, transform.forward * _wallCheckDistance, Color.red);

            var angle = Vector3.Angle(-transform.forward, hit.normal);
            
            return !IsClimbing &&
                   isHit &&
                   MoveDirection.magnitude > InputSystem.settings.defaultDeadzoneMin &&
                   angle <= _validClimbAngle;
        }
        
        private bool HasClimbEnded()
        {
            return IsClimbing && 
                   _cancelClimbAction.action.triggered;
        }

        private bool CanClimbUp()
        {
            Debug.DrawRay(CharacterTop(), transform.forward * _wallCheckDistance, Color.magenta);
            var origin = CharacterTop() + (transform.up * 0.1f) + transform.forward * (CharacterController.radius * 2);
            
            
            
            return false;
        }

        private MoveInfoData GetMoveInfo()
        {
            MoveInfoData data = default;
            data.HitInfos = new Dictionary<MoveInfoFlags, RaycastHit>();
            
            float distance = _wallCheckDistance * 2;
            var offset = Vector3.zero;
            
            if (MoveInput.x > InputSystem.settings.defaultDeadzoneMin)
            {
                offset.x += CharacterController.radius;

                if (Physics.Raycast(CharacterCenter() + offset, transform.forward, out var hit,
                    distance, _climbableLayers))
                {
                    data.MoveDirections |= MoveInfoFlags.Right;
                    data.HitInfos[MoveInfoFlags.Right] = hit;
                    
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.red);
                }
                
                offset.x = 0;
            }
            
            if (MoveInput.x < -InputSystem.settings.defaultDeadzoneMin)
            {
                offset.x -= CharacterController.radius;
                
                if (Physics.Raycast(CharacterCenter() + offset, transform.forward, out var hit,
                    distance, _climbableLayers))
                {
                    data.MoveDirections |= MoveInfoFlags.Left;
                    data.HitInfos[MoveInfoFlags.Left] = hit;
                    
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.red);
                }
                
                offset.x = 0;
            }
            
            if (MoveInput.y > InputSystem.settings.defaultDeadzoneMin)
            {
                offset.y += CharacterController.height * 0.5f;
                
                if (Physics.Raycast(CharacterCenter() + offset, transform.forward, out var hit,
                    distance, _climbableLayers))
                {
                    data.MoveDirections |= MoveInfoFlags.Up;
                    data.HitInfos[MoveInfoFlags.Up] = hit;
                    
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.red);
                }
                offset.y = 0;
            }
            
            if (MoveInput.y < -InputSystem.settings.defaultDeadzoneMin)
            {
                offset.y -= CharacterController.height * 0.5f;
                
                if (Physics.Raycast(CharacterCenter() + offset, transform.forward, out var hit,
                    distance, _climbableLayers))
                {
                    data.MoveDirections |= MoveInfoFlags.Down;
                    data.HitInfos[MoveInfoFlags.Down] = hit;
                    
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(CharacterCenter() + offset, transform.forward * distance, Color.red);
                }
                
                offset.y = 0;
            }
            
            return data;
        }
        
        [Flags]
        public enum MoveInfoFlags
        {
            Left = 1 << 0, 
            Right = 1 << 1,
            Up = 1 << 2,
            Down = 1 << 3
        }
        
        [Serializable]
        public struct MoveInfoData
        {
            public MoveInfoFlags MoveDirections;
            public Dictionary<MoveInfoFlags, RaycastHit> HitInfos;
        }
    }
}
