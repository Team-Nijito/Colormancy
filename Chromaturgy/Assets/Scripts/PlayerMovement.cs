using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 0f;
    [SerializeField] private float RunSpeed = 0f;

    private CharacterController controller = null;

    // Start is called before the first frame update
    void Start() => controller = GetComponent<CharacterController>();

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
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        }.normalized;

        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(movement * RunSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(movement * walkSpeed * Time.deltaTime);
        }
    }


}
