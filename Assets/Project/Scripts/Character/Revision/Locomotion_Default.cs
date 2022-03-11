using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class Locomotion
    {
        [Title("Movement")]
        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private float _friction = 20;
        [SerializeField] private float _lookSpeed = 20;
        
        [Title("Air Control")]
        [SerializeField] private float _airAcceleration = 20;
        [SerializeField] private float _maxAirSpeed = 3;

        [Title("Jumping")]
        [SerializeField] private int _maxJumpCount = 1;
        [SerializeField] private float _jumpPower = 10;
        [SerializeField] private LayerMask _wallJumpLayer;
        [SerializeField] private float _postWallJumpDelay = 0.1f;

        private void UpdateDefaultRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (MoveDirection == Vector3.zero) return;
            
            currentRotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(MoveDirection, transform.up), deltaTime * _lookSpeed);
        }

        private void UpdateDefaultVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 targetVelocity;
            if (IsGrounded)
            {
                var currentVelocityMagnitude = currentVelocity.magnitude;
                var effectiveGroundNormal = _motor.GroundingStatus.GroundNormal;
                if (currentVelocityMagnitude > 0f && _motor.GroundingStatus.SnappingPrevented)
                {
                    // Take the normal from where we're coming from
                    var groundPointToCharacter = _motor.TransientPosition - _motor.GroundingStatus.GroundPoint;
                    if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                    {
                        effectiveGroundNormal = _motor.GroundingStatus.OuterGroundNormal;
                    }
                    else
                    {
                        effectiveGroundNormal = _motor.GroundingStatus.InnerGroundNormal;
                    }
                }

                // Reorient velocity on slope
                currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) *
                                  currentVelocity.magnitude;

                // Calculate target velocity
                var inputRight = Vector3.Cross(MoveDirection, _motor.CharacterUp);
                var reorientedInput = Vector3.Cross(_motor.GroundingStatus.GroundNormal, inputRight).normalized *
                                      MoveDirection.magnitude;
                targetVelocity = reorientedInput * _moveSpeed;

                // Smooth movement Velocity
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1 - Mathf.Exp(-_friction * deltaTime));
            }
            else
            {
                if (MoveDirection.sqrMagnitude > 0f && _lastWallJumpTime + _postWallJumpDelay <= Time.time)
                {
                    //Apply air movement
                    targetVelocity = MoveDirection * _maxAirSpeed;

                    // Prevent climbing on un-stable slopes with air movement
                    if (_motor.GroundingStatus.FoundAnyGround)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3
                            .Cross(Vector3.Cross(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal),
                                _motor.CharacterUp).normalized;
                        targetVelocity = Vector3.ProjectOnPlane(targetVelocity, perpenticularObstructionNormal);
                    }

                    Vector3 velocityDiff =
                        Vector3.ProjectOnPlane(targetVelocity - currentVelocity, Vector3.down * _gravityFactor);
                    currentVelocity += velocityDiff * (_airAcceleration * deltaTime);
                }

                //Apply gravity
                currentVelocity += Vector3.down * (_gravityFactor * deltaTime);
            }

            _jumpedThisFrame = false;
            if (_jumpRequested)
            {
                if (_isOnWall || (!_isJumping && IsAnyGrounded))
                {
                    // Calculate jump direction before ungrounding
                    Vector3 jumpDirection = _motor.CharacterUp;

                    if (_isOnWall)
                    {
                        if (_wallJumpCollider == _lastWallJumpCollider)
                        {
                            _isOnWall = false;
                            return;
                        }

                        jumpDirection = _wallJumpNormal + _motor.CharacterUp;
                        _lastWallJumpTime = Time.time;
                        _lastWallJumpCollider = _wallJumpCollider;
                    }
                    else if (_motor.GroundingStatus.FoundAnyGround && !_motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = _motor.GroundingStatus.GroundNormal;
                    }

                    _motor.ForceUnground();

                    currentVelocity += (jumpDirection * _jumpPower) -
                                       Vector3.Project(currentVelocity, _motor.CharacterUp);
                    _isJumping = true;
                    _jumpedThisFrame = true;
                }
            }

            _isOnWall = false;
        }

        private void PostUpdateDefault(float deltaTime)
        {
            if (_jumpRequested)
            {
                _jumpRequested = false;
            }
            
            if (IsGrounded && !_jumpedThisFrame)
            {
                _isJumping = false;
            }
        }
    }
}
