using Cinemachine;
using UnityEngine;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(CameraZone))]
    public class StraddleZone : MonoBehaviour
    {
        private const string LOG_FORMAT = nameof(StraddleZone) + ".{0} :: {1}";

        [SerializeField] private CinemachinePathBase _path;
        public CinemachinePathBase Path => _path;
        
        private CameraZone _cameraZone;

        private void OnEnable()
        {
            _cameraZone = GetComponent<CameraZone>();
            
            if (!_cameraZone) return;
            
            _cameraZone.OnTriggerEnterEvent += EnterCameraZone;
            _cameraZone.OnTriggerExitEvent += ExitCameraZone;
        }

        private void OnDisable()
        {
            if (!_cameraZone) return;
            
            _cameraZone.OnTriggerEnterEvent -= EnterCameraZone;
            _cameraZone.OnTriggerExitEvent -= ExitCameraZone;
        }

        private void EnterCameraZone(Collider other)
        {
            if (!other) return;

            if (!other.TryGetComponent(out PlayerCharacter character))
            {
                Debug.LogErrorFormat(LOG_FORMAT, nameof(ExitCameraZone), $"Could not find {nameof(PlayerCharacter)} on {other.name}");
                return;
            }
            
            character.BeginStraddle(this);
        }
        
        private void ExitCameraZone(Collider other)
        {
            if (!other) return;

            if (!other.TryGetComponent(out PlayerCharacter character))
            {
                Debug.LogErrorFormat(LOG_FORMAT, nameof(ExitCameraZone), $"Could not find {nameof(PlayerCharacter)} on {other.name}");
                return;
            }
            
            character.EndStraddle();
        }
    }
}
