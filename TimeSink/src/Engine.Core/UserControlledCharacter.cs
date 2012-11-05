using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

//using SynapseGaming.LightingSystem.Effects;
//using SynapseGaming.LightingSystem.Rendering;

using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using Microsoft.Xna.Framework.Content;
//using SynapseGaming.LightingSystem.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TimeSink.Engine.Core
{
    public class UserControlledCharacter 
        : IPhysicsEnabledBody, IKeyboardControllable, ICollideable
    {
        const float PLAYER_MASS = 100f;

      //  SpriteContainer playerSprites;
        float playerRotation = 0.0f;
        //private BaseRenderableEffect playerTexture;
        private Texture2D playerTexture;
        private SpriteBatch playerSprites;
        private GravityPhysics physics;
        private SoundEffect jumpSound;
        private bool jumpToggleGuard = true;
        private bool touchingGround = false;

        //private AACollisionRectangle collisionGeometry;
        public ICollisionGeometry CollisionGeometry
        {
            get 
            { 
                return new CollisionRectangle(
                    new Rectangle(
                        (int)physics.Position.X,
                        (int)physics.Position.Y,
                        128, 129
                    )
                );
            }
        }

        public UserControlledCharacter(Vector2 position)
        {
            physics = new GravityPhysics(position, PLAYER_MASS)
            {
                GravityEnabled = true
            };
        }

        public void Load(ContentManager content /*, SpriteManager manager, SceneInterface scene*/)
        {
            playerTexture = content.Load<Texture2D>("Textures/giroux");

            jumpSound = content.Load<SoundEffect>("Audio/Sounds/Hop");
            // First create and submit the empty player container.
           /* playerSprites = manager.CreateSpriteContainer();
            scene.ObjectManager.Submit(playerSprites);*/
          
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            playerSprites = spriteBatch;

            playerSprites.Begin();

        /*    playerSprites.Add(
                playerTexture, 
                Vector2.One * 0.32f, 
                physics.Position, 
                0, 
                0);*/
            playerSprites.Draw(playerTexture, physics.Position, Color.White);

            playerSprites.End();
        }

        public void Update_Pre(GameTime gameTime)
        {
            touchingGround = false;
        }

        public void Update_Post(GameTime gameTime)
        {
            if (!touchingGround)
                GravityEnabled = true;
        }

        public IPhysicsParticle PhysicsController { get { return physics; } }

        public void HandleKeyboardInput(GameTime gameTime)
        {
            // Get the gamepad state.
            var gamepadstate = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            // Get the time scale since the last update call.
            var timeframe = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var amount = 300f;
            var movedirection = new Vector2();

            // Grab the keyboard state.
            var keyboard = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            #region Gamepad Handling
            if (gamepad.DPad.Left.Equals(ButtonState.Pressed))
                movedirection.X -= 1.0f;
            if (gamepad.DPad.Right.Equals(ButtonState.Pressed))
                movedirection.X += 1.0f;
            if (gamepad.ThumbSticks.Left.X != 0)
                movedirection.X += gamepad.ThumbSticks.Left.X;
            if (gamepad.Buttons.A.Equals(ButtonState.Pressed))
            {
                //Insert Jump Logic Here
                jumpSound.Play();
            }
            #endregion

            #region Keyboard Handling
            // Get the keyboard direction.
            //if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            //    movedirection.Y -= 1.0f;
            //if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            //    movedirection.Y += 1.0f;
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
                movedirection.X -= 1.0f;
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
                movedirection.X += 1.0f;
            if (keyboard.IsKeyDown(Keys.Space) 
                || keyboard.IsKeyDown(Keys.W) 
                || keyboard.IsKeyDown(Keys.Up))
            {
                if (jumpToggleGuard && touchingGround)
                {
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

        public bool GravityEnabled
        {
            get { return physics.GravityEnabled; }
            set { physics.GravityEnabled = value; }
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(WorldGeometry world, CollisionInfo info)
        {
            //GravityEnabled = false;
            //PhysicsController.Velocity = Vector2.Zero;

            //Handle whether collision should disable gravity
            if (info.MinimumTranslationVector.Y > 0)
            {
                touchingGround = true;
                GravityEnabled = false;
                physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
            }

            //Let's think about the proper way to do gravity
            //Each frame, we check for collision with the ground
            //   ... this is fairly simple, since OnCollidedWith should be called each
            //       tick that the player is touching the ground
            //How do we detect if that hasn't been called?
            //A: Set a flag in OnCollidedWith, check if that flag is enabled in Update
            //   ... this requires that Update gets called after collision handling
            //   ... expose Update_PreCollision / PostCollision???
        }
    }
}
