using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class CameraManager : Manager<CameraManager>
    {
        public static Camera Main { get; private set; }
        public static Transform MainTransform { get; private set; }
        public static CinemachineBrain Brain { get; private set; }
        [ShowInInspector]
        public static CinemachineVirtualCamera Current { get; private set; }

        [ShowInInspector]
        private static Dictionary<int, CinemachineVirtualCamera> _cameraLookUp =
            new Dictionary<int, CinemachineVirtualCamera>();

        private static PlayerCharacter _currentPlayer;
        
        #region MonoBehaviour

        private void OnEnable()
        {
            PlayerManager.OnPlayerRegisteredEvent += OnPlayerRegistered;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerRegisteredEvent -= OnPlayerRegistered;
        }

        #endregion MonoBehaviour
        
        private void OnPlayerRegistered(PlayerCharacter player)
        {
            _currentPlayer = player;
        }
        
        protected override IEnumerator InitializeManager()
        {
            Main = GetComponentInChildren<Camera>();
            if (!Main) yield break;

            MainTransform = Main.transform;

            Brain = Main.GetComponent<CinemachineBrain>();
            if (!Brain) yield break;

            yield return base.InitializeManager();
        }

        public static void RegisterCamera(CinemachineVirtualCamera camera, bool setToPriority = false)
        {
            if (camera == null) return;

            var instanceId = camera.GetInstanceID();
            if (instanceId == -1) return;
            
            _cameraLookUp[instanceId] = camera;

            if (setToPriority) SwitchToCamera(camera);
            else camera.Priority = 0;
        }
        
        public static void UnRegisterCamera(CinemachineVirtualCamera camera)
        {
            if (camera == null) return;

            var instanceId = camera.GetInstanceID();
            if (instanceId == -1) return;
            if (!_cameraLookUp.ContainsKey(instanceId)) return;

            _cameraLookUp.Remove(instanceId);
        }

        public static void SwitchToCamera(CinemachineVirtualCamera camera)
        {
            if (camera == null) camera = _currentPlayer.CharacterCamera;
            
            var instanceId = camera.GetInstanceID();
            if (instanceId == -1) return;

            if (!_cameraLookUp.ContainsKey(instanceId)) return;
            
            if (Current) Current.Priority = 0;
            camera.Priority = 10;
            Current = camera;
        }
    }
}
