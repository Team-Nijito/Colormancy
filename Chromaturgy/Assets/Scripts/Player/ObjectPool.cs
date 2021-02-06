using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class ObjectPool : MonoBehaviour
{
    // This class is used to "pool" (instancing objects, deactivating them, reactivating and using them, but never deleting) 
    // player objects such as projectiles, look up "object pooling" for the pro and cons vs instancing and deleting gameobjects

    // PROBLEM: don't deal with object pooling for now, problems when players join and leave (extra objects stay when other players leave)
    // we can activate / deactivate ... but clean up is hard (lol). Just instantiate and delete.

    public enum ObjectType {
        None,
        Paintball
    }

    public static ObjectPool m_SharedInstance;

    private Transform m_objectPoolFolder;

    [SerializeField]
    private string m_paintballsFolderName = "Paintballs";
    private Transform m_paintballsFolder;

    private List<GameObject> m_paintballs;
    [SerializeField]
    private GameObject m_paintball;
    [SerializeField]
    private int m_amountPaintballsToPool;

    void Awake()
    {
        m_SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // currently instanciating and deleting rather than object pooling

        //GameObject tmp = GameObject.Find(m_paintballsFolderName);
        //m_paintballsFolder = tmp.transform;

        //m_paintballs = new List<GameObject>();
        //for(int i = 0; i < m_amountPaintballsToPool; ++i)
        //{
        //    tmp = Instantiate(m_paintball, m_paintballsFolder);
        //    tmp.SetActive(false);
        //    m_paintballs.Add(tmp);
        //}
    }

    public GameObject GetPooledObject(ObjectType oType)
    {
        if (ObjectType.Paintball == oType)
        {
            for (int i = 0; i < m_amountPaintballsToPool; ++i)
            {
                if (!m_paintballs[i].activeInHierarchy)
                {
                    return m_paintballs[i];
                }
            }
            return null;
        }
        return null;
    }

    public IEnumerator Deactivate(GameObject t, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Rigidbody tmp;
        if (tmp = t.GetComponent<Rigidbody>())
        {
            // if rigidbody exists, reset velocity
            tmp.velocity = Vector3.zero;
        }
        t.SetActive(false);
    }

    public int GetNumPooledObjects(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Paintball:
                return m_amountPaintballsToPool;
            default:
                return 0;
        }
    }

    public string GetNameOfPooledObjectsFolder(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Paintball:
                return m_paintballsFolderName;
            default:
                return "N/A Folder";
        }
    }
}
