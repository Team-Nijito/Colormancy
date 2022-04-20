using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableScript : MonoBehaviour
{
    public class Triangle
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;

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
        public List<TriangleTreeNode> treeNodes;
        public List<Triangle> triangles;
        public Bounds bounds;

        public TriangleTreeNode(Bounds bounds)
        {
            this.bounds = bounds;
            triangles = new List<Triangle>();
            treeNodes = new List<TriangleTreeNode>();
        }
    }

    private List<Triangle> triangles;
    private TriangleTreeNode octTreeRoot;

    public float threshold;

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

            // build the tree

            Bounds bounds = m.bounds;
            octTreeRoot = new TriangleTreeNode(bounds);
            octTreeRoot.triangles = triangles;

            CreateOctree(octTreeRoot, 0, 2);

            // test painting here
            TestTrianglePaint();
            //TestBarycentric();
        }
    }

    private Texture2D tex;

    private void TestTrianglePaint()
    {
        tex = new Texture2D(100, 100);

        for (int i = 0; i < 100; ++i)
        {
            for (int j = 0; j < 100; ++j)
            {
                tex.SetPixel(i, j, Color.black);
            }
        }

        for (int i = 0; i < octTreeRoot.triangles.Count; ++i)
        {
            PaintTriangleUV(octTreeRoot.triangles[i], Vector3.one, 1.5f);
        }

        tex.Apply();

        Material mat = GetComponent<MeshRenderer>().material;
        mat.SetTexture("_PaintTex", tex);
    }

    private void TestBarycentric()
    {
        float u, v, w;
        Barycentric(
            (octTreeRoot.triangles[0].uv1 + octTreeRoot.triangles[0].uv2) / 2,
            octTreeRoot.triangles[0].uv1,
            octTreeRoot.triangles[0].uv2,
            octTreeRoot.triangles[0].uv3,
            out u,
            out v,
            out w);

        print(u);
        print(v);
        print(w);
    }

    private void PaintTriangleUV(Triangle t, Vector3 source, float r)
    {
        // get pixel location
        Vector2 v1 = t.uv1 * 100;
        Vector2 v2 = t.uv2 * 100;
        Vector2 v3 = t.uv3 * 100;

        int miny = (int)Mathf.Min(Mathf.Min(v1.y, v2.y), v3.y);
        int maxy = (int)Mathf.Max(Mathf.Max(v1.y, v2.y), v3.y);

        // fill in the texture based on these values
        for (int row = miny; row <= maxy; ++row)
        {
            // scanline method

            // find the min and max bounds using lines of the triangle
            float s = (v2.y - v1.y) / (v2.x - v1.x);
            // y - y1 = m(x - x1)
            // (y - y1)/m = x - x1
            // (y - y1)/m + x1 = x
            // clamp the value based on the line it's based on
            float x1 = Mathf.Clamp((row - v1.y) / s + v1.x, Mathf.Min(v1.x, v2.x), Mathf.Max(v1.x, v2.x));

            // do the same for the other lines
            s = (v3.y - v2.y) / (v3.x - v2.x);
            float x2 = Mathf.Clamp((row - v2.y) / s + v2.x, Mathf.Min(v2.x, v3.x), Mathf.Max(v2.x, v3.x));

            s = (v3.y - v1.y) / (v3.x - v1.x);
            float x3 = Mathf.Clamp((row - v1.y) / s + v1.x, Mathf.Min(v1.x, v3.x), Mathf.Max(v1.x, v3.x));

            int minx = (int)Mathf.Min(x1, Mathf.Min(x2, x3));
            int maxx = (int)Mathf.Max(x1, Mathf.Max(x2, x3));

            for (int column = minx; column <= maxx; ++column)
            {
                // calculate the barycentric coordinates to find world space position of uv
                float u, v, w;
                Barycentric(
                    new Vector2(column, row),
                    v1,
                    v2,
                    v3,
                    out u,
                    out v,
                    out w);

                Vector3 worldPos = u * t.p1 + v * t.p2 + w * t.p3;
                float distance = Vector3.Distance(worldPos, source);

                if (distance < r)
                {
                    if (distance < r - threshold)
                    {
                        tex.SetPixel(column, row, Color.white);
                    }
                    else
                    {
                        float l = 1 - (distance - (r - threshold)) / threshold;

                        tex.SetPixel(column, row,
                            new Color(
                                l,
                                l,
                                l,
                                1));
                    }
                }
                    
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestTrianglePaint();
            Debug.Log("Painted!");
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
    void Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out float u, out float v, out float w)
    {
        Vector2 v0 = b - a, v1 = c - a, v2 = p - a;
        float den = v0.x * v1.y - v1.x * v0.y;
        v = (v2.x * v1.y - v1.x * v2.y) / den;
        w = (v0.x * v2.y - v2.x * v0.y) / den;
        u = 1.0f - v - w;
    }
}
