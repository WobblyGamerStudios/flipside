using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wgs.Core;

namespace Wgs.FlipSide
{
    public class InteractionManager : Manager<InteractionManager>
    {
        private static List<IInteractor> _interactors = new List<IInteractor>();
        private static List<IInteractable> _interactables = new List<IInteractable>();
        
        #region MonoBehaviour

        private void FixedUpdate()
        {
            
        }

        #endregion MonoBehaviour

        #region Interactor

        public static void RegisterInteractor(IInteractor interactor)
        {
            if (interactor == null) return;

            if (_interactors.Contains(interactor)) return;
            _interactors.Add(interactor);
        }

        public static void UnregisterInteractor(IInteractor interactor)
        {
            if (interactor == null) return;

            if (!_interactors.Contains(interactor)) return;
            _interactors.Remove(interactor);
        }

        private void ProcessInteractors()
        {
            foreach (var interactor in _interactors)
            {
                if (!interactor.TryGetTargets(out var targets))
                {
                    continue;
                }
                
                
            }
        }

        #endregion Interactor

        #region Interactable

        public static void RegisterInteractable(IInteractable interactable)
        {
            if (interactable == null) return;

            if (_interactables.Contains(interactable)) return;
            _interactables.Add(interactable);
        }

        public static void UnregisterInteractable(IInteractable interactable)
        {
            if (interactable == null) return;

            if (!_interactables.Contains(interactable)) return;
            _interactables.Remove(interactable);
        }

        #endregion Interactable
    }
}
