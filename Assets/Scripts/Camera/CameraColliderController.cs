using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColliderController : MonoBehaviour {

	private Collider2D coll;

	void Start()
	{
		coll = GetComponent<Collider2D> ();
		Debug.Log ("tests");
	}

	void OnTriggerEnter2D( Collider2D collider )
	{
		if (collider.gameObject.tag != "Player")
			Physics2D.IgnoreCollision (coll, collider);
	}
}
