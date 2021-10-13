using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        [Title("Climb")] 
        [SerializeField] private LayerMask _climbableLayers = -1;
        [SerializeField] private LinearState _climbState;
        [SerializeField] private float _climbSpeed;

        public bool IsClimbing { get; private set; }
        
        private Climbable _currentClimbable;
        
        public void OnClimbableEnter(Climbable climbable)
        {
            IsClimbing = true;
            _currentClimbable = climbable;
        }

        public void OnClimbableExit(Climbable climbable)
        {
            IsClimbing = false;
            _currentClimbable = null;
        }

        private void ProcessClimb()
        {
            
        }
    }
}
