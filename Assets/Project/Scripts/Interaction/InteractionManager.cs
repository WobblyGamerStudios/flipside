using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class InteractionManager : Manager<InteractionManager>
    {
        #region MonoBehaviour

        private void FixedUpdate()
        {
            
        }

        #endregion MonoBehaviour

        #region Interactor

        public static void RegisterInteractor(IInteractor interactor)
        {
            
        }

        public static void UnregisterInteractor(IInteractor interactor)
        {
            
        }

        #endregion Interactor

        #region Interactable

        public static void RegisterInteractable(IInteractable interactable)
        {
            
        }

        public static void UnregisterInteractable(IInteractable interactable)
        {
            
        }

        #endregion Interactable
    }
}
