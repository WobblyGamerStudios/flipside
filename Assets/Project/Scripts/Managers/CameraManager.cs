using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class CameraManager : Manager<CameraManager>
    {
        public static Camera Main { get; private set; }
        public static Transform MainTransform { get; private set; }
        public static CinemachineBrain Brain { get; private set; }
        public static CinemachineVirtualCamera Current { get; private set; }

        private static Dictionary<string, CinemachineVirtualCamera> _cameraLookUp =
            new Dictionary<string, CinemachineVirtualCamera>();

        private PlayerCharacter _currentPlayer;
        
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

        public static void RegisterCamera(CinemachineVirtualCamera camera, string tag = "", bool setToPriority = false)
        {
            if (string.IsNullOrEmpty(tag)) tag = camera.name;

            _cameraLookUp[tag] = camera;

            if (setToPriority) SwitchToCamera(camera);
            else camera.Priority = 0;
        }
        
        public static void UnRegisterCamera(CinemachineVirtualCamera camera, string tag = "")
        {
            if (string.IsNullOrEmpty(tag)) tag = camera.name;

            if (!_cameraLookUp.ContainsKey(tag)) return;

            _cameraLookUp.Remove(tag);
        }

        public static void SwitchToCamera(string tag)
        {
            if (!_cameraLookUp.ContainsKey(tag)) return;
            
            SwitchToCamera(_cameraLookUp[tag]);
        }

        public static void SwitchToCamera(CinemachineVirtualCamera camera)
        {
            if (Current) Current.Priority = 0;
            camera.Priority = 10;
            Current = camera;
        }
    }
}
