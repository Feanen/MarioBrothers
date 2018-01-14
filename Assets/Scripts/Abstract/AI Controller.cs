using UnityEngine;

public abstract class AIController : MonoBehaviour {
	
	[SerializeField]
	protected Vector2 velocity;
	[SerializeField]
	protected float gravity;
	[SerializeField]
	protected LayerMask groundMask;
	[SerializeField]
	protected LayerMask wallMask;

	protected bool grounded;
	protected bool isWalkingRight = false;

	public enum AIState
	{
		falling,
		walking
	}

	protected AIState state = AIState.falling;

	protected virtual void Start () {

		grounded = false;
		Fall ();
	}

	protected virtual void FixedUpdate () {

		UpdatePosition ();
	}

	protected virtual void UpdatePosition() {

		Vector3 pos = transform.localPosition;
		Vector3 scale = transform.localScale;

		if (state == AIState.falling) {

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

	protected virtual Vector3 CheckGroundRays(Vector3 pos) {

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

			state = AIState.walking;
		} else {
			if (state != AIState.falling)
				Fall ();
		}

		return pos;
	}

	protected virtual void CheckWallRays(Vector3 pos, float dir) {

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

			if (hitRay.collider.tag == AppTagsAndLayers.GROUND_TAG ) {

				isWalkingRight = !isWalkingRight;
			}
		}
	}

	protected void Fall() {

		velocity.y = 0;
		state = AIState.falling;
		grounded = false;
	}
}
