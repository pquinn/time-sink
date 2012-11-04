using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Rendering;

using MechanicsTest.Collisions;
using MechanicsTest.Controller;
using MechanicsTest.Physics;
using SynapseGaming.LightingSystem.Core;
using Microsoft.Xna.Framework.Content;

namespace MechanicsTest
{
    public class UserControlledCharacter 
        : IPhysicsEnabledBody, IKeyboardControllable, ICollideable
    {
        const float PLAYER_MASS = 100f;

        SpriteContainer playerSprites;
        float playerRotation = 0.0f;
        private BaseRenderableEffect playerTexture;
        private GravityPhysics physics;
        private bool gravityToggleGuard = true;

        //private CollisionRectangle collisionGeometry;
        public ICollisionGeometry CollisionGeometry
        {
            get 
            { 
                return new CollisionRectangle(
                    new Rectangle(
                        (int)physics.Position.X,
                        (int)physics.Position.Y,
                        128, 128
                    )
                );
            }
        }

        public UserControlledCharacter(Vector2 position)
        {
            physics = new GravityPhysics(position, PLAYER_MASS)
            {
                GravityEnabled = false
            };
        }

        public void Load(ContentManager content, SpriteManager manager, SceneInterface scene)
        {
            playerTexture = content.Load<BaseRenderableEffect>("Materials/Dude");

            // First create and submit the empty player container.
            playerSprites = manager.CreateSpriteContainer();
            scene.ObjectManager.Submit(playerSprites);
        }

        public void Draw(GameTime gameTime)
        {
            playerSprites.Begin();

            playerSprites.Add(
                playerTexture, 
                Vector2.One * 0.32f, 
                physics.Position, 
                0, 
                0);

            playerSprites.End();
        }

        public void Update(GameTime gameTime, StarterGame gameWorld)
        {

        }

        public IPhysicsParticle PhysicsController { get { return physics; } }

        public void HandleKeyboardInput(GameTime gameTime)
        {
            // Get the gamepad state.
            var gamepadstate = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            // Get the time scale since the last update call.
            var timeframe = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var amount = 1f;
            var movedirection = new Vector2();

            // Grab the keyboard state.
            var keyboard = Keyboard.GetState();

            // Get the keyboard direction.
            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
                movedirection.Y += 1.0f;
            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
                movedirection.Y -= 1.0f;
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
                movedirection.X += 1.0f;
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
                movedirection.X -= 1.0f;
            if (keyboard.IsKeyDown(Keys.Space))
            {
                if (gravityToggleGuard)
                {
                    physics.GravityEnabled = !physics.GravityEnabled;
                    if (!physics.GravityEnabled)
                        physics.Velocity = Vector2.Zero;
                    gravityToggleGuard = false;
                }
            }
            else
            {
                gravityToggleGuard = true;
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

        public bool GravityEnabled
        {
            get { return physics.GravityEnabled; }
            set { physics.GravityEnabled = value; }
        }
    }
}
