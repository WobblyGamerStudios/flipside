using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class Interactable : MonoBehaviour
    {
        private const string LOG_FORMAT = nameof(Interactable) + ".{0} :: {1}";

        [SerializeField] private GameObject _ui;

        private void Awake()
        {
            _ui.SetActive(false);
        }

        public virtual void OnEnter()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnEnter), "");
            _ui.SetActive(true);
        }

        public virtual void OnExit()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnExit), "");
            _ui.SetActive(false);
        }

        public void OnActivate()
        {
            Debug.LogFormat(LOG_FORMAT, nameof(OnActivate), "");
        }
    }
}
