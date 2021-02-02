using UnityEngine;

namespace WarriorAnimsFREE
{
	public class AnimatorParentMove:MonoBehaviour
	{
		public Animator anim;
		public WarriorMovementControllerFREE warriorMovementController;

		void OnAnimatorMove()
		{
			if(!warriorMovementController.isMoving)
			{
				transform.parent.rotation = anim.rootRotation;
				transform.parent.position += anim.deltaPosition;
			}
		}
	}
}
