using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [FoldoutGroup("Climb"), SerializeField]
        private LayerMask _climbableLayers = -1;
        [FoldoutGroup("Climb"), SerializeField]
        private FreeClimbState freeClimbState;
        [FoldoutGroup("Climb"), SerializeField, Range(0, 180)]
        private float _validClimbAngle;
        [FoldoutGroup("Climb"), SerializeField]
        private float _attachThreshold = 0.05f;
        [FoldoutGroup("Climb"), SerializeField]
        private float _reattachDelay = 0.5f;
        [FoldoutGroup("Climb"), SerializeField]
        private float _climbSpeed;
        [FoldoutGroup("Climb"), SerializeField]
        private float _wallCheckDistance = 0.1f;
        [FoldoutGroup("Climb"), SerializeField]
        private float _distanceFromWall = 0.25f;
        [FoldoutGroup("Climb"), SerializeField] 
        private CharacterSize _climbSize;
        [FoldoutGroup("Climb"), SerializeField]
        private InputActionProperty _cancelClimbAction;

        private bool _isDetached;
        private float _timeSinceLastDetach;

        private void ProcessClimb()
        {
            ClimbMove();

            if (HasClimbStarted(out RaycastHit hit))
            {
                CurrentState = PlayerState.Climb | PlayerState.Free;

                TrySetState(freeClimbState);

                //Set starting position
                ModifyCharacterSize(_climbSize);
                CharacterController.Warp(hit.point + (hit.normal * _distanceFromWall));

                //Reset velocity
                Velocity = Vector3.zero;
                return;
            }

            if (HasClimbEnded())
            {
                CurrentState = PlayerState.Move;
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

        public RaycastHit Hit { get; private set; }

        private MoveInfoData moveInfo;

        private void ClimbMove()
        {
            if (!CurrentState.HasFlag(PlayerState.Climb | PlayerState.Free)) return;

            Physics.Raycast(CharacterTopHemisphere(CharacterController.height), transform.forward, out var hit,
                _wallCheckDistance * 2,
                _climbableLayers);

            Hit = hit;

            moveInfo = GetMoveInfo();

            if (CanClimbUp())
            {
                freeClimbState.ClimbUp();
            }

            var wallMoveDirection = Quaternion.LookRotation(-Hit.normal, Vector3.up) * MoveInput;
            var direction = new Vector3
            {
                x = moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Left) ||
                    moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Right)
                    ? wallMoveDirection.x
                    : 0,
                y = moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Up) ||
                    moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Down)
                    ? MoveInput.y
                    : 0,
                z = moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Left) ||
                    moveInfo.MoveDirections.HasFlag(MoveInfoFlags.Right)
                    ? wallMoveDirection.z
                    : 0
            };

            freeClimbState.Value = Vector2.Lerp(freeClimbState.Value, MoveInput, Time.deltaTime * 10);

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

            return !CurrentState.HasFlag(PlayerState.Free) &&
                   isHit &&
                   MoveDirection.magnitude > InputSystem.settings.defaultDeadzoneMin &&
                   angle <= _validClimbAngle;
        }

        private bool HasClimbEnded()
        {
            return CurrentState.HasFlag(PlayerState.Free) &&
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
