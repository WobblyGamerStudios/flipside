using Cinemachine;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class CameraZone : TriggerZone
    {
        [SerializeField] protected CinemachineVirtualCamera _camera;
        
        protected override void OnEnable()
        {
            CameraManager.RegisterCamera(_camera);
            
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            CameraManager.UnRegisterCamera(_camera);
            
            base.OnDisable();
        }

        protected override void OnEnter(Collider other)
        {
            CameraManager.SwitchToCamera(_camera);
            
            OnTriggerEnterEvent?.Invoke(other);
        }

        protected override void OnStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }

        protected override void OnExit(Collider other)
        {
            CameraManager.SwitchToCamera(null);
            
            OnTriggerExitEvent?.Invoke(other);
        }

        #region Events

        public delegate void TriggerDelegate(Collider other);
        
        public event TriggerDelegate OnTriggerEnterEvent;
        public event TriggerDelegate OnTriggerStayEvent;
        public event TriggerDelegate OnTriggerExitEvent;

        #endregion Events
    }
}
