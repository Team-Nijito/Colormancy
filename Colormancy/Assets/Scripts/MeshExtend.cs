using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshExtend : MonoBehaviour
{
    // houses the octree
    public class MeshTriangle
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;

        public MeshTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv1, Vector2 uv2, Vector3 uv3)
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
        public List<TriangleTreeNode> treeNodes;
        public List<MeshTriangle> triangles;
        public Bounds bounds;

        public TriangleTreeNode(Bounds bounds)
        {
            this.bounds = bounds;
            triangles = new List<MeshTriangle>();
            treeNodes = new List<TriangleTreeNode>();
        }
    }

    private List<MeshTriangle> triangles;
    public int TriangleCount { get { return triangles.Count; } }
    private TriangleTreeNode octTreeRoot;

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
            triangles = new List<MeshTriangle>();

            for (int i = 0; i < m.triangles.Length / 3; ++i)
            {
                triangles.Add(new MeshTriangle(
                    m.vertices[m.triangles[i * 3]],
                    m.vertices[m.triangles[i * 3 + 1]],
                    m.vertices[m.triangles[i * 3 + 2]],
                    m.uv[m.triangles[i * 3]],
                    m.uv[m.triangles[i * 3 + 1]],
                    m.uv[m.triangles[i * 3 + 2]]
                    ));
            }

            // build the tree

            Bounds bounds = m.bounds;
            octTreeRoot = new TriangleTreeNode(bounds);
            octTreeRoot.triangles = triangles;

            CreateOctree(octTreeRoot, 0, 2);
        }
    }

    private void CreateOctree(TriangleTreeNode node, int depth, int maxDepth)
    {
        if (depth > maxDepth)
            return;

        ++depth;

        //     6 7
        // 2 3 4 5
        // 0 1

        for (int o = 0; o < 8; ++o)
        {
            Bounds newBounds = new Bounds();

            switch (o)
            {
                case 0:
                    newBounds.SetMinMax(node.bounds.min, node.bounds.center);
                    break;
                case 1:
                    newBounds.SetMinMax(
                        node.bounds.min + new Vector3(node.bounds.size.x / 2, 0, 0),
                        node.bounds.center + new Vector3(node.bounds.size.x / 2, 0, 0)
                        );
                    break;
                case 2:
                    newBounds.SetMinMax(
                        node.bounds.min + new Vector3(0, node.bounds.size.y / 2, 0),
                        node.bounds.center + new Vector3(0, node.bounds.size.y / 2, 0)
                        );
                    break;
                case 3:
                    newBounds.SetMinMax(
                        node.bounds.min + new Vector3(node.bounds.size.x / 2, node.bounds.size.y / 2, 0),
                        node.bounds.center + new Vector3(node.bounds.size.x / 2, node.bounds.size.y / 2, 0)
                        );
                    break;
                case 4:
                    newBounds.SetMinMax(
                        node.bounds.min + new Vector3(0, 0, node.bounds.size.z / 2),
                        node.bounds.center + new Vector3(0, 0, node.bounds.size.z / 2)
                        );
                    break;
                case 5:
                    newBounds.SetMinMax(
                        node.bounds.min + new Vector3(node.bounds.size.x / 2, 0, node.bounds.size.z / 2),
                        node.bounds.center + new Vector3(node.bounds.size.x / 2, 0, node.bounds.size.z / 2)
                        );
                    break;
                case 6:
                    newBounds.SetMinMax(
                        node.bounds.min + new Vector3(0, node.bounds.size.y / 2, node.bounds.size.z / 2),
                        node.bounds.center + new Vector3(0, node.bounds.size.y / 2, node.bounds.size.z / 2)
                        );
                    break;
                case 7:
                    newBounds.SetMinMax(node.bounds.center, node.bounds.max);
                    break;
            }

            TriangleTreeNode newNode = new TriangleTreeNode(newBounds);
            for (int i = 0; i < node.triangles.Count; ++i)
            {
                if (newBounds.Contains(node.triangles[i].p1) ||
                    newBounds.Contains(node.triangles[i].p2) ||
                    newBounds.Contains(node.triangles[i].p3))
                    newNode.triangles.Add(node.triangles[i]);
            }
            node.treeNodes.Add(newNode);
            CreateOctree(newNode, depth, maxDepth);
        }
    }

    /// <summary>
    /// Returns ranges from [0, 1] for u, v, w based on a, b, c respectively
    /// </summary>
    /// <param name="p"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <param name="w"></param>
    public static void Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out float u, out float v, out float w)
    {
        Vector2 v0 = b - a, v1 = c - a, v2 = p - a;
        float den = v0.x * v1.y - v1.x * v0.y;
        v = (v2.x * v1.y - v1.x * v2.y) / den;
        w = (v0.x * v2.y - v2.x * v0.y) / den;
        u = 1.0f - v - w;
    }

    public MeshTriangle GetTriangle(int triangleIndex)
    {
        return triangles[triangleIndex];
    }

    public bool GetClosestRayIntersectionPoint(RayMagnitude ray, out Vector3 closestWorldIntersectionPoint, out MeshTriangle meshTriangle)
    {
        // TODO: improve this ray check
        Vector3 closestPoint = new Vector3();
        float closestDistance = 1000f;
        int closestTriangleIndex = 0;

        ray.origin = transform.InverseTransformPoint(ray.origin);

        for (int i = 0; i < triangles.Count; i++)
        {
            if (IsRayIntersectingTriangle(ray, triangles[i], out Vector3 worldIntersectionPoint))
            {
                if (closestDistance > Vector3.Distance(ray.origin, worldIntersectionPoint))
                {
                    closestDistance = Vector3.Distance(ray.origin, worldIntersectionPoint);
                    closestPoint = worldIntersectionPoint;
                    closestTriangleIndex = i;
                }
            }
        }

        closestWorldIntersectionPoint = closestPoint;
        closestWorldIntersectionPoint = transform.TransformPoint(closestWorldIntersectionPoint);
        meshTriangle = triangles[closestTriangleIndex];
        
        if (closestDistance == 1000f)
        {
            return false;
        }

        return true;
    }

    public static bool IsRayIntersectingTriangle(RayMagnitude ray, MeshTriangle m, out Vector3 localIntersectionPoint)
    {
        localIntersectionPoint = Vector3.zero;

        Vector3 edge1, edge2, h, s, q;
        float a, f, u, v;
        edge1 = m.p2 - m.p1;
        edge2 = m.p2 - m.p1;
        h = Vector3.Cross(ray.direction, edge2);
        a = 1 - Mathf.Abs(Vector3.Dot(edge1, h));
        if (a > -Mathf.Epsilon && a < Mathf.Epsilon)
        {
            return false;    // This ray is parallel to this triangle.
        }
        f = 1.0f / a;
        s = ray.origin - m.p1;
        u = f * Vector3.Dot(s, h);
        if (u < 0.0 || u > 1.0)
        {
            return false;
        }
        q = Vector3.Cross(s, edge1);
        v = f * Vector3.Dot(ray.direction, q);
        if (v < 0.0 || u + v > 1.0)
        {
            return false;
        }
        // At this stage we can compute t to find out where the intersection point is on the line.
        float t = f * Vector3.Dot(edge2, q);
        if (t > Mathf.Epsilon) // ray intersection
        {
            localIntersectionPoint = ray.origin + ray.direction * t;
            return true;
        }
        else // This means that there is a line intersection but not a ray intersection.
            return false;
    }
}
