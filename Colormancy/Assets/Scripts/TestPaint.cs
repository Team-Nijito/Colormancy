using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPaint : MonoBehaviour
{
    public GameObject paintObject;

    public GameObject brushSphere;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log(paintObject.transform.InverseTransformPoint(Vector3.zero));

        PaintableScript paintableScript = paintObject.GetComponent<PaintableScript>();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            paintableScript.PaintMesh(Color.red, 0.1f);
        }

        SurfacePaintingManager.Instance.PaintSphere(brushSphere.transform.position, 1f, Color.red, 0.1f);

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RayMagnitude r = new RayMagnitude(brushSphere.transform.position, Vector3.down * 100f);

            if (SurfacePaintingManager.Instance.CheckForPaintableSurface(r, out Vector3 intersectionPoint, out Vector3 normal))
            {
                SurfacePaintingManager.Instance.PaintSphere(intersectionPoint, 1f, Color.red, 0.1f);
            }
        }
    }
}
