using System.Collections;
using UnityEngine;

namespace WarriorAnimsFREE
{
	public enum Warrior
	{
		Archer,
		Brute,
		Crossbow,
		Hammer,
		Karate,
		Knight,
		Mage,
		Ninja,
		Sorceress,
		Spearman,
		Swordsman,
		TwoHanded
	}

	public class WarriorControllerFREE:MonoBehaviour
    {
        //Components.
        [HideInInspector] public WarriorMovementControllerFREE warriorMovementController;
        [HideInInspector] public WarriorInputControllerFREE warriorInputController;
		[HideInInspector] public WarriorTimingFREE warriorTiming;
		[HideInInspector] public Animator animator;
        [HideInInspector] public IKHands ikHands;
		public Warrior warrior;

        //Strafing/action.
        [HideInInspector] public bool canAction = true;
		[HideInInspector] int attack;

		//Animation speed control. (doesn't affect lock timing)
		public float animationSpeed = 1;

        #region Initialization

        private void Awake()
        {
			//Get Movement Controller.
			warriorMovementController = GetComponent<WarriorMovementControllerFREE>();

			//Add Input and Timing Controllers.
            warriorInputController = gameObject.AddComponent<WarriorInputControllerFREE>();
			warriorTiming = gameObject.AddComponent<WarriorTimingFREE>();
			warriorTiming.warriorController = this;

			//Add IKHands.
			ikHands = GetComponent<IKHands>();

			//Setup Animator, add AnimationEvents script.
			animator = GetComponentInChildren<Animator>();
			if(animator == null)
			{
				Debug.LogError("ERROR: There is no Animator component for character.");
				Destroy(this);
			}
			else
			{
				animator.gameObject.AddComponent<WarriorCharacterAnimatorEvents>();
				animator.GetComponent<WarriorCharacterAnimatorEvents>().warriorController = this;
				animator.gameObject.AddComponent<AnimatorParentMove>();
				animator.GetComponent<AnimatorParentMove>().anim = animator;
				animator.GetComponent<AnimatorParentMove>().warriorMovementController = warriorMovementController;
				animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
				animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			}
		}

        #endregion

        #region Updates

        private void Update()
        {
            UpdateAnimationSpeed();
			//Character is on ground.
			if(warriorMovementController.MaintainingGround() && canAction)
			{
				Attacking();
			}
			Toggles();
        }

        private void UpdateAnimationSpeed()
        {
            animator.SetFloat("AnimationSpeed", animationSpeed);
        }

		#endregion

		#region Combat

		private void Attacking()
		{
			if(warriorInputController.inputAttack0)
			{
				Attack(1);
			}
		}

		public bool Jump()
		{
			return warriorInputController.inputJump;
		}

        public void Attack(int attackNumber)
        {
            if(canAction)
            {
                //Ground attacks.
                if(warriorMovementController.MaintainingGround())
                {
                    //Stationary attack.
                    if(!warriorMovementController.isMoving)
                    {
						Lock(true, true, true, 0, warriorTiming.TimingLock(warrior, ("attack" + attackNumber.ToString())));
					}
                }
                //Trigger the animation.
                animator.SetInteger("Action", attackNumber);
                animator.SetTrigger("AttackTrigger");
            }
        }

		IEnumerator _Attack1()
		{
			StopAllCoroutines();
			animator.SetInteger("Action", 1);
			animator.SetTrigger("AttackTrigger");
			Lock(true, true, true, 0, warriorTiming.TimingLock(warrior, "attack1"));
			attack = 1;
			yield return null;
		}

		#endregion

		#region Misc

		private void Toggles()
		{
			//Slow time toggle.
			if(Input.GetKeyDown(KeyCode.T))
			{
				if(Time.timeScale != 1)
				{
					Time.timeScale = 1;
				}
				else
				{
					Time.timeScale = 0.25f;
				}
			}
			//Pause toggle.
			if(Input.GetKeyDown(KeyCode.P))
			{
				if(Time.timeScale != 1)
				{
					Time.timeScale = 1;
				}
				else
				{
					Time.timeScale = 0f;
				}
			}
			//Debug toggle.
			if(Input.GetKeyDown(KeyCode.L))
			{
				VariablesDebug();
			}
		}

        /// <summary>
        /// Keep character from doing actions.
        /// </summary>
        private void LockAction()
        {
            canAction = false;
        }

        /// <summary>
        /// Let character move and act again.
        /// </summary>
        private void UnLock(bool movement, bool actions)
        {
            if(movement)
            {
                warriorMovementController.UnlockMovement();
            }
            if(actions)
            {
                canAction = true;
            }
        }

        private IEnumerator _GetCurrentAnimationLength()
        {
            yield return new WaitForEndOfFrame();
            float length = animator.GetCurrentAnimatorClipInfo(0).Length;
            Debug.Log(length);
        }

        /// <summary>
        /// Lock character movement and/or action, on a delay for a set time.
        /// </summary>
        /// <param name="lockMovement">If set to <c>true</c> lock movement.</param>
        /// <param name="lockAction">If set to <c>true</c> lock action.</param>
        /// <param name="timed">If set to <c>true</c> timed.</param>
        /// <param name="delayTime">Delay time.</param>
        /// <param name="lockTime">Lock time.</param>
        public void Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            StopCoroutine("_Lock");
            StartCoroutine(_Lock(lockMovement, lockAction, timed, delayTime, lockTime));
        }

        //Timed -1 = infinite, 0 = no, 1 = yes.
        public IEnumerator _Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            if(delayTime > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }
            if(lockMovement)
            {
                warriorMovementController.LockMovement();
            }
            if(lockAction)
            {
                LockAction();
            }
            if(timed)
            {
                if(lockTime > 0)
                {
                    yield return new WaitForSeconds(lockTime);
					UnLock(lockMovement, lockAction);
                }
            }
        }

        public void AnimatorDebug()
        {
            Debug.Log("ANIMATOR SETTINGS---------------------------");
            Debug.Log("Moving: " + animator.GetBool("Moving"));
            Debug.Log("Strafing: " + animator.GetBool("Strafing"));
            Debug.Log("Aiming: " + animator.GetBool("Aiming"));
            Debug.Log("Stunned: " + animator.GetBool("Stunned"));
            Debug.Log("Blocking: " + animator.GetBool("Blocking"));
            Debug.Log("Jumping: " + animator.GetInteger("Jumping"));
            Debug.Log("Action: " + animator.GetInteger("Action"));
            Debug.Log("Velocity X: " + animator.GetFloat("Velocity X"));
            Debug.Log("Velocity Z: " + animator.GetFloat("Velocity Z"));
        }

		public void VariablesDebug()
		{
			Debug.Log("VARIABLE SETTINGS---------------------------");
			Debug.Log("canAction: " + canAction);
			Debug.Log("attack: " + attack);
		}

		#endregion

	}
}