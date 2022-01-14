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

        [SerializeField] private InputUi _keyboardUi;
        [SerializeField] private InputUi _xboxGamepadUi;
        [SerializeField] private InputUi _psGamepadUi;

        public static InputUi CurrentInputUi { get; private set; }

        public static bool IsActive => CurrentInputUi && CurrentInputUi.IsActive;

        protected override IEnumerator InitializeManager()
        {
            _keyboardUi.Hide();
            _xboxGamepadUi.Hide();
            _psGamepadUi.Hide();
            
            return base.InitializeManager();
        }

        public static void Show(string displayName, string layoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(layoutName) || string.IsNullOrEmpty(controlPath)) return;

            if (InputSystem.IsFirstLayoutBasedOnSecond(layoutName, "DualShockGamepad"))
            {
                Debug.LogFormat(LOG_FORMAT, nameof(Show), "Showing PS4 controller");
            }
            else if (InputSystem.IsFirstLayoutBasedOnSecond(layoutName, "Gamepad"))
            {
                Debug.LogFormat(LOG_FORMAT, nameof(Show), "Showing gamepad");
            }
            else if(InputSystem.IsFirstLayoutBasedOnSecond(layoutName, "Keyboard"))
            {
                Debug.LogFormat(LOG_FORMAT, nameof(Show), "Showing keyboard and mouse");
            }
        }

        public static void Hide()
        {
            if (CurrentInputUi) CurrentInputUi.Hide();
        }
    }
}
