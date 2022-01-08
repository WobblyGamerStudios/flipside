using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string FALL = "Fall";
        
        [FoldoutGroup(FALL), SerializeField] private ClipState _fallState;
        [FoldoutGroup(FALL), SerializeField] private float _minFallDistance = 0.5f;

        private void InitializeFall()
        {
            _fallState.Initialize(Animancer);
        }
        
        public void CheckFall()
        {
            if (IsGrounded || State == State.Falling || _isJumping || _isRolling) return;
            
            var fallRay = new Ray(transform.position, Vector3.down);
            Debug.DrawRay(fallRay.origin, fallRay.direction * _minFallDistance, Color.red, 3);
            if (Physics.Raycast(fallRay, _minFallDistance, _traversableLayers))
            {
                TrySetState(_moveState);
            }
            else
            {
                TrySetState(_fallState);
                State = State.Falling;
            }
        }
    }
}
