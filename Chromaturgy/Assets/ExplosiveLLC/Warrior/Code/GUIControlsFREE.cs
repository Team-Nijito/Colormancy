using UnityEngine;

namespace WarriorAnimsFREE
{
	public class GUIControlsFREE:MonoBehaviour
	{
		private WarriorControllerFREE warriorController;
		private WarriorMovementControllerFREE warriorMovementController;

		private void Awake()
		{
			warriorController = GetComponent<WarriorControllerFREE>();
			warriorMovementController = GetComponent<WarriorMovementControllerFREE>();
		}

		private void OnGUI()
		{
			if(warriorController.canAction)
			{
				Attacking();
				Jumping();
			}
		}

		private void Attacking()
		{
			if(warriorMovementController.MaintainingGround())
			{
				if(!warriorMovementController.isMoving)
				{
					if(GUI.Button(new Rect(25, 85, 100, 30), "Attack1"))
					{
						warriorController.Attack(1);
					}
				}
			}
		}

		private void Jumping()
		{
			if(warriorMovementController.canJump && warriorController.canAction)
			{
				if(warriorMovementController.MaintainingGround())
				{
					if(GUI.Button(new Rect(25, 175, 100, 30), "Jump"))
					{
						if(warriorMovementController.canJump)
						{
							warriorMovementController.currentState = WarriorState.Jump;
							warriorMovementController.warriorState = WarriorState.Jump;
						}
					}
				}
			}
		}
	}
}