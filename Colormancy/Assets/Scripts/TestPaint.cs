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
    }
}
