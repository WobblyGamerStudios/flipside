using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Context = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Wgs.FlipSide
{
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	public class Orbit : MonoBehaviour
	{
		[SerializeField] private InputActionProperty _mouseOrbitAction;
		[SerializeField] private InputActionProperty _controllerOrbitAction;
		[SerializeField] private bool _invertY;
		[SerializeField] private float _lookSpeed;
        
		private CinemachineVirtualCamera _camera;
        
		private void OnValidate()
		{
			if (!_camera) _camera = GetComponent<CinemachineVirtualCamera>();
		}

		private void Awake()
		{
			if (!_camera) _camera = GetComponent<CinemachineVirtualCamera>();
		}

		private void Update()
		{
			var lookMovement = _controllerOrbitAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
			lookMovement.Normalize();
			lookMovement.y = _invertY ? -lookMovement.y : lookMovement.y;
			lookMovement.x = -lookMovement.x * 180f; 

			var component = _camera.GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachinePOV;
			if (component == null) return;
            
			component.m_HorizontalAxis.Value += lookMovement.x * _lookSpeed * Time.deltaTime;
			component.m_VerticalAxis.Value += lookMovement.y * (_lookSpeed * 100) * Time.deltaTime;
		}
	}
}