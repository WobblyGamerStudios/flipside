using UnityEngine;
using UnityEngine.InputSystem;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class Interactable : MonoBehaviour
    {
        private const string LOG_FORMAT = nameof(Interactable) + ".{0} :: {1}";

        [SerializeField] private InputActionReference _actionReference;
        public InputActionReference ActionReference => _actionReference;
        
        [SerializeField] private GameObject _ui;
        
        private void Awake()
        {
            _ui.SetActive(false);
        }

        public virtual void OnEnter()
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string);
            var controlPath = default(string);
            
            if (_actionReference == null) return;
            if (_actionReference.action == null) return;

            var bindingIndex = InputManager.GetBindingIndex(_actionReference);
            if (bindingIndex == -1) return;
            displayString = _actionReference.action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath);

            Debug.LogFormat(LOG_FORMAT, nameof(OnEnter), $"Name : {_actionReference.action.name}\n" +
                                                         $"Display : {displayString}\n" +
                                                         $"Layout : {deviceLayoutName}\n" +
                                                         $"Path : {controlPath}");
            
            _ui.SetActive(true);
            if (UiManager.HasInstance) UiManager.Instance.DisplayInputUi(
                _actionReference.action.name, 
                displayString, 
                deviceLayoutName, 
                controlPath);
        }

        public virtual void OnExit()
        {
            _ui.SetActive(false);
            if (UiManager.HasInstance) UiManager.Instance.HideInputUi();
        }

        public void OnActivate()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnActivate), "");
        }
    }
}
