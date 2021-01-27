using UnityEngine;
using Photon.Pun;

public class PlayerMouse : MonoBehaviourPunCallbacks
{
    // Handles the behavior of mouse reticle, and turning the player (towards the mouse)
    [SerializeField]
    private float m_basicClickDamage = 5f;
    [SerializeField]
    private float m_basicClickManaConsumption = 10f;

    private GameObject m_playerCharacter = null;

    private PlayerMovement m_pmScript;
    private ManaScript m_mScript;
    private RaycastHit m_data;
    private Animator m_animator;

    private void Start()
    {
        m_mScript = GetComponent<ManaScript>();
        m_pmScript = GetComponent<PlayerMovement>();
        m_animator = GetComponentInChildren<Animator>();
        m_playerCharacter = m_pmScript.m_character;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0) && photonView.IsMine && PhotonNetwork.IsConnected) //  && !m_pmScript.m_isMoving
        {
            // Cannot attack while moving

            PlayerFacingMouse(mousePosition);
            
            // Trigger attack animation 
            m_animator.SetInteger("Action", 1);
            m_animator.SetTrigger("AttackTrigger");

            if (m_data.collider.gameObject)
            {
                //print(m_data.collider.name);
                DebugClickDamage(m_basicClickDamage);
            }
        }
    }

    private void DebugClickDamage(float damage)
    {
        // the collider is attached to an enemy IF
        // 1) the current gameObject the collider is attached to has a healthscript
        // 2) the current gameObject is a child (or descendent) of any gameObject that has a healthscript
        HealthScript hscript = m_data.collider.gameObject.GetComponent<HealthScript>();
        hscript = hscript == null ? m_data.collider.gameObject.GetComponentInParent<HealthScript>() : hscript;

        // test: each attack consumes 10 mana
        if (hscript && m_mScript.GetEffectiveMana() >= m_basicClickManaConsumption)
        {
            m_mScript.ConsumeMana(10);
            hscript.TakeDamage(damage);
        }
    }

    public Vector3 GetMouseWorldPosition(string focusTag = "", int depth = 200)
    {
        // Shoots a ray from the camera through the position of the mouse, and returns a Vector3
        // if this ray collides with any collider, otherwise returns Vector3.zero

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (focusTag == "")
        {
            if (Physics.Raycast(ray, out hitData, depth) && hitData.transform.tag != "Ignore Raycast")
            {
                m_data = hitData;
                return hitData.point;
            }
            return Vector3.zero;
        }
        if (Physics.Raycast(ray, out hitData, depth) && hitData.transform.tag == focusTag)
        {
            m_data = hitData;
            return hitData.point;
        }
        return Vector3.zero;
    }

    private void PlayerFacingMouse(Vector3 mousePos)
    {
        if (m_playerCharacter && mousePos != Vector3.zero)
        {
            // if you're testing out local player, and the among us-looking character is always looking down
            // replace the 0 below (2nd argument in Vector3 constructor) with transform.position.y
            Vector3 targetPosition = new Vector3(mousePos.x, 0, mousePos.z);
            m_playerCharacter.transform.LookAt(targetPosition);
        }
    }
}
