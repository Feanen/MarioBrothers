using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour, IEnemy {

	[SerializeField]
	private Vector2 velocity;
	[SerializeField]
	private float gravity;
	[SerializeField]
	private float timeBeforeDestroy;
	[SerializeField]
	private LayerMask groundMask;
	[SerializeField]
	private LayerMask wallMask;

	private Animator animatorController;

	private bool grounded;
	private bool isWalkingRight = false;
	private bool shouldDie = false;
	private float timeToDestroy;

	public enum GoombaState
	{
		walking,
		dead,
		falling
	}

	private GoombaState state = GoombaState.falling;

	void Start () {
		
		animatorController = GetComponent<Animator> ();
		enabled = false;
		grounded = false;
		Fall ();
	}

	void FixedUpdate () {

		UpdatePosition ();

		CheckedCrush ();

		UpdateAnimationStates ();
	}

	void UpdateAnimationStates()
	{
		
		if (state == GoombaState.dead)
			animatorController.SetBool (AppAnimationVariables.IS_DEAD, true);
	}

	public void Crush() {

		state = GoombaState.dead;
		shouldDie = true;
	}

	void CheckedCrush() {
		
		if (shouldDie) {

			GetComponent<BoxCollider2D> ().enabled = false;

			if (timeToDestroy <= timeBeforeDestroy) {
				timeToDestroy += Time.deltaTime;
			} else {
				shouldDie = false;
				Destroy (this.gameObject);
			}

		}
	}

	void UpdatePosition() {

		if (state != GoombaState.dead) {
			
			Vector3 pos = transform.localPosition;
			Vector3 scale = transform.localScale;

			if (state == GoombaState.falling) {

				pos.y += velocity.y * Time.deltaTime;
				velocity.y -= gravity * Time.deltaTime;
			}

			if (isWalkingRight) {
				pos.x += velocity.x * Time.deltaTime;
				scale.x = 1;
			} else {
				pos.x -= velocity.x * Time.deltaTime;
				scale.x = -1;
			}

			if (velocity.y <= 0)
				pos = CheckGroundRays (pos);

			CheckWallRays (pos, scale.x);

			transform.localPosition = pos;
			transform.localScale = scale;
		}
	}

	Vector3 CheckGroundRays(Vector3 pos) {

		Vector2 originLeft = new Vector2 (pos.x - .05f, pos.y - AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originMiddle = new Vector2 (pos.x, pos.y - AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originRight = new Vector2 (pos.x + .05f, pos.y - AppConsts.APP_TILE_HEIGHT / 2);

		RaycastHit2D groundLeft = Physics2D.Raycast (originLeft, Vector2.down, velocity.y * Time.deltaTime, groundMask);
		RaycastHit2D groundMiddle = Physics2D.Raycast (originMiddle, Vector2.down, velocity.y * Time.deltaTime, groundMask);
		RaycastHit2D groundRight = Physics2D.Raycast (originRight, Vector2.down, velocity.y * Time.deltaTime, groundMask);

		if (groundLeft.collider != null || groundMiddle.collider != null || groundRight.collider != null) {

			RaycastHit2D ray = groundLeft;

			if (groundLeft)
				ray = groundLeft;
			else if (groundMiddle)
				ray = groundMiddle;
			else if (groundRight)
				ray = groundRight;

			pos.y = ray.collider.bounds.center.y + ray.collider.bounds.size.y / 2 + AppConsts.APP_TILE_HEIGHT / 2;
			velocity.y = 0;
			grounded = true;
			Debug.Log ("tesst");
			state = GoombaState.walking;
		} else {
			if (state != GoombaState.falling)
				Fall ();
		}

		return pos;
	}

	void CheckWallRays(Vector3 pos, float dir) {

		Vector2 originTop = new Vector2 (pos.x + .06f * dir, pos.y + AppConsts.APP_TILE_HEIGHT / 2 - .01f);
		Vector2 originMiddle = new Vector2 (pos.x + .06f * dir, pos.y);
		Vector2 originBottom = new Vector2 (pos.x + .06f * dir, pos.y - AppConsts.APP_TILE_HEIGHT / 2 + .01f);

		RaycastHit2D wallTop = Physics2D.Raycast (originTop, new Vector2(dir, 0), velocity.y * Time.deltaTime, wallMask);
		RaycastHit2D wallMiddle = Physics2D.Raycast (originMiddle, new Vector2(dir, 0), velocity.y * Time.deltaTime, wallMask);
		RaycastHit2D wallBottom = Physics2D.Raycast (originBottom, new Vector2(dir, 0), velocity.y * Time.deltaTime, wallMask);

		if (wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null) {

			RaycastHit2D hitRay = wallTop;

			if (wallTop) {
				hitRay = wallTop;
			} else if (wallMiddle) {
				hitRay = wallMiddle;
			} else if (wallBottom) {
				hitRay = wallBottom;
			}

			if (hitRay.collider.tag == AppTagsAndLayers.GROUND_TAG || ( hitRay.collider.tag == AppTagsAndLayers.ENEMY_TAG && hitRay.collider.gameObject != this.gameObject) ) {

				isWalkingRight = !isWalkingRight;
			}
		}
	}

	void OnBecameVisible() {

		this.enabled = true;
	}

	void Fall() {

		velocity.y = 0;
		state = GoombaState.falling;
		grounded = false;
	}
		
	public string GetScriptName() {
		Debug.Log (this.GetType ().Name);
		return this.GetType().Name;
	}
}
