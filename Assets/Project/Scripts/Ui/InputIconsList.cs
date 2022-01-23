using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Wgs.FlipSide
{
    [Serializable]
    public class InputIconsList
    {
        [ValueDropdown("AddIconValue", IsUniqueList = true, DrawDropdownForListElements = false, DropdownTitle = "Icons")]
        [ListDrawerSettings(Expanded = true)]
        [SerializeField]
        private List<InputIconsValue> _inputIconValues = new List<InputIconsValue>();

        public InputIconsValue this[int index]
        {
            get => _inputIconValues[index];
            set => _inputIconValues[index] = value;
        }

        public int Count => _inputIconValues.Count;

        public Sprite this[PlatformInputTypes inputType, string control]
        {
            get
            {
                foreach (var iconValue in _inputIconValues)
                {
                    
                }

                return null;
            }
        }

#if UNITY_EDITOR
        private IEnumerable AddIconValue()
        {
            return Enum.GetValues(typeof(PlatformInputTypes)).Cast<PlatformInputTypes>()
                .Except(_inputIconValues.Select(x => x.Value))
                .Select(x => new InputIconsValue(x))
                .AppendWith(_inputIconValues)
                .Select(x => new ValueDropdownItem(x.Value.ToString(), x));
        }
#endif
		
    }

#if UNITY_EDITOR
	
    internal class InputIconsListDrawer : Sirenix.OdinInspector.Editor.OdinValueDrawer<InputIconsList>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }
	
#endif
}
