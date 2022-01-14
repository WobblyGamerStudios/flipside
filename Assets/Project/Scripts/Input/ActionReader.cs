using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Wgs.FlipSide
{
    public class ActionReader : MonoBehaviour
    {
        [SerializeField] private InputActionReference _reference;

        [Title("Debug")]
        [OnInspectorGUI]
        [ShowInInspector]
        public string Bindings => _reference ? string.Join(",", _reference.action.bindings) : "None";
    }
}
