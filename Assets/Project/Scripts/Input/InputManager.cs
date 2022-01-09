using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Wgs.FlipSide
{
    [AddComponentMenu("InputManager.FlipSide")]
    public class InputManager : Core.InputManager
    {
        protected override IEnumerator InitializeManager()
        {
            yield return base.InitializeManager();
            
            OnInputUserChanged(_inputUser, InputUserChange.ControlSchemeChanged, _inputUser.pairedDevices[0]);
        }

        protected override void OnInputUserChanged(InputUser user, InputUserChange change, InputDevice device)
        {
            if (change != InputUserChange.ControlSchemeChanged) return;

            if (!user.valid) return;
            if (user.controlScheme == null) return;

            var scheme = user.controlScheme.Value.name switch
            {
                "Keyboard" => ControlScheme.Keyboard,
                "XboxGamepad" => ControlScheme.XboxGamepad,
                "PlayStationGamepad" => ControlScheme.PlayStationGamepad,
                _ => throw new ArgumentOutOfRangeException()
            };
			
            OnControlSchemeChangedEvent?.Invoke(scheme);
            
            base.OnInputUserChanged(user, change, device);
        }

        #region Events

        public static event Action<ControlScheme> OnControlSchemeChangedEvent;

        #endregion Events
    }
    
    public enum ControlScheme
    {
        Keyboard,
        XboxGamepad,
        PlayStationGamepad
    }
}
