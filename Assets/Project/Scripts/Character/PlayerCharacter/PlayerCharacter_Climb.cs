using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string CLIMB = "Climb";

        [FoldoutGroup(CLIMB), SerializeField]
        private float _climbCheckDistance = 0.3f;
        
        [FoldoutGroup(CLIMB), SerializeField]
        private float _groundedCheckOffset = 0.3f;
        
        [FoldoutGroup(CLIMB), SerializeField]
        private float _inAirCheckOffset = 0.3f;
        
        [FoldoutGroup(CLIMB), Title("Debug"), SerializeField]
        private bool _showGizmos;

        private void InitializeClimb()
        {
            
        }
        
        private void ProcessClimb()
        {
            
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

        private void DrawClimbGizmos()
        {
            
        }
    }
}
