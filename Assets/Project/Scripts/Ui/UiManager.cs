using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class UiManager : Manager<UiManager>
    {
        private const string LOG_FORMAT = nameof(UiManager) + ".{0} :: {1}";
        
        [SerializeField] private KeyboardMouseInputUi _keyboardUi;
        [SerializeField] private GamepadInputUi _gamepadUi;
        
        public static IInputUi CurrentInputUi { get; private set; }

        protected override IEnumerator InitializeManager()
        {
            // _keyboardUi.Hide();
            // _xboxGamepadUi.Hide();
            // _psGamepadUi.Hide();
            
            return base.InitializeManager();
        }

        #region InputUi

        public void DisplayInputUi(string actionName, string displayName, string layoutName, string controlPath)
        {
            HideInputUi();
            
            if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(layoutName) || string.IsNullOrEmpty(controlPath)) return;

            if (InputSystem.IsFirstLayoutBasedOnSecond(layoutName, "DualShockGamepad"))
            {
                if (_gamepadUi) _gamepadUi.DisplayGamepadUi(actionName, PlatformInputTypes.Ps4, controlPath);
            }
            else if (InputSystem.IsFirstLayoutBasedOnSecond(layoutName, "Gamepad"))
            {
                if (_gamepadUi) _gamepadUi.DisplayGamepadUi(actionName, PlatformInputTypes.Xbox, controlPath);
            }
            else if(InputSystem.IsFirstLayoutBasedOnSecond(layoutName, "Keyboard"))
            {
                if(_keyboardUi) _keyboardUi.DisplayKeyBoardUi(displayName, controlPath);
            }
        }

        public void HideInputUi()
        {
            if (_gamepadUi) _gamepadUi.HideGamepadUi();
            if (_keyboardUi) _keyboardUi.HideKeyboardUi();
        }
        
        #endregion InputUi

    }
    
    public enum PlatformInputTypes
    {
        Ps4,
        Xbox,
        Nintendo
    }
}
