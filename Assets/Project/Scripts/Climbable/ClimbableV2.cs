using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(BoxCollider))]
    public class ClimbableV2 : MonoBehaviour
    {
        //[SerializeField, Range(0, 1)] private float _traversablePercentage = 1;
        [SerializeField] private bool _canAttachFromGround;
        public bool CanAttachFromGround => _canAttachFromGround;

        [SerializeField] private ClimbableDirections _directions;
        public ClimbableDirections Directions => _directions;
        
        public bool IsBeingClimbed { get; set; }
        public Vector3 StartPoint { get; private set; }

        private BoxCollider _collider;
        private PlayerCharacter _currentCharacter;
        
        private void OnValidate()
        {
            if (_collider) return;
            
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void Awake()
        {
            if (_collider) return;
            
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Equals("Player") || IsBeingClimbed) return;

            if (!other.gameObject.TryGetComponent(out _currentCharacter))
            {
                //log
                return;
            }
            _currentCharacter.CurrentClimbable = this;
            
            CalculatePoint();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_currentCharacter) return;
            
            CalculatePoint();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.tag.Equals("Player") || IsBeingClimbed) return;

            if (_currentCharacter) _currentCharacter.CurrentClimbable = null;
            
            _currentCharacter = null;
            IsBeingClimbed = false;
        }

        private void CalculatePoint()
        {
            var clampedPosition = _currentCharacter.transform.position.ClampToBounds(_collider.bounds);

            if (!Directions.HasFlag(ClimbableDirections.Horizontal))
            {
                clampedPosition.x = _collider.bounds.center.x;
                clampedPosition.z = _collider.bounds.center.z;
            }
            
            if (!Directions.HasFlag(ClimbableDirections.Vertical)) clampedPosition.y = _collider.bounds.center.y;
            
            StartPoint = clampedPosition;
        }

        public Transform testSphere;
        public Vector3 GetMoveDirection()
        {
            if (!_currentCharacter) return Vector3.zero;

            var characterCenter = transform.InverseTransformPoint(_currentCharacter.transform.position) +
                                  (Vector3.up * (_currentCharacter.CharacterController.height * 0.5f));

            var absoluteMoveInput = _currentCharacter.MoveInput;
            if (absoluteMoveInput.x > 0) absoluteMoveInput.x = 1;
            if (absoluteMoveInput.x < 0) absoluteMoveInput.x = -1;
            if (absoluteMoveInput.y > 0) absoluteMoveInput.y = 1;
            if (absoluteMoveInput.y < 0) absoluteMoveInput.y = -1;
            
            var move = new Vector3(-absoluteMoveInput.x * (_currentCharacter.CharacterController.radius),
                absoluteMoveInput.y * _currentCharacter.CharacterController.height * 0.5f);
            
            var checkPos = characterCenter + move;
            testSphere.localPosition = checkPos;
            
            var wallMoveDirection = Quaternion.LookRotation(-transform.forward, Vector3.up) * _currentCharacter.MoveInput;

            var direction = new Vector3
            {
                x = Mathf.Abs(checkPos.x) < _collider.bounds.extents.x ? wallMoveDirection.x : 0,
                y = Mathf.Abs(checkPos.y) < _collider.bounds.extents.y ? _currentCharacter.MoveInput.y : 0,
                z = Mathf.Abs(checkPos.x) < _collider.bounds.extents.x ? wallMoveDirection.z : 0,
            };
            
            return direction;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_collider) return;

            // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            //
            // Gizmos.color = CoreColor.Orange;
            // Vector3 colliderSize = _collider.size;
            // Vector3 size = new Vector3(colliderSize.x * _traversablePercentage, colliderSize.y * _traversablePercentage,
            //     colliderSize.z);
            // Gizmos.DrawCube(_collider.center, size);
        }
    }
}
