using UnityEngine;
using Photon.Pun;

public class SpellController : MonoBehaviour
{
    public enum SpellColor
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Violet,
        Brown,
        Silver,
        Maroon
    }

    public Mesh[] spellMeshes;
    public Material[] spellMaterials;

    private Task spellDecay;

    private SpellColor greater;
    private SpellColor lesser;
    private SpellColor shape;

    private Color paintColor;

    public float explosionRadius;
    public float centerThreshold;

    // call this right after initialization and before the next frame
    public void SetSpellColors(SpellColor g, SpellColor l, SpellColor s)
    {
        greater = g;
        lesser = l;
        shape = s;
    }

    void Start()
    {
        // load in the correct model based on the shape
        GetComponent<MeshFilter>().mesh = spellMeshes[(int)shape];

        // load in the material with the proper shader
        GetComponent<Renderer>().material = spellMaterials[(int)shape];
    }

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

            //spellDecay.Stop();
            //gameObject.SetActive(false);
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            Destroy(gameObject);
        }

    }

    // honest to god I have no idea how you're supposed to legitly change the color
    public void ChangeColor(Vector3 newColor)
    {
        paintColor = new Color(newColor.x, newColor.z, newColor.z);
    }

    // every spell has an associated decay rate which is represented by a Task which is basically a IEnumerator but with more features
    // (pausing, checking when Task is finished ... etc)
    //public void SetSpellDecay(Task countDown)
    //{
    //    spellDecay = countDown;
    //}
}
