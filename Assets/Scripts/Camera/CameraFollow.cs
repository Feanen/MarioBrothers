using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	private Camera cmr;
	private Collider2D coll;

	private Transform playerTrans;

	//private Vector2 topRightCorner;

	//public static float CAMERA_VIEWPORT_RIGHT_SIDE_X;

	private float xDifference;

	private float xThres;

	[SerializeField] 
	private float speed = 0;

	void Start () {

		cmr = GetComponent<Camera> ();
		coll = GetComponent<Collider2D> ();
		//optimizing X start coordinate for various aspect ratio
		transform.position = new Vector3 (cmr.orthographicSize * cmr.aspect, cmr.orthographicSize - AppConsts.APP_TILE_HEIGHT / 2, transform.position.z);
		coll.offset = new Vector2 ( - cmr.orthographicSize * cmr.aspect, Vector2.zero.y);
		//topRightCorner = new Vector2 (1, 1);
		playerTrans = GameObject.Find (AppConsts.PLAYER_NAME).transform;
		xThres = AppConsts.CAMERA_THRESHOLD_COEFFICIENT * cmr.orthographicSize * cmr.aspect;

	}

	void LateUpdate () {

		xDifference = playerTrans.position.x - transform.position.x;

		if ( xDifference >= xThres ) {
			transform.position = Vector3.MoveTowards (transform.position, new Vector3(playerTrans.position.x, transform.position.y, transform.position.z), speed * Time.deltaTime);
		}
	
		//Vector2 tmpPos = camera.ViewportToWorldPoint (topRightCorner);
		//CAMERA_VIEWPORT_RIGHT_SIDE_X = tmpPos.x;
	}

	void OnTriggerEnter2D( Collider2D collider )
	{
		if (collider.gameObject.tag != "Player")
			Physics2D.IgnoreCollision (coll, collider);
	}
}
