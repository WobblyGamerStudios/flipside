using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

namespace Wgs.FlipSide
{
    public static class CharacterControllerExtensions
    {
        public static void Warp(this CharacterController controller, Vector3 position)
        {
            controller.Warp(position, controller.transform.rotation);
        }

        public static void Warp(this CharacterController controller, Vector3 position, Quaternion rotation)
        {
            controller.enabled = false;
            controller.transform.position = position;
            controller.transform.rotation = rotation;
            controller.enabled = true;
        }
    }
}
