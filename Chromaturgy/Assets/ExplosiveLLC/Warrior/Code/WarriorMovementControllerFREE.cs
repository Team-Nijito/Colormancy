using UnityEngine;

namespace WarriorAnimsFREE
{
    public enum WarriorState
    {
        Idle = 0,
        Move = 1,
        Jump = 2,
        DoubleJump = 3,
        Fall = 4,
    }

    public class WarriorMovementControllerFREE:SuperStateMachine
    {
        //Components.
        private SuperCharacterController superCharacterController;
        private WarriorControllerFREE warriorController;
        private WarriorInputControllerFREE warriorInputController;
        private Rigidbody rb;
        private Animator animator;
        public WarriorState warriorState;

        [HideInInspector] public Vector3 lookDirection { get; private set; }

        //Jumping.
        [HideInInspector] public bool canJump;
        public float gravity = 25.0f;
        public float jumpAcceleration = 5.0f;
        public float jumpHeight = 3.0f;

        //Movement.
        [HideInInspector] public Vector3 currentVelocity;
        [HideInInspector] public bool isMoving = false;
        [HideInInspector] public bool canMove = true;
		public float movementAcceleration = 90.0f;
        public float walkSpeed = 4f;
        public float runSpeed = 6f;
        private readonly float rotationSpeed = 40f;
        public float groundFriction = 50f;

        //Air control.
        public float inAirSpeed = 6f;

        private void Start()
        {
            superCharacterController = GetComponent<SuperCharacterController>();
            warriorController = GetComponent<WarriorControllerFREE>();
            warriorInputController = GetComponent<WarriorInputControllerFREE>();
            animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody>();
            if(rb != null)
            {
                //Set restraints on startup if using Rigidbody.
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
            //Set currentState to idle on startup.
            currentState = WarriorState.Idle;
			warriorState = WarriorState.Idle;
			SwitchCollisionOn();
        }

		#region Updates

		/*void Update () {
		 * Update is normally run once on every frame update. We won't be using it in this case, since the SuperCharacterController component sends a callback Update called SuperUpdate. SuperUpdate is recieved by the SuperStateMachine, and then fires further callbacks depending on the state
		}*/

		//Put any code in here you want to run BEFORE the state's update function. This is run regardless of what state you're in.
		protected override void EarlyGlobalSuperUpdate()
        {
        }

        //Put any code in here you want to run AFTER the state's update function.  This is run regardless of what state you're in.
        protected override void LateGlobalSuperUpdate()
        {
            //Move the player by our velocity every frame.
            transform.position += currentVelocity * superCharacterController.deltaTime;
            //If alive and is moving, set animator.
            if(canMove)
            {
                if(currentVelocity.magnitude > 0 && warriorInputController.HasMoveInput())
                {
                    isMoving = true;
                    animator.SetBool("Moving", true);
                    animator.SetFloat("Velocity Z", currentVelocity.magnitude);
                }
                else
                {
                    isMoving = false;
                    animator.SetBool("Moving", false);
					animator.SetFloat("Velocity Z", 0);
				}
            }
            RotateTowardsMovementDir();
        }

		#endregion

		#region Gravity / Jumping

		private bool AcquiringGround()
        {
            return superCharacterController.currentGround.IsGrounded(false, 0.01f);
        }

        public bool MaintainingGround()
        {
			return superCharacterController.currentGround.IsGrounded(true, 0.5f);
        }

        public void RotateGravity(Vector3 up)
        {
            lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
        }

        // Calculate the initial velocity of a jump based off gravity and desired maximum height attained
        private float CalculateJumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }

		#endregion

		#region States

		//Below are the state functions. Each one is called based on the name of the state, so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call Jump_SuperUpdate()
		private void Idle_EnterState()
        {
            superCharacterController.EnableSlopeLimit();
            superCharacterController.EnableClamping();
            canJump = true;
            animator.SetInteger("Jumping", 0);
			animator.SetBool("Moving", false);
        }

        //Run every frame we are in the idle state.
        private void Idle_SuperUpdate()
        {
            //If Jump.
            if(warriorInputController.allowedInput && warriorController.Jump())
            {
                currentState = WarriorState.Jump;
				warriorState = WarriorState.Jump;
                return;
            }
			//In air.
            if(!MaintainingGround())
            {
                currentState = WarriorState.Fall;
				warriorState = WarriorState.Fall;
                return;
            }
            if(warriorInputController.HasMoveInput() && canMove)
            {
                currentState = WarriorState.Move;
				warriorState = WarriorState.Move;
                return;
            }
            //Apply friction to slow to a halt.
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction * superCharacterController.deltaTime);
        }

        //Run once when exit the idle state.
        private void Idle_ExitState()
		{
        }

		//Run once when exit the idle state.
		private void Idle_MoveState()
		{
			animator.SetBool("Moving", true);
		}

		private void Move_SuperUpdate()
        {
            //If Jump.
            if(warriorInputController.allowedInput && warriorController.Jump())
            {
                currentState = WarriorState.Jump;
				warriorState = WarriorState.Jump;
                return;
            }
			//Fallling.
            if(!MaintainingGround())
            {
                currentState = WarriorState.Fall;
				warriorState = WarriorState.Fall;
                return;
            }
            //Set speed determined by movement type.
            if(warriorInputController.HasMoveInput() && canMove)
            {
                //Keep strafing animations from playing.
                animator.SetFloat("Velocity X", 0F);
                //Run.
                currentVelocity = Vector3.MoveTowards(currentVelocity, warriorInputController.moveInput * runSpeed, movementAcceleration * superCharacterController.deltaTime);
            }
            else
            {
                currentState = WarriorState.Idle;
				warriorState = WarriorState.Idle;
            }
        }

        private void Jump_EnterState()
        {
            superCharacterController.DisableClamping();
            superCharacterController.DisableSlopeLimit();
            currentVelocity += superCharacterController.up * CalculateJumpSpeed(jumpHeight, gravity);
            canJump = false;
            animator.SetInteger("Jumping", 1);
            animator.SetTrigger("JumpTrigger");
        }

        private void Jump_SuperUpdate()
        {
            Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
            Vector3 verticalMoveDirection = currentVelocity - planarMoveDirection;
            //Falling.
            if(currentVelocity.y < 0)
            {
                currentVelocity = planarMoveDirection;
                currentState = WarriorState.Fall;
				warriorState = WarriorState.Fall;
				return;
            }
            planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, warriorInputController.moveInput * inAirSpeed, jumpAcceleration * superCharacterController.deltaTime);
            verticalMoveDirection -= superCharacterController.up * gravity * superCharacterController.deltaTime;
            currentVelocity = planarMoveDirection + verticalMoveDirection;
        }

		private void Fall_EnterState()
		{
			superCharacterController.DisableClamping();
			superCharacterController.DisableSlopeLimit();
			canJump = false;
			animator.SetInteger("Jumping", 2);
			animator.SetTrigger("JumpTrigger");
		}

		private void Fall_SuperUpdate()
		{
			if(AcquiringGround())
			{
				currentVelocity = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
				currentState = WarriorState.Idle;
				warriorState = WarriorState.Idle;
				return;
			}
			//Normal gravity.
			currentVelocity -= superCharacterController.up * gravity * superCharacterController.deltaTime;
		}

		private void Fall_ExitState()
		{
			animator.SetInteger("Jumping", 0);
			animator.SetTrigger("JumpTrigger");
		}

		#endregion

		/// <summary>
		/// Character Dash.
		/// </summary>
		/// <param name="1">Forward.</param>
		/// <param name="2">Right.</param>
		/// <param name="3">Backward.</param>
		/// <param name="4">Left.</param>
		public void Dash(int dash)
        {
            animator.SetInteger("Action", dash);
            animator.SetTrigger("DashTrigger");
			warriorController.Lock(true, true, true, 0, warriorController.warriorTiming.TimingLock(warriorController.warrior, "dash"));
        }

        public void SwitchCollisionOff()
        {
            canMove = false;
            superCharacterController.enabled = false;
            animator.applyRootMotion = true;
            if(rb != null)
            {
                rb.isKinematic = false;
            }
        }

        public void SwitchCollisionOn()
        {
            canMove = true;
            superCharacterController.enabled = true;
            animator.applyRootMotion = false;
            if(rb != null)
            {
                rb.isKinematic = true;
            }
        }

        private void RotateTowardsMovementDir()
        {
            if(warriorInputController.moveInput != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(warriorInputController.moveInput), Time.deltaTime * rotationSpeed);
            }
        }

        //Keep character from moving.
        public void LockMovement()
        {
            animator.SetBool("Moving", false);
            animator.applyRootMotion = true;
			canMove = false;
            currentVelocity = new Vector3(0, 0, 0);
        }

        public void UnlockMovement()
        {
            canMove = true;
            animator.applyRootMotion = false;
        }
    }
}