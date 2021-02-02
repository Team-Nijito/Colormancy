using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTester : MonoBehaviour
{
    private Mesh mesh;

    public bool enableSpiral;

    public float angleSpeed = 0.01f;
    public float radius = 0;
    public Color color;
    private float angle;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(1, 1, 1, 0);
        }

        mesh.colors = colors;
    }

    void FixedUpdate()
    {
        if (enableSpiral)
        {
            float x = radius * Mathf.Sin(angle);
            float z = radius * Mathf.Cos(angle);

            Vector3 spherePosition = new Vector3(x, 0, z);

            angle += angleSpeed;
            radius += angleSpeed / 2;

            if (angle > 2 * Mathf.PI)
                angle -= 2 * Mathf.PI;

            Vector3[] vertices = mesh.vertices;
            Color[] colors = mesh.colors;

            for (int i = 0; i < colors.Length; i++)
            {
                if (Vector3.Distance(vertices[i], spherePosition) < 0.5)
                    colors[i] = color;
            }

            // assign the array of colors to the Mesh.
            mesh.colors = colors;
        }
    }
}
