using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableScript : MonoBehaviour
{
    public class Triangle
    {
        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;
        private Vector2 uv1;
        private Vector2 uv2;
        private Vector2 uv3;

        public Triangle (Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.uv1 = uv1;
            this.uv2 = uv2;
            this.uv3 = uv3;
        }

        public bool IsWithinRadius(Vector3 point, float r)
        {
            return Vector3.Distance(point, p1) < r ||
                Vector3.Distance(point, p2) < r ||
                Vector3.Distance(point, p3) < r;
        }
    }

    public class TriangleTreeNode
    {

    }

    private List<Triangle> triangles;


    // Start is called before the first frame update
    void Awake()
    {
        Mesh m = null;

        // generate uv clusters here
        if (TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
        {
            m = meshFilter.mesh;
        }

        if (m)
        {
            triangles = new List<Triangle>();

            for (int i = 0; i < m.triangles.Length / 3; ++i)
            {
                triangles.Add(new Triangle(
                    m.vertices[m.triangles[i * 3]],
                    m.vertices[m.triangles[i * 3 + 1]],
                    m.vertices[m.triangles[i * 3 + 2]],
                    m.uv[m.triangles[i * 3]],
                    m.uv[m.triangles[i * 3 + 1]],
                    m.uv[m.triangles[i * 3 + 2]]
                    ));
            }
        }
    }

    void Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out float u, out float v, out float w)
    {
        Vector2 v0 = b - a, v1 = c - a, v2 = p - a;
        float den = v0.x * v1.y - v1.x * v0.y;
        v = (v2.x * v1.y - v1.x * v2.y) / den;
        w = (v0.x * v2.y - v2.x * v0.y) / den;
        u = 1.0f - v - w;
    }
}
