using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public interface IDetectable
    {
        void OnEnter();
        void OnExit();
        void OnActivate();
    }
}
