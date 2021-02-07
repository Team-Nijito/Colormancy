﻿using System.Collections;
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
                // sharedmesh because unity automatically draws instanced
                Mesh mesh = objects[i].GetComponent<MeshFilter>().sharedMesh;

                List<Vector3> vertices = new List<Vector3>();
                mesh.GetVertices(vertices);

                List<Color> colors = new List<Color>();

                // if there are no colors yet, then fill it all with a white color
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
        float l = 0;
        float colorLerp = 0;
        float originToVertex = 0;

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
                originToVertex = Vector3.Distance(worldVertex, origin);

                if (originToVertex < radius)
                {
                    l = originToVertex / radius;
                    if (l < threshold)
                        l = 0;
                    // Perlin smoothstep 
                    l = l * l * l * (l * (l * 6 - 15) + 10);

                    Color vertexColor = colors[k];

                    // jank smoothing calculation to create solid center
                    colorLerp = Mathf.Clamp(1 - (1 - l) * 2, 0, 1);

                    vertexColor.r = Mathf.Lerp(paintColor.r, colors[k].r, colorLerp);
                    vertexColor.g = Mathf.Lerp(paintColor.g, colors[k].g, colorLerp);
                    vertexColor.b = Mathf.Lerp(paintColor.b, colors[k].b, colorLerp);
                    // use bitmask for lerp
                    vertexColor.a = Mathf.Clamp(1 - l + colors[k].a, 0, 1);

                    colors[k] = vertexColor;
                }
            }

            mesh.SetColors(colors);
        }
    }

    public static void UnpaintSphere(Vector3 origin, float radius, float threshold = 0.5f) {
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius, 1 << 8);
        float l = 0;
        float colorLerp = 0;
        float originToVertex = 0;
        Color paintColor = new Color(1, 1, 1, 0);

        foreach (Collider c in hitColliders) {
            Mesh mesh = c.gameObject.GetComponent<MeshFilter>().mesh;
            Matrix4x4 localToWorld = c.transform.localToWorldMatrix;

            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            mesh.GetVertices(vertices);
            mesh.GetColors(colors);

            for (int k = 0; k < colors.Count; k++) {
                // create vectors for calculations
                Vector3 worldVertex = localToWorld.MultiplyPoint3x4(vertices[k]);
                originToVertex = Vector3.Distance(worldVertex, origin);

                if (originToVertex < radius) {
                    l = originToVertex / radius;
                    if (l < threshold)
                        l = 0;
                    // Perlin smoothstep 
                    l = l * l * l * (l * (l * 6 - 15) + 10);

                    Color vertexColor = colors[k];

                    // jank smoothing calculation to create solid center
                    colorLerp = Mathf.Clamp(1 - (1 - l) * 2, 0, 1);

                    vertexColor.r = Mathf.Lerp(paintColor.r, colors[k].r, colorLerp);
                    vertexColor.g = Mathf.Lerp(paintColor.g, colors[k].g, colorLerp);
                    vertexColor.b = Mathf.Lerp(paintColor.b, colors[k].b, colorLerp);
                    // use bitmask for lerp
                    vertexColor.a = Mathf.Clamp(l - colors[k].a, 0, 1);

                    colors[k] = vertexColor;
                }
            }

            mesh.SetColors(colors);
        }
    }

    // every x amount of fixed update ticks, do paint
    public static int paintingTickFrequency = 8;
}
