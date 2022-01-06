using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class FollowUtility : MonoBehaviour
    {
        [SerializeField] private FollowType _followType;
        
        [ShowIf("_followType", FollowType.Target)]
        [SerializeField] private Transform _target;

        private Transform _playerTransform;
        private Transform _cameraTransform;
        
        private void Awake()
        {
            CameraManager.InvokeAfterInitialization(() => _cameraTransform = CameraManager.MainTransform);
        }

        private void OnEnable()
        {
            PlayerManager.OnPlayerRegisteredEvent += OnPlayerRegistered;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerRegisteredEvent -= OnPlayerRegistered;
        }

        private void OnPlayerRegistered(PlayerCharacter player)
        {
            _playerTransform = player.transform;
        }

        private void Update()
        {
            Vector3 targetPosition;
            switch (_followType)
            {
                case FollowType.Camera:
                    if (!_cameraTransform) return;
                    targetPosition = _cameraTransform.position;
                    break;
                case FollowType.Player:
                    if (!_playerTransform) return;
                    targetPosition = _playerTransform.position;
                    break;
                case FollowType.Target:
                    if (!_target) return;
                    targetPosition = _target.position;
                    break;
                default:
                    targetPosition = Vector3.zero;
                    break;
            }

            var vector = targetPosition - transform.position;
            transform.rotation = Quaternion.LookRotation(-vector);
        }
    }

    public enum FollowType
    {
        Camera = 0,
        Player,
        Target
    }
}
