using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintParticleManager : MonoBehaviour
{
    public List<PaintParticleSystem> paintParticleSystems;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // update positions first
        foreach (PaintParticleSystem p in paintParticleSystems)
        {
            p.UpdatePositions();
        }

        // check for merging
        //for (int i = 0; < )
    }
}
