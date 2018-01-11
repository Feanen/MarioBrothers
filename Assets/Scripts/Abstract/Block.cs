using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour, IHitting {

	protected bool canBounce = true;

	protected float bounceHeight = AppConsts.BLOCK_BOUNCE_HEIGHT;
	protected float bounceSpeed = AppConsts.BLOCK_BOUNCE_SPEED;
	protected Vector2 originPosition;

	protected Animator animController;
	protected PlayerController.PlayerState playerState;

	public enum BlockState
	{
		hittable,
		final
	}

	protected BlockState blockState = BlockState.hittable;

	void Start () {
		
		animController = GetComponent<Animator> ();
		originPosition = transform.localPosition;
	}

	void FixedUpdate() {

		UpdateAnimation ();
	}

	protected void UpdateAnimation() {
		if (blockState == BlockState.final)
			animController.SetBool (AppAnimationVariables.NOT_HITTABLE, true);
	}

	public void HittingBlock( PlayerController.PlayerState plrState ) {

		playerState = plrState;

		if (canBounce) {
			StartCoroutine (Bounce ());
		}
	}

	virtual protected IEnumerator Bounce() {
		
		while (true) {
			
			transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + bounceSpeed * Time.deltaTime);

			if (transform.localPosition.y >= originPosition.y + bounceHeight) {
				break;
			}

			yield return null;
		}

		while (true) {

			transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - bounceSpeed * Time.deltaTime);

			if (transform.localPosition.y <= originPosition.y) {

				transform.localPosition = originPosition;
				break;
			}

			yield return null;
		}
	}
}
