using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballController : MonoBehaviour
{
    public Color paintColor;
    public float explosionRadius;
    public float centerThreshold;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            Matrix4x4 localToWorld = collision.transform.localToWorldMatrix;
            
            Collider[] hitColliders = Physics.OverlapSphere(collision.GetContact(0).point, explosionRadius);

            // check all colliders in area
            foreach (Collider c in hitColliders)
            {
                Mesh mesh = c.gameObject.GetComponent<MeshFilter>().mesh;
                localToWorld = c.transform.localToWorldMatrix;

                // cloned to prevent multiple referencing by creating a shallow reference
                Vector3[] originalVertices = (Vector3[])mesh.vertices.Clone();
                Color[] originalColors = (Color[])mesh.colors.Clone();

                if (mesh.colors.Length == 0)
                {
                    originalColors = new Color[originalVertices.Length];

                    for (int i = 0; i < originalColors.Length; i++)
                        originalColors[i] = new Color(1, 1, 1, 1);
                }
                Color[] colors = new Color[originalVertices.Length];

                for (int i = 0; i < colors.Length; i++)
                {
                    // create vectors for calculations
                    Vector3 worldVertex = localToWorld.MultiplyPoint3x4(originalVertices[i]);

                    if (Vector3.Distance(worldVertex, collision.GetContact(0).point) < explosionRadius)
                    {
                        float l = (explosionRadius - Vector3.Distance(worldVertex, collision.GetContact(0).point)) / explosionRadius;
                        if (l > centerThreshold)
                            l = 1;
                        // Perlin smoothstep 
                        l = l * l * l * (l * (l * 6 - 15) + 10);

                        // jank smoothing calculation to create solid center
                        // use bitmask for lerp
                        colors[i].r = Mathf.Lerp(paintColor.r, originalColors[i].r, Mathf.Clamp(1 - l * 2, 0, 1));
                        colors[i].g = Mathf.Lerp(paintColor.g, originalColors[i].g, Mathf.Clamp(1 - l * 2, 0, 1));
                        colors[i].b = Mathf.Lerp(paintColor.b, originalColors[i].b, Mathf.Clamp(1 - l * 2, 0, 1));
                        colors[i].a = Mathf.Clamp(originalColors[i].a - l, 0, 1);
                    }
                    else
                        colors[i] = originalColors[i];
                }

                mesh.colors = colors;
            }

            Destroy(gameObject);
        }

    }
}
