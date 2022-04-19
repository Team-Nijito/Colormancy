using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePaintingManager : MonoBehaviour
{
    private Texture2D tex;
    public Material mat;

    // Start is called before the first frame update
    void Start()
    {
        tex = new Texture2D(64, 64);
        tex.SetPixel(2, 2, new Color(0, 0, 0, 0));
        tex.Apply();

        mat.SetTexture("_PaintTex", tex);

        PaintSphere();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PaintSphere()
    {

    }
}
