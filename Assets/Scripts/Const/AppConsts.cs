public abstract class AppConsts {

	public const float OBJECT_SCALE = 1.0f;
	public const float CAMERA_THRESHOLD_COEFFICIENT = 0.3f;
	public const float APP_TILE_HEIGHT = 0.16f;

	//Player consts
	public const string PLAYER_NAME = "Player";
	public const float MARIO_WIDTH = APP_TILE_HEIGHT;
	public const float SMALL_MARIO_HEIGHT = APP_TILE_HEIGHT;
	public const float BIG_MARIO_HEIGHT = APP_TILE_HEIGHT * 2;

	public const float BLOCK_BOUNCE_HEIGHT = APP_TILE_HEIGHT * .5f ;
	public const float BLOCK_BOUNCE_SPEED = 1;

	//path to resource objects
	public const string COIN_OBJECT_PATH = "Collectable/coin";
	public const string MUSHROOM_OBJECT_PATH = "Collectable/mushroom";
	public const string FLOWER_OBJECT_PATH = "Collectable/flower";

	//player states
	public const string PLAYER_STATE_SMALL = "small";
	public const string PLAYER_STATE_BIG = "big";
	public const string PLAYER_STATE_FIRE = "fire";
	public const string PLAYER_STATE_INVUL = "invulnerable";
}
