using System;
using TMPro;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class GamepadInputUi : InputUi
    {
        [SerializeField] private ActionHighlight _northHighlight;
        [SerializeField] private ActionHighlight _eastHighlight;
        [SerializeField] private ActionHighlight _southHighlight;
        [SerializeField] private ActionHighlight _westHighlight;

        public override void Show(ActionType type)
        {
            _northHighlight.Highlight.SetActive(false);
            _eastHighlight.Highlight.SetActive(false);
            _southHighlight.Highlight.SetActive(false);
            _westHighlight.Highlight.SetActive(false);

            switch (type)
            {
                case ActionType.None:
                    break;
                case ActionType.Interact:
                    _westHighlight.Highlight.SetActive(true);
                    _westHighlight.Label.text = type.ToString();
                    break;
                case ActionType.Drop:
                    _eastHighlight.Highlight.SetActive(true);
                    _eastHighlight.Label.text = type.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
            base.Show(type);
        }

        [Serializable]
        public class ActionHighlight
        {
            public GameObject Highlight;
            public TMP_Text Label;
        }
    }
}
