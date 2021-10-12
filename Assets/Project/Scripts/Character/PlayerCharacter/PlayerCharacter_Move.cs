using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        public enum GroundedFoot
        {
            Left, 
            Right
        }
        
        [Title("Move")] 
        [SerializeField] private LinearState _moveState;
        [SerializeField] private float _moveSpeed = 3;
        [SerializeField] private float _friction = 10;
        [SerializeField] private float _gravityFactor = 15;
        [SerializeField] private float _airAcceleration = 20;
        [SerializeField] private float _maxAirSpeed = 3;
        [SerializeField] private InputActionProperty _moveAction;
        
        public Vector2 MoveInput { get; private set; }
        public Vector3 MoveDirection { get; private set; }

        private Vector3 _moveInputClamped;
        private GroundedFoot _groundedFoot;
        
        private void ProcessMovement()
        {
            CalculateMoveDirection();

            if (IsGrounded)
            {
                Vector3 targetVelocity = MoveDirection * TraversalSpeed;
                targetVelocity = GetDirectionTangentToSurface(targetVelocity.normalized, GroundNormal) *
                                 targetVelocity.magnitude;

                //Apply ground velocity
                Velocity = Vector3.Lerp(Velocity, targetVelocity, _friction * Time.deltaTime);
            }
            else
            {
                if (!IsJumping)
                {
                    //Apply air movement
                    Velocity += MoveDirection * (_airAcceleration * Time.deltaTime);


                    //Clamp air movement to max speed
                    float verticalVelocity = Velocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(Velocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _maxAirSpeed);
                    Velocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                }

                //Apply gravity
                Velocity += Vector3.down * (_gravityFactor * Time.deltaTime);
            }

            _moveState.Blend = MoveDirection.magnitude;
        }
        
        private void CalculateMoveDirection()
        {
            if (!CharacterController) return;
            
            MoveInput = _moveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            _moveInputClamped = Vector3.ClampMagnitude(new Vector3(MoveInput.x, 0, MoveInput.y), 1);

            var projection = Vector3.ProjectOnPlane(_cameraTransform.rotation * Vector3.forward, transform.up).normalized;

            var targetDirection = Quaternion.LookRotation(projection, transform.up) * _moveInputClamped;
            if (IsSliding || IsRolling)
            {
                MoveDirection = Vector3.Lerp(MoveDirection, targetDirection, 3 * Time.deltaTime);
            }
            else
            {
                MoveDirection = targetDirection;
            }
        }
        
        // Gets a reoriented direction that is tangent to a given slope
        private Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(surfaceNormal, directionRight).normalized;
        }

        private void SetFoot(int index)
        {
            _groundedFoot = (GroundedFoot) index;
        }
    }
}
