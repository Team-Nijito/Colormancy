using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintParticleSystem : MonoBehaviour
{
    public class PaintParticle
    {
        public float radius;
        public float color;
        public bool connected;

        public Vector3 worldPos;
        public Vector3 acceleration;
        public Vector3 velocity;

        public bool IsConnectedTo(PaintParticle paintParticle)
        {
            return Vector3.Distance(paintParticle.worldPos, worldPos) <= radius + paintParticle.radius / 2;
        }
    }

    public List<PaintParticle> particles;
    public Bounds boundingBox;

    public void UpdatePositions()
    {
        // check to see if there's an interaction for each set of particles
        // Ray velocityRay = 

        // update the position based on acceleration and velocity here

        // check to see if there needs to be a split
        /*
        for (int i = 0; i < particles.Count; i++)
        {
            if (particles[i].connected == false)
            {
                bool singleConnection = false;
                for (int j = i + 1; j < particles.Count; j++)
                {
                    if (particles[i].IsConnectedTo(particles[j]))
                    {
                        singleConnection = true;
                        particles[j].connected = true;
                    }
                }

                if (singleConnection == false)
                {
                    // make a new particle here
                }
            }

            // disconnect in the futuree
            particles[i].connected = false;
        }
        */

        boundingBox = GetBoundingBox();
        
        // set the cube size to reflect this bounding box
    }

    public Bounds GetBoundingBox()
    {
        Vector3 min = new Vector3(1000, 1000, 1000);
        Vector3 max = new Vector3(-1000, -1000, -1000);

        foreach (PaintParticle p in particles)
        {
            // check for min
            min.x = Mathf.Min(p.worldPos.x - p.radius, min.x);
            min.y = Mathf.Min(p.worldPos.y - p.radius, min.y);
            min.z = Mathf.Min(p.worldPos.z - p.radius, min.z);

            // check for max
            max.x = Mathf.Min(p.worldPos.x + p.radius, max.x);
            max.y = Mathf.Min(p.worldPos.y + p.radius, max.y);
            max.z = Mathf.Min(p.worldPos.z + p.radius, max.z);
        }

        Bounds boundingBox = new Bounds();
        boundingBox.SetMinMax(min, max);
        return boundingBox;
    }

    public void MergeParticles(PaintParticleSystem paintParticleSystem)
    {
        particles.AddRange(paintParticleSystem.particles);
    }
}
