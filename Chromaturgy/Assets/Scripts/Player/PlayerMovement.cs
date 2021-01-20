using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float m_walkSpeed = 0f;
    public float m_runSpeed = 0f;
    [SerializeField] private float m_gravity = 9.81f;

    private CharacterController m_controller = null;
    private float m_vSpeed = 0f; // current vertical velocity
    
    // Start is called before the first frame update
    void Start() => m_controller = GetComponent<CharacterController>();

    // Update is called once per frame
    void Update()
    {
        ProcessPlayerInput();
    }

    // Takes in player's iputs for movement
    private void ProcessPlayerInput()
    {
        // Normalize movement vector, so that we don't move faster when
        // we move diagonally
        Vector3 movement = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0,
            z = Input.GetAxisRaw("Vertical")
        }.normalized;

        //CONVERT direction from local to world relative to camera
        movement = Camera.main.transform.TransformDirection(movement);

        // Calculate gravity + update m_vSpeed and movement.y
        m_vSpeed -= m_gravity * Time.deltaTime;
        if (m_controller.isGrounded) m_vSpeed = 0;
        movement.y = m_vSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            m_controller.Move(movement * m_runSpeed * Time.deltaTime);
        else
            m_controller.Move(movement * m_walkSpeed * Time.deltaTime);
    }
}
