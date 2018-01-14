using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMarioController : PlayerController {

	override protected void Start () {
		base.Start ();
		playerState = PlayerState.fire;
		playerTileHeight = AppConsts.BIG_MARIO_HEIGHT;
		LoadParameters ();
	}
}
