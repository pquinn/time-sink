using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Game.Entities.Weapons;

using Engine.Game.Entities;

namespace TimeSink.Engine.Game.Entities
{
    public class UserControlledCharacter : Entity, IHaveHealth, IHaveShield, IHaveMana
    {
        const float PLAYER_MASS = 100f;

        enum BodyStates { Neutral, Idle, StartWalking, Walking, StopWalking, StartRunning, Running, StopRunning, Jumping, Shooting };
        int currentState;

        //Texture strings for content loading
        const string PLAYER_TEXTURE_NAME = "Textures/Sprites/SpriteSheet";
        const string JUMP_SOUND_NAME = "Audio/Sounds/Hop";
        const string BODY_START_WALK = "Textures/Sprites/SpriteSheets/StartWalk";
        const string BODY_START_RUN = "Textures/Sprites/SpriteSheets/StartRun";
        const string BODY_WALK = "Textures/Sprites/SpriteSheets/BodyWalk";
        const string BODY_RUN = "Textures/Sprites/SpriteSheets/Running";
        const string BODY_JUMP = "Textures/Sprites/SpriteSheets/jump";
        const string ARM_MOVE = "Textures/Sprites/SpriteSheets/arm move";
        const string HAIR_MOVE = "Textures/Sprites/SpriteSheets/HairMove";
        const string HAND_CLOSE = "Textures/Sprites/SpriteSheets/openClose";
        const string HEAD_STATES = "Textures/Sprites/SpriteSheets/headStates";
        const string IDLE_BODY_HEAD_HAIR = "Textures/Sprites/SpriteSheets/IdleBody+Head+Hair";
        const string BODY_NEUTRAL = "Textures/Sprites/Body/Body_Neutral";

        //Animation holders which create Renderings and set up relative positions
        //**We will not sizes/relative positions once all sprites are resized to 128x256
        Animation bodyWalk = new Animation(7, BODY_WALK, 120, 198, new Vector2(0, 45));
        Animation bodyRun = new Animation(8, BODY_RUN, 209, 191, Vector2.Zero);
        Animation bodyStartWalk = new Animation(2, BODY_START_WALK, 109, 198, new Vector2(0, 45));
        Animation bodyStartRun = new Animation(2, BODY_START_RUN, 81, 198, Vector2.Zero);
        Animation bodyJump = new Animation(4, BODY_JUMP, 136, 159, Vector2.Zero);
        Animation hairMove = new Animation(2, HAIR_MOVE, 66, 63, Vector2.Zero);
        Animation armMove = new Animation(2, ARM_MOVE, 51, 85, new Vector2(12,65));
        Animation idle = new Animation(6, IDLE_BODY_HEAD_HAIR, 95, 245, Vector2.Zero);




        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 500;
        const float MAX_ARROW_INIT_SPEED = 1500;
        public const int X_OFFSET = 60;
        public const int Y_OFFSET = 80;

        private GravityPhysics physics;
        private SoundEffect jumpSound;
        private bool jumpToggleGuard = true;
        private bool __touchingGroundFlag = false;
        private bool touchingGround = false;
        private bool jumpStarted = false;
        private Rectangle sourceRect;
        private float health;
        private float mana;
        private float shield;

        private List<IInventoryItem> inventory;
        private int activeItem;

        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        public float Shield
        {
            get { return shield; }
            set { shield = value; }
        }

        public float Mana
        {
            get { return mana; }
            set { mana = value; }
        }

        private float playerRotation = 0.0f;
        private int facing = 1; // 1 for right, -1 for left

        // not sure if these should be public
        private Vector2 direction;
        public Vector2 Direction
        {
            get { return direction; }
            private set { direction = value; }
        }
        private double holdTime;
        public double HoldTime
        {
            get { return holdTime; }
            set { holdTime = value; }
        }
        private bool inHold;
        public bool InHold
        {
            get { return inHold; }
            set { inHold = value; }
        }


        float timer = 0f;
        float idleInterval = 2000f;
        float interval = 200f;
        float jumpInterval = 100f;
        int currentFrame = 0;
        int spriteWidth = 130;
        int spriteHeight = 242;

        public override ICollisionGeometry CollisionGeometry
        {
            get
            {
                //var colSet = new CollisionSet();
                //colSet.Geometry.Add(new CollisionRectangle(
                //    new Rectangle(
                //        (int)physics.Position.X,
                //        (int)physics.Position.Y,
                //        75, 110)));
                //colSet.Geometry.Add(new CollisionRectangle(
                //    new Rectangle(
                //        (int)physics.Position.X + 50,
                //        (int)physics.Position.Y + 111,
                //        50, 132)));
                //return colSet;
                return new CollisionRectangle(new Rectangle(
                    (int)physics.Position.X,
                    (int)physics.Position.Y,
                    100, 242
                ));
            }
        }

        public UserControlledCharacter(Vector2 position)
        {
            physics = new GravityPhysics(position, PLAYER_MASS)
            {
                GravityEnabled = true
            };
            health = 100;
            direction = new Vector2(1, 0);

            // this seems stupid
            activeItem = 0;
            inventory = new List<IInventoryItem>();
            inventory.Add(new Arrow());
            inventory.Add(new Dart());
        }

        public override void Load(EngineGame game)
        {
            game.TextureCache.LoadResource(PLAYER_TEXTURE_NAME);
            game.TextureCache.LoadResource(BODY_JUMP);
            game.TextureCache.LoadResource(BODY_RUN);
            game.TextureCache.LoadResource(BODY_WALK);
            game.TextureCache.LoadResource(BODY_START_RUN);
            game.TextureCache.LoadResource(BODY_START_WALK);
            game.TextureCache.LoadResource(ARM_MOVE);
            game.TextureCache.LoadResource(HAIR_MOVE);
            game.TextureCache.LoadResource(HAND_CLOSE);
            game.TextureCache.LoadResource(HEAD_STATES);
            game.TextureCache.LoadResource(BODY_NEUTRAL);
            

            jumpSound = game.SoundCache.LoadResource(JUMP_SOUND_NAME);
        }

        public override void Update(GameTime gameTime, EngineGame game)
        {
            if (!__touchingGroundFlag)
                GravityEnabled = true;
            touchingGround = __touchingGroundFlag;
            __touchingGroundFlag = false;
        }

        public override IPhysicsParticle PhysicsController { get { return physics; } }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            // Get the gamepad state.
            var gamepadstate = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            // Get the time scale since the last update call.
            var timeframe = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var amount = 150f;
            var movedirection = new Vector2();

            // Grab the keyboard state.
            var keyboard = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            var d = InputManager.Instance.Pressed(Keys.D);
            var a = InputManager.Instance.Pressed(Keys.A);

            //Update the animation timer by the timeframe in milliseconds
            timer += (timeframe * 1000);

            UpdateAnimationStates();

            #region Movement
            if (gamepad.DPad.Left.Equals(ButtonState.Pressed))
            {
                movedirection.X -= 1.0f;
                if (touchingGround)
                {
                    currentState = (int)BodyStates.Walking;
                    AnimateRight(gameTime);
                }
            }
            if (gamepad.DPad.Right.Equals(ButtonState.Pressed))
            {
                movedirection.X += 1.0f;
                if (touchingGround)
                {
                    currentState = (int)BodyStates.Walking;
                    AnimateRight(gameTime);
                }
            }
            if (gamepad.ThumbSticks.Left.X != 0)
            {
                movedirection.X += gamepad.ThumbSticks.Left.X;
                if (touchingGround)
                {
                    currentState = (int)BodyStates.Walking;
                    AnimateRight(gameTime);
                }
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                movedirection.X -= 1.0f;
                if (touchingGround)
                {
                    if (currentState != (int)BodyStates.Walking)
                    {
                        currentState = (int)BodyStates.StartWalking;
                    }
                    else
                    {
                        currentState = (int)BodyStates.Walking;
                        //AnimateRight(gameTime);
                    }
                }
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                movedirection.X += 1.0f;
                if (touchingGround)
                {
                    if (currentState != (int)BodyStates.Walking)
                    {
                        bodyWalk.Reset();
                        currentState = (int)BodyStates.StartWalking;
                    }
                    else
                    {
                        currentState = (int)BodyStates.Walking;
                        //AnimateRight(gameTime);
                    }
                }
            }
            #endregion

            #region Direction

            var up = InputManager.Instance.Pressed(Keys.Up);
            var down = InputManager.Instance.Pressed(Keys.Down);
            var right = InputManager.Instance.Pressed(Keys.Right);
            var left = InputManager.Instance.Pressed(Keys.Left);

            if (up && right)
            {
                direction = new Vector2(0.707106769f, -0.707106769f);
                facing = 1;
            }
            else if (up && left)
            {
                direction = new Vector2(-0.707106769f, -0.707106769f);
                facing = -1;
            }
            else if (down && right)
            {
                direction = new Vector2(0.707106769f, 0.707106769f);
                facing = 1;
            }
            else if (down && left)
            {
                direction = new Vector2(-0.707106769f, 0.707106769f);
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
            }
            else if (left)
            {
                direction = new Vector2(-1, 0);
                facing = -1;
            }
            else
            {
                direction = new Vector2(1, 0) * facing;
            }


            #endregion

            #region Jumping
            if (keyboard.IsKeyDown(Keys.Space)
                || keyboard.IsKeyDown(Keys.W)
                || gamepad.Buttons.A.Equals(ButtonState.Pressed))
            {
                if (jumpToggleGuard && touchingGround)
                {
                    currentState = (int)BodyStates.Jumping;
                    jumpStarted = true;
                    jumpSound.Play();
                    physics.Velocity -= new Vector2(0, 500);
                    jumpToggleGuard = false;
                    GravityEnabled = true;
                }
            }
            else if (touchingGround)
            {
                jumpToggleGuard = true;
            }

            #endregion

            #region Shooting

            if (InputManager.Instance.IsNewKey(Keys.F))
            {
                currentState = (int)BodyStates.Shooting;
                holdTime = gameTime.TotalGameTime.TotalSeconds;
                inHold = true;
            }
            else if (!InputManager.Instance.Pressed(Keys.F) && inHold)
            {
                inventory[activeItem].Use(this, world, gameTime, holdTime);
            }

            if (InputManager.Instance.IsNewKey(Keys.G))
            {
                if (activeItem == inventory.Count - 1)
                {
                    activeItem = 0;
                }
                else
                {
                    activeItem++;
                }
            }

            #endregion

            //No keys are pressed and we're on the ground, we're neutral
            if ((keyboard.GetPressedKeys().GetLength(0) == 0) && 
                (touchingGround))
            {
                if ((currentState == (int)BodyStates.Walking))
                {
                    bodyStartWalk.Reset();
                    currentState = (int)BodyStates.StopWalking;
                }
                else if ((currentState != (int)BodyStates.Idle) && (currentState != (int)BodyStates.StopWalking))
                {
                    idle.Reset();
                    currentState = (int)BodyStates.Neutral;
                }
            }

            if (movedirection != Vector2.Zero)
            {
                // Normalize direction to 1.0 magnitude to avoid walking faster at angles.
                movedirection.Normalize();
            }

            // Increment animation unless idle.
            if (amount != 0.0f)
            {
                // Rotate the player towards the controller direction.
                playerRotation = (float)(Math.Atan2(movedirection.Y, movedirection.X) + Math.PI / 2.0);

                // Move player based on the controller direction and time scale.
                physics.Position += movedirection * timeframe * amount;
            }
        }

        protected void UpdateAnimationStates()
        {
            if ((currentState == (int)BodyStates.Idle) && (timer >= interval))
            {
                idle.UpdateFrame();
                timer = 0f;
            }
            else if ((currentState == (int)BodyStates.Neutral) && timer >= idleInterval)
            {
                currentState = (int)BodyStates.Idle;
                timer = 0f;
            }

            else if (currentState == (int)BodyStates.Walking && timer >= interval)
            {
                bodyWalk.UpdateFrame();
                timer = 0f;
            }
            else if (currentState == (int)BodyStates.StartWalking && timer >= interval)
            {
                if (bodyStartWalk.CurrentFrame == (bodyStartWalk.TotalFrames))
                {
                    currentState = (int)BodyStates.Walking;
                }
                else
                {
                    bodyStartWalk.UpdateFrame();
                }
                timer = 0f;
            }
            else if (currentState == (int)BodyStates.StopWalking && timer >= interval)
            {
                bodyStartWalk.Reverse();
                if (bodyStartWalk.CurrentFrame == 0)
                {
                    currentState = (int)BodyStates.Neutral;
                    bodyStartWalk.Reset();
                    bodyWalk.Reset();
                }
                timer = 0f;
            }
            else if (currentState == (int)BodyStates.Jumping && timer >= interval)
            {
                if (bodyJump.CurrentFrame != (bodyJump.TotalFrames - 1))
                {
                    bodyJump.UpdateFrame();
                }
                timer = 0f;
            }
        }
        protected void AnimateRight(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > interval)
            {
                currentFrame++;

                if (currentFrame > 8)
                {
                    currentFrame = 0;
                }
                timer = 0f;
            }

        }
        protected void AnimateJump(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer > jumpInterval)
            {
                currentFrame++;

                if (currentFrame > 12)
                {
                    currentFrame = 12;
                }
                timer = 0f;
            }
        }

        public bool GravityEnabled
        {
            get { return physics.GravityEnabled; }
            set { physics.GravityEnabled = value; }
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(WorldGeometry world, CollisionInfo info)
        {
            // Handle whether collision should disable gravity
            if (info.MinimumTranslationVector.Y > 0)
            {
                if (!jumpToggleGuard)
                {
                    bodyJump.Reset();
                }
                __touchingGroundFlag = true;
                GravityEnabled = false;
                physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
            }
        }

        private void CreateAnimationStack(Stack<IRendering> stack)
        {
            #region neutral
            if (currentState == (int)BodyStates.Neutral)
            {
                stack.Push(armMove.Rendering);
                stack.Push(hairMove.Rendering);
                stack.Push(bodyStartWalk.Rendering);
            }
            #endregion
            #region idle
            else if (currentState == (int)BodyStates.Idle)
            {
                stack.Push(armMove.Rendering);
                stack.Push(idle.Rendering);
            }
            #endregion
            #region start walking
            else if (currentState == (int)BodyStates.StartWalking)
            {
                stack.Push(armMove.Rendering);
                stack.Push(bodyStartWalk.Rendering);
            }
            #endregion
            #region stop walking
            else if (currentState == (int)BodyStates.StopWalking)
            {
                stack.Push(armMove.Rendering);
                stack.Push(bodyStartWalk.Rendering);
            }
            #endregion
            #region walking
            else if (currentState == (int)BodyStates.Walking)
            {
                stack.Push(armMove.Rendering);
                stack.Push(bodyWalk.Rendering);
            }
            #endregion
            #region jumping
            else if (currentState == (int)BodyStates.Jumping)
            {
                stack.Push(armMove.Rendering);
                stack.Push(bodyJump.Rendering);
            }
            #endregion
            #region shooting
            if (currentState == (int)BodyStates.Shooting)
            {
                stack.Push(armMove.Rendering);
                stack.Push(hairMove.Rendering);
                stack.Push(bodyStartWalk.Rendering);
            }
            #endregion
        }

        public override IRendering Rendering
        {
            get
            {
               /* Stack<IRendering> stack = new Stack<IRendering>();
                stack.
                return new StackableRendering(*/
                Console.WriteLine(physics.Position);
                Stack<IRendering> stack = new Stack<IRendering>();
                CreateAnimationStack(stack);

                Console.WriteLine(physics.Position.Y);
                return new StackableRendering(stack,physics.Position,0,Vector2.One);
                /*return new BasicRendering(
>>>>>>> ecf9b3b086b551162df87af2e83db4fe0dad82b1
                    PLAYER_TEXTURE_NAME,
                    physics.Position,
                    0,
                    Vector2.One,
                    sourceRect
                );*/
            }
        }

        public void RegisterDot(DamageOverTimeEffect dot)
        {
        }

    }
}
