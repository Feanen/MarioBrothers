using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField]
	private LayerMask groundMask;
	[SerializeField]
	private LayerMask wallMask;
	[SerializeField]
	private Vector2 velocity; 
	[SerializeField]
	private float jumpHeight;
	[SerializeField]
	private float bounceHeight;
	[SerializeField]
	private float gravity;

	private bool walk, walk_right, walk_left, jump;
	private bool grounded = false;
	private bool bounce = false;

	private Animator animator;

	public enum PlayerState
	{
		small,
		big,
		fire,
		invulnerable
	}

	private PlayerState playerState = PlayerState.small;

	public enum AnimationState
	{
		idle,
		walking,
		jumping,
		attacking,
		crouching,
		dragging,
		bouncing
	}

	private AnimationState animState = AnimationState.idle;

	void Start() {

		animator = GetComponent<Animator> ();
	}
		
	void FixedUpdate() {

		CheckInput ();

		UpdatePosition ();

		CheckAnimationStates ();
	}

	void UpdatePosition() {

		Vector3 pos = transform.localPosition;
		Vector3 scale = transform.localScale;

		if (walk) {
			if (walk_left) {
				pos.x -= velocity.x * Time.deltaTime;
				scale.x = -AppConsts.OBJECT_SCALE;
			} else if (walk_right) {
				pos.x += velocity.x * Time.deltaTime;
				scale.x = AppConsts.OBJECT_SCALE;
			}

			pos = CheckWallRays (pos, scale.x);
		}

		if (jump && animState != AnimationState.jumping) {

			animState = AnimationState.jumping;
			velocity = new Vector2 (velocity.x, jumpHeight);
			grounded = false;
		}

		if (animState == AnimationState.jumping) {

			pos.y += velocity.y * Time.deltaTime;
			velocity.y -= gravity * Time.deltaTime;
		}

		if (bounce && animState != AnimationState.bouncing) {

			animState = AnimationState.bouncing;
			velocity = new Vector2 (velocity.x, bounceHeight);
		}

		if (animState == AnimationState.bouncing) {

			pos.y += velocity.y * Time.deltaTime;
			velocity.y -= gravity * Time.deltaTime;
		}


		if (velocity.y <= 0) {
			pos = CheckGroundRays (pos);
		}

		if (velocity.y >= 0) {
			pos = CheckCeilingRays (pos);
		}

		transform.localPosition = pos;
		transform.localScale = scale;
	}

	void CheckInput() {

		bool input_left = Input.GetKey (KeyCode.LeftArrow);
		bool input_right = Input.GetKey (KeyCode.RightArrow);
		bool input_space = Input.GetKey (KeyCode.Space);

		walk = input_left || input_right;
		walk_left = input_left && !input_right;
		walk_right = !input_left && input_right;
		jump = input_space;
	}

	void CheckAnimationStates() {

		if (walk && grounded) {
			animator.SetBool (AppAnimationVariables.IS_WALKING, true);
			animator.SetBool (AppAnimationVariables.IS_JUMPING, false);
		}

		if (!walk && grounded) {
			animator.SetBool (AppAnimationVariables.IS_WALKING, false);
			animator.SetBool (AppAnimationVariables.IS_JUMPING, false);
		}

		if (jump) {
			animator.SetBool (AppAnimationVariables.IS_JUMPING, true);
		}
	}

	Vector3 CheckGroundRays(Vector3 pos) {
		
		Vector2 originLeft = new Vector2 (pos.x - .06f, pos.y - AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originMiddle = new Vector2 (pos.x, pos.y - AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originRight = new Vector2 (pos.x + .06f, pos.y - AppConsts.APP_TILE_HEIGHT / 2);
		RaycastHit2D groundLeft = Physics2D.Raycast (originLeft, Vector3.down, velocity.y * Time.deltaTime, groundMask);
		RaycastHit2D groundMiddle = Physics2D.Raycast (originMiddle, Vector3.down, velocity.y * Time.deltaTime, groundMask);
		RaycastHit2D groundRight = Physics2D.Raycast (originRight, Vector3.down, velocity.y * Time.deltaTime, groundMask);

		if (groundLeft.collider != null || groundMiddle.collider != null || groundRight.collider != null) {

			RaycastHit2D hitRay = groundLeft;

			if (groundLeft) {
				hitRay = groundLeft;
			} else if (groundMiddle) {
				hitRay = groundMiddle;
			} else if (groundRight) {
				hitRay = groundRight;
			}


//			if (hitRay.collider.tag == AppTagsAndLayers.ENEMY_TAG && pos.y >= hitRay.collider.transform.position.y + hitRay.collider.bounds.size.y) {
//
//				hitRay.collider.GetComponent<GoombaController> ().Crush ();
//				bounce = true;
//
//			} else if (hitRay.collider.tag == AppTagsAndLayers.ENEMY_TAG && this.transform.position.y < hitRay.collider.transform.position.y + hitRay.collider.bounds.size.y) {
//				
//			}

			pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + AppConsts.APP_TILE_HEIGHT / 2;

			grounded = true;

			animState = AnimationState.idle;

			velocity.y = 0;

		} else {

			if (animState != AnimationState.jumping) {

				Fall ();
			}
		}

		return pos;
	}

	Vector3 CheckWallRays(Vector3 pos, float dir) {
		
		//стартовая точка лучей
		Vector2 originTop = new Vector2 (pos.x + dir * .06f, pos.y + .06f);
		Vector2 originMiddle = new Vector2 (pos.x + dir * .06f, pos.y);
		Vector2 originBottom = new Vector2 (pos.x + dir * .06f, pos.y - .06f);

		//сами лучи
		RaycastHit2D wallTop = Physics2D.Raycast (originTop, new Vector2 (dir, 0), velocity.x * Time.deltaTime, wallMask);
		RaycastHit2D wallMiddle = Physics2D.Raycast (originMiddle, new Vector2 (dir, 0), velocity.x * Time.deltaTime, wallMask);
		RaycastHit2D wallBottom = Physics2D.Raycast (originBottom, new Vector2 (dir, 0), velocity.x * Time.deltaTime, wallMask);

		//проверка на коллизии
		if (wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null) {

			pos.x -= velocity.x * Time.deltaTime * dir;
		}

		return pos;
	}
		
	//метод, определяющий RayCasts для определения коллизий с объектами над головой
	Vector3 CheckCeilingRays (Vector3 pos) {

		Vector2 originLeft = new Vector2 (pos.x - .06f, pos.y + AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originMiddle = new Vector2 (pos.x, pos.y + AppConsts.APP_TILE_HEIGHT / 2);
		Vector2 originRight = new Vector2 (pos.x + .06f, pos.y + AppConsts.APP_TILE_HEIGHT / 2);

		RaycastHit2D ceilLeft = Physics2D.Raycast (originLeft, Vector3.up, velocity.y * Time.deltaTime, groundMask);
		RaycastHit2D ceilMiddle = Physics2D.Raycast (originMiddle, Vector3.up, velocity.y * Time.deltaTime, groundMask);
		RaycastHit2D ceilRight = Physics2D.Raycast (originRight, Vector3.up, velocity.y * Time.deltaTime, groundMask);

		if (ceilLeft.collider != null || ceilMiddle.collider != null || ceilRight.collider != null) {

			RaycastHit2D hitRay = ceilLeft;

			if (ceilLeft) {
				hitRay = ceilLeft;
			} else if (ceilMiddle) {
				hitRay = ceilMiddle;
			} else if (ceilRight) {
				hitRay = ceilRight;
			}

			/*if (hitRay.collider.tag == GameTagsAndLayers.QUESTION_BLOCK_TAG) {

				hitRay.collider.GetComponent<CoinInBlockController> ().QuestionBlockBounce ();
			}*/

			pos.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - AppConsts.APP_TILE_HEIGHT / 2;

			Fall ();
		}

		return pos;
	}


	void Fall() {

		velocity.y = 0;
		animState = AnimationState.jumping;
		grounded = false;
		bounce = false;
	}
}
