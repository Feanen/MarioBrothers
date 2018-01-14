using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMarioController : PlayerController {

	private bool bigToFireStateComplete = false;

	override protected void Start () {
		base.Start ();
		playerState = PlayerState.big;
		playerTileHeight = AppConsts.BIG_MARIO_HEIGHT;
		LoadParameters ();
	}

	override protected void CheckAnimationStates() {

		base.CheckAnimationStates ();

		if (!bigToFireStateComplete && playerState == PlayerState.fire) {

			animState = AnimationState.transforming;

			animator.SetBool (AppAnimationVariables.IS_CHANGING_STATE, true);

			enableInput = false;
			//Debug.Log (animator.GetCurrentAnimatorStateInfo (0).);

			if (animator.GetBool (AppAnimationVariables.IS_CHANGING_STATE) && animator.GetCurrentAnimatorStateInfo (0).IsName (AppAnimationVariables.ANIM_SMALL_TO_BIG)) {
				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime <= 1) {
					velocity = Vector3.zero;
				} else {
					animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load ("AnimationControllers/fireMarioController", typeof(RuntimeAnimatorController));
					velocity = Vector2.right;
					bigToFireStateComplete = !bigToFireStateComplete;
					enableInput = true;
					this.gameObject.AddComponent<FireMarioController> ();
					SaveParameters ();
					Destroy (this);
				}
			}	
		}
	}
}
