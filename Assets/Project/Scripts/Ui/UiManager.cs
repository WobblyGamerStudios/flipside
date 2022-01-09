using System.Collections;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class UiManager : Manager<UiManager>
    {
        private const string LOG_FORMAT = nameof(UiManager) + ".{0} :: {1}";

        [SerializeField] private CanvasGroup _keyboardUi;
        [SerializeField] private CanvasGroup _xboxGamepadUi;
        [SerializeField] private CanvasGroup _psGamepadUi;

        public static CanvasGroup ActiveInputUi { get; private set; }

        private static bool _isShowing;
        
        protected override IEnumerator InitializeManager()
        {
            InputManager.OnControlSchemeChangedEvent += OnControlSchemeChanged;

            _keyboardUi.alpha = 0;
            _xboxGamepadUi.alpha = 0;
            _psGamepadUi.alpha = 0;
            
            return base.InitializeManager();
        }

        private void OnControlSchemeChanged(ControlScheme scheme)
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnControlSchemeChanged), $"InputManager control scheme changed to {scheme}");

            if (ActiveInputUi) ActiveInputUi.alpha = 0;
            ActiveInputUi = scheme switch
            {
                ControlScheme.Keyboard => _keyboardUi,
                ControlScheme.XboxGamepad => _xboxGamepadUi,
                ControlScheme.PlayStationGamepad => _psGamepadUi,
                _ => null
            };
            
            if (_isShowing && ActiveInputUi) ActiveInputUi.alpha = 1;
        }

        public static void Show()
        {
            if (ActiveInputUi) ActiveInputUi.alpha = 1;
            _isShowing = true;
        }

        public static void Hide()
        {
            if (ActiveInputUi) ActiveInputUi.alpha = 0;
            _isShowing = false;
        }
    }
}
