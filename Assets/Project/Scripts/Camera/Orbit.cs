using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Context = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace Wgs.FlipSide
{
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	public class Orbit : MonoBehaviour
	{
		[SerializeField] private InputActionProperty _controllerOrbitAction;
		[SerializeField] private bool _invertHorizontal;
		[SerializeField] private bool _invertVertical;
		[SerializeField] private float _horizontalSpeed = 100;
		[SerializeField] private float _verticalSpeed = 1;
        
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
			
			lookMovement.y = _invertVertical ? -lookMovement.y : lookMovement.y;
			lookMovement.x = (_invertHorizontal ? -lookMovement.x : lookMovement.x) * 180f; 

			var component = _camera.GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachinePOV;
			if (component == null) return;
            
			component.m_HorizontalAxis.Value += lookMovement.x * _verticalSpeed * Time.deltaTime;
			component.m_VerticalAxis.Value += lookMovement.y * _horizontalSpeed * Time.deltaTime;
		}
	}
}