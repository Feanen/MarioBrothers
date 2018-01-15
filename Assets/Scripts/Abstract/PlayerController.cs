using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour {

	[SerializeField]
	protected LayerMask groundMask;
	[SerializeField]
	protected LayerMask wallMask;
	[SerializeField]
	protected Vector2 velocity; 
	[SerializeField]
	protected float jumpHeight;
	[SerializeField]
	protected float bounceHeight;
	[SerializeField]
	protected float gravity;
	[SerializeField]
	protected float maxSpeed = 2;


	protected bool walk, walk_right, walk_left, jump;
	protected bool grounded = false;
	protected bool bounce = false;
	//private bool smallToBigStateComplete = false;
	protected bool enableInput = true;

	protected float playerTileWidth;
	protected float playerTileHeight;
	protected float acceleration = .001f;

	protected Animator animator;

	public enum PlayerState
	{
		small,
		big,
		fire,
		invulnerable
	}

	protected PlayerState playerState;

	public enum AnimationState
	{
		idle,
		walking,
		jumping,
		attacking,
		crouching,
		dragging,
		bouncing,
		transforming
	}

	protected AnimationState animState = AnimationState.idle;

	protected virtual void Start() {

		animator = GetComponent<Animator> ();
		playerTileWidth = AppConsts.MARIO_WIDTH;
	}
		
	protected virtual void FixedUpdate() {

		CheckInput ();

		UpdatePosition ();

		CheckAnimationStates ();

	}

	protected virtual void UpdatePosition() {

		Vector3 pos = transform.localPosition;
		Vector3 scale = transform.localScale;

		if (walk) {

			if (velocity.x < maxSpeed) {
				velocity = new Vector2 (velocity.x + acceleration, velocity.y);
				if (animator.GetCurrentAnimatorStateInfo (0).IsName("walking")) {
					animator.speed = velocity.x;
				}
			}

			if (walk_left) {
				pos.x -= velocity.x * Time.deltaTime;
				scale.x = - AppConsts.OBJECT_SCALE;
			} else if (walk_right) {
				pos.x += velocity.x * Time.deltaTime;
				scale.x = AppConsts.OBJECT_SCALE;
			}

			pos = CheckWallRays (pos, scale.x);
		}

		if (jump && animState != AnimationState.jumping && !bounce) {

			animState = AnimationState.jumping;
			velocity = new Vector2 (velocity.x, jumpHeight);
			grounded = false;
		}

		if (animState == AnimationState.jumping ) {

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

	protected virtual void CheckInput() {

		if (enableInput) {
			
			bool input_left = Input.GetKey (KeyCode.LeftArrow);
			bool input_right = Input.GetKey (KeyCode.RightArrow);
			bool input_space = Input.GetKey (KeyCode.Space);

			walk = input_left || input_right;
			walk_left = input_left && !input_right;
			walk_right = !input_left && input_right;
			jump = input_space;
		} else {
			jump = false;
			Fall ();	
		}
	}

	protected virtual void CheckAnimationStates() {

		if (animState != AnimationState.transforming) {
			
			if (walk && grounded) {
				animator.SetBool (AppAnimationVariables.IS_WALKING, true);
				animator.SetBool (AppAnimationVariables.IS_JUMPING, false);
			}

			if (!walk && grounded) {
				animator.SetBool (AppAnimationVariables.IS_WALKING, false);
				animator.SetBool (AppAnimationVariables.IS_JUMPING, false);
			}

			if ( jump || velocity.y < -Time.deltaTime * 10) {
				animator.SetBool (AppAnimationVariables.IS_JUMPING, true);
			}
		} 
	}

	protected virtual Vector3 CheckGroundRays(Vector3 pos) {
		
		Vector2 originLeft = new Vector2 (pos.x - (playerTileWidth * .5f - .02f), pos.y);
		Vector2 originMiddle = new Vector2 (pos.x, pos.y);
		Vector2 originRight = new Vector2 (pos.x + (playerTileWidth * .5f - .02f), pos.y);
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

			if (hitRay.collider.tag == AppTagsAndLayers.ENEMY_TAG && hitRay.point.y >= hitRay.collider.transform.position.y /*+ hitRay.collider.bounds.size.y / 2*/) {

				hitRay.collider.gameObject.GetComponent<IEnemy> ().Crush ();

				bounce = true;
			}

			pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2/* + AppConsts.APP_TILE_HEIGHT*/;

			if (bounce && hitRay.collider.tag == AppTagsAndLayers.ENEMY_TAG)
				grounded = false;
			else
				grounded = true;

			animState = AnimationState.idle;

			velocity.y = 0;

		} else {

			if (animState != AnimationState.jumping ) {

				Fall ();
			}
		}

		return pos;
	}

	protected virtual Vector3 CheckWallRays(Vector3 pos, float dir) {
		
		//стартовая точка лучей
		Vector2 originTop = new Vector2 (pos.x + dir * (playerTileWidth * .5f - .01f), pos.y + playerTileHeight - .01f);
		Vector2 originMiddle = new Vector2 (pos.x + dir * (playerTileWidth * .5f - .01f), pos.y + playerTileHeight * .5f);
		Vector2 originBottom = new Vector2 (pos.x + dir * (playerTileWidth * .5f - .01f), pos.y + .01f);

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
	protected virtual Vector3 CheckCeilingRays (Vector3 pos) {

		Vector2 originLeft = new Vector2 (pos.x - (playerTileWidth * .5f - .02f), pos.y + playerTileHeight -.01f);
		Vector2 originMiddle = new Vector2 (pos.x, pos.y + playerTileHeight -.01f);
		Vector2 originRight = new Vector2 (pos.x + (playerTileWidth * .5f - .02f), pos.y + playerTileHeight -.01f);

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

			if (hitRay.collider.tag == AppTagsAndLayers.GROUND_TAG && hitRay.collider.GetComponent<Block>()) {

				hitRay.collider.GetComponent<Block> ().HittingBlock ( playerState );
			}

			pos.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - playerTileHeight;

			Fall ();
		}

		return pos;
	}
		
	protected virtual void Fall() {

		velocity.y = 0;
		animState = AnimationState.jumping;
		grounded = false;
		bounce = false;
	}

	//getters / setters
	public PlayerState getPlayerState() {
		return playerState;
	}

	public void setPlayerState(PlayerState plState) {
		playerState = plState;
	}

	protected void SaveParameters() {
		PlayerPrefs.SetInt ("groundMask", groundMask.value);
		PlayerPrefs.SetInt ("wallMask", wallMask.value);
		PlayerPrefs.SetFloat ("velocity.x", velocity.x);
		PlayerPrefs.SetFloat ("velocity.y", velocity.y);
		PlayerPrefs.SetFloat ("gravity", gravity);
		PlayerPrefs.SetFloat ("jumpHeight", jumpHeight);
		PlayerPrefs.SetFloat ("bounceHeight", bounceHeight);
		PlayerPrefs.SetInt ("firstSave", 1);
	}

	protected void LoadParameters() {
		groundMask.value = PlayerPrefs.GetInt ("groundMask");
		wallMask.value = PlayerPrefs.GetInt ("wallMask");
		velocity = new Vector2 (PlayerPrefs.GetFloat("velocity.x"), PlayerPrefs.GetFloat("velocity.y"));
		gravity = PlayerPrefs.GetFloat("gravity");
		jumpHeight = PlayerPrefs.GetFloat ("jumpHeight");
		bounceHeight = PlayerPrefs.GetFloat ("bounceHeight");
	}
}
