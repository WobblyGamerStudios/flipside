using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class StaticCameraZone : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private Transform p1, p2;

        public float value;
        
        private void OnEnable()
        {
            CameraManager.RegisterCamera(_camera);
        }

        private void OnDisable()
        {
            CameraManager.UnRegisterCamera(_camera);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((_layerMask.value & (1 << other.gameObject.layer)) > 0)
                CameraManager.SwitchToCamera(_camera);
        }

        private void OnTriggerStay(Collider other)
        {
            if ((_layerMask.value & (1 << other.gameObject.layer)) <= 0) return;
            if (_camera == null) return;
            
            var component = _camera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineTrackedDolly;
            if (component == null) return;

            value = Vector3.Distance(other.transform.position, p1.position) / Vector3.Distance(p1.position, p2.position);
            
            component.m_PathPosition = value;
        }

        private void OnTriggerExit(Collider other)
        {
            if ((_layerMask.value & (1 << other.gameObject.layer)) > 0)
                CameraManager.SwitchToCamera(null);
        }
    }
}
