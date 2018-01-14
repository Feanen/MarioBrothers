using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerController : PowerUpsController {

	void Start () {
		playerState = PlayerController.PlayerState.fire;
		base.Start ();
	}
	
	override protected void UpdatePosition() {

		Vector3 pos = transform.localPosition;
		Vector3 scale = transform.localScale;

		CheckWallRays (pos, scale.x);

		CheckCeilingRays (pos);

		transform.localPosition = pos;
		transform.localScale = scale;
	}

	protected void CheckCeilingRays( Vector3 pos ) {

		Vector2 originLeft = new Vector2 (pos.x - .06f, pos.y + AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originMiddle = new Vector2 (pos.x, pos.y + AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originRight = new Vector2 (pos.x + .06f, pos.y + AppConsts.APP_TILE_HEIGHT / 2);

		RaycastHit2D ceilLeft = Physics2D.Raycast (originLeft, Vector3.up, velocity.y * Time.deltaTime, ceilMask);
		RaycastHit2D ceilMiddle = Physics2D.Raycast (originMiddle, Vector3.up, velocity.y * Time.deltaTime, ceilMask);
		RaycastHit2D ceilRight = Physics2D.Raycast (originRight, Vector3.up, velocity.y * Time.deltaTime, ceilMask);

		if (ceilLeft.collider != null || ceilMiddle.collider != null || ceilRight.collider != null) {

			RaycastHit2D hitRay = ceilLeft;

			if (ceilLeft) {
				hitRay = ceilLeft;
			} else if (ceilMiddle) {
				hitRay = ceilMiddle;
			} else if (ceilRight) {
				hitRay = ceilRight;
			}

			if (hitRay.collider.tag == AppTagsAndLayers.PLAYER_TAG ) {

				ChangePlayerState ( hitRay.collider.gameObject );
			}
		}
	}

	override protected void CheckWallRays(Vector3 pos, float dir) {

		Vector2 originTop = new Vector2 (pos.x + .07f * dir, pos.y + AppConsts.APP_TILE_HEIGHT / 2 - .01f);
		Vector2 originMiddle = new Vector2 (pos.x + .07f * dir, pos.y);
		Vector2 originBottom = new Vector2 (pos.x + .07f * dir, pos.y - AppConsts.APP_TILE_HEIGHT / 2 + .01f);

		RaycastHit2D wallTop = Physics2D.Raycast (originTop, new Vector2 (dir, 0), velocity.y * Time.deltaTime, wallMask);
		RaycastHit2D wallMiddle = Physics2D.Raycast (originMiddle, new Vector2 (dir, 0), velocity.y * Time.deltaTime, wallMask);
		RaycastHit2D wallBottom = Physics2D.Raycast (originBottom, new Vector2 (dir, 0), velocity.y * Time.deltaTime, wallMask);

		if (wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null) {

			RaycastHit2D hitRay = wallTop;

			if (wallTop) {
				hitRay = wallTop;
			} else if (wallMiddle) {
				hitRay = wallMiddle;
			} else if (wallBottom) {
				hitRay = wallBottom;
			}

			if (hitRay.collider.tag == AppTagsAndLayers.PLAYER_TAG) {

				ChangePlayerState (hitRay.collider.gameObject);
			}

		}
	}
}
