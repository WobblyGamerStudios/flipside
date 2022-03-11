using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Wgs.Core;

namespace Wgs.FlipSide
{
	public class Orbit : MonoBehaviour, AxisState.IInputAxisProvider
	{
		[SerializeField] private InputActionReference _lookAction;
		[SerializeField] private float _gamepadInputMultiplier = 1;
		
		private Vector2 _lookValue;
		private string _currentInputScheme;

		private void OnEnable()
		{
			InputManager.OnInputChangeEvent += InputChanged;
		}

		private void OnDisable()
		{
			InputManager.OnInputChangeEvent -= InputChanged;
		}
		
		private void InputChanged(string schemeName, string deviceName)
		{
			_currentInputScheme = schemeName;
			
			switch (_currentInputScheme)
			{
				case "Keyboard":
					Cursor.lockState = CursorLockMode.Locked;
					break;
				case "Gamepad":
					Cursor.lockState = CursorLockMode.None;
					break;
			}
		}

		public float GetAxisValue(int axis)
		{
			if (!Application.isFocused)
			{
				_lookValue = Vector2.zero;
				return 0 ;
			}

			_lookValue = _lookAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
			
			switch (axis)
			{
				case 0 :
					return _currentInputScheme switch
					{
						//X axis
						"Keyboard" => _lookValue.x,
						"Gamepad" => _lookValue.x * _gamepadInputMultiplier,
						_ => 0
					};
				case 1:
					//Y axis
					return _currentInputScheme switch
					{
						//X axis
						"Keyboard" => _lookValue.y,
						"Gamepad" => _lookValue.y * _gamepadInputMultiplier,
						_ => 0
					};
				case 2 :
					//Zoom
					break;
			}
			
			return 0;
		}
	}
}