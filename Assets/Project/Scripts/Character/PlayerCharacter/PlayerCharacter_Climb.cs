using System;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Wgs.Core;

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
        private LedgeClimbState _ledgeClimbState;
        [FoldoutGroup(CLIMB), SerializeField] 
        private float _ledgeClimbSpeed;
        [FoldoutGroup(CLIMB), SerializeField] 
        private float _distAboveHead;
        [FoldoutGroup(CLIMB), SerializeField] 
        private float _distInFront;
        [FoldoutGroup(CLIMB), SerializeField] 
        private float _minClimbDist;
        [FoldoutGroup(CLIMB), SerializeField] 
        private CharacterSize _climbSize;
        
        [Title("Input")]
        [FoldoutGroup(CLIMB), SerializeField] 
        private InputActionProperty _dropAction;

        private Vector3 _bracedPoint;
        private Vector3 _freeHangPoint;

        private void InitializeClimb()
        {
            _ledgeClimbState.Initialize(Animancer);
        }
        
        private void ProcessClimb()
        {
            if (State != State.Climbing && CanAttachToLedge())
            {
                State = State.Climbing;
                _lastLostContactTime = Time.time;
                Velocity = Vector3.zero;
                ModifyCharacterSize(_climbSize);
                CharacterController.Warp(_bracedPoint);
                CharacterController.enabled = false;
                TrySetState(_ledgeClimbState);
            }

            if (State == State.Climbing)
            {
                //Check if we can move left/right
                //Check if we can jump left/Right
                //Check if we can drop down
                //Check if we can jump up
                //Check if we can climb up
             
                //Move character
                transform.position =
                    Vector3.MoveTowards(transform.position, Vector3.Lerp(_bracedPoint, _freeHangPoint, _ledgeClimbState.BracedToFreeHangBlend), Time.deltaTime * _ledgeClimbSpeed);
                
                _ledgeClimbState.Value = Mathf.Abs(MoveInput.x) > InputSystem.settings.defaultDeadzoneMin ? MoveInput.x : 0;
                
                if (_dropAction.action.triggered)
                {
                    State = State.Ground;
                    TrySetState(_moveState);
                    ModifyCharacterSize(_defaultSize);
                    CharacterController.enabled = true;
                    CheckFall();
                }
            }
        }

        private bool CanAttachToLedge()
        {
            var topOfHead = CharacterPosition(CharacterController.height);
            var aboveHead = topOfHead + Vector3.up * _distAboveHead;
            var inFrontOfHead = aboveHead + transform.forward * (CharacterController.radius + _distInFront);

            Debug.DrawLine(topOfHead, aboveHead);
            Debug.DrawLine(aboveHead, inFrontOfHead);

            var ledgeRay = new Ray(inFrontOfHead, Vector3.down);
            if (!Physics.Raycast(ledgeRay, out var ledgeHit, _distAboveHead * 2, _ledgeLayerMask)) return false;
            
            //Hit ledge, attach
            Debug.Log("Attach to ledge");

            var filter = ledgeHit.transform.GetComponent<MeshFilter>();
            var mesh = filter.mesh;

            var index = ledgeHit.triangleIndex * 3;
            _freeHangPoint = ledgeHit.transform.TransformPoint(mesh.vertices[mesh.triangles[index + 1]]);
            _bracedPoint = _freeHangPoint + (-ledgeHit.transform.forward * 0.25f);

            return true;
        }

        private void DetermineHorizontalStatus(out bool canMoveLeft, out bool canMoveRight, out bool canHopLeft, out bool canHopRight)
        {
            canMoveLeft = default;
            canMoveRight = default;
            canHopLeft = default;
            canHopRight = default;
        }
        
        private void DetermineVerticalStatus(out bool canMoveUp, out bool canDropDown)
        {
            canMoveUp = default;
            canDropDown = default;
        }

        private void DrawClimbGizmos()
        {
            Gizmos.color = CoreColor.Orange;
        }
    }
}
