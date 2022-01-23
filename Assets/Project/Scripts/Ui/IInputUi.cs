using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public interface IInputUi
    {
        void Show(string displayName, string layoutName, string controlName);
        void Hide();
    }
}
