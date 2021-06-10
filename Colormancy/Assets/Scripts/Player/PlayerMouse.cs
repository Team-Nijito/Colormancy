using UnityEngine;
using Photon.Pun;

public class PlayerMouse : MonoBehaviourPunCallbacks
{
    // Handles the behavior of mouse reticle, and turning the player (towards the mouse)

    [Tooltip("If you click within this radius from the player, the player will not turn to attack.")]
    [SerializeField]
    private float m_ignoreTurnRadius = 1f; // this prevents the player from glitching out if you click on the player directly

    private GameObject m_playerCharacter;

    private PlayerMovement m_pmScript;
    private PlayerAttack m_paScript;
    //private RaycastHit m_data;
    private Animator m_animator;

    private void Start()
    {
        m_pmScript = GetComponent<PlayerMovement>();
        m_paScript = GetComponent<PlayerAttack>();
        m_animator = GetComponentInChildren<Animator>();
        m_playerCharacter = m_pmScript.m_character;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            Vector3 mousePosition = GetMouseWorldPosition();

            if (Input.GetMouseButton(0) && m_pmScript.CanMove)
            {
                // paintball attack
                if ((new Vector3(mousePosition.x, 0, mousePosition.z) - new Vector3(transform.position.x, 0, transform.position.z)).magnitude > m_ignoreTurnRadius)
                {
                    // only turn player if we're not clicking directly on the player or near the player
                    PlayerFacingMouse(mousePosition);
                }

                if (m_animator && m_paScript.isAttackReady())
                {
                    // Trigger attack animation 
                    photonView.RPC("TriggerPlayerAttackAnim", RpcTarget.All);
                    photonView.RPC("ShootPaintball", RpcTarget.All, false, mousePosition);
                }
            }
        }
    }

    public Vector3 GetMouseWorldPosition(string focusTag = "", int depth = 200)
    {
        /// THIS IS NO LONGER IMPLEMENTED THE SAME WAY
        // Shoots a ray from the camera through the position of the mouse, and returns a Vector3
        // if this ray collides with any collider, otherwise returns Vector3.zero

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitData;
        //if (focusTag == "")
        //{
        //    if (Physics.Raycast(ray, out hitData, depth, ~m_layersToIgnore))
        //    {
        //        m_data = hitData;
        //        return hitData.point;
        //    }
        //    return Vector3.zero;
        //}
        //if (Physics.Raycast(ray, out hitData, depth) && hitData.transform.CompareTag(focusTag))
        //{
        //    m_data = hitData;
        //    return hitData.point;
        //}
        //return Vector3.zero;

        // NOW IS IMPLEMENTED BASED ON FLAT PLANE GENERATED FROM USER POSITION
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 planeCenter = transform.position;
        Vector3 planeNormal = Vector3.up;

        float t = Vector3.Dot((planeCenter - ray.origin), planeNormal) / Vector3.Dot(ray.direction, planeNormal);

        return ray.origin + ray.direction * t;
    }

    public void PlayerFacingMouse(Vector3 mousePos)
    {
        if (m_playerCharacter && mousePos != Vector3.zero)
        {
            // if you're testing out local player, and the among us-looking character is always looking down
            // replace the 0 below (2nd argument in Vector3 constructor) with transform.position.y
            Vector3 targetPosition = new Vector3(mousePos.x, m_playerCharacter.transform.position.y, mousePos.z);
            m_playerCharacter.transform.LookAt(targetPosition);
        }
    }

    [PunRPC]
    public void TriggerPlayerAttackAnim()
    {
        // Trigger attack animation 
        m_animator.SetInteger("Action", 1);
        m_animator.SetTrigger("AttackTrigger");
    }
}
