using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    void Start()
    {
        // find all paintable gameobjects and set their initial colors
        GameObject [] objects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            // paintable layer
            if (objects[i].layer == 8)
            {
                Debug.Log(objects[i].name);
                Mesh mesh = objects[i].GetComponent<MeshFilter>().mesh;

                // cloned to prevent multiple referencing by creating a shallow reference
                List<Vector3> vertices = new List<Vector3>();
                mesh.GetVertices(vertices);

                List<Color> colors = new List<Color>();

                if (mesh.colors.Length == 0)
                {
                    for (int k = 0; k < vertices.Count; k++)
                        colors.Add(new Color(1, 1, 1, 0));
                }

                mesh.SetColors(colors);
            }
        }
    }

    public static void PaintSphere(Color paintColor, Vector3 origin, float radius, float threshold = 0.5f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius, 1 << 8);

        foreach (Collider c in hitColliders)
        {
            Mesh mesh = c.gameObject.GetComponent<MeshFilter>().mesh;
            Matrix4x4 localToWorld = c.transform.localToWorldMatrix;

            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            mesh.GetVertices(vertices);
            mesh.GetColors(colors);

            for (int k = 0; k < colors.Count; k++)
            {
                // create vectors for calculations
                Vector3 worldVertex = localToWorld.MultiplyPoint3x4(vertices[k]);

                if (Vector3.Distance(worldVertex, origin) < radius)
                {
                    float l = Vector3.Distance(worldVertex, origin) / radius;
                    if (l < radius * threshold)
                        l = 0;
                    // Perlin smoothstep 
                    l = l * l * l * (l * (l * 6 - 15) + 10);

                    // jank smoothing calculation to create solid center
                    // use bitmask for lerp
                    Color vertexColor = colors[k];

                    vertexColor.r = Mathf.Lerp(paintColor.r, colors[k].r, Mathf.Clamp(1 - (1 - l) * 2, 0, 1));
                    vertexColor.g = Mathf.Lerp(paintColor.g, colors[k].g, Mathf.Clamp(1 - (1 - l) * 2, 0, 1));
                    vertexColor.b = Mathf.Lerp(paintColor.b, colors[k].b, Mathf.Clamp(1 - (1 - l) * 2, 0, 1));
                    vertexColor.a = Mathf.Clamp(1 - l + colors[k].a, 0, 1);

                    colors[k] = vertexColor;
                }
            }

            mesh.SetColors(colors);
        }
    }
}
