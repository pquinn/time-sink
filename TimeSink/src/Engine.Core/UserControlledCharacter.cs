﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;

using Microsoft.Xna.Framework.Content;
//using SynapseGaming.LightingSystem.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Caching;
using System.Collections.Generic;

namespace TimeSink.Engine.Core
{
    public class UserControlledCharacter 
        : IPhysicsEnabledBody, IKeyboardControllable, ICollideable, IRenderable
    {
        const float PLAYER_MASS = 100f;

        float playerRotation = 0.0f;

        //private BaseRenderableEffect playerTexture;
       // private Texture2D playerTexture;
        private SpriteBatch playerSprites;
        private GravityPhysics physics;
        private SoundEffect jumpSound;
        private Stack<Tuple<string, Vector2>> spriteStack;
        public InMemoryResourceCache<Texture2D> SpriteTextureCache { get; private set; }
        private bool jumpToggleGuard = true;
        private bool touchingGround = false;

        public ICollisionGeometry CollisionGeometry
        {
            get 
            { 
                return new CollisionRectangle(
                    new Rectangle(
                        (int)physics.Position.X,
                        (int)physics.Position.Y,
                        71, 200
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

        public void Load(ContentManager content)
        {
            SpriteTextureCache = new InMemoryResourceCache<Texture2D>(
                new ContentManagerProvider<Texture2D>(content));
            spriteStack = new Stack<Tuple<string, Vector2>>();

            SpriteTextureCache.LoadResource("Textures/Sprites/Body/Body_Neutral");
            SpriteTextureCache.LoadResource("Textures/Sprites/Body/Arms/Arm_Neutral");
            SpriteTextureCache.LoadResource("Textures/Sprites/Body/Arms/Hands/Hand_Neutral");
            SpriteTextureCache.LoadResource("Textures/Sprites/Head/Face_Neutral");
            SpriteTextureCache.LoadResource("Textures/Sprites/Head/Hair/Hair_Neutral");

            spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Body/Arms/Hands/Hand_Neutral", new Vector2(37, 80)));
            spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Body/Arms/Arm_Neutral", new Vector2(23, 20)));
            spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Head/Hair/Hair_Neutral", new Vector2(15, -45)));
            spriteStack.Push(new Tuple<string,Vector2>("Textures/Sprites/Head/Face_Neutral", new Vector2(45, -38)));
            spriteStack.Push(new Tuple<string, Vector2>("Textures/Sprites/Body/Body_Neutral", Vector2.Zero));
            
            
            //playerTexture = content.Load<Texture2D>("Textures/Sprites/Body/Body_Neutral");

            jumpSound = content.Load<SoundEffect>("Audio/Sounds/Hop");
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

        /*    playerSprites.Add(
                playerTexture, 
                Vector2.One * 0.32f, 
                physics.Position, 
                0, 
                0);*/
         //   playerSprites.Draw(playerTexture, physics.Position, Color.White);


            spriteBatch.End();
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

            #region Movement
            if (gamepad.DPad.Left.Equals(ButtonState.Pressed))
                movedirection.X -= 1.0f;
            if (gamepad.DPad.Right.Equals(ButtonState.Pressed))
                movedirection.X += 1.0f;
            if (gamepad.ThumbSticks.Left.X != 0)
                movedirection.X += gamepad.ThumbSticks.Left.X;
            
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
                movedirection.X -= 1.0f;
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
                movedirection.X += 1.0f;
            #endregion

            #region Jumping
            if (keyboard.IsKeyDown(Keys.Space) 
                || keyboard.IsKeyDown(Keys.W) 
                || keyboard.IsKeyDown(Keys.Up)
                || gamepad.Buttons.A.Equals(ButtonState.Pressed))
            {
                if (jumpToggleGuard && touchingGround)
                {
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
            // Handle whether collision should disable gravity
            if (info.MinimumTranslationVector.Y > 0)
            {
                touchingGround = true;
                GravityEnabled = false;
                physics.Velocity = new Vector2(physics.Velocity.X, Math.Min(0, physics.Velocity.Y));
            }
        }

        public IRendering Rendering
        {
            get {return  new StackableRendering(spriteStack, this.physics.Position); }
        }
    }
}
