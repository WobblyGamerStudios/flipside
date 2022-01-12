using System.Collections;
using UnityEngine;
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
            InputManager.OnControlSchemeChangedEvent += OnControlSchemeChanged;

            _keyboardUi.Hide();
            _xboxGamepadUi.Hide();
            _psGamepadUi.Hide();
            
            return base.InitializeManager();
        }

        private void OnControlSchemeChanged(ControlScheme scheme)
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnControlSchemeChanged), $"InputManager control scheme changed to {scheme}");

            if (CurrentInputUi) CurrentInputUi.Hide();
            CurrentInputUi = scheme switch
            {
                ControlScheme.Keyboard => _keyboardUi,
                ControlScheme.XboxGamepad => _xboxGamepadUi,
                ControlScheme.PlayStationGamepad => _psGamepadUi,
                _ => null
            };
        }

        public static void Show(ActionType type)
        {
            if (CurrentInputUi) CurrentInputUi.Show(type);
        }

        public static void Hide()
        {
            if (CurrentInputUi) CurrentInputUi.Hide();
        }
    }
}
