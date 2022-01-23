using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wgs.FlipSide
{
    [Serializable]
    public struct InputIconsValue : IEquatable<InputIconsValue>
    {
        [ReadOnly, HideLabel]
        public PlatformInputTypes Value;

        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, NumberOfItemsPerPage = 5)]
        public List<IconReference> Icons;
        
        public InputIconsValue(PlatformInputTypes value)
        {
            Value = value;
            Icons = new List<IconReference>();

            foreach (IconTypes iconType in Enum.GetValues(typeof(IconTypes)))
            {
                Icons.Add(new IconReference(iconType));
            }
        }
        
        public bool Equals(InputIconsValue other)
        {
            return other.Value.Equals(Value);
        }
        
        [Serializable]
        public class IconReference
        {
            [ReadOnly] 
            [HideLabel]
            public IconTypes Id;
            [HideLabel]
            public Sprite Icon;
            
            public IconReference(){}

            public IconReference(IconTypes iconType)
            {
                Id = iconType;
            }

            public IconReference(IconTypes iconType, Sprite icon)
            {
                Id = iconType;
                Icon = icon;
            }
        }
        
        public enum IconTypes
        {
            LeftStick,
            LeftStickPress,
            RightStick,
            RightStickPress,
            Buttons,
            ButtonNorth,
            ButtonEast,
            ButtonSouth,
            ButtonWest,
            Dpad,
            DpadNorth,
            DpadEast, 
            DpadSouth,
            DpadWest,
            Start,
            Select,
            LeftTrigger,
            RightTrigger,
            LeftShoulder,
            RightShoulder
        }
    }
}
