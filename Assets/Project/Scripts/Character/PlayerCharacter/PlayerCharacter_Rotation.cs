using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string ROTATION = "Rotation";
        
        [FoldoutGroup(ROTATION), SerializeField] private Transform _cameraTransform;
        [FoldoutGroup(ROTATION), SerializeField] private float _rotateSpeed = 20;
        
        private void ProcessRotation()
        {
            var lookDirection = GetLookDirection();
            if (lookDirection == Vector3.zero) return;
            
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(lookDirection, transform.up), Time.deltaTime * _rotateSpeed);
        }

        private Vector3 GetLookDirection()
        {
            if (State == State.Climbing)
            {
                var climbRay = new Ray(CharacterPosition(CharacterController.height * 0.5f), transform.forward);
                if (Physics.Raycast(climbRay, out var hit, CharacterController.radius * 2, _wallLayerMask))
                {
                    return -hit.normal;
                }
            }

            return MoveDirection;
        }
    }
}
