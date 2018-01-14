using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qPowerUpBlockDay : Block {

	[SerializeField]
	private float powerUpVerticalSpeed;

	override protected IEnumerator Bounce() {

		blockState = BlockState.final;
		canBounce = false;

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
				InstantiatePowerUp ();
				break;
			}

			yield return null;
		}
			
		yield return null;
	}

	private void InstantiatePowerUp() {

		GameObject powerUp = null;

		if (playerState.ToString () == AppConsts.PLAYER_STATE_SMALL) {
			powerUp = LoadRes (AppConsts.MUSHROOM_OBJECT_PATH);
		} else if (playerState.ToString () == AppConsts.PLAYER_STATE_BIG || playerState.ToString () == AppConsts.PLAYER_STATE_FIRE) {
			powerUp = LoadRes (AppConsts.FLOWER_OBJECT_PATH);
		}
					
		powerUp.transform.SetParent (this.transform.parent);
		powerUp.transform.localPosition = new Vector3 (originPosition.x, originPosition.y, transform.position.z + 1);
		StartCoroutine (MovePowerUp (powerUp));
	}

	IEnumerator MovePowerUp(GameObject powerUp) {

		while (true) {

			powerUp.transform.localPosition = new Vector2 (powerUp.transform.localPosition.x, powerUp.transform.localPosition.y + powerUpVerticalSpeed * Time.deltaTime);

			if (powerUp.transform.localPosition.y >= originPosition.y + AppConsts.APP_TILE_HEIGHT) {
				powerUp.transform.localPosition = new Vector2( originPosition.x, originPosition.y + AppConsts.APP_TILE_HEIGHT);
				break;
			}

			yield return null;
		}

	}

	private GameObject LoadRes( string path ) {
		return Instantiate (Resources.Load( path, typeof(GameObject))) as GameObject;
	}
}
