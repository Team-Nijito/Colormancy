using Photon.Pun;
using UnityEngine;

namespace Chromaturgy.MovementScripts
{
    // This is meant for player movement and Photon
    // particularly addressing the issue of each player controlling their own character

    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviourPun
    {
        [SerializeField] private float movementSpeed = 0f;

        private CharacterController controller = null;

        // Start is called before the first frame update
        void Start() => controller = GetComponent<CharacterController>();

        // Update is called once per frame
        void Update()
        {
            // Only accept controls from the correct player
            if (photonView.IsMine)
            {
                ProcessInput();
            }
        }

        private void ProcessInput()
        {
            // Movement vector is normalized so that diagonal movement isn't faster
            Vector3 movement = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0f,
                z = Input.GetAxisRaw("Vertical")
            }.normalized;

            // Time.deltatime is already handled for us in PlayerController SimpleMove
            controller.SimpleMove(movement * movementSpeed);
        }
    }
}

