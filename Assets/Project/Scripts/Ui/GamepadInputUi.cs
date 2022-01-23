using System;
using UnityEngine;

namespace Wgs.FlipSide
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GamepadInputUi : MonoBehaviour
    {
        [SerializeField] private PadUi _padUi;
        [SerializeField] private InputIconsList _inputIcons;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        public void DisplayGamepadUi(string actionName, PlatformInputTypes inputTypes, string controlName)
        {
            switch (controlName)
            {
                case "buttonSouth":
                    _padUi.HighlightButton(actionName, PadUi.Direction.South);
                    break;
                case "buttonNorth": 
                    _padUi.HighlightButton(actionName, PadUi.Direction.North);
                    break;
                case "buttonEast": 
                    _padUi.HighlightButton(actionName, PadUi.Direction.East);
                    break;
                case "buttonWest": 
                    _padUi.HighlightButton(actionName, PadUi.Direction.West);
                    break;
                case "start": break;
                case "select": break;
                case "leftTrigger": break;
                case "rightTrigger": break;
                case "leftShoulder": break;
                case "rightShoulder": break;
                case "dpad": break;
                case "dpad/up": break;
                case "dpad/down": break;
                case "dpad/left": break;
                case "dpad/right": break;
                case "leftStick": break;
                case "rightStick": break;
                case "leftStickPress": break;
                case "rightStickPress": break;
            }
            
            _canvasGroup.alpha = 1;
        }
        
        public void HideGamepadUi()
        {
            _canvasGroup.alpha = 0;
        }
    }
}
