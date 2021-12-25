using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class Interactable : MonoBehaviour, IDetectable
    {
        private const string LOG_FORMAT = nameof(Interactable) + ".{0} :: {1}";
        
        public virtual void OnEnter()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnEnter), "");
        }

        public virtual void OnExit()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnExit), "");
        }

        public void OnActivate()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnActivate), "");
        }
    }
}
