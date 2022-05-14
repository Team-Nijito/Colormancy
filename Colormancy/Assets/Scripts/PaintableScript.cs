using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableScript : MonoBehaviour
{
    public MeshExtend meshExtend;
    private Texture2D paintTex;

    public int width;

    // Start is called before the first frame update
    void Awake()
    {
        if (!TryGetComponent<MeshExtend>(out MeshExtend meshExtend))
        {
            this.meshExtend = gameObject.AddComponent<MeshExtend>();
        }

        CreateTexture();
    }

    private void Start()
    {
        SurfacePaintingManager.Instance.AddPaintableScript(this);
    }

    public void CreateTexture()
    {
        paintTex = new Texture2D(100, 100);

        for (int i = 0; i < 100; ++i)
        {
            for (int j = 0; j < 100; ++j)
            {
                paintTex.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }
    }

    public void PaintMesh(Color c, float threshold)
    {
        for (int i = 0; i < meshExtend.TriangleCount; i++)
        {
            PaintTriangleUV(meshExtend.GetTriangle(i), transform.position, 100f, c, threshold);
        }

        ApplyPaintTexture();
    }

    public void PaintMesh(Vector3 worldPosition, float radius, Color c, float threshold)
    {
        // oh god do it the awful way
        for (int i = 0; i < meshExtend.TriangleCount; i++)
        {
            PaintTriangleUV(meshExtend.GetTriangle(i), worldPosition, radius, c, threshold);
        }

        ApplyPaintTexture();
    }

    private void ApplyPaintTexture()
    {
        paintTex.Apply();

        Material mat = GetComponent<MeshRenderer>().material;
        mat.SetTexture("_PaintTex", paintTex);
    }

    private void PaintTriangleUV(MeshExtend.MeshTriangle t, Vector3 sourceWorldPosition, float r, Color c, float threshold)
    {
        // get pixel location
        Vector2 v1 = t.uv1 * 100;
        Vector2 v2 = t.uv2 * 100;
        Vector2 v3 = t.uv3 * 100;

        int miny = (int)Mathf.Min(Mathf.Min(v1.y, v2.y), v3.y);
        int maxy = (int)Mathf.Max(Mathf.Max(v1.y, v2.y), v3.y);

        // fill in the texture based on these values
        for (int column = miny; column <= maxy; ++column)
        {
            // scanline method

            // find the min and max bounds using lines of the triangle
            float s = (v2.y - v1.y) / (v2.x - v1.x);
            // y - y1 = m(x - x1)
            // (y - y1)/m = x - x1
            // (y - y1)/m + x1 = x
            // clamp the value based on the line it's based on
            float x1 = Mathf.Clamp((column - v1.y) / s + v1.x, Mathf.Min(v1.x, v2.x), Mathf.Max(v1.x, v2.x));

            // do the same for the other lines
            s = (v3.y - v2.y) / (v3.x - v2.x);
            float x2 = Mathf.Clamp((column - v2.y) / s + v2.x, Mathf.Min(v2.x, v3.x), Mathf.Max(v2.x, v3.x));

            s = (v3.y - v1.y) / (v3.x - v1.x);
            float x3 = Mathf.Clamp((column - v1.y) / s + v1.x, Mathf.Min(v1.x, v3.x), Mathf.Max(v1.x, v3.x));

            int minx = (int)Mathf.Min(x1, Mathf.Min(x2, x3));
            int maxx = (int)Mathf.Max(x1, Mathf.Max(x2, x3));

            for (int row = minx; row <= maxx; ++row)
            {
                // calculate the barycentric coordinates to find world space position of uv
                float u, v, w;
                MeshExtend.Barycentric(
                    new Vector2(row, column),
                    v1,
                    v2,
                    v3,
                    out u,
                    out v,
                    out w);

                Vector3 localPos = u * t.p1 + v * t.p2 + w * t.p3;
                float distance = Vector3.Distance(transform.TransformPoint(localPos), sourceWorldPosition);

                if (distance < r)
                {
                    if (distance < r - threshold)
                    {
                        paintTex.SetPixel(row, column, c);
                    }
                    else
                    {
                        float l = Mathf.Clamp01(1 - (distance - (r - threshold)) / threshold);
                        Color currentPixelColor = paintTex.GetPixel(row, column);
                        l = Mathf.Max(l, currentPixelColor.a);

                        paintTex.SetPixel(row, column,
                            new Color(
                                c.r,
                                c.g,
                                c.b,
                                l));
                    }
                }
            }
        }
    }
}
