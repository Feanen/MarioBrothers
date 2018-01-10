using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qCoinBlockController : Block {

	[SerializeField]
	private float coinMoveSpeed;
	[SerializeField]
	private float gravity;

	void FixedUpdate () {

		UpdateAnimation ();
	}

	void InstantiateCoin () {

		//GameObject instance = Instantiate(Resources.Load("enemy", typeof(GameObject))) as GameObject;

		GameObject coin = Instantiate (Resources.Load( AppConsts.COIN_OBJECT_PATH, typeof(GameObject))) as GameObject;
		coin.transform.SetParent (this.transform.parent);
		coin.transform.localPosition = new Vector2 (originPosition.x, originPosition.y + AppConsts.APP_TILE_HEIGHT);
		StartCoroutine (MoveCoin (coin));
	}

	IEnumerator MoveCoin(GameObject coin) {

		while (true) {

			coin.transform.localPosition = new Vector2 (coin.transform.localPosition.x, coin.transform.localPosition.y + coinMoveSpeed * Time.deltaTime);

			coinMoveSpeed -= gravity;

			if (coinMoveSpeed < 0)
				break;

			yield return null;
		}

		while (true) {

			coin.transform.localPosition = new Vector2 (coin.transform.localPosition.x, coin.transform.localPosition.y + coinMoveSpeed * Time.deltaTime);

			coinMoveSpeed -= gravity;

			if (coin.transform.localPosition.y <= originPosition.y + AppConsts.APP_TILE_HEIGHT) {

				Destroy (coin.gameObject);
				break;
			}

			yield return null;
		}
	}

	override protected IEnumerator Bounce() {

		InstantiateCoin ();

		StartCoroutine(base.Bounce ());

		blockState = BlockState.final;
		canBounce = false;

		yield return null;
	}
}
