using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallMarioController : PlayerController {

	private bool smallToBigStateComplete = false;

	override protected void Start () {
		base.Start ();
		playerState = PlayerState.small;
		playerTileHeight = AppConsts.SMALL_MARIO_HEIGHT;
		//checkFirstSaveParameters();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	override protected void CheckAnimationStates() {

		base.CheckAnimationStates ();

		if (!smallToBigStateComplete && playerState == PlayerState.big) {

			animState = AnimationState.transforming;

			animator.SetBool (AppAnimationVariables.IS_CHANGING_STATE, true);

			enableInput = false;
			//Debug.Log (animator.GetCurrentAnimatorStateInfo (0).);

			if (animator.GetBool (AppAnimationVariables.IS_CHANGING_STATE) && animator.GetCurrentAnimatorStateInfo (0).IsName (AppAnimationVariables.ANIM_SMALL_TO_BIG)) {
				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime <= 1) {
					velocity = Vector3.zero;
				} else {
					animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load ("AnimationControllers/bigMarioController", typeof(RuntimeAnimatorController));
					velocity = Vector2.right;
					smallToBigStateComplete = !smallToBigStateComplete;
					enableInput = true;
					this.gameObject.AddComponent<BigMarioController> ();
					SaveParameters ();
					Destroy (this);
				}
			}	
		}
	}

	private void checkFirstSaveParameters() {

		if (PlayerPrefs.GetInt ("firstSave") == 1) {
			LoadParameters ();
		} else {
			SaveParameters ();
		}
	}
}
