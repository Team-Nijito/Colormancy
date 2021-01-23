using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    // Handles the behavior of mouse reticle, and turning the player (towards the mouse)

    //public GameObject m_thingToSpawn = null;
    public bool m_isPlayerFacingMouse = false;

    [SerializeField]
    private GameObject m_playerCharacter = null;

    private RaycastHit m_data;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0))
        {
            //Instantiate(m_thingToSpawn, mousePosition, Quaternion.identity);
            if (m_data.collider.gameObject)
            {
                //print(m_data.collider.name);
                DebugClickDamage(5);
            }
        }
        if (m_isPlayerFacingMouse)
        {
            PlayerFacingMouse(mousePosition);
        }
    }

    private void DebugClickDamage(int damage)
    {
        // the collider is attached to an enemy IF
        // 1) the current gameObject the collider is attached to has a healthscript
        // 2) the current gameObject is a child (or descendent) of any gameObject that has a healthscript
        HealthScript hscript = m_data.collider.gameObject.GetComponent<HealthScript>();
        hscript = hscript == null ? m_data.collider.gameObject.GetComponentInParent<HealthScript>() : hscript;
        if (hscript)
        {
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
            Vector3 targetPosition = new Vector3(mousePos.x, transform.position.y, mousePos.z);
            m_playerCharacter.transform.LookAt(targetPosition);
        }
    }
}
