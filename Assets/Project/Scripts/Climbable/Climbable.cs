using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wgs.FlipSide
{
    public class Climbable : MonoBehaviour
    {
        [SerializeField] private bool AllowVertical;
        [SerializeField] private bool AllowHorizontal;

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}
