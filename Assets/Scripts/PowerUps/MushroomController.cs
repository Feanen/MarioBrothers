using UnityEngine;

public class MushroomController : PowerUpsController {

	override protected void Start () {

		isWalkingRight = Random.value > .5f;
		playerState = PlayerController.PlayerState.big;
		base.Start ();
	}
}	

