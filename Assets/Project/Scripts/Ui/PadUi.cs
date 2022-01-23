using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wgs.FlipSide
{
    public class PadUi : MonoBehaviour
    {
        public enum Direction
        {
            North, 
            East,
            South,
            West
        }
        
        [SerializeField] private Image _background;
        [SerializeField] private PadButton _northButton;
        [SerializeField] private PadButton _eastButton;
        [SerializeField] private PadButton _southButton;
        [SerializeField] private PadButton _westButton;

        public void HighlightButton(string label, Direction direction)
        {
            _northButton.CanvasGroup.alpha = 0;
            _eastButton.CanvasGroup.alpha = 0;
            _southButton.CanvasGroup.alpha = 0;
            _westButton.CanvasGroup.alpha = 0;

            PadButton highlight = direction switch
            {
                Direction.North => _northButton,
                Direction.East => _eastButton,
                Direction.South => _southButton,
                Direction.West => _westButton,
                _ => null
            };
            
            if(highlight == null) return;

            highlight.CanvasGroup.alpha = 1;
            highlight.Label.text = label;
        }
    }

    [Serializable]
    public class PadButton
    {
        public CanvasGroup CanvasGroup;
        public Image IconMidGround;
        public Image Icon;
        public RectTransform LabelTransform;
        public TMP_Text Label;
    }
}
