using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class FlipSideCharacter : Character
    {
        [Title("Move")]
        [SerializeField] private LinearState _moveState;
        [SerializeField] private InputActionProperty _moveAction;
        
        public Vector2 MoveInput { get; private set; }
        public Vector3 MoveDirection { get; private set; }
        
        private Vector3 _moveInputClamped;

        private void ProcessMovement()
        {
            CalculateMoveDirection();
            
            _moveState.Blend = MoveDirection.magnitude;
        }
        
        private void CalculateMoveDirection()
        {
            if (!CharacterController) return;
            
            MoveInput = _moveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            _moveInputClamped = Vector3.ClampMagnitude(new Vector3(MoveInput.x, 0, MoveInput.y), 1);

            var projection = Vector3.ProjectOnPlane(_cameraTransform.rotation * Vector3.forward, transform.up).normalized;
            MoveDirection = Quaternion.LookRotation(projection, transform.up) * _moveInputClamped;
        }
    }
}
