using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public abstract class InputUi : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public bool IsActive => _canvasGroup && _canvasGroup.alpha >= 1;
        
        public virtual void Show(ActionType type)
        {
            if (!_canvasGroup) return;
            _canvasGroup.alpha = 1;
        }

        public virtual void Hide()
        {
            if (!_canvasGroup) return;
            _canvasGroup.alpha = 0;
        }
    }
    
    public enum ActionType
    {
        None = 0,
        Interact,
        Drop
    }
}
