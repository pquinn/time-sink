using System;
using System.Linq;
using System.Collections.Generic;
using Autofac;
using log4net;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Objects;
using TimeSink.Entities.Inventory;
using Engine.Defaults;
using TimeSink.Entities.Actions;
using TimeSink.Entities.Triggers;
using TimeSink.Entities.Utils;

namespace TimeSink.Entities
{
    [SerializableEntity("defb4f64-1021-420d-8069-e24acebf70bb")]
    public class UserControlledCharacter : Entity, IHaveHealth, IHaveShield, IHaveMana
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UserControlledCharacter));
        private double nextLogTime = 0;
        private readonly double LOG_INTERVAL = 1000; //1 second = 1000 milliseconds

        const float MOTOR_TORQUE = 90;

        const float PLAYER_MASS = 130f;
        const string EDITOR_NAME = "User Controlled Character";
        const float DEPTH = -100f;

        private static readonly Guid GUID = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");
        enum BodyStates
        {
            #region neutral
            NeutralRight, NeutralLeft,
            NeutralRightTorch, NeutralLeftTorch,
            IdleRightOpen, IdleRightClosed, IdleLeftOpen, IdleLeftClosed,
            FacingBack, FacingForward,
            #endregion
            #region walking
            WalkingStartRight, WalkingRight, WalkingEndRight, WalkingStartLeft, WalkingLeft, WalkingEndLeft,
            WalkingShootRight, WalkingShoot2Right, WalkingShoot3Right, WalkingDrawnRight,
            WalkingShootLeft, WalkingShoot2Left, WalkingShoot3Left, WalkingDrawnLeft,
            WalkingTorchRight, WalkingTorchLeft, WalkingTorchStartLeft, WalkingTorchStartRight,
            WalkingTorchEndLeft, WalkingTorchEndRight,
            #endregion
            #region running
            RunningStartRight, RunningRight, RunningStopRight, RunningStartLeft, RunningLeft, RunningStopLeft,
            #endregion
            #region jumping
            JumpingRight, JumpingLeft,
            JumpingRightTorch, JumpingLeftTorch,
            #endregion
            #region shooting
            ShootingArrowRight, ShootingArrowLeft,
            ShootingArrowNeutRight, ShootingArrowNeutLeft,
            #endregion
            #region ducking
            DuckingRight, DuckingLeft, DuckingRightBow, DuckingLeftBow, DuckShootRightBow, DuckShootLeftBow,
            #endregion
            #region knockback
            KnockbackRight, KnockbackLeft,
            #endregion
            #region climbing
            ClimbingBack, ClimbingBackNeut,
            ClimbingLeft, ClimbingRight,
            ClimbingLeftNeutral, ClimbingRightNeutral,
            ClimbingLookRight, ClimbingLookLeft,
            HorizontalClimbLeft, HorizontalClimbRight, HorizontalClimbLeftNeut, HorizontalClimbRightNeut
            #endregion
        };
        
        BodyStates currentState;

        //Texture strings for content loading
        const string JUMP_SOUND_NAME = "Audio/Sounds/Hop";
        const string JUMP_IMPACT_SOUND = "Audio/Sounds/JumpImpact";
        const string ARROW_RELEASE_SOUND = "Audio/Sounds/ArrowRelease";
        const string TAKE_DAMAGE_SOUND = "Audio/Sounds/TakeDamage";

        const string EDITOR_PREVIEW = "Textures/Body_Neutral";

        #region neutral
        const string NEUTRAL_RIGHT = "Textures/Sprites/SpriteSheets/Body_Neutral";
        const string NEUTRAL_LEFT = "Textures/Sprites/SpriteSheets/Neutral_Left";
        const string IDLE_CLOSED_HAND = "Textures/Sprites/SpriteSheets/Idle_OpenHand";
        const string IDLE_OPEN_HAND = "Textures/Sprites/SpriteSheets/Idle_OpenHand";
        const string NEUTRAL_RIGHT_TORCH = "Textures/Sprites/SpriteSheets/Body_Walking_Right_Torch_Neut";
        const string NEUTRAL_LEFT_TORCH = "Textures/Sprites/SpriteSheets/Body_Walking_Torch_Left_Neut";
        #endregion
        #region walking
        const string WALKING_RIGHT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Right_Intermediate";
        const string WALKING_RIGHT = "Textures/Sprites/SpriteSheets/Body_Walking_Right";
        const string WALKING_LEFT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Intermediate_Left";
        const string WALKING_LEFT = "Textures/Sprites/SpriteSheets/BodyWalkLeft";
        const string WALKING_TORCH_RIGHT = "Textures/Sprites/SpriteSheets/Body_Walking_Right_Torch";
        const string WALKING_TORCH_LEFT = "Textures/Sprites/SpriteSheets/Body_Walking_Torch_Left";
        const string WALKING_TORCH_RIGHT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Right_Torch_Intermediate";
        const string WALKING_TORCH_LEFT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Walking_Left_Torch_Intermediate";
        #endregion
        #region Running
        const string RUNNING_RIGHT = "Textures/Sprites/SpriteSheets/RunningRight";
        const string RUNNING_LEFT = "Textures/Sprites/SpriteSheets/RunningLeft";
        const string RUNNING_RIGHT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Running_Intermediate_Right";
        const string RUNNING_LEFT_INTERMEDIATE = "Textures/Sprites/SpriteSheets/Body_Running_Intermediate_Left";
        #endregion
        #region jumping
        const string JUMPING_LEFT = "Textures/Sprites/SpriteSheets/JumpingLeft";
        const string JUMPING_RIGHT = "Textures/Sprites/SpriteSheets/Jumping_Right";
        const string JUMPING_RIGHT_TORCH = "Textures/Sprites/SpriteSheets/Jump_Torch_Right";
        const string JUMPING_LEFT_TORCH = "Textures/Sprites/SpriteSheets/Jump_Torch_Left";
        #endregion
        const string FACING_BACK = "Textures/Sprites/SpriteSheets/Backward";
        const string FACING_FORWARD = "Textures/Sprites/SpriteSheets/Facing_Forward";
        #region Knockback
        const string KNOCKBACK_RIGHT = "Textures/Sprites/SpriteSheets/KnockBackRight";
        const string KNOCKBACK_LEFT = "Textures/Sprites/SpriteSheets/KnockBackLeft";
        #endregion
        #region Ducking
        const string DUCK_LEFT = "Textures/Sprites/SpriteSheets/DuckingLeft";
        const string DUCK_RIGHT = "Textures/Sprites/SpriteSheets/DuckingRight";
        const string DUCK_LEFT_BOW = "Textures/Sprites/SpriteSheets/DuckLeftBow";
        const string DUCK_RIGHT_BOW = "Textures/Sprites/SpriteSheets/DuckingRightBow";
        const string DUCK_LEFT_SHOOT_BOW = "Textures/Sprites/SpriteSheets/DuckLeftShoot";
        const string DUCK_RIGHT_SHOOT_BOW = "Textures/Sprites/SpriteSheets/DuckShootRightBow";
        #endregion
        #region climbing
        const string CLIMBING_LEFT = "Textures/Sprites/SpriteSheets/ClimbingLeft";
        const string CLIMBING_RIGHT = "Textures/Sprites/SpriteSheets/ClimbingRight";
        const string CLIMBING_NEUTRAL_LEFT = "Textures/Sprites/SpriteSheets/ClimbingLeftNeut";
        const string CLIMBING_NEUTRAL_RIGHT = "Textures/Sprites/SpriteSheets/ClimbingRightNeut";
        const string CLIMBING_LOOKING_RIGHT = "Textures/Sprites/SpriteSheets/ClimbingLeftLookRight";
        const string CLIMBING_LOOKING_LEFT = "Textures/Sprites/SpriteSheets/ClimbingRightLookLeft";
        const string HORIZ_CLIMBING_LEFT = "Textures/Sprites/SpriteSheets/HorizClimbLeft";
        const string HORIZ_CLIMBING_RIGHT = "Textures/Sprites/SpriteSheets/HorizClimbRight";
        const string HORIZ_CLIMBING_LEFT_NEUT = "Textures/Sprites/SpriteSheets/HorizontalClimbLeftNeut";
        const string HORIZ_CLIMBING_RIGHT_NEUT = "Textures/Sprites/SpriteSheets/HorizontalClimbRightNeut";
        const string CLIMBING_BACK = "Textures/Sprites/SpriteSheets/ClimbingBack";
        const string CLIMBING_NEUT = "Textures/Sprites/SpriteSheets/ClimbingNeut";
        #endregion
        const string SHOOT_ARROW_RIGHT = "Textures/Sprites/SpriteSheets/ShootArrowRight";
        const string SHOOT_ARROW_LEFT = "Textures/Sprites/SpriteSheets/ShootArrowLeft";
        #region walking+shooting
        const string WALK_SHOOT_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShootLeft";
        const string WALK_SHOOT_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShootRight";
        const string WALK_SHOOT2_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot2Left";
        const string WALK_SHOOT2_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot2Right";
        const string WALK_SHOOT3_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot3Left";
        const string WALK_SHOOT3_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkShoot3Right";
        const string WALK_DRAWN_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkRightDrawn";
        const string WALK_DRAWN_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/BodyWalkLeftDrawn";
        const string SHOOT_NEUT_LEFT = "Textures/Sprites/SpriteSheets/ShootingWalking/Body_Neutral_Shooting_Left";
        const string SHOOT_NEUT_RIGHT = "Textures/Sprites/SpriteSheets/ShootingWalking/Body_Neutral_Shooting_Right";
        #endregion

        private Dictionary<BodyStates, NewAnimationRendering> animations;

        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 500;
        const float MAX_ARROW_INIT_SPEED = 1500;
        public static float X_OFFSET = PhysicsConstants.PixelsToMeters(-5);
        public static float Y_OFFSET = PhysicsConstants.PixelsToMeters(-30);
        const int RECHARGE_WAIT_TIME = 3000;
        const int RECHARGE_AMOUNT = 20;
        const int SHIELD_MAX = 50;
        const float HEAL_MANA_BURN_PER_MILLI = .003f;
        const float MANA_TO_HEALTH_SCALE = 2f;

        private SoundEffect jumpSound;
        private SoundEffect jumpImpactSound;
        private SoundEffect arrowSound;
        private SoundEffect takeDamageSound;
        private bool jumpToggleGuard = true;
        private Rectangle sourceRect;
        private float health;
        private float mana;
        private float shield;
        private Ladder canClimb = null;
        private bool climbing = false;
        private PlaceTorchTrigger onTorchGround;
        public Torch HoldingTorch { get; set; }
        private World _world;
        private IList<ItemPopup> popups = new List<ItemPopup>();
        public IList<ItemPopup> Popups { get { return popups; } set { popups = value; } }
        private IInventoryItem onPickup;
        public IInventoryItem OnPickup { get { return onPickup; } set { onPickup = value; } }
        public HashSet<DamageOverTimeEffect> Dots { get; set; }
        public DoorType DoorType { get; set; }

        public Ladder CanClimb { get { return canClimb; } set { canClimb = value; } }
        public bool Climbing { get { return climbing; } set { climbing = value; } }
        private bool InKnockback { get; set; }

        private bool manaRegenEnabled = true;
        private const float MANA_REGEN_RATE = .2f; //percent/sec
        private const float CHARGE_MANA_COST = 5f; //mana/percent
        public const float MAX_MANA = 100;
        public const float MAX_HEALTH = 100;
        private bool chargingWeapon = false;
        private float chargePercent = 0f;
        private int timeSinceLastHit = 0;

        private Emitter healEmitter;

        private bool playerControlled = true;

        public List<IInventoryItem> Inventory { get; set; }
        public override IMenuItem InventoryItem
        {
            get
            {
                return Inventory.Count != 0
                    ? Inventory[activeItem]
                    : null;
            }
        }
        private int activeItem;
        private ItemPopup currentItemPrompt;

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(EDITOR_PREVIEW)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Rotation = playerRotation
                };
            }
        }

        [SerializableField]
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        [SerializableField]
        public float Shield
        {
            get { return shield; }
            set { shield = value; }
        }

        [SerializableField]
        public float Mana
        {
            get { return mana; }
            set { mana = value; }
        }

        private float playerRotation = 0.0f;
        private int facing = 1; // 1 for right, -1 for left

        // not sure if these should be public
        private Vector2 direction;
        [SerializableField]
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        private double holdTime;
        [SerializableField]
        public double HoldTime
        {
            get { return holdTime; }
            set { holdTime = value; }
        }
        private bool inHold;
        [SerializableField]
        public bool InHold
        {
            get { return inHold; }
            set { inHold = value; }
        }

        float timer = 0f;
        float shotTimer = 0f;
        float invulnTimer = 0f;
        float damageTimer = 0f;
        float knockbackTimer = 0f;
        const float knockbackDur = 500f;
        const float invulnInterval = 1500f;
        const float damageFlashInterval = 300f;
        float idleInterval = 2000f;
        float interval = 200f;
        float bowInterval = 150f;
        float shotInterval = 750f;
        int currentFrame = 0;
        int spriteWidth = 35;
        int spriteHeight = 130;
        bool isRunning;
        bool isDucking;
        bool invulnerable = false;
        bool invulnFlash = false;
        bool damageFlash = false;
        public bool Invulnerable { get { return invulnerable; } set { invulnerable = value; } }
        public Body WheelBody { get; set; }
        public Body LadderSensor { get; set; }

        #region logging metrics
        int numberOfJumps = 0;
        float damageTaken = 0;

        float idleTime = 0f;
        float totalIdleTime = 0f;
        bool isIdleLogged = false;

        float totalSprintingTime = 0f;
        #endregion

        /// <summary>
        /// Can the character slide?
        /// </summary>
        public bool CanSlide { get { return slideTriggers.Any(); } }

        public bool CanJump { get; set; }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList
                    .Concat(WheelBody.FixtureList)
                    .ToList();
            }
        }

        public UserControlledCharacter()
            : this(Vector2.Zero)
        {
        }

        public UserControlledCharacter(Vector2 position)
        {
            ResetSummaryMetrics();
            //physics = new GravityPhysics(position, PLAYER_MASS)
            //{
            //    GravityEnabled = true
            //};
            Position = position;
            health = 100;  //@update
            shield = SHIELD_MAX;
            direction = new Vector2(1, 0);

            // this seems stupid
            activeItem = 0;
            Inventory = new List<IInventoryItem>();
            //inventory.Add(new Dart());

            animations = CreateAnimations();

            Dots = new HashSet<DamageOverTimeEffect>();

            slideTriggers = new HashSet<SlideTrigger>();

            CanJump = true;
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var soundCache = engineRegistrations.Resolve<IResourceCache<SoundEffect>>();
            jumpSound = soundCache.LoadResource(JUMP_SOUND_NAME);
            arrowSound = soundCache.LoadResource(ARROW_RELEASE_SOUND);
            takeDamageSound = soundCache.LoadResource(TAKE_DAMAGE_SOUND);
            jumpImpactSound = soundCache.LoadResource(JUMP_IMPACT_SOUND);
        }

        public void TakeDamage(float val, bool doesKnockBack)
        {
            if (EngineGame.Instance.ScreenManager.CurrentGameplay != null)
            {
                if (!Invulnerable)
                {
                    if (doesKnockBack)
                    {
                        Invulnerable = true;
                        damageFlash = true;
                    }

                    //if (Shield > 0)
                    //{
                    //    val = Math.Min(Shield, val);
                    //    Shield -= val;
                    //    timeSinceLastHit = 0;
                    //}
                    //else
                    //{
                        Health -= val;
                        damageTaken += val;
                        Logger.Info(String.Format("DAMAGED: {0}", val));
                    //}


                    Engine.UpdateHealth();
                    PlaySound(takeDamageSound);
                }
                if (CanClimb != null && Climbing)
                {
                    CanClimb.DismountCharacter(this);
                }
                if (doesKnockBack)
                {
                    InKnockback = true;
                    if (RightFacingBodyState())
                    {
                        currentState = BodyStates.KnockbackRight;
                        Physics.LinearVelocity = Vector2.Zero;
                        Physics.ApplyLinearImpulse(new Vector2(-10, 0));
                    }
                    else if (LeftFacingBodyState())
                    {
                        currentState = BodyStates.KnockbackLeft;
                        Physics.LinearVelocity = Vector2.Zero;
                        Physics.ApplyLinearImpulse(new Vector2(10, 0));
                    }
                }
            }
        }

        public override void OnUpdate(GameTime gameTime, EngineGame game)
        {
            RemoveInactiveDots();

            if (!BridgeHanging())
                TouchingGround = false;

            var startMid = Physics.Position + new Vector2(0, PhysicsConstants.PixelsToMeters(spriteHeight) / 2);
            var startLeft = WheelBody.Position + new Vector2(-PhysicsConstants.PixelsToMeters(spriteWidth) / 2, 0);
            var startRight = WheelBody.Position + new Vector2(PhysicsConstants.PixelsToMeters(spriteWidth) / 2, 0);
            if (healEmitter != null)
            {
                healEmitter.Position = Physics.Position;
            }

            RayCastCallback cb = delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
            {
                if (fixture.Body.UserData is WorldGeometry2 || fixture.Body.UserData is MovingPlatform || fixture.Body.UserData is TutorialBreakBridge)
                {
                    if (jumpToggleGuard == false)
                    {
                        PlaySound(jumpImpactSound);
                    }
                    jumpToggleGuard = true;
                    TouchingGround = true;
                    WheelBody.CollidesWith = Category.All;
                    return 0;
                }
                else
                {
                    return -1;
                }
            };

            var distMid = new Vector2(0, .1f);
            var distSides = distMid + WheelBody.Position - Physics.Position;

            game.LevelManager.PhysicsManager.World.RayCast(cb, startMid, startMid + distMid);
            game.LevelManager.PhysicsManager.World.RayCast(cb, startLeft, startLeft + distSides);
            game.LevelManager.PhysicsManager.World.RayCast(cb, startRight, startRight + distSides);

            foreach (DamageOverTimeEffect dot in Dots)
            {
                if (dot.Active && !Invulnerable)
                    TakeDamage(dot.Tick(gameTime), dot.DoesKnockBack);
            }

            //timeSinceLastHit += gameTime.ElapsedGameTime.Milliseconds;
            //if (timeSinceLastHit >= RECHARGE_WAIT_TIME)
            //{
            //    Recharge(gameTime.ElapsedGameTime.Milliseconds);
            //}

            if (Inventory.Count > 0 && Inventory[0] is EnergyGun)
            {
                var gun = ((EnergyGun)Inventory[0]);
                gun.OnUpdate(gameTime, Engine);

                if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.Shoot)){
                    gun.Fire(this, Engine, gameTime, 0, chargingWeapon);
                }
            }

            if (gameTime.TotalGameTime.TotalMilliseconds >= nextLogTime)
            {
                LogMetricSnapshot();
                nextLogTime = gameTime.TotalGameTime.TotalMilliseconds + LOG_INTERVAL;
            }

            //log idle time and location
            if (IdleState())
            {
                if (!isIdleLogged)
                {
                    Logger.Info(String.Format("IDLE(pos): {0}", FormatPosition(Position)));
                    isIdleLogged = true;
                }
                totalIdleTime += gameTime.ElapsedGameTime.Milliseconds;
                idleTime += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                if (isIdleLogged)
                {
                    Logger.Info(String.Format("IDLE(ms): {0} ms", idleTime));
                    idleTime = 0f;
                    isIdleLogged = false;
                }
            }

            if (isRunning)
            {
                totalSprintingTime += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (isSliding && !CanSlide)
                StopSliding();

            if (health <= 0)
            {
                Logger.Info(String.Format("DEATH: {0}", FormatPosition(Position)));
                var save = (Save)Engine.LevelManager.LevelCache["Save"];
                Engine.MarkAsLoadLevel(save.LevelPath, save.SpawnPoint, true);
            }

            if (ignoreOneWays)
                WheelBody.CollidesWith = Category.Cat1 | Category.Cat31;
            ignoreOneWays = false;

            MotorJoint.MaxMotorTorque = TouchingGround ? MOTOR_TORQUE : 0;
        }

        private void Recharge(int ellapsedTime)
        {
            Shield = Math.Min(SHIELD_MAX, Shield + ellapsedTime / 1000f * RECHARGE_AMOUNT);
            Engine.UpdateHealth();
        }

        private void RemoveInactiveDots()
        {
            Dots.RemoveWhere(x => x.Finished);
        }

        public void AddInventoryItem(IInventoryItem item)
        {
            Inventory.Add(item);
            activeItem = Inventory.IndexOf(item);
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            if (playerControlled)
            {
                sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
                // Get the gamepad state.
                var gamepadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

                // Get the time scale since the last update call.
                var timeFrame = (float)gameTime.ElapsedGameTime.TotalSeconds;
                var timeFrame_ms = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                var amount = 1f;
                var climbAmount = 6f;
                var moveDirection = new Vector2();

                // Grab the keyboard state.
                var keyboard = Keyboard.GetState();
                var gamepad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
                //  var d = InputManager.Instance.Pressed(Keys.D);
                //  var a = InputManager.Instance.Pressed(Keys.A);

                //Update the animation timer by the timeframe in milliseconds
                timer += timeFrame_ms;
                shotTimer += timeFrame_ms;
                if (InKnockback)
                {
                    MotorJoint.MotorSpeed = 0;
                    knockbackTimer += timeFrame_ms;

                    if (knockbackTimer >= knockbackDur)
                    {
                        InKnockback = false;
                        knockbackTimer = 0f;
                    }
                }
                if (Invulnerable)
                {
                    invulnTimer += timeFrame_ms;
                    if (damageFlash)
                    {
                        damageTimer += timeFrame_ms;
                    }

                    if ((int)invulnTimer % 10 == 0 && damageTimer == 0)
                    {
                        invulnFlash = !invulnFlash;
                    }
                }

                if (TouchingGround)
                    Physics.Friction = WheelBody.Friction = 10;
                else
                    Physics.Friction = WheelBody.Friction = .01f;

                #region Movement

                if (invulnTimer >= invulnInterval)
                {
                    invulnTimer = 0f;
                    Invulnerable = false;
                }
                if (damageTimer >= damageFlashInterval)
                {
                    damageTimer = 0f;
                    damageFlash = false;
                }

                if (!InKnockback)
                {
                    if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.Sprint))
                    {
                        isRunning = true;
                    }
                    else
                        isRunning = false;

                    if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.MoveLeft))
                    {
                        float THRESHHOLD = PhysicsConstants.PixelsToMeters(5);
                        facing = -1;
                        if (ClimbingState())
                        {
                            Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                        }

                        if ((currentState == BodyStates.ClimbingBack || currentState == BodyStates.ClimbingBackNeut) &&
                             canClimb != null && canClimb.VineWall)
                        {
                            if (Physics.Position.X >= (canClimb.Position.X - (PhysicsConstants.PixelsToMeters(canClimb.Width) / 2)) + THRESHHOLD)
                            {
                                moveDirection.X -= 1.0f;// Physics.Position = new Vector2(Physics.Position.X - PhysicsConstants.PixelsToMeters(5), Physics.Position.Y);
                                Physics.LinearDamping = 15f;
                            }
                        }

                        else if (currentState == BodyStates.ClimbingBack)
                        {
                            // Do Nothing -- Normal Ladder
                        }
                        else if (RightFacingBodyState() && ClimbingState())
                        {
                            currentState = BodyStates.ClimbingLookLeft;
                        }
                        else if (LeftFacingBodyState() && ClimbingState())
                        {
                            currentState = BodyStates.ClimbingLeftNeutral;
                        }
                        else
                        {
                            if ((!swinging || Physics.LinearVelocity.X <= 0) && !isDucking)
                                moveDirection.X -= 1.0f;

                            if (TouchingGround)
                            {
                                if (isRunning)
                                {
                                    if (currentState != BodyStates.RunningLeft)
                                    {
                                        animations[BodyStates.RunningLeft].CurrentFrame = 0;
                                        currentState = BodyStates.RunningStartLeft;
                                    }
                                }
                                else if (isDucking)
                                {
                                }
                                else if (InventoryItem is Arrow && HoldingTorch == null)
                                {
                                    if (currentState != BodyStates.WalkingShootLeft &&
                                        currentState != BodyStates.WalkingShoot2Left &&
                                        currentState != BodyStates.WalkingShoot3Left &&
                                        currentState != BodyStates.WalkingDrawnLeft)
                                    {
                                        animations[BodyStates.WalkingShootLeft].CurrentFrame = 0;
                                        currentState = BodyStates.WalkingShootLeft;
                                    }
                                }

                                else if (currentState != BodyStates.WalkingTorchLeft && HoldingTorch != null)
                                {
                                    animations[BodyStates.WalkingTorchLeft].CurrentFrame = 0;
                                    currentState = BodyStates.WalkingTorchStartLeft;
                                }
                                else if (currentState != BodyStates.WalkingLeft && HoldingTorch == null)
                                {
                                    animations[BodyStates.WalkingLeft].CurrentFrame = 0;
                                    currentState = BodyStates.WalkingStartLeft;
                                }
                            }
                            else if (currentState == BodyStates.HorizontalClimbLeft || currentState == BodyStates.HorizontalClimbRight ||
                                    currentState == BodyStates.HorizontalClimbLeftNeut || currentState == BodyStates.HorizontalClimbRightNeut)
                            {
                                currentState = BodyStates.HorizontalClimbLeft;
                            }
                            else if (HoldingTorch == null)
                            {
                                currentState = BodyStates.JumpingLeft;
                            }
                            else
                            {
                                currentState = BodyStates.WalkingTorchLeft;
                            }
                        }
                        //TODO -- add logic for climbing state / animation
                    }
                    if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.MoveRight))
                    {
                        float THRESHHOLD = PhysicsConstants.PixelsToMeters(5);
                        facing = 1;

                        if (ClimbingState())
                        {
                            Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                        }

                        if ((currentState == BodyStates.ClimbingBack || currentState == BodyStates.ClimbingBackNeut) &&
                             canClimb != null && canClimb.VineWall)
                        {
                            if (Physics.Position.X <= (canClimb.Position.X + (PhysicsConstants.PixelsToMeters(canClimb.Width) / 2)) - THRESHHOLD)
                            {
                                moveDirection.X += 1.0f;
                                Physics.LinearDamping = 15f;
                            }
                        }
                        else if (currentState == BodyStates.ClimbingBack)
                        {
                            // Do Nothing
                        }
                        else if (LeftFacingBodyState() && ClimbingState())
                        {
                            currentState = BodyStates.ClimbingLookRight;
                        }
                        else if (RightFacingBodyState() && ClimbingState())
                        {
                            currentState = BodyStates.ClimbingRightNeutral;
                        }
                        else
                        {
                            if ((!swinging || Physics.LinearVelocity.X >= 0) && !isDucking)
                                moveDirection.X += 1.0f;

                            if (TouchingGround)
                            {
                                if (isRunning)
                                {
                                    if (currentState != BodyStates.RunningRight)
                                    {
                                        animations[BodyStates.RunningRight].CurrentFrame = 0;
                                        currentState = BodyStates.RunningStartRight;
                                    }
                                }

                                else if (isDucking)
                                {
                                }
                                else if (InventoryItem is Arrow && HoldingTorch == null)
                                {
                                    if (currentState != BodyStates.WalkingShootRight &&
                                        currentState != BodyStates.WalkingShoot2Right &&
                                        currentState != BodyStates.WalkingShoot3Right &&
                                        currentState != BodyStates.WalkingDrawnRight)
                                    {
                                        animations[BodyStates.WalkingShootRight].CurrentFrame = 0;
                                        currentState = BodyStates.WalkingShootRight;
                                    }
                                }
                                else if (currentState != BodyStates.WalkingTorchRight && HoldingTorch != null)
                                {
                                    animations[BodyStates.WalkingTorchRight].CurrentFrame = 0;
                                    currentState = BodyStates.WalkingTorchStartRight;
                                }
                                else if (currentState != BodyStates.WalkingRight && HoldingTorch == null)
                                {
                                    animations[BodyStates.WalkingRight].CurrentFrame = 0;
                                    currentState = BodyStates.WalkingStartRight;
                                }
                                else if (HoldingTorch == null)
                                {
                                    currentState = BodyStates.WalkingRight;
                                }
                                else
                                {
                                    currentState = BodyStates.WalkingTorchRight;
                                }
                            }
                            else if (currentState == BodyStates.HorizontalClimbLeft || currentState == BodyStates.HorizontalClimbRight ||
                                    currentState == BodyStates.HorizontalClimbLeftNeut || currentState == BodyStates.HorizontalClimbRightNeut)
                            {
                                currentState = BodyStates.HorizontalClimbRight;
                            }
                            else
                            {
                                currentState = BodyStates.JumpingRight;
                            }
                        }
                        //TODO -- add logic for climbing state / animation
                    }
                    if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.DownAction))
                    {
                        #region Climbing

                        if (ClimbingState())
                        {
                            Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                        }
                        if (canClimb != null)
                        {
                            TouchingGround = false;
                            canClimb.Physics.IsSensor = true;
                            Climbing = true;
                            Physics.IgnoreGravity = WheelBody.IgnoreGravity = true;
                            WheelBody.CollidesWith = Category.Cat1;

                            if (!canClimb.Sideways)
                                currentState = BodyStates.ClimbingBack;
                            else if (RightFacingBodyState())
                                currentState = BodyStates.ClimbingRight;
                            else
                                currentState = BodyStates.ClimbingLeft;
                            /*
                             var v = new Vector2(0, PhysicsConstants.PixelsToMeters(5));
                             Physics.Position += v;
                             WheelBody.Position += v;
                             */
                            moveDirection.Y += 1.0f;
                            Physics.LinearDamping = 15f;
                        }
                        #endregion
                        //Sliding
                        else if (TouchingGround)
                        {

                            if (DoorType == DoorType.Down)
                            {
                                currentState = BodyStates.FacingForward;
                            }
                            else if (TouchingGround && !inHold && !isDucking)
                            {
                                if (CanSlide)
                                {
                                    StartSliding(moveDirection.X >= 0
                                        ? MoveDirection.Right
                                        : MoveDirection.Left);
                                }
                                else
                                {
                                    isDucking = true;

                                    var pos = Physics.Position;

                                    Physics.Dispose();
                                    _world.RemoveJoint(MotorJoint);

                                    float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                                    float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                                    Physics = BodyFactory.CreateBody(_world);
                                    var r = FixtureFactory.AttachRectangle(
                                        PhysicsConstants.PixelsToMeters(Width),
                                        PhysicsConstants.PixelsToMeters(Height / 2),
                                        1.4f,
                                        new Vector2(0, PhysicsConstants.PixelsToMeters(15)),
                                        Physics);

                                    r.CollidesWith = Category.Cat1;
                                    r.CollisionCategories = Category.Cat3;
                                    r.UserData = "Rectangle";
                                    r.Shape.Density = 7;

                                    Physics.FixedRotation = true;
                                    Physics.Position = pos;
                                    Physics.BodyType = BodyType.Dynamic;
                                    Physics.Friction = 10.0f;
                                    Physics.IsBullet = true;

                                    MotorJoint = JointFactory.CreateRevoluteJoint(_world, Physics, WheelBody, Vector2.Zero);
                                    MotorJoint.MotorEnabled = true;
                                    MotorJoint.MaxMotorTorque = 10;

                                    if (LeftFacingBodyState())
                                    {
                                        if (HoldingTorch == null && Inventory.Count != 0 && Inventory[activeItem] is Arrow)
                                        {
                                            currentState = BodyStates.DuckingLeftBow;
                                        }
                                        else
                                            currentState = BodyStates.DuckingLeft;
                                    }
                                    else
                                    {
                                        if (HoldingTorch == null && Inventory.Count != 0 && Inventory[activeItem] is Arrow)
                                        {
                                            currentState = BodyStates.DuckingRightBow;
                                        }
                                        else
                                            currentState = BodyStates.DuckingRight;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isDucking)
                        {
                            ReRegisterPhysics();
                            isDucking = false;
                        }
                        if (isSliding)
                        {
                            StopSliding();
                        }
                    }

                #endregion

                    #region Direction

                    var up = InputManager.Instance.ActionHeld(InputManager.ButtonActions.AimUp);
                    var down = InputManager.Instance.ActionHeld(InputManager.ButtonActions.AimDown);
                    var right = InputManager.Instance.ActionHeld(InputManager.ButtonActions.AimRight);
                    var left = InputManager.Instance.ActionHeld(InputManager.ButtonActions.AimLeft);
                    if (!ClimbingState() && !swinging && !VineBridgeState())
                    {
                        if (up && right)
                        {
                            facing = 1;
                            direction = new Vector2(0.707106769f, -0.707106769f);
                       //     if (InHold)
                            if(InventoryItem is Arrow)
                            {
                                currentState = BodyStates.ShootingArrowRight;
                                animations[currentState].CurrentFrame = animations[currentState].NumFrames - 1;
                            }
                            else
                            {
                                currentState = NeutralState();
                            }
                        }
                        else if (up && left)
                        {
                            direction = new Vector2(-0.707106769f, -0.707106769f);
                            facing = -1;
                            if (InventoryItem is Arrow)
                            {
                                currentState = BodyStates.ShootingArrowLeft;
                                animations[currentState].CurrentFrame = animations[currentState].NumFrames - 1;
                            }
                            else
                            {
                                currentState = NeutralState();
                            }
                        }
                        else if (down && right)
                        {
                            direction = new Vector2(0.707106769f, 0.707106769f);
                            currentState = BodyStates.NeutralRight;
                            facing = 1;
                        }
                        else if (down && left)
                        {
                            direction = new Vector2(-0.707106769f, 0.707106769f);
                            currentState = BodyStates.NeutralLeft;
                            facing = -1;
                        }
                        else if (up)
                        {
                            direction = new Vector2(0, -1);
                        }
                        else if (down)
                        {
                            direction = new Vector2(0, 1);
                        }
                        else if (right)
                        {
                            direction = new Vector2(1, 0);
                            facing = 1;
                            currentState = NeutralState();
                        }
                        else if (left)
                        {
                            direction = new Vector2(-1, 0);
                            facing = -1;
                            currentState = NeutralState();
                        }
                        else
                        {
                            direction = new Vector2(1, 0) * facing;
                        }
                    }

                    #endregion

                    #region Jumping
                    if (InputManager.Instance.ActionPressed(InputManager.ButtonActions.Jump))
                    {
                        if (BridgeHanging())
                        {
                            if (isDucking)
                            {
                                ReRegisterPhysics();
                                isDucking = false;
                            }
                            vineBridge.ForceSeperation(this);
                            if (!InputManager.Instance.ActionHeld(InputManager.ButtonActions.DownAction))
                                PerformJump();
                        }
                        else if (swinging)
                        {
                            ForceVineSeperate();
                            PerformJump(.2f);
                        }
                        else if ((canClimb != null) && !TouchingGround && jumpToggleGuard)
                        {
                            Physics.LinearDamping = canClimb.LinearDamping;
                            Physics.IgnoreGravity = WheelBody.IgnoreGravity = false;
                            PerformJump();
                        }
                        else if (jumpToggleGuard && TouchingGround)
                        {
                            if (!InputManager.Instance.ActionHeld(InputManager.ButtonActions.DownAction))
                            {
                                PerformJump();
                            }
                            else
                            {
                                WheelBody.CollidesWith = Category.Cat1;
                                PerformJump(-1);
                                ignoreOneWays = true;
                            }
                        }

                        numberOfJumps++;
                    }
                    //if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.DownAction) &&
                    //    InputManager.Instance.ActionPressed(InputManager.ButtonActions.Jump))
                    //{
                    //    if (TouchingGround)
                    //    {
                    //        PerformJump();
                    //    }
                    //}
                    #endregion

                    #region climbing
                    if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.UpAction))
                    {
                        if ((canClimb != null))
                        {
                            Climbing = true;
                            //Insert anim state change here for climbing anim
                            Physics.LinearVelocity = WheelBody.LinearVelocity = Vector2.Zero;
                            Physics.IgnoreGravity = WheelBody.IgnoreGravity = true;
                            TouchingGround = false;
                            jumpToggleGuard = true;
                            const float THRESHHOLD = 3;
                            float playerTopLeft = Physics.Position.Y - PhysicsConstants.PixelsToMeters(Height / 2);
                            float ladderTopLeft = canClimb.Position.Y - PhysicsConstants.PixelsToMeters(canClimb.Height / 2);
                            if (!canClimb.VineWall)
                            {
                                if (canClimb.Sideways)
                                {
                                    //We are to the right of the ladder

                                    if (playerTopLeft >= ladderTopLeft || !canClimb.LimitedHeight)
                                    {
                                        if (playerTopLeft <= ladderTopLeft + PhysicsConstants.PixelsToMeters(THRESHHOLD) && canClimb.LimitedHeight)
                                        {
                                            //do nothing;
                                        }
                                        else if (Physics.Position.X >= canClimb.Position.X)
                                        {
                                            currentState = BodyStates.ClimbingLeft;
                                            Physics.Position = new Vector2(CanClimb.Position.X + (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) +
                                                                                                 (PhysicsConstants.PixelsToMeters(this.Width) / 2),
                                                                           Physics.Position.Y);

                                            WheelBody.Position = new Vector2(CanClimb.Position.X + (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) +
                                                                                                 (PhysicsConstants.PixelsToMeters(this.Width) / 2),
                                                                           WheelBody.Position.Y);
                                            moveDirection.Y -= 1.0f;
                                            Physics.LinearDamping = 15f;
                                        }
                                        //We are to the left of the ladder
                                        else if (Physics.Position.X < canClimb.Position.X)
                                        {
                                            currentState = BodyStates.ClimbingRight;
                                            Physics.Position = new Vector2(CanClimb.Position.X - (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) -
                                                                                                 (PhysicsConstants.PixelsToMeters(this.Width) / 2),
                                                                           Physics.Position.Y);
                                            WheelBody.Position = new Vector2(CanClimb.Position.X - (PhysicsConstants.PixelsToMeters(CanClimb.Width) / 2) -
                                                                                                 (PhysicsConstants.PixelsToMeters(this.Width) / 2),
                                                                           WheelBody.Position.Y);
                                            moveDirection.Y -= 1.0f;
                                            Physics.LinearDamping = 15f;
                                        }
                                    }

                                    else if (canClimb.LimitedHeight)
                                    {
                                        if (RightFacingBodyState())
                                        {
                                            currentState = BodyStates.ClimbingRight;
                                        }
                                        else if (LeftFacingBodyState())
                                        {
                                            currentState = BodyStates.ClimbingLeft;
                                        }
                                        moveDirection.Y += 1.0f;
                                        Physics.LinearDamping = 15f;
                                    }
                                }

                                else
                                {
                                    currentState = BodyStates.ClimbingBack;
                                    Physics.Position = new Vector2(canClimb.Position.X,
                                                                   Physics.Position.Y);

                                    WheelBody.Position = new Vector2(canClimb.Position.X,
                                                                   WheelBody.Position.Y);
                                    moveDirection.Y -= 1.0f;
                                    Physics.LinearDamping = 15f;
                                }
                            }
                            else
                            {
                                currentState = BodyStates.ClimbingBack;
                                Physics.Position = new Vector2(Physics.Position.X,
                                                               Physics.Position.Y);

                                WheelBody.Position = new Vector2(WheelBody.Position.X,
                                                               WheelBody.Position.Y);
                                moveDirection.Y -= 1.0f;
                                Physics.LinearDamping = 15f;
                            }
                        }
                        else if (DoorType == DoorType.Up)
                        {
                            currentState = BodyStates.FacingBack;
                        }
                    }
                    #endregion

                    #region Shooting

                    if (InputManager.Instance.IsNewAction(InputManager.ButtonActions.ChargeShot))
                    {
                        if (Inventory.Count != 0)
                        {
                            Inventory[activeItem].ChargeInitiated(this, gameTime);
                        }
                    }
                    else if (InputManager.Instance.ActionReleased(InputManager.ButtonActions.ChargeShot))
                    {
                        if (Inventory.Count != 0)
                        {
                            Inventory[activeItem].ChargeReleased(this, gameTime);
                        }
                    }

                    if (InputManager.Instance.ActionPressed(InputManager.ButtonActions.Shoot))
                    {
                        if (shotTimer >= shotInterval && HoldingTorch == null && Inventory.Count != 0 && (Inventory[activeItem] is Arrow || Inventory[activeItem] is EnergyGun) && !climbing)
                        {
                            currentState = facing == -1
                                ? isDucking
                                    ? BodyStates.DuckShootLeftBow
                                    : BodyStates.ShootingArrowLeft
                                : isDucking
                                    ? BodyStates.DuckShootRightBow
                                    : BodyStates.ShootingArrowRight;

                            //currentState = BodyStates.ShootingRight;
                            holdTime = gameTime.TotalGameTime.TotalSeconds;
                            inHold = true;
                        }
                    }
                    else if (!InputManager.Instance.ActionHeld(InputManager.ButtonActions.Shoot) && inHold)
                    {
                        if (!ClimbingState() && !swinging && !VineBridgeState() &&
                            shotTimer >= shotInterval && HoldingTorch == null && Inventory.Count != 0 && (Inventory[activeItem] is Arrow || Inventory[activeItem] is EnergyGun))
                        {
                            PlaySound(arrowSound);
                            Inventory[activeItem].Use(this, world, gameTime, holdTime, chargingWeapon);
                            var shooting = animations[currentState].CurrentFrame = 0;

                            currentState = facing == -1
                                ? isDucking
                                    ? BodyStates.DuckingLeftBow
                                    : BodyStates.ShootingArrowNeutLeft
                                : isDucking
                                    ? BodyStates.DuckingRightBow
                                    : BodyStates.ShootingArrowNeutRight;
                        }

                        inHold = false;
                        shotTimer = 0f;
                    }

                    if (InputManager.Instance.IsNewKey(Keys.G))
                    {
                        if (activeItem == Inventory.Count - 1)
                        {
                            activeItem = 0;
                        }
                        else
                        {
                            activeItem++;
                        }
                        EngineGame.Instance.ScreenManager.CurrentGameplay.UpdatePrimaryItems(this);
                    }

                    #endregion

                    #region abilities

                    if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.Heal) && Health < MAX_HEALTH && Mana > 0)
                    {
                        var manaUsage = Math.Min(Mana, gameTime.ElapsedGameTime.Milliseconds * HEAL_MANA_BURN_PER_MILLI);

                        mana -= manaUsage; 
                        Health = Math.Min(MAX_HEALTH, Health + manaUsage * MANA_TO_HEALTH_SCALE);

                        if (healEmitter == null)
                        {
                            healEmitter = new Emitter(new Vector2(100f, 100f), new Vector2(0f, -1f),
                                          new Vector2(-.5f, .5f), new Vector2(1000f, 1000f),
                                          Vector2.One, Vector2.One, Color.White, Color.Red, Color.White, Color.Red,
                                          new Vector2(0f, PhysicsConstants.PixelsToMeters(.25f)), new Vector2(0, PhysicsConstants.PixelsToMeters(.25f)), 100, Vector2.Zero, "Textures/Objects/heal", new Random(), Physics.Position,
                                          PhysicsConstants.PixelsToMeters(Width * 2), PhysicsConstants.PixelsToMeters(Height * 2));
                            Engine.LevelManager.RegisterEntity(healEmitter);
                        }

                        Engine.UpdateHealth();
                    }
                    else
                    {
                        if (healEmitter != null)
                        {
                            Engine.LevelManager.UnregisterEntity(healEmitter);

                            healEmitter.Clear();
                        }
                        healEmitter = null;
                    }


                    #endregion

                    if (InputManager.Instance.ActionPressed(InputManager.ButtonActions.Interact))
                    {
                        if (onPickup != null)
                        {
                            if (onPickup is Torch)
                            {
                                Inventory.Add(onPickup);
                                ((Torch)onPickup).WeldToPlayer(this);
                                HoldingTorch = (Torch)onPickup;
                                onPickup = null;
                                // EngineGame.Instance.LevelManager.RenderManager.UnregisterRenderable(currentItemPrompt);
                                currentState = BodyStates.NeutralRightTorch;

                            }
                        }
                        else if (onTorchGround != null && HoldingTorch != null)
                        {
                            ((Torch)HoldingTorch).PlaceTorch(this, onTorchGround);
                            onPickup = HoldingTorch;
                            Inventory.Remove(HoldingTorch);
                            activeItem = 0;
                            if (Inventory != null)
                            {
                                EngineGame.Instance.ScreenManager.CurrentGameplay.UpdatePrimaryItems(this);
                            }
                            HoldingTorch = null;
                            if (InventoryItem is Arrow)
                            {
                                if (RightFacingBodyState())
                                {
                                    currentState = BodyStates.WalkingShootRight;
                                }
                                else
                                    currentState = BodyStates.WalkingShootRight;
                            }
                            else if (RightFacingBodyState())
                                currentState = BodyStates.NeutralRight;
                            else
                                currentState = BodyStates.NeutralLeft;

                        }
                    }
                    //No keys are pressed and we're on the ground, we're neutral
                    if (keyboard.GetPressedKeys().GetLength(0) == 0 && InputManager.Instance.NoButtonsPressed(gamepad))
                    {
                        if (healEmitter != null)
                        {
                            Engine.LevelManager.UnregisterEntity(healEmitter);
                            healEmitter.Clear();
                        }
                        healEmitter = null;
                        if (TouchingGround && timer >= interval)
                        {
                            if (currentState == BodyStates.WalkingRight)
                            {
                                currentState = BodyStates.WalkingEndRight;
                                timer = 0f;
                            }
                            else if (currentState == BodyStates.WalkingLeft)
                            {
                                currentState = BodyStates.WalkingEndLeft;
                                timer = 0f;
                            }
                            else if (currentState == BodyStates.WalkingTorchRight)
                            {
                                currentState = BodyStates.WalkingTorchEndRight;
                                timer = 0f;
                            }

                            else if (currentState == BodyStates.WalkingTorchLeft)
                            {
                                currentState = BodyStates.WalkingTorchEndLeft;
                                timer = 0f;
                            }
                            else if (currentState == BodyStates.NeutralLeft)
                            {
                                animations[BodyStates.IdleRightOpen].CurrentFrame = 0;
                            }
                            else if (currentState == BodyStates.NeutralRight)
                            {
                                animations[BodyStates.IdleRightOpen].CurrentFrame = 0;
                            }
                            else if (currentState == BodyStates.RunningLeft)
                            {
                                currentState = BodyStates.RunningStopLeft;
                                timer = 0f;
                            }
                            else if (currentState == BodyStates.RunningRight)
                            {
                                currentState = BodyStates.RunningStopRight;
                                timer = 0f;
                            }
                            else if (currentState == BodyStates.WalkingShootLeft)
                            {
                                currentState = BodyStates.ShootingArrowNeutLeft; //Add in Stopping Animation
                                timer = 0f;
                            }
                            else if (currentState == BodyStates.WalkingShootRight)
                            {
                                currentState = BodyStates.ShootingArrowNeutRight; //Add in Stopping Animation
                                timer = 0f;
                            }
                        }
                        //Set to climbing neutral states
                        if (LeftFacingBodyState() && ClimbingState())
                        {
                            currentState = BodyStates.ClimbingLeftNeutral;
                        }
                        else if (RightFacingBodyState() && ClimbingState())
                        {
                            currentState = BodyStates.ClimbingRightNeutral;
                        }
                        else if (currentState == BodyStates.HorizontalClimbRight)
                        {
                            currentState = BodyStates.HorizontalClimbRightNeut;
                        }
                        else if (currentState == BodyStates.HorizontalClimbLeft)
                        {
                            currentState = BodyStates.HorizontalClimbLeftNeut;
                        }
                        if (currentState == BodyStates.ClimbingBack)
                        {
                            animations[BodyStates.ClimbingBack].CurrentFrame = 0;
                            currentState = BodyStates.ClimbingBackNeut;
                        }
                        if (currentState == BodyStates.DuckingLeft)
                        {
                            animations[BodyStates.DuckingLeft].CurrentFrame = 0;
                            currentState = BodyStates.NeutralLeft;
                        }
                        if (currentState == BodyStates.DuckingRight)
                        {
                            currentState = BodyStates.NeutralRight;
                            animations[BodyStates.DuckingRight].CurrentFrame = 0;
                        }
                        if (currentState == BodyStates.DuckingLeftBow)
                        {
                            animations[BodyStates.DuckingLeftBow].CurrentFrame = 0;
                            currentState = BodyStates.NeutralLeft;
                        }
                        if (currentState == BodyStates.DuckingRightBow)
                        {
                            currentState = BodyStates.NeutralRight;
                            animations[BodyStates.DuckingRightBow].CurrentFrame = 0;
                        }
                    }

                    if (moveDirection != Vector2.Zero)
                    {
                        // Normalize direction to 1.0 magnitude to avoid walking faster at angles.
                        moveDirection.Normalize();
                    }

                    if (!isSliding)
                    {
                        // Increment animation unless idle.
                        if (amount != 0.0f)
                        {
                            // Rotate the player towards the controller direction.
                            playerRotation = (float)(Math.Atan2(moveDirection.Y, moveDirection.X) + Math.PI / 2.0);

                            if (!ClimbingState())
                            {
                                // Move player based on the controller direction and time scale.
                                //Physics.ApplyLinearImpulse(movedirection * amount);
                                MovePlayer(moveDirection.X);
                            }
                            else
                                Physics.ApplyLinearImpulse(moveDirection * climbAmount);

                            MotorJoint.MotorSpeed = moveDirection.X * 10;
                        }
                    }

                }
            }
            //ClampVelocity();

            UpdateAnimationStates();
        }

        enum MoveDirection
        {
            Left = -1,
            Right = 1
        };

        private void MovePlayer(float dir)
        {
            if (dir == 0) return;

            float x_vel = WALK_X_CLAMP;

            if (swinging)
                x_vel = SWING_X_CLAMP;
            else if (!TouchingGround)
                x_vel = RUN_X_CLAMP * .8f;
            else if (isRunning)
            {
                x_vel = RUN_X_CLAMP;
                //if (!TouchingGround)
                //{
                //    x_vel = x_vel * .8f;
                //}
            }

            var accel = 1f;

            var vel = Physics.LinearVelocity.X;

            float desiredVel = 0;

            MoveDirection d = dir <= 0
                ? MoveDirection.Left
                : MoveDirection.Right;

            x_vel *= Math.Abs(dir);

            switch (d)
            {
                case MoveDirection.Left:
                    desiredVel = Math.Max(vel - accel, -x_vel);
                    break;
                case MoveDirection.Right:
                    desiredVel = Math.Min(vel + accel, x_vel);
                    break;
            }

            var velChange = desiredVel - vel;
            var impulse = Physics.Mass * velChange;
            Physics.ApplyLinearImpulse(Vector2.UnitX * impulse);
        }

        private bool BridgeHanging()
        {
            return vineBridge != null && vineBridge.Hanging;
        }

        private void PerformJump(float percentOfMax = 1)
        {
            if (CanJump)
            {
                jumpToggleGuard = false;
                PlaySound(jumpSound);
                Physics.ApplyLinearImpulse(new Vector2(0, -22f * percentOfMax));

                if (facing > 0)
                {
                    if (HoldingTorch != null)
                    {
                        currentState = BodyStates.JumpingRightTorch;
                        animations[BodyStates.JumpingRightTorch].CurrentFrame = 0;
                    }
                    else
                    {
                        currentState = BodyStates.JumpingRight;
                        animations[BodyStates.JumpingRight].CurrentFrame = 0;
                    }
                }
                else
                {
                    if (HoldingTorch != null)
                    {
                        currentState = BodyStates.JumpingLeftTorch;
                        animations[BodyStates.JumpingLeftTorch].CurrentFrame = 0;
                    }
                    else
                    {
                        currentState = BodyStates.JumpingLeft;
                        animations[BodyStates.JumpingLeft].CurrentFrame = 0;
                    }
                }
            }
        }

        private const float WALK_X_CLAMP = 4;
        private const float WALK_Y_CLAMP = 15;
        private const float RUN_X_CLAMP = 10;
        private const float SWING_X_CLAMP = 8;

        protected void UpdateAnimationStates()
        {
            if (currentState == BodyStates.IdleRightOpen && timer >= interval)
            {
                var idle = animations[BodyStates.IdleRightOpen];
                idle.CurrentFrame = (idle.CurrentFrame + 1) % idle.NumFrames;
                timer = 0f;
            }
            else if ((currentState == BodyStates.NeutralRight) && timer >= idleInterval)
            {
                currentState = BodyStates.IdleRightOpen;
                timer = 0f;
            }
            else if (currentState == BodyStates.NeutralRightTorch && timer >= interval)
            {
                var idle = animations[BodyStates.NeutralRightTorch];
                idle.CurrentFrame = (idle.CurrentFrame + 1) % idle.NumFrames;
                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingRight && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingRight];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingLeft && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingLeft];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingTorchRight && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingTorchRight];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingStartRight && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingRight].CurrentFrame = 0;
                currentState = BodyStates.WalkingRight;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingTorchStartRight && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingTorchRight].CurrentFrame = 0;
                currentState = BodyStates.WalkingTorchRight;
                timer = 0f;
            }
            else if ((currentState == BodyStates.WalkingEndRight ||
                      currentState == BodyStates.RunningStopRight) && timer >= interval)
            {
                currentState = BodyStates.NeutralRight;
                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingTorchEndRight && timer >= interval)
            {
                currentState = BodyStates.NeutralRightTorch;
                timer = 0f;
            }

            else if ((currentState == BodyStates.WalkingEndLeft ||
                      currentState == BodyStates.RunningStopLeft) && timer >= interval)
            {
                currentState = BodyStates.NeutralLeft;
                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingTorchLeft && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingTorchLeft];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingTorchStartLeft && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingTorchLeft].CurrentFrame = 0;
                currentState = BodyStates.WalkingTorchLeft;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingStartLeft && timer >= interval)
            {
                var walking = animations[BodyStates.WalkingLeft].CurrentFrame = 0;
                currentState = BodyStates.WalkingLeft;
                facing = -1;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingEndLeft && timer >= interval)
            {
                currentState = BodyStates.NeutralLeft;
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingTorchEndLeft && timer >= interval)
            {
                currentState = BodyStates.NeutralLeftTorch;
                timer = 0f;
            }
            #region Running
            else if (currentState == BodyStates.RunningStartRight && timer >= interval)
            {
                var walking = animations[BodyStates.RunningRight].CurrentFrame = 0;
                currentState = BodyStates.RunningRight;
                timer = 0f;
            }
            else if (currentState == BodyStates.RunningStartLeft && timer >= interval)
            {
                var walking = animations[BodyStates.RunningLeft].CurrentFrame = 0;
                currentState = BodyStates.RunningLeft;
                timer = 0f;
            }
            else if (currentState == BodyStates.RunningRight && timer >= interval)
            {
                var walking = animations[BodyStates.RunningRight];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }
            else if (currentState == BodyStates.RunningLeft && timer >= interval)
            {
                var walking = animations[BodyStates.RunningLeft];
                walking.CurrentFrame = (walking.CurrentFrame + 1) % walking.NumFrames;
                timer = 0f;
            }
            #endregion

            #region Shooting
            else if (currentState == BodyStates.ShootingArrowLeft && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.ShootingArrowLeft];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1 && shooting.CurrentFrame != shooting.NumFrames - 2)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            else if (currentState == BodyStates.ShootingArrowRight && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.ShootingArrowRight];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1 && shooting.CurrentFrame != shooting.NumFrames - 2)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShootLeft && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot2Left].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot2Left].NumFrames;
                    currentState = BodyStates.WalkingShoot2Left;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot2Left && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot3Left].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Left].NumFrames;
                    currentState = BodyStates.WalkingShoot3Left;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot3Left && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingDrawnLeft].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Left].NumFrames;
                    currentState = BodyStates.WalkingDrawnLeft;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingDrawnLeft && timer >= interval)
            {
                var shooting = animations[currentState];
                shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }

            else if (currentState == BodyStates.WalkingShootRight && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot2Right].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot2Right].NumFrames;
                    currentState = BodyStates.WalkingShoot2Right;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot2Right && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingShoot3Right].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Right].NumFrames;
                    currentState = BodyStates.WalkingShoot3Right;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingShoot3Right && timer >= interval)
            {
                var shooting = animations[currentState];
                if (inHold)
                {
                    animations[BodyStates.WalkingDrawnRight].CurrentFrame = (shooting.CurrentFrame + 1) %
                                                                                animations[BodyStates.WalkingShoot3Right].NumFrames;
                    currentState = BodyStates.WalkingDrawnRight;
                }
                else
                    shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.WalkingDrawnRight && timer >= interval)
            {
                var shooting = animations[currentState];
                shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;

                timer = 0f;
            }
            else if (currentState == BodyStates.DuckShootLeftBow && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.DuckShootLeftBow];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            else if (currentState == BodyStates.DuckShootRightBow && timer >= bowInterval)
            {
                var shooting = animations[BodyStates.DuckShootRightBow];
                if (inHold)
                {
                    if (shooting.CurrentFrame != shooting.NumFrames - 1)
                        shooting.CurrentFrame = (shooting.CurrentFrame + 1) % shooting.NumFrames;
                }
                timer = 0f;
            }
            #endregion

            if (currentState == BodyStates.JumpingRight && timer >= interval)
            {
                if (!TouchingGround && Physics.LinearVelocity.Y < 0)
                {
                    animations[BodyStates.JumpingRight].CurrentFrame = 1;
                }
                else if (!TouchingGround && Physics.LinearVelocity.Y > 0)
                {
                    animations[BodyStates.JumpingRight].CurrentFrame = 2;
                }
                else if (animations[BodyStates.JumpingRight].CurrentFrame == 3)
                {
                    currentState = BodyStates.NeutralRight;
                }
                else if (TouchingGround)
                {
                    animations[BodyStates.JumpingRight].CurrentFrame = 3;
                }

                timer = 0f;
            }
            if (currentState == BodyStates.JumpingRightTorch && timer >= interval)
            {
                if (!TouchingGround && Physics.LinearVelocity.Y < 0)
                {
                    animations[BodyStates.JumpingRightTorch].CurrentFrame = 1;
                }
                else if (!TouchingGround && Physics.LinearVelocity.Y > 0)
                {
                    animations[BodyStates.JumpingRightTorch].CurrentFrame = 2;
                }
                else if (animations[BodyStates.JumpingRightTorch].CurrentFrame == 3)
                {
                    currentState = BodyStates.NeutralRightTorch;
                }
                else if (TouchingGround)
                {
                    animations[BodyStates.JumpingRightTorch].CurrentFrame = 3;
                }

                timer = 0f;
            }

            if (currentState == BodyStates.JumpingLeft && timer >= interval)
            {
                if (!TouchingGround && Physics.LinearVelocity.Y < 0)
                {
                    animations[BodyStates.JumpingLeft].CurrentFrame = 1;
                }
                else if (!TouchingGround && Physics.LinearVelocity.Y > 0)
                {
                    animations[BodyStates.JumpingLeft].CurrentFrame = 2;
                }
                else if (animations[BodyStates.JumpingLeft].CurrentFrame == 3)
                {
                    currentState = BodyStates.NeutralLeft;
                }
                else if (TouchingGround)
                {
                    animations[BodyStates.JumpingLeft].CurrentFrame = 3;
                }

                timer = 0f;
            } 
            if (currentState == BodyStates.JumpingLeftTorch && timer >= interval)
            {
                if (!TouchingGround && Physics.LinearVelocity.Y < 0)
                {
                    animations[BodyStates.JumpingLeftTorch].CurrentFrame = 1;
                }
                else if (!TouchingGround && Physics.LinearVelocity.Y > 0)
                {
                    animations[BodyStates.JumpingLeftTorch].CurrentFrame = 2;
                }
                else if (animations[BodyStates.JumpingLeftTorch].CurrentFrame == 3)
                {
                    currentState = BodyStates.NeutralLeftTorch;
                }
                else if (TouchingGround)
                {
                    animations[BodyStates.JumpingLeftTorch].CurrentFrame = 3;
                }

                timer = 0f;
            }
            if (currentState == BodyStates.ClimbingLeft && timer >= interval)
            {
                var climbing = animations[BodyStates.ClimbingLeft];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                facing = -1;
                timer = 0f;
            }
            if (currentState == BodyStates.ClimbingRight && timer >= interval)
            {
                var climbing = animations[BodyStates.ClimbingRight];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                timer = 0f;
            }
            if (currentState == BodyStates.HorizontalClimbLeft && timer >= interval)
            {
                var climbing = animations[BodyStates.HorizontalClimbLeft];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                facing = -1;
                timer = 0f;
            }
            if (currentState == BodyStates.HorizontalClimbRight && timer >= interval)
            {
                var climbing = animations[BodyStates.HorizontalClimbRight];
                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                timer = 0f;
            }
            if (currentState == BodyStates.ClimbingBack && timer >= interval)
            {
                var climbing = animations[BodyStates.ClimbingBack];

                climbing.CurrentFrame = (climbing.CurrentFrame + 1) % climbing.NumFrames;
                timer = 0f;
            }
            if (currentState == BodyStates.KnockbackRight && timer >= interval)
            {
                var knockback = animations[BodyStates.KnockbackRight];
                if (knockback.CurrentFrame == knockback.NumFrames - 1)
                {
                    knockback.CurrentFrame = 0;
                    currentState = BodyStates.NeutralRight;
                }
                else
                {
                    knockback.CurrentFrame = (knockback.CurrentFrame + 1) % knockback.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.KnockbackLeft && timer >= interval)
            {
                var knockback = animations[BodyStates.KnockbackLeft];
                if (knockback.CurrentFrame == knockback.NumFrames - 1)
                {
                    knockback.CurrentFrame = 0;
                    currentState = BodyStates.NeutralLeft;
                }
                else
                {
                    knockback.CurrentFrame = (knockback.CurrentFrame + 1) % knockback.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingLeft && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingLeft];
                if (ducking.CurrentFrame != ducking.NumFrames - 2)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingRight && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingRight];
                if (ducking.CurrentFrame != ducking.NumFrames - 2)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingLeftBow && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingLeftBow];
                if (ducking.CurrentFrame != ducking.NumFrames - 1)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
            if (currentState == BodyStates.DuckingRightBow && timer >= interval)
            {
                var ducking = animations[BodyStates.DuckingRightBow];
                if (ducking.CurrentFrame != ducking.NumFrames - 1)
                {
                    ducking.CurrentFrame = (ducking.CurrentFrame + 1) % ducking.NumFrames;
                }
                timer = 0f;
            }
        }

        bool OnCollidedWith(Fixture f, PlaceTorchTrigger torchGround, Fixture c, Contact info)
        {
            if (HoldingTorch != null)
            {
                var TEXTURE = "Textures/Keys/e-Key";

                if (Engine != null && Engine.GamepadEnabled)
                {
                    TEXTURE = InputManager.Instance.GamepadTextures[InputManager.ButtonActions.Interact];
                }
                currentItemPrompt = new ItemPopup(TEXTURE,
                                                    torchGround.Physics.Position, TextureCache);

                EngineGame.Instance.LevelManager.RenderManager.RegisterRenderable(currentItemPrompt);
            }

            onTorchGround = torchGround;
            return true;
        }
        void OnSeparation(Fixture f1, PlaceTorchTrigger torchGround, Fixture f2)
        {
            EngineGame.Instance.LevelManager.RenderManager.UnregisterRenderable(currentItemPrompt);
            onTorchGround = null;
        }

        bool OnCollidedWith(Fixture f, WorldGeometry2 wg, Fixture c, Contact info)
        {
            if (c.UserData is OneWayPlatform && climbing)
                return false;

            else
                return info.Enabled;

        }

        bool OnCollidedWith(Fixture f, Torch torch, Fixture c, Contact info)
        {
            OnPickup = torch;
            var TEXTURE = "Textures/Keys/e-Key";

            if (Engine != null && Engine.GamepadEnabled)
            {
                TEXTURE = InputManager.Instance.GamepadTextures[InputManager.ButtonActions.Interact];
            }

            currentItemPrompt = new ItemPopup(
                TEXTURE,
                torch.Physics.Position - new Vector2(0, PhysicsConstants.PixelsToMeters(torch.Height) / 2),
                TextureCache);

            EngineGame.Instance.LevelManager.RenderManager.RegisterRenderable(currentItemPrompt);

            return true;
        }
        void OnSeparation(Fixture f1, Torch torch, Fixture f2)
        {
            OnPickup = null;
            EngineGame.Instance.LevelManager.RenderManager.UnregisterRenderable(currentItemPrompt);
        }

        private VineBridge vineBridge;
        bool OnCollidedWith(Fixture f, VineBridge bridge, Fixture vbf, Contact info)
        {
            if (LeftFacingBodyState())
                currentState = BodyStates.HorizontalClimbLeft;
            else
                currentState = BodyStates.HorizontalClimbRight;

            vineBridge = bridge;

            return true;
        }

        void OnSeparation(Fixture f1, VineBridge bridge, Fixture f2)
        {
            if (LeftFacingBodyState())
                currentState = BodyStates.JumpingLeft;
            else
                currentState = BodyStates.JumpingRight;
        }

        public bool OnCollidedWith(Fixture f1, Bramble bramble, Fixture f2, Contact info)
        {
            if (!Invulnerable)
            {
                /*  this.RegisterDot(bramble.dot);
                  bramble.dot.Active = true;
                  return true;*/
                TakeDamage(10, true);
                return true;
            }
            else
                return false;
        }

        public void OnSeparation(Fixture f1, Bramble bramble, Fixture f2)
        {
            bramble.dot.Active = false;
        }

        private WeldJoint vineJoint;
        private bool swinging;
        private bool leftVine = true;
        bool OnCollidedWith(Fixture f, Vine vine, Fixture vf, Contact info)
        {
            if (!swinging && leftVine)
            {
                var pointOnVine = new Vector2(0, PhysicsConstants.PixelsToMeters((int)(vine.Height * .2)));

                Position = vine.VineAnchor.GetWorldPoint(pointOnVine);
                vineJoint = JointFactory.CreateWeldJoint(
                    vine.VineAnchor,
                    Physics,
                    Vector2.Zero);
                Physics.FixedRotation = false;
                vine.VineAnchor.ApplyLinearImpulse(Physics.LinearVelocity, Physics.Position);

                _world.AddJoint(vineJoint);
                swinging = true;
                leftVine = false;
            }

            return true;
        }

        void OnSeparation(Fixture f1, Vine vine, Fixture f2)
        {
            leftVine = true;
        }

        private void ForceVineSeperate()
        {
            Physics.AngularVelocity = 0;
            Physics.Rotation = 0;
            Physics.FixedRotation = true;
            _world.RemoveJoint(vineJoint);
            swinging = false;
        }

        public override List<IRendering> Renderings
        {
            get
            {
                var anim = animations[currentState];
                if (invulnFlash)
                {
                    anim.UpdateTint(new Color(0, 0, 0, 0));
                }
                else if (damageFlash)
                {
                    anim.UpdateTint(new Color(255f, 0, 0, .5f));
                }
                else
                    anim.UpdateTint(Color.White);
                anim.Position = PhysicsConstants.MetersToPixels(Physics.Position);
                anim.Rotation = Physics.Rotation;
                if (Popups.Count != 0)
                {
                    var stack = new Stack<IRendering>();
                    foreach (ItemPopup i in Popups)
                    {
                        i.Renderings.ForEach(
                            x => stack.Push(x));
                    }

                    var ret = new ParentedRendering(anim, stack);
                    return new List<IRendering>() { ret };
                }

                return new List<IRendering>() { anim };
            }
        }

        private Dictionary<BodyStates, NewAnimationRendering> CreateAnimations()
        {
            var dictionary = new Dictionary<BodyStates, NewAnimationRendering>();
            Color invulnTint = Color.White;
            #region Neutral

            dictionary.Add(
                BodyStates.NeutralRight,
                new NewAnimationRendering(
                    NEUTRAL_RIGHT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.NeutralLeft,
                 new NewAnimationRendering(
                    NEUTRAL_LEFT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.FacingForward,
                 new NewAnimationRendering(
                    FACING_FORWARD,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.NeutralRightTorch,
                new NewAnimationRendering(
                    NEUTRAL_RIGHT_TORCH,
                    new Vector2(93f, 180f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.NeutralLeftTorch,
                new NewAnimationRendering(
                    NEUTRAL_LEFT_TORCH,
                    new Vector2(93f, 180f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion

            #region Idle

            dictionary.Add(
                BodyStates.IdleRightOpen,
                new NewAnimationRendering(
                        IDLE_OPEN_HAND,
                        new Vector2(77f, 154f),
                        5,
                        Vector2.Zero,
                        0,
                        Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(
                BodyStates.IdleRightClosed,
                new NewAnimationRendering(
                        IDLE_CLOSED_HAND,
                        new Vector2(77f, 154f),
                        5,
                        Vector2.Zero,
                        0,
                        Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            #endregion

            #region Walking

            dictionary.Add(BodyStates.WalkingStartRight,
                new NewAnimationRendering(
                    WALKING_RIGHT_INTERMEDIATE,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.WalkingRight,
                new NewAnimationRendering(
                    WALKING_RIGHT,
                    new Vector2(77f, 154f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.WalkingEndRight,
                new NewAnimationRendering(
                    WALKING_RIGHT_INTERMEDIATE,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.WalkingStartLeft,
                new NewAnimationRendering(
                    WALKING_LEFT_INTERMEDIATE,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.WalkingLeft,
                new NewAnimationRendering(
                    WALKING_LEFT,
                    new Vector2(77f, 154f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.WalkingEndLeft,
                new NewAnimationRendering(
                    WALKING_LEFT_INTERMEDIATE,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingTorchRight,
                new NewAnimationRendering(
                    WALKING_TORCH_RIGHT,
                    new Vector2(93f, 180f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingTorchStartRight,
                new NewAnimationRendering(
                    WALKING_TORCH_RIGHT_INTERMEDIATE,
                    new Vector2(93f, 180f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingTorchEndRight,
                new NewAnimationRendering(
                    WALKING_TORCH_RIGHT_INTERMEDIATE,
                    new Vector2(93f, 180f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingTorchLeft,
                new NewAnimationRendering(
                    WALKING_TORCH_LEFT,
                    new Vector2(93f, 180f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingTorchStartLeft,
                new NewAnimationRendering(
                    WALKING_TORCH_LEFT_INTERMEDIATE,
                    new Vector2(93f, 180f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingTorchEndLeft,
                new NewAnimationRendering(
                    WALKING_TORCH_LEFT_INTERMEDIATE,
                    new Vector2(93f, 180f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion

            #region Running
            dictionary.Add(BodyStates.RunningStopLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT_INTERMEDIATE,
                    new Vector2(154f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.RunningStopRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT_INTERMEDIATE,
                    new Vector2(154f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.RunningStartLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT_INTERMEDIATE,
                    new Vector2(154f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.RunningStartRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT_INTERMEDIATE,
                    new Vector2(154f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.RunningLeft,
                new NewAnimationRendering(
                    RUNNING_LEFT,
                    new Vector2(154f, 154f),
                    8,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.RunningRight,
                new NewAnimationRendering(
                    RUNNING_RIGHT,
                    new Vector2(154f, 154f),
                    8,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion
            #region Jumping

            dictionary.Add(BodyStates.JumpingRight,
                new NewAnimationRendering(
                    JUMPING_RIGHT,
                    new Vector2(76.75f, 154f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.JumpingLeft,
                new NewAnimationRendering(
                    JUMPING_LEFT,
                    new Vector2(76.75f, 154f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.JumpingRightTorch,
                new NewAnimationRendering(
                    JUMPING_RIGHT_TORCH,
                    new Vector2(92f, 170f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.JumpingLeftTorch,
                new NewAnimationRendering(
                    JUMPING_LEFT_TORCH,
                    new Vector2(92f, 170f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion

            #region Climbing
            dictionary.Add(BodyStates.ClimbingBack,
                new NewAnimationRendering(
                    CLIMBING_BACK,
                    new Vector2(77f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.ClimbingBackNeut,
                new NewAnimationRendering(
                    CLIMBING_NEUT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.ClimbingLeft,
               new NewAnimationRendering(
                    CLIMBING_LEFT,
                    new Vector2(77f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.ClimbingRight,
               new NewAnimationRendering(
                    CLIMBING_RIGHT,
                    new Vector2(77f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.ClimbingRightNeutral,
               new NewAnimationRendering(
                    CLIMBING_NEUTRAL_RIGHT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.ClimbingLeftNeutral,
               new NewAnimationRendering(
                    CLIMBING_NEUTRAL_LEFT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.ClimbingLookRight,
               new NewAnimationRendering(
                    CLIMBING_LOOKING_RIGHT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.ClimbingLookLeft,
               new NewAnimationRendering(
                    CLIMBING_LOOKING_LEFT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.HorizontalClimbLeft,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_LEFT,
                    new Vector2(77f, 154f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.HorizontalClimbRight,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_RIGHT,
                    new Vector2(77f, 154f),
                    4,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            dictionary.Add(BodyStates.HorizontalClimbRightNeut,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_RIGHT_NEUT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(BodyStates.HorizontalClimbLeftNeut,
               new NewAnimationRendering(
                    HORIZ_CLIMBING_LEFT_NEUT,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion

            #region Shooting

            dictionary.Add(
                BodyStates.ShootingArrowLeft,
                 new NewAnimationRendering(
                    SHOOT_ARROW_LEFT,
                    new Vector2(154f, 185f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.ShootingArrowRight,
                 new NewAnimationRendering(
                    SHOOT_ARROW_RIGHT,
                    new Vector2(154f, 185f),
                    5,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion
            #region WalkingShooting

            dictionary.Add(
                BodyStates.WalkingDrawnLeft,
                 new NewAnimationRendering(
                    WALK_DRAWN_LEFT,
                    new Vector2(94f, 169f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingDrawnRight,
                 new NewAnimationRendering(
                    WALK_DRAWN_RIGHT,
                    new Vector2(94f, 171f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingShootLeft,
                 new NewAnimationRendering(
                    WALK_SHOOT_LEFT,
                    new Vector2(158f, 146f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingShootRight,
                 new NewAnimationRendering(
                    WALK_SHOOT_RIGHT,
                    new Vector2(159f, 146f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingShoot2Left,
                 new NewAnimationRendering(
                    WALK_SHOOT2_LEFT,
                    new Vector2(135f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingShoot2Right,
                 new NewAnimationRendering(
                    WALK_SHOOT2_RIGHT,
                    new Vector2(135f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingShoot3Left,
                 new NewAnimationRendering(
                    WALK_SHOOT3_LEFT,
                    new Vector2(104f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.WalkingShoot3Right,
                 new NewAnimationRendering(
                    WALK_SHOOT3_RIGHT,
                    new Vector2(104f, 147f),
                    6,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.ShootingArrowNeutLeft,
                 new NewAnimationRendering(
                    SHOOT_NEUT_LEFT,
                    new Vector2(158f, 146f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.ShootingArrowNeutRight,
                 new NewAnimationRendering(
                    SHOOT_NEUT_RIGHT,
                    new Vector2(159f, 146f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion

            #region Knockback
            dictionary.Add(
                BodyStates.KnockbackRight,
                new NewAnimationRendering(
                    KNOCKBACK_RIGHT,
                    new Vector2(77f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.KnockbackLeft,
                new NewAnimationRendering(
                    KNOCKBACK_LEFT,
                    new Vector2(77f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });
            #endregion

            #region Ducking
            dictionary.Add(
                BodyStates.DuckingLeft,
                 new NewAnimationRendering(
                    DUCK_LEFT,
                    new Vector2(154f, 154f),
                    3,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.DuckingRight,
                 new NewAnimationRendering(
                    DUCK_RIGHT,
                    new Vector2(154f, 154f),
                    3,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.DuckingLeftBow,
                 new NewAnimationRendering(
                    DUCK_LEFT_BOW,
                    new Vector2(154f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.DuckShootLeftBow,
                 new NewAnimationRendering(
                    DUCK_LEFT_SHOOT_BOW,
                    new Vector2(154f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.DuckingRightBow,
                 new NewAnimationRendering(
                    DUCK_RIGHT_BOW,
                    new Vector2(154f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            dictionary.Add(
                BodyStates.DuckShootRightBow,
                 new NewAnimationRendering(
                    DUCK_RIGHT_SHOOT_BOW,
                    new Vector2(154f, 154f),
                    2,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            #endregion

            dictionary.Add(
                BodyStates.FacingBack,
                 new NewAnimationRendering(
                    FACING_BACK,
                    new Vector2(77f, 154f),
                    1,
                    Vector2.Zero,
                    0,
                    Vector2.One,
                    invulnTint) { DepthWithinLayer = -100 });

            return dictionary;
        }

        private Dictionary<BodyStates, Body> CreatePhysics()
        {
            var dictionary = new Dictionary<BodyStates, Body>();

            Width = spriteWidth;
            Height = spriteHeight;
            float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
            float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);


            #region Neutral

            //newBody = 

            #endregion

            return dictionary;
        }

        public bool RightFacingBodyState()
        {
            return (currentState == BodyStates.ClimbingRight ||
                    currentState == BodyStates.ClimbingRightNeutral ||
                    currentState == BodyStates.ClimbingLookLeft ||
                    currentState == BodyStates.WalkingEndRight ||
                    currentState == BodyStates.WalkingStartRight ||
                    currentState == BodyStates.WalkingRight ||
                    currentState == BodyStates.WalkingDrawnRight ||
                    currentState == BodyStates.WalkingShootRight ||
                    currentState == BodyStates.WalkingShoot2Right ||
                    currentState == BodyStates.WalkingShoot3Right ||
                    currentState == BodyStates.WalkingTorchRight ||
                    currentState == BodyStates.WalkingTorchStartRight ||
                    currentState == BodyStates.WalkingTorchEndRight ||
                    currentState == BodyStates.ShootingArrowRight ||
                    currentState == BodyStates.ShootingArrowNeutRight ||
                    currentState == BodyStates.RunningRight ||
                    currentState == BodyStates.NeutralRight ||
                    currentState == BodyStates.NeutralRightTorch ||
                    currentState == BodyStates.IdleRightOpen ||
                    currentState == BodyStates.IdleRightClosed ||
                    currentState == BodyStates.DuckingRight ||
                    currentState == BodyStates.DuckingRightBow ||
                    currentState == BodyStates.DuckShootRightBow ||
                    currentState == BodyStates.JumpingRight);
        }
        public bool LeftFacingBodyState()
        {
            return (currentState == BodyStates.ClimbingLeft ||
                    currentState == BodyStates.ClimbingLeftNeutral ||
                    currentState == BodyStates.ClimbingLookRight ||
                    currentState == BodyStates.WalkingEndLeft ||
                    currentState == BodyStates.WalkingStartLeft ||
                    currentState == BodyStates.WalkingLeft ||
                    currentState == BodyStates.WalkingDrawnLeft ||
                    currentState == BodyStates.WalkingShootLeft ||
                    currentState == BodyStates.WalkingShoot2Left ||
                    currentState == BodyStates.WalkingShoot3Left ||
                    currentState == BodyStates.ShootingArrowLeft ||
                    currentState == BodyStates.ShootingArrowNeutLeft ||
                    currentState == BodyStates.RunningLeft ||
                    currentState == BodyStates.NeutralLeft ||
                    currentState == BodyStates.IdleLeftOpen ||
                    currentState == BodyStates.IdleLeftClosed ||
                    currentState == BodyStates.DuckingLeft ||
                    currentState == BodyStates.DuckingLeftBow ||
                    currentState == BodyStates.DuckShootLeftBow ||
                    currentState == BodyStates.JumpingLeft);
        }
        public bool ClimbingState()
        {
            return (currentState == BodyStates.ClimbingBack ||
                    currentState == BodyStates.ClimbingBackNeut ||
                    currentState == BodyStates.ClimbingLeft ||
                    currentState == BodyStates.ClimbingRight ||
                    currentState == BodyStates.ClimbingLeftNeutral ||
                    currentState == BodyStates.ClimbingRightNeutral ||
                    currentState == BodyStates.ClimbingLookLeft ||
                    currentState == BodyStates.ClimbingLookRight);
        }
        public bool VineBridgeState()
        {
            return (currentState == BodyStates.HorizontalClimbLeft ||
                    currentState == BodyStates.HorizontalClimbRight ||
                    currentState == BodyStates.HorizontalClimbLeftNeut ||
                    currentState == BodyStates.HorizontalClimbRightNeut);
        }

        public bool IdleState()
        {
            return (currentState == BodyStates.IdleLeftClosed ||
                    currentState == BodyStates.IdleLeftOpen ||
                    currentState == BodyStates.IdleRightClosed ||
                    currentState == BodyStates.IdleRightOpen);
        }

        public void DismountLadder(float oldDamping)
        {
            if (Climbing)
            {
                if (RightFacingBodyState())
                {
                    currentState = BodyStates.JumpingRight;
                    Physics.ApplyLinearImpulse(new Vector2(3, -2));
                }
                else if (LeftFacingBodyState())
                {
                    currentState = BodyStates.JumpingLeft;
                    Physics.ApplyLinearImpulse(new Vector2(-3, -2));
                }
                else
                    currentState = BodyStates.JumpingRight;
            }
            Physics.IgnoreGravity = false;
            Physics.LinearDamping = oldDamping;
            Climbing = false;
            WheelBody.CollidesWith = Category.Cat1 | Category.Cat31;
        }


        public void RegisterDot(DamageOverTimeEffect dot)
        {
            Dots.Add(dot);
        }

        private bool initialized;
        public RevoluteJoint MotorJoint { get; set; }
        public float RopeAttachHeight;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            const float SPRITE_OFFSET = 5;
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                _world = world;

                Width = spriteWidth;
                Height = spriteHeight;
                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                Physics = BodyFactory.CreateBody(world, Position, this);
                DoorType = DoorType.None;

                var wPos = Position +
                           new Vector2(0, (spriteHeightMeters - spriteWidthMeters) / 2 +
                           PhysicsConstants.PixelsToMeters(SPRITE_OFFSET));
                WheelBody = BodyFactory.CreateBody(world, wPos, this);

                var r = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters - spriteWidthMeters / 2,
                    1.4f,
                    new Vector2(0, -spriteWidthMeters / 4 + PhysicsConstants.PixelsToMeters(SPRITE_OFFSET)),
                    Physics);

                var c = FixtureFactory.AttachCircle(
                    spriteWidthMeters / 2,
                    1.4f,
                    WheelBody,
                    new Vector2(0, 0));
                var l = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1.4f,
                    new Vector2(0, PhysicsConstants.PixelsToMeters(SPRITE_OFFSET)),
                    Physics);

                l.IsSensor = true;
                l.UserData = "Ladder";
                l.Shape.Density = 0;

                r.CollidesWith = Category.Cat1;
                r.CollisionCategories = Category.Cat3;
                c.CollidesWith = Category.Cat1 | Category.Cat31;
                c.CollisionCategories = Category.Cat3;
                c.UserData = "Circle";
                r.UserData = "Rectangle";

                var rSens = r.Clone(r.Body);
                var cSens = c.Clone(c.Body);

                MotorJoint = JointFactory.CreateRevoluteJoint(world, Physics, WheelBody, Vector2.Zero);
                MotorJoint.MotorEnabled = true;
                MotorJoint.MaxMotorTorque = MOTOR_TORQUE;

                rSens.IsSensor = true;
                rSens.Shape.Density = 0;
                cSens.IsSensor = true;
                cSens.Shape.Density = 0;

                rSens.CollidesWith = Category.All;
                cSens.CollidesWith = Category.All;
                rSens.CollisionCategories = Category.Cat2;
                cSens.CollisionCategories = Category.Cat2;

                Physics.BodyType = BodyType.Dynamic;
                Physics.FixedRotation = true;
                Physics.Friction = 10.0f;
                WheelBody.BodyType = BodyType.Dynamic;
                WheelBody.Friction = 10.0f;
                Physics.IsBullet = true;

                RopeAttachHeight = -4 * (PhysicsConstants.PixelsToMeters(Height) / 9);

                var ropeSensor = FixtureFactory.AttachCircle(
                    .08f, 5, Physics, new Vector2(0, RopeAttachHeight));
                ropeSensor.Friction = 5f;
                ropeSensor.Restitution = 1f;
                ropeSensor.UserData = this;
                ropeSensor.IsSensor = true;
                ropeSensor.CollidesWith = Category.Cat4;
                ropeSensor.CollisionCategories = Category.Cat4;

                ropeSensor.RegisterOnCollidedListener<VineBridge>(OnCollidedWith);
                ropeSensor.RegisterOnSeparatedListener<VineBridge>(OnSeparation);

                var vineSensor = FixtureFactory.AttachCircle(.1f, 5, Physics, Vector2.Zero);
                vineSensor.Friction = 5f;
                vineSensor.Restitution = 1f;
                vineSensor.UserData = this;
                vineSensor.IsSensor = true;
                vineSensor.CollidesWith = Category.Cat5;
                vineSensor.CollisionCategories = Category.Cat5;

                vineSensor.RegisterOnCollidedListener<Vine>(OnCollidedWith);
                vineSensor.RegisterOnSeparatedListener<Vine>(OnSeparation);

                Physics.RegisterOnCollidedListener<Bramble>(OnCollidedWith);
                Physics.RegisterOnSeparatedListener<Bramble>(OnSeparation);
                r.RegisterOnCollidedListener<Torch>(OnCollidedWith);
                r.RegisterOnSeparatedListener<Torch>(OnSeparation);
                c.RegisterOnCollidedListener<PlaceTorchTrigger>(OnCollidedWith);
                c.RegisterOnSeparatedListener<PlaceTorchTrigger>(OnSeparation);
                c.RegisterOnCollidedListener<WorldGeometry2>(OnCollidedWith);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
            WheelBody.Dispose();
        }

        public void Reset(Vector2 newPos, IComponentContext engineRegistrations)
        {
            //Physics.Dispose();
            //WheelBody.Dispose();
            //InitializePhysics(true, engineRegistrations);

            //Physics.Position = newPos;
        }


        #region Logging

        private void LogMetricSnapshot()
        {
            Logger.Info(String.Format("STATUS - h: {0} m: {1} p: {2}", Health, Mana, FormatPosition(Position)));
        }

        private String FormatPosition(Vector2 input)
        {
            return String.Format("{0} {1}", Position.X, Position.Y);
        }

        public void LogLevelSummary()
        {
            Logger.Info("Level finished:");
            Logger.Info(String.Format("Jumps: {0}", numberOfJumps));
            Logger.Info(String.Format("Damage taken: {0}", damageTaken));
            Logger.Info(String.Format("Idle time(ms): {0}", totalIdleTime));
            Logger.Info(String.Format("Sprinting time(ms): {0}", totalSprintingTime));
            ResetSummaryMetrics();
        }

        private void ResetSummaryMetrics()
        {
            numberOfJumps = 0;
            damageTaken = 0f;
            idleTime = 0f;
            totalIdleTime = 0f;
            totalSprintingTime = 0f;
            isIdleLogged = false;
        }

        #endregion

        private void ReRegisterPhysics()
        {
            Physics.Dispose();
            Width = spriteWidth;
            Height = spriteHeight;
            float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
            float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

            Physics = BodyFactory.CreateBody(_world, Position, this);
            DoorType = DoorType.None;

            var wPos = Position +
           new Vector2(0, (spriteHeightMeters - spriteWidthMeters) / 2 +
           PhysicsConstants.PixelsToMeters(5));

            WheelBody.Position = wPos;
            // Physics.Position = new Vector2(Physics.Position.X, Physics.Position.Y - (spriteHeightMeters / 2));

            var r = FixtureFactory.AttachRectangle(
                spriteWidthMeters,
                spriteHeightMeters - spriteWidthMeters / 2,
                1.4f,
                new Vector2(0, -spriteWidthMeters / 4 + PhysicsConstants.PixelsToMeters(5)),
                Physics);

            var l = FixtureFactory.AttachRectangle(
                spriteWidthMeters,
                spriteHeightMeters,
                1.4f,
                new Vector2(0, 0 + PhysicsConstants.PixelsToMeters(5)),
                Physics);

            l.IsSensor = true;
            l.UserData = "Ladder";
            l.Shape.Density = 0;

            r.CollidesWith = Category.Cat1;
            r.CollisionCategories = Category.Cat3;
            r.UserData = "Rectangle";

            var rSens = r.Clone(r.Body);

            MotorJoint = JointFactory.CreateRevoluteJoint(_world, Physics, WheelBody, Vector2.Zero);
            MotorJoint.MotorEnabled = true;
            MotorJoint.MaxMotorTorque = 10;

            rSens.IsSensor = true;
            rSens.Shape.Density = 0;

            rSens.CollidesWith = Category.All;
            rSens.CollisionCategories = Category.Cat2;

            Physics.BodyType = BodyType.Dynamic;
            Physics.FixedRotation = true;
            Physics.Friction = 10.0f;
            WheelBody.BodyType = BodyType.Dynamic;
            WheelBody.Friction = 10.0f;
            Physics.IsBullet = true;

            RopeAttachHeight = -4 * (PhysicsConstants.PixelsToMeters(Height) / 9);

            var ropeSensor = FixtureFactory.AttachCircle(
                .08f, 5, Physics, new Vector2(0, RopeAttachHeight));
            ropeSensor.Friction = 5f;
            ropeSensor.Restitution = 1f;
            ropeSensor.UserData = this;
            ropeSensor.IsSensor = true;
            ropeSensor.CollidesWith = Category.Cat4;
            ropeSensor.CollisionCategories = Category.Cat4;

            ropeSensor.RegisterOnCollidedListener<VineBridge>(OnCollidedWith);
            ropeSensor.RegisterOnSeparatedListener<VineBridge>(OnSeparation);

            var vineSensor = FixtureFactory.AttachCircle(.1f, 5, Physics, Vector2.Zero);
            vineSensor.Friction = 5f;
            vineSensor.Restitution = 1f;
            vineSensor.UserData = this;
            vineSensor.IsSensor = true;
            vineSensor.CollidesWith = Category.Cat5;
            vineSensor.CollisionCategories = Category.Cat5;

            vineSensor.RegisterOnCollidedListener<Vine>(OnCollidedWith);
            vineSensor.RegisterOnSeparatedListener<Vine>(OnSeparation);

            Physics.RegisterOnCollidedListener<Bramble>(OnCollidedWith);
            Physics.RegisterOnSeparatedListener<Bramble>(OnSeparation);
            r.RegisterOnCollidedListener<Torch>(OnCollidedWith);
            r.RegisterOnSeparatedListener<Torch>(OnSeparation);
        }

        HashSet<SlideTrigger> slideTriggers;

        private bool isSliding;
        private bool ignoreOneWays;

        public void AddSlideTrigger(SlideTrigger st)
        {
            slideTriggers.Add(st);
        }

        public void RemoveSlideTrigger(SlideTrigger st)
        {
            if (slideTriggers.Remove(st) && !CanSlide)
                StopSliding();
        }

        void StartSliding(MoveDirection dir)
        {
            MotorJoint.MotorSpeed = dir == MoveDirection.Right
                ? 75
                : -75;
            WheelBody.Friction = Single.MaxValue;
            isSliding = true;
        }

        void StopSliding()
        {
            MotorJoint.MotorSpeed = 0;
            WheelBody.Friction = 5f;
            isSliding = false;
        }

        BodyStates NeutralState()
        {
            if (facing == 1)
            {
                if (InventoryItem is Arrow)
                {
                    if (InHold)
                    {
                        return BodyStates.ShootingArrowRight;
                    }
                    else
                    {
                        animations[BodyStates.ShootingArrowRight].CurrentFrame = 0;
                        return BodyStates.ShootingArrowRight;
                    }
                }
                else
                    return BodyStates.NeutralRight;
            }
            else
            {
                if (InventoryItem is Arrow)
                {
                    if (InHold)
                    {
                        return BodyStates.ShootingArrowLeft;
                    }
                    else
                    {
                        animations[BodyStates.ShootingArrowLeft].CurrentFrame = 0;
                        return BodyStates.ShootingArrowLeft;
                    }
                }
                else
                    return BodyStates.NeutralLeft;
            }
        }
        public void Electrocute()
        {

        }
    }
}