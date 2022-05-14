using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PaintBlob
{
    Vector3 position;
    Vector3 velocity;

}

public class PaintBlobScript : MonoBehaviour
{
    public List<PaintBlob> paintBlobs;

    public void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
