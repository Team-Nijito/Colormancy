using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriangleIntersection : MonoBehaviour
{
    private Vector3 p0 = new Vector3(0, 0, 0);
    private Vector3 p1 = new Vector3(1, 0, 0);
    private Vector3 p2 = new Vector3(0, 0, 1);

    private RayMagnitude rm;

    private void Start()
    {
        MeshExtend.MeshTriangle triangle = new MeshExtend.MeshTriangle(
            p0,
            p1,
            p2,
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(0, 0)
            );

        rm = new RayMagnitude(
            new Vector3(0, 5, 0),
            new Vector3(0, -100, 0)
            );

        if (MeshExtend.IsRayIntersectingTriangle(rm, triangle, out Vector3 localIntersectionPoint))
        {
            Debug.Log("what");
        }
    }

    private void Update()
    {
        Debug.DrawLine(p0, p1, Color.red);
        Debug.DrawLine(p2, p1, Color.red);
        Debug.DrawLine(p0, p2, Color.red);

        Debug.DrawRay(rm.origin, rm.direction, Color.red);
    }
}
