using System;
using System.Collections;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class UiManager : Manager<UiManager>
    {
        private const string LOG_FORMAT = nameof(UiManager) + ".{0} :: {1}";

        [SerializeField] private GameObject _keyboardUi;
        [SerializeField] private GameObject _xboxGamepadUi;
        [SerializeField] private GameObject _psGamepadUi;
        
        protected override IEnumerator InitializeManager()
        {
            InputManager.OnControlSchemeChangedEvent += OnControlSchemeChanged;
            
            return base.InitializeManager();
        }

        private void OnControlSchemeChanged(InputManager.ControlScheme scheme)
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnControlSchemeChanged), $"InputManager control scheme changed to {scheme}");

            _keyboardUi.SetActive(scheme == InputManager.ControlScheme.Keyboard);
            _xboxGamepadUi.SetActive(scheme == InputManager.ControlScheme.XboxGamepad);
            _psGamepadUi.SetActive(scheme == InputManager.ControlScheme.PlayStationGamepad);
        }
    }
}
