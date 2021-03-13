using UnityEngine;

namespace WarriorAnimsFREE
{
    public class WarriorInputControllerFREE:MonoBehaviour
    {
        //Inputs.
        [HideInInspector] public bool inputJump;
        [HideInInspector] public bool inputAttack0;
        [HideInInspector] public float inputHorizontal = 0;
        [HideInInspector] public float inputVertical = 0;

        //Variables
        [HideInInspector] public bool allowedInput = true;
        [HideInInspector] public Vector3 moveInput;

        /// <summary>
        /// Input abstraction for easier asset updates using outside control schemes.
        /// </summary>
        private void Inputs()
        {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
            inputVertical = Input.GetAxisRaw("Vertical");
            inputAttack0 = Input.GetButtonDown("Attack0");
            inputJump = Input.GetButtonDown("Jump");
        }

        private void Start()
        {
            allowedInput = true;
        }

        private void Update()
        {
            Inputs();
            moveInput = CameraRelativeInput(inputHorizontal, inputVertical);
        }

        /// <summary>
        /// Movement based off camera facing.
        /// </summary>
        private Vector3 CameraRelativeInput(float inputX, float inputZ)
        {
            //Forward vector relative to the camera along the x-z plane   
            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            //Right vector relative to the camera always orthogonal to the forward vector.
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            Vector3 relativeVelocity = inputHorizontal * right + inputVertical * forward;
            //Reduce input for diagonal movement.
            if(relativeVelocity.magnitude > 1)
            {
                relativeVelocity.Normalize();
            }
            return relativeVelocity;
        }

		/// <summary>
		/// Checks all input sources and returns if any are active.
		/// </summary>
		public bool HasAnyInput()
        {
            if(allowedInput && moveInput != Vector3.zero && inputJump != false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		/// <summary>
		/// Checks move input and returns if active.
		/// </summary>
		public bool HasMoveInput()
        {
            if(allowedInput && moveInput != Vector3.zero)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
	}
}