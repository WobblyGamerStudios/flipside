using System;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string CLIMB = "Climb";

        [FoldoutGroup(CLIMB), SerializeField] 
        private LayerMask _wallLayerMask;
        [FoldoutGroup(CLIMB), SerializeField] 
        private LayerMask _ledgeLayerMask;
        [FoldoutGroup(CLIMB), SerializeField]
        private ClipState _climbState;
        [FoldoutGroup(CLIMB), SerializeField] 
        private float _distAboveHead;
        [FoldoutGroup(CLIMB), SerializeField] 
        private float _wallCheckDist;
        [FoldoutGroup(CLIMB), SerializeField] 
        private float _minClimbDist;
        [FoldoutGroup(CLIMB), SerializeField] 
        private CharacterSize _climbSize;
        
        [Title("Input")]
        [FoldoutGroup(CLIMB), SerializeField] 
        private InputActionProperty _dropAction;

        private void InitializeClimb()
        {
            _climbState.Initialize(Animancer);
        }
        
        private void ProcessClimb()
        {
            if (State != State.Climbing && CanAttachToLedge(out Vector3 attachPoint))
            {
                State = State.Climbing;
                _lastLostContactTime = Time.time;
                Velocity = Vector3.zero;
                ModifyCharacterSize(_climbSize);
                CharacterController.Warp(attachPoint + Vector3.down * CharacterController.height);
                TrySetState(_climbState);
            }

            if (State == State.Climbing && _dropAction.action.triggered)
            {
                State = State.Ground;
                TrySetState(_moveState);
                ModifyCharacterSize(_defaultSize);
                CheckFall();
            }
        }

        private bool CanAttachToLedge(out Vector3 position)
        {
            position = default;
            
            Debug.DrawLine(CharacterPosition(CharacterController.height), CharacterPosition(CharacterController.height + _distAboveHead));

            var wallRay = new Ray(CharacterPosition(CharacterController.height + _distAboveHead), transform.forward);
            if (Physics.Raycast(wallRay, out var wallHit, _wallCheckDist, _wallLayerMask))
            {
                Debug.DrawRay(wallRay.origin, wallRay.direction * _wallCheckDist, Color.green);

                var ledgeRay = new Ray(wallHit.point, Vector3.down);
                if (Physics.Raycast(ledgeRay, out var ledgeHit, _distAboveHead, _ledgeLayerMask))
                {
                    Debug.DrawRay(ledgeRay.origin, ledgeRay.direction * _distAboveHead, Color.green);
                    
                    //Hit ledge, attach
                    Debug.Log("Attach to ledge");
                    position = ledgeHit.point + wallHit.normal * _minClimbDist;
                    return true;
                }
            }
            else
            {
                Debug.DrawRay(wallRay.origin, wallRay.direction * _wallCheckDist);
            }

            return false;
        }

        private bool CanStartClimbing()
        {
            return false;
        }

        private bool HasClimbStarted()
        {
            return false;
        }

        private bool HasClimbEnded()
        {
            return false;
        }
    }
}
