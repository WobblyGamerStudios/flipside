using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public interface IInteractor
    {
        bool TryGetTargets(out List<IInteractable> targets);
    }
}
