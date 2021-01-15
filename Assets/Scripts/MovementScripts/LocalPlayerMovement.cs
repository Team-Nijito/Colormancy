using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromaturgy.MovementScripts
{
    // Debugging movement script

    [RequireComponent(typeof(CharacterController))]
    public class LocalPlayerMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 0f;

        private CharacterController controller = null;

        // Start is called before the first frame update
        void Start() => controller = GetComponent<CharacterController>();

        // Update is called once per frame
        void Update() => ProcessInput();

        private void ProcessInput()
        {
            // Movement vector is normalized so that diagonal movement isn't faster
            Vector3 movement = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0f,
                z = Input.GetAxisRaw("Vertical")
            }.normalized;

            //convert vector from local to world relative to camera to make isometric view
            movement = Camera.main.transform.TransformDirection(movement);

            // Time.deltatime is already handled for us in PlayerController SimpleMove
            controller.SimpleMove(movement * movementSpeed);
        }
    }
}
