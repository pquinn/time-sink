using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

using System;

using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

using TimeSink.Engine.Game.Entities.Weapons;

namespace TimeSink.Engine.Game.Entities
{
    public class UserControlledCharacter : Entity
    {
        const float PLAYER_MASS = 100f;

        const string PLAYER_TEXTURE_NAME = "Textures/Sprites/SpriteSheet";
        const string JUMP_SOUND_NAME = "Audio/Sounds/Hop";
        const float MAX_ARROW_HOLD = 1;
        const float MIN_ARROW_INIT_SPEED = 500;
        const float MAX_ARROW_INIT_SPEED = 1500;

        private GravityPhysics physics;
        private SoundEffect jumpSound;
        private bool jumpToggleGuard = true;
        private bool __touchingGroundFlag = false;
        private bool touchingGround = false;
        private bool jumpStarted = false;
        private Rectangle sourceRect;

        private float playerRotation = 0.0f;
        private Vector2 direction = new Vector2(1, 0);
        private int facing = 1; // 1 for right, -1 for left
        private double holdTime;
        private bool inHold;

        float timer = 0f;
        float interval = 150f;
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
                return new AACollisionRectangle(new Rectangle(
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

        }

        public override void Load(EngineGame game)
        {
            game.TextureCache.LoadResource(PLAYER_TEXTURE_NAME);

            /*   SpriteTextureCache.LoadResource("Textures/Sprites/Body/Body_Neutral");
               SpriteTextureCache.LoadResource("Textures/Sprites/Body/Arms/Arm_Neutral");
               SpriteTextureCache.LoadResource("Textures/Sprites/Body/Arms/Hands/Hand_Neutral");
               SpriteTextureCache.LoadResource("Textures/Sprites/Head/Face_Neutral");
               SpriteTextureCache.LoadResource("Textures/Sprites/Head/Hair/Hair_Neutral");

               spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Body/Arms/Hands/Hand_Neutral", new Vector2(37, 80)));
               spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Body/Arms/Arm_Neutral", new Vector2(23, 20)));
               spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Head/Hair/Hair_Neutral", new Vector2(15, -45)));
               spriteStack.Push(new Tuple<string,Vector2>("Textures/Sprites/Head/Face_Neutral", new Vector2(45, -38)));
               spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Body/Body_Neutral", Vector2.Zero));
               */

            //playerTexture = content.Load<Texture2D>("Textures/Sprites/Body/Body_Neutral");

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
            var amount = 300f;
            var movedirection = new Vector2();

            // Grab the keyboard state.
            var keyboard = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            #region Movement
            if (gamepad.DPad.Left.Equals(ButtonState.Pressed))
            {
                movedirection.X -= 1.0f;
                if (touchingGround)
                    AnimateRight(gameTime);
            }
            if (gamepad.DPad.Right.Equals(ButtonState.Pressed))
            {
                movedirection.X += 1.0f;
                if (touchingGround)
                    AnimateRight(gameTime);
            }
            if (gamepad.ThumbSticks.Left.X != 0)
            {
                movedirection.X += gamepad.ThumbSticks.Left.X;
                if (touchingGround)
                    AnimateRight(gameTime);
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                movedirection.X -= 1.0f;
                if (touchingGround)
                    AnimateRight(gameTime);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                movedirection.X += 1.0f;
                if (touchingGround)
                    AnimateRight(gameTime);
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
                    jumpStarted = true;
                    currentFrame = 10;
                    AnimateJump(gameTime);
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
            else if (jumpStarted)
            {
                AnimateJump(gameTime);
            }

            #endregion

            #region Shooting

            if (InputManager.Instance.IsNewKey(Keys.F))
            {
                holdTime = gameTime.TotalGameTime.TotalSeconds;
                inHold = true;
            }
            else if (!InputManager.Instance.Pressed(Keys.F) && inHold)
            {
                inHold = false;
                Arrow arrow = new Arrow(
                    new Vector2(physics.Position.X + 60,
                                physics.Position.Y + 80));
                var elapsedTime = Math.Min(gameTime.TotalGameTime.TotalSeconds - holdTime, MAX_ARROW_HOLD);
                // linear interp: y = 500 + (x - 0)(1300 - 500)/(MAX_HOLD-0) x = elapsedTime
                float speed =
                    MIN_ARROW_INIT_SPEED + (MAX_ARROW_INIT_SPEED - MIN_ARROW_INIT_SPEED) /
                                           MAX_ARROW_HOLD * 
                                           (float)elapsedTime; 
                Vector2 initialVelocity = speed * direction;
                arrow.physics.Velocity += initialVelocity;
                world.Entities.Add(arrow);
                world.RenderManager.RegisterRenderable(arrow);
                world.PhysicsManager.RegisterPhysicsBody(arrow);
                world.CollisionManager.RegisterCollisionBody(arrow);
            }

            #endregion

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
                    currentFrame = 13;
                }
                __touchingGroundFlag = true;
                GravityEnabled = false;
                physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
            }
        }

        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(
                    PLAYER_TEXTURE_NAME,
                    physics.Position,
                    0,
                    Vector2.One,
                    sourceRect
                );
            }
        }
    }
}
