using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public partial class PlayerCharacter : Character
    {
        private const string STRADDLE = "Straddle";

        [FoldoutGroup(STRADDLE), SerializeField] 
        private LinearState _straddleState;
        [FoldoutGroup(STRADDLE), SerializeField] 
        private float _straddleSpeed = 1;
        
        private StraddleZone _currentStraddleZone;
        private bool _isStraddle;
        private float _progress;
        
        private void InitializeStraddle()
        {
            _straddleState.Initialize(Animancer);
        }
        
        public void BeginStraddle(StraddleZone straddleZone)
        {
            Debug.LogFormat(LOG_FORMAT, nameof(BeginStraddle), "");

            _currentStraddleZone = straddleZone;
            Velocity = Vector3.zero;
            TrySetState(_straddleState);
            _progress = 0;
            _isStraddle = true;
        }

        private void ProcessStraddle()
        {
            if (!_isStraddle) return;

            var move = MoveInput.normalized;
            var isMoving = Mathf.Abs(move.y) > InputSystem.settings.defaultDeadzoneMin;
            if (isMoving)
            {
                _progress = _currentStraddleZone.Path.StandardizeUnit(_progress + Mathf.Sign(move.y) * _straddleSpeed * Time.deltaTime, CinemachinePathBase.PositionUnits.Normalized);
            }

            var offset =
                _currentStraddleZone.Path.EvaluatePositionAtUnit(_progress,
                    CinemachinePathBase.PositionUnits.Normalized) - transform.position;

            CharacterController.Move(offset.normalized * Time.deltaTime);
            transform.rotation = _currentStraddleZone.Path.EvaluateOrientationAtUnit(_progress, CinemachinePathBase.PositionUnits.Normalized);

            _straddleState.Value = move.y;
        }

        public void EndStraddle()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(EndStraddle), "");

            _isStraddle = false;
            CharacterController.Warp(transform.position);
            TrySetState(_moveState);
            _currentStraddleZone = null;
        }
    }
}
