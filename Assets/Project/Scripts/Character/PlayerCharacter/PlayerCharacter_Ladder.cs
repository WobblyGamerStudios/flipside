using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    // public partial class PlayerCharacter : Character
    // {
    //     private const string GROUP = "Ladder";
    //     private const float LADDER_DETACH_DELAY = 0.4f;
    //     
    //     [FoldoutGroup(GROUP), LabelText("ClimbState"), SerializeField]
    //     private LadderClimbState _ladderClimbState;
    //     [FoldoutGroup(GROUP), LabelText("LayerMask"), SerializeField] 
    //     private LayerMask _ladderLayerMask;
    //     [FoldoutGroup(GROUP), LabelText("MinDistance"), SerializeField]
    //     private float _minLadderDistance = 0.3f;
    //     [FoldoutGroup(GROUP), LabelText("MinHeight"), SerializeField]
    //     private float _minLadderHeight = 0.1f;
    //     [FoldoutGroup(GROUP), LabelText("CheckAngle"), SerializeField, Range(0, 180)]
    //     private float _ladderCheckAngle = 30;
    //     [FoldoutGroup(GROUP), LabelText("ClimbSpeed"), SerializeField]
    //     private float _ladderClimbSpeed = 1;
    //     [FoldoutGroup(GROUP), LabelText("EndClimbSpeed"), SerializeField]
    //     private float _ladderEndClimbSpeed = 1.5f;
    //     [FoldoutGroup(GROUP), LabelText("CharacterSize"), SerializeField]
    //     private CharacterSize _ladderCharacterSize;
    //     
    //     private Ladder _currentLadder;
    //     private float _ladderDetachTime;
    //     private Vector3 _destination;
    //     
    //     private void ProcessLadderClimb()
    //     {
    //         if (CurrentState.HasFlag(PlayerState.Ladder))
    //         {
    //             transform.position += transform.up * (MoveInput.y * (Time.deltaTime * _ladderClimbSpeed));
    //             _ladderClimbState.Value = MoveInput.y;
    //         }
    //         else if(CurrentState.HasFlag(PlayerState.EndClimb))
    //         {
    //             transform.position = Vector3.MoveTowards(transform.position, _destination,  Time.deltaTime * _ladderEndClimbSpeed);
    //         }
    //
    //         if (HasLadderClimbStarted())
    //         {
    //             _forwardLook = _currentLadder.Forward;
    //
    //             var startPoint = _currentLadder.Position;
    //             startPoint.y = transform.position.y + _minLadderHeight;
    //             startPoint.z += (-_forwardLook * _minLadderDistance).z;
    //
    //             _currentLadder.IsClimbing = true;
    //             TrySetState(_ladderClimbState);
    //             ModifyCharacterSize(_ladderCharacterSize);
    //             CharacterController.Warp(startPoint);
    //             CharacterController.enabled = false;
    //             
    //             CurrentState = PlayerState.Climb | PlayerState.Ladder;
    //             
    //             Velocity = Vector3.zero;
    //             
    //             return;
    //         }
    //
    //         if (CurrentState.HasFlag(PlayerState.Climb | PlayerState.EndClimb)) return;
    //
    //         if (HasLadderClimbEnded())
    //         {
    //             if (CurrentState.HasFlag(PlayerState.EndClimb))
    //             {
    //                 _destination = CharacterPosition() + transform.forward * CharacterController.radius;
    //                 _ladderClimbState.LadderClimbFinish();
    //                 CharacterController.detectCollisions = false;
    //                 return;
    //             }
    //             
    //             CharacterController.enabled = true;
    //             _ladderDetachTime = Time.time;
    //             CurrentState = PlayerState.Move;
    //             ModifyCharacterSize(_defaultSize);
    //             TrySetState(_moveState);
    //             
    //             _currentLadder.IsClimbing = false;
    //             _currentLadder = null;
    //         }
    //     }
    //
    //     private bool HasLadderClimbStarted()
    //     {
    //         if (CurrentState.HasFlag(PlayerState.Climb)) return false;
    //         if (MoveInput.magnitude < InputSystem.settings.defaultDeadzoneMin) return false;
    //         if (Time.time - _ladderDetachTime < LADDER_DETACH_DELAY) return false;
    //         if (!Physics.Raycast(CharacterPosition(_minLadderHeight), transform.forward, out var hit, _minLadderDistance,
    //             _ladderLayerMask)) return false;
    //
    //         _currentLadder = hit.transform.GetComponentInParent<Ladder>();
    //         if (!_currentLadder) return false;
    //         
    //         var angle = Vector3.Angle(-transform.forward, hit.normal);
    //         return angle <= _ladderCheckAngle;
    //     }
    //     
    //     private bool HasLadderClimbEnded()
    //     {
    //         if (!CurrentState.HasFlag(PlayerState.Ladder)) return false;
    //
    //         var top = CharacterPosition(CharacterController.height * 0.5f);
    //         top.y -= 0.05f;
    //         if (!Physics.Raycast(top, transform.forward, _minLadderDistance, _ladderLayerMask))
    //         {
    //             CurrentState = PlayerState.Climb | PlayerState.EndClimb | PlayerState.IgnoreFall;
    //             return true;
    //         }
    //         
    //         return  _rollAction.action.triggered ||
    //                 Physics.Raycast(transform.position, Vector3.down, _minLadderHeight);
    //     }
    //
    //     public void LadderClimbComplete()
    //     {
    //         CurrentState = PlayerState.Move;
    //         CharacterController.enabled = true;
    //         CharacterController.detectCollisions = true;
    //         ModifyCharacterSize(_defaultSize);
    //         TrySetState(_moveState);
    //         
    //         _currentLadder.IsClimbing = false;
    //         _currentLadder = null;
    //     }
    // }
}
