using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wgs.Core;

public class EdgePointDetection : MonoBehaviour
{
    public LayerMask _layerMask;

    private Vector3 _point;
    
    private void Update()
    {
        var ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, _layerMask))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            MeshFilter filter = hit.transform.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;

            int index = hit.triangleIndex * 3;
            _point = mesh.vertices[mesh.triangles[index + 1]];
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.white);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.25f);

        Gizmos.color = CoreColor.Purple;
        Gizmos.DrawSphere(_point, 0.1f);
    }

    public struct DebugData
    {
        public float dot;
        public float distance;
    }
}
