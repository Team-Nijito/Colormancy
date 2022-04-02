using UnityEngine;
using Photon.Pun;

public class EnableFreeCam : MonoBehaviourPunCallbacks
{
    private MonoBehaviour[] m_components = null;
    private GameObject[] m_transformChildren = null;

    [SerializeField] private GameObject m_freeCamPrefab = null;
    private bool m_inFreeCamMode = false;

    private GameObject m_cachedCanvas = null; // cached between switching from free cam and switching to

    // Start is called before the first frame update
    void Start()
    {
        MonoBehaviour[] allComponents = GetComponents<MonoBehaviour>();
        m_components = new MonoBehaviour[allComponents.Length - 2]; // all components except this one and the PhotonView
        int slow = 0;

        for (int i = 0; i < allComponents.Length; i++)
        {
            MonoBehaviour type = allComponents[i];
            if (type.GetType() != typeof(EnableFreeCam) && type.GetType() != typeof(PhotonView))
            {
                m_components[slow] = type;
                slow++;
            }
        }

        m_transformChildren = new GameObject[transform.childCount];
        for (int i = 0; i < m_transformChildren.Length; i++)
        {
            m_transformChildren[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (!m_inFreeCamMode && Input.GetKey(KeyCode.O))
        {
            m_inFreeCamMode = true;
            HandControlToFreeCam();
        }
    }

    private void EnableEverything()
    {
        foreach (GameObject g in m_transformChildren)
        {
            g.SetActive(true);
        }
        foreach (MonoBehaviour m in m_components)
        {
            m.enabled = true;
        }
    }

    private void DisableEverything()
    {
        foreach (MonoBehaviour m in m_components)
        {
            m.enabled = false;
        }
        foreach (GameObject g in m_transformChildren)
        {
            // Allow the mage character to remain
            if (g.name != "Mage")
            {
                g.SetActive(false);
            }
        }
    }

    private void SetCanvas(bool setActive)
    {
        // Enable / disable the canvas
        if (m_cachedCanvas == null)
        {
            m_cachedCanvas = GameObject.Find("Canvas");
        }
        m_cachedCanvas.SetActive(setActive);
    }

    public void HandControlFromFreeCam()
    {
        EnableEverything();
        SetCanvas(true);
        photonView.Owner.TagObject = gameObject;
        m_inFreeCamMode = false;
    }

    public void HandControlToFreeCam()
    {
        GameObject freeCam = Instantiate(m_freeCamPrefab);
        freeCam.transform.position = transform.position;
        DontDestroyOnLoad(freeCam);
        freeCam.GetComponent<FreeCam>().SetHumanForm(gameObject);
        photonView.Owner.TagObject = freeCam;
        DisableEverything();
        SetCanvas(false);
    }
}
