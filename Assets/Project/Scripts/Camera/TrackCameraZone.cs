using Cinemachine;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class TrackCameraZone : CameraZone
    {
        [SerializeField] private Transform p1, p2;

        private float _length;
        
        protected override void OnEnable()
        {
            _length = Vector3.Distance(p1.position, p2.position);
            
            base.OnEnable();
        }

        protected override void OnStay(Collider other)
        {
            if (_camera == null) return;
            
            var component = _camera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineTrackedDolly;
            if (component == null) return;

            var current = Vector3.Distance(other.transform.position, p1.position);
            component.m_PathPosition = current / _length;
            
            base.OnStay(other);
        }
    }
}
