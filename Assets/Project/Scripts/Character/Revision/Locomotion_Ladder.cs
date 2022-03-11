using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public enum LadderClimbState
    {
        Attaching,
        Climbing,
        Detaching
    }
    
    public partial class Locomotion 
    {
        [Title("Ladder")] 
        [SerializeField] private LayerMask _ladderLayer;
        [SerializeField] private float _climbLadderSpeed = 3;
        [SerializeField] private float _attachDetachDuration = 0.25f;
        [SerializeField] private InputActionReference _dropAction;
        
        private Climbable_Old _activeClimbableOld;
        private LadderClimbState _ladderClimbState;
        private Vector3 _ladderTargetPosition;
        private Quaternion _ladderTargetRotation;
        private Vector3 _attachDetachPosition;
        private Quaternion _attachDetachRotation;
        private Quaternion _rotationBeforeClimbingLadder;
        private float _attachDetachTimer;

        private void UpdateLadderRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (_ladderClimbState)
            {
                case LadderClimbState.Climbing:
                    currentRotation = Quaternion.Inverse(_activeClimbableOld.transform.rotation);
                    break;
                case LadderClimbState.Attaching:
                case LadderClimbState.Detaching:
                    currentRotation = Quaternion.Slerp(_attachDetachRotation, _ladderTargetRotation, _attachDetachTimer / _attachDetachDuration);
                    break;
            }
        }

        private void UpdateLadderVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            currentVelocity = Vector3.zero;

            switch (_ladderClimbState)
            {
                case LadderClimbState.Climbing:
                    currentVelocity = (MoveInput.y * _activeClimbableOld.transform.up).normalized * _climbLadderSpeed;
                    break;
                case LadderClimbState.Attaching:
                case LadderClimbState.Detaching:
                    Vector3 tmpPosition = Vector3.Lerp(_attachDetachPosition, _ladderTargetPosition, (_attachDetachTimer / _attachDetachDuration));
                    currentVelocity = _motor.GetVelocityForMovePosition(_motor.TransientPosition, tmpPosition, deltaTime);
                    break;
            }
        }

        private void PreUpdateClimbing(float deltaTime)
        {
            //Find nearby climbables
        }

        private void PostUpdateClimbing(float deltaTime)
        {
            switch (_ladderClimbState)
            {
                case LadderClimbState.Climbing:
                    _activeClimbableOld.ClosestPointOnClimbable(_motor.TransientPosition, out var pointResult);

                    SetClimbState(LadderClimbState.Detaching);

                    switch (pointResult)
                    {
                        case PointResult.IsWithin:
                            break;
                    }
                    
                    // if (pointResult > 0)
                    // {
                    //     _ladderTargetPosition = _activeClimbable.TopReleasePoint.position;
                    //     _ladderTargetRotation = _activeClimbable.TopReleasePoint.rotation;
                    // }
                    // else if (pointResult < 0)
                    // {
                    //     _ladderTargetPosition = _activeClimbable.BottomReleasePoint.position;
                    //     _ladderTargetRotation = _activeClimbable.BottomReleasePoint.rotation;
                    // }

                    break;
                case LadderClimbState.Attaching:
                case LadderClimbState.Detaching:
                    if (_attachDetachTimer >= _attachDetachDuration)
                    {
                        if (_ladderClimbState == LadderClimbState.Attaching)
                        {
                            SetClimbState(LadderClimbState.Climbing);
                        }
                        else if (_ladderClimbState == LadderClimbState.Detaching)
                        {
                            StopClimbingLadder();
                        }
                    }

                    _attachDetachTimer += deltaTime;
                    break;
            }
        }

        private void CheckForLadder(Collider collider)
        {
            if (_ladderLayer != (_ladderLayer | 1 << collider.gameObject.layer)) return;

            var climbable = collider.gameObject.GetComponent<Climbable_Old>();
            if (!climbable) return;

            StartClimbingLadder(climbable);
        }

        private void StartClimbingLadder(Climbable_Old climbableOld)
        {
            _activeClimbableOld = climbableOld;
            _rotationBeforeClimbingLadder = _motor.TransientRotation;
            
            _motor.SetMovementCollisionsSolvingActivation(false);
            _motor.SetGroundSolvingActivation(false);
            SetClimbState(LadderClimbState.Attaching);

            _ladderTargetPosition = _activeClimbableOld.ClosestPointOnClimbable(_motor.TransientPosition, out var pointResult);
            _ladderTargetRotation = Quaternion.Inverse(_activeClimbableOld.transform.rotation);

            _locomotionState = LocomotionState.Climbing;
        }

        private void StopClimbingLadder()
        {
            if (!_activeClimbableOld) return;

            _motor.SetMovementCollisionsSolvingActivation(true);
            _motor.SetGroundSolvingActivation(true);
            SetClimbState(LadderClimbState.Detaching);
            _ladderTargetPosition = _motor.TransientPosition;
            _ladderTargetRotation = _rotationBeforeClimbingLadder;
            
            _locomotionState = LocomotionState.Default;
        }

        private void SetClimbState(LadderClimbState state)
        {
            _ladderClimbState = state;
            _attachDetachTimer = 0;
            _attachDetachPosition = _motor.TransientPosition;
            _attachDetachRotation = _motor.TransientRotation;
        }
    }
}
