using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayMagnitude
{
    public Vector3 origin;
    public Vector3 direction;

    public RayMagnitude(Vector3 origin, Vector3 direction)
    {
        this.origin = origin;
        this.direction = direction;
    }
}

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

    public bool CheckForPaintableSurface(RayMagnitude ray, out Vector3 intersectionPoint, out Vector3 normal)
    {
        Vector3 closestIntersection = new Vector3();
        // TODO: Finish programming for normal
        Vector3 intersectionNormal = new Vector3();
        bool foundIntersection = false;

        // brute force method
        for (int i = 0; i < paintableScripts.Count; i++)
        {
            if (paintableScripts[i].GetComponent<MeshExtend>().GetClosestRayIntersectionPoint(ray, out Vector3 objectIntersection, out MeshExtend.MeshTriangle meshTriangle))
            {
                closestIntersection = objectIntersection;
                foundIntersection = true;
            }
        }

        intersectionPoint = closestIntersection;
        normal = intersectionNormal;

        return foundIntersection;
    }
}
