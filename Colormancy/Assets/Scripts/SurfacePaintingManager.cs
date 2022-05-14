using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePaintingManager : MonoBehaviour
{
    private static SurfacePaintingManager _instance;
    public static SurfacePaintingManager Instance { get { return _instance; } }

    private List<PaintableScript> paintableScripts;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        // instantiate scripts
        paintableScripts = new List<PaintableScript>();
    }

    public void AddPaintableScript(PaintableScript script) {
        if (!paintableScripts.Contains(script))
            paintableScripts.Add(script);
    }

    public void PaintSphere(Vector3 worldPosition, float radius, Color c, float threshold)
    {
        // optimize later
        foreach (PaintableScript p in paintableScripts)
        {
            p.PaintMesh(worldPosition, radius, c, threshold);
        }
    }
}
