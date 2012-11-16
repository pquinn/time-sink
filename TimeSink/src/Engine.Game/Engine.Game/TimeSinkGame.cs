//-----------------------------------------------
// Synapse Gaming - SunBurn Starter Kit
//-----------------------------------------------
//
// Provides an empty solution for creating new SunBurn based games and
// projects.
// 
// To use:
//   -Run the solution from Visual Studio
//   -When running press F11 to open the in-game editor
//   -Import new models into the content repository (using the Scene Object tab)
//   -Drag models from the repository into the scene tree-view
//   -Add lights, adjust materials, the environment, and more
//
// Please see the included Readme.htm for details and documentation.
//
//-----------------------------------------------------------------------------

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Include the necessary SunBurn namespaces.
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Sound;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Caching;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TimeSink.Entities;
using TimeSink.Entities.Enemies;
#endregion


namespace TimeSink.Engine.Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TimeSinkGame : EngineGame
    {
        const float viewWidth = 2f;

        // Default XNA members.
        GraphicsDeviceManager graphics;

        // Controller related.
        const float moveScale = 100.0f;

        UserControlledCharacter character;

        Enemy dummy;
        NormalCentipede normalCentipede;
        FlyingCentipede flyingCentipede;
        WorldGeometry world;
        MovingPlatform movingPlatform;
        Trigger trigger;


        SoundObject backgroundTrack;
        SoundEffect backHolder;


        public UserControlledCharacter Character
        {
            get { return character; }
        }

        public TimeSinkGame()
            : base()
        {
            // Default XNA setup.
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            character = new UserControlledCharacter(Vector2.Zero);

            dummy = new Enemy(PhysicsConstants.PixelsToMeters(new Vector2(620, 350)));
            world = new WorldGeometry();

            movingPlatform = new MovingPlatform(PhysicsConstants.PixelsToMeters(new Vector2(600, 150)),
                                                PhysicsConstants.PixelsToMeters(new Vector2(50, 150)),
                                                4f, 64, 128);

            //normalCentipede = new NormalCentipede(new Vector2(200, 400), new Vector2(200, 400), new Vector2(300, 400));
            flyingCentipede = new FlyingCentipede(PhysicsConstants.PixelsToMeters(new Vector2(100, 300)));
            normalCentipede = new NormalCentipede(PhysicsConstants.PixelsToMeters(new Vector2(200, 400)),
                                                  PhysicsConstants.PixelsToMeters(new Vector2(60, 0)));


            // Required for lighting system.
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            Entities.Add(character);

            Entities.Add(dummy);
            Entities.Add(normalCentipede);
            Entities.Add(flyingCentipede);
            Entities.Add(movingPlatform);
            Entities.Add(world);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            PhysicsManager.RegisterPhysicsBody(character);
            PhysicsManager.RegisterPhysicsBody(world);

            PhysicsManager.RegisterPhysicsBody(movingPlatform);
            PhysicsManager.RegisterPhysicsBody(dummy);
            PhysicsManager.RegisterPhysicsBody(normalCentipede);
            PhysicsManager.RegisterPhysicsBody(flyingCentipede);

            RenderManager.RegisterRenderable(character);
            RenderManager.RegisterRenderable(dummy);
            RenderManager.RegisterRenderable(normalCentipede);
            RenderManager.RegisterRenderable(flyingCentipede);
            RenderManager.RegisterRenderable(world);
            RenderManager.RegisterRenderable(movingPlatform);

            FixtureFactory.AttachRectangle(
                PhysicsConstants.PixelsToMeters(100),
                PhysicsConstants.PixelsToMeters(50),
                1,
                PhysicsConstants.PixelsToMeters(new Vector2(300, 400)),
                world.PhysicsBody,
                world);

            FixtureFactory.AttachPolygon(
                new FarseerPhysics.Common.Vertices() {
                    PhysicsConstants.PixelsToMeters(new Vector2(500, 300)),
                    PhysicsConstants.PixelsToMeters(new Vector2(600, 280)),
                    PhysicsConstants.PixelsToMeters(new Vector2(620, 340)),
                    PhysicsConstants.PixelsToMeters(new Vector2(520, 360))
                },
                1,
                world.PhysicsBody,
                world);

            FixtureFactory.AttachRectangle(
                PhysicsConstants.PixelsToMeters(GraphicsDevice.Viewport.Width),
                PhysicsConstants.PixelsToMeters(10),
                1,
                PhysicsConstants.PixelsToMeters(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height)),
                world.PhysicsBody,
                world);

            CollisionManager.RegisterCollideable(world);
            CollisionManager.RegisterCollideable(character);
            CollisionManager.RegisterCollideable(dummy);
            CollisionManager.RegisterCollideable(normalCentipede);

            //CollisionManager.RegisterCollideable(trigger);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            backHolder = Content.Load<SoundEffect>("Audio/Music/Four");
            backgroundTrack = new SoundObject(backHolder);
            backgroundTrack.Dynamic.IsLooped = true;
            // backgroundTrack.PlaySound();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            //Camera.Position = new Vector3(
            //    -GraphicsDevice.Viewport.Width / 2 + PhysicsConstants.MetersToPixels(Character.Physics.Position.X),
            //    -GraphicsDevice.Viewport.Height / 2 + PhysicsConstants.MetersToPixels(Character.Physics.Position.Y),
            //    0);

            // Calculate the view.
            view = ProcessCameraInput(gameTime);


            HandleInput(gameTime);

            base.Update(gameTime);
        }


        /// <summary>
        /// Move player based on controller input.
        /// </summary>
        /// <param name="gametime"></param>
        private void HandleInput(GameTime gametime)
        {
            if (InputManager.Instance.Pressed(Keys.M))
            {
                backgroundTrack.TogglePauseSound();
            }

            if (InputManager.Instance.IsNewKey(Keys.C))
            {
                showCollisionGeometry = !showCollisionGeometry;
            }

            character.HandleKeyboardInput(gametime, this);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (showCollisionGeometry)
            {
                //CollisionManager.Draw(SpriteBatch, TextureCache, Camera.Transform);
            }
        }


        #region Controller code

        // Scene/camera supporting members.
        bool firstMouseSample = true;
        Vector3 viewPosition = new Vector3(86.5f, 11.2f, 57.0f);
        Vector3 viewRotation = new Vector3(-2.2f, 0.16f, 0.0f);
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.Identity;

        private bool showCollisionGeometry;

#if WINDOWS_PHONE
        /// <summary>
        /// Handles controller input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public Matrix ProcessCameraInput(GameTime gameTime)
        {
            if (IsActive)
            {
                TouchCollection touches = TouchPanel.GetState();
                Vector3 move = new Vector3();

                Viewport viewport = GraphicsDevice.Viewport;
                int hotspotheight = viewport.Height / 5;
                int hotspotbottom = viewport.Height - hotspotheight;

                for (int t = 0; t < touches.Count; t++)
                {
                    TouchLocation loc = touches[t];

                    if (loc.State != TouchLocationState.Moved)
                        continue;

                    if (loc.Position.Y > hotspotheight && loc.Position.Y < hotspotbottom)
                    {
                        TouchLocation lastloc;

                        if (loc.TryGetPreviousLocation(out lastloc))
                        {
                            viewRotation.X += (loc.Position.X - lastloc.Position.X) * -0.01f;
                            viewRotation.Y -= (loc.Position.Y - lastloc.Position.Y) * -0.01f;
                        }
                    }
                    else
                    {
                        if (loc.Position.Y > hotspotbottom)
                            move.Z -= 1.0f;
                        else
                            move.Z += 1.0f;
                    }
                }

                float timescale = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float rotatescale = 3.0f * timescale;
                float movescale = timescale * moveScale;

                Quaternion rot = Quaternion.CreateFromYawPitchRoll(viewRotation.X, viewRotation.Y, viewRotation.Z);

                viewPosition += Vector3.Transform(new Vector3(movescale, 0, movescale) * move, rot);
            }

            // Convert the camera rotation and movement into a view transform.
            Matrix rotation = Matrix.CreateFromYawPitchRoll(viewRotation.X, viewRotation.Y, viewRotation.Z);
            Vector3 target = viewPosition + Vector3.Transform(Vector3.Backward, rotation);
            return Matrix.CreateLookAt(viewPosition, target, Vector3.Up);
        }
#else
        /// <summary>
        /// Handles controller input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public Matrix ProcessCameraInput(GameTime gameTime)
        {
            if (IsActive)
            {
                GamePadState gamepad = GamePad.GetState(PlayerIndex.One);
                KeyboardState keyboard = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();

                float timescale = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float rotatescale = 3.0f * timescale;
                float movescale = timescale * moveScale;

                // Get the right trigger, which affects speed.
                if (gamepad.IsConnected)
                {
                    rotatescale *= (1.0f - gamepad.Triggers.Right * 0.5f);
                    movescale *= (1.0f - gamepad.Triggers.Right * 0.5f);
                }
                else if (mouse.RightButton == ButtonState.Pressed)
                    movescale *= 0.25f;

                // If the gamepad is connected use its input instead of the mouse and keyboard.
                if (gamepad.IsConnected)
                    viewRotation -= new Vector3(gamepad.ThumbSticks.Right.X * rotatescale, gamepad.ThumbSticks.Right.Y * rotatescale, 0.0f);
                else
                {
                    GraphicsDevice device = GraphicsDevice;
                    int halfx = device.Viewport.Width / 2;
                    int halfy = device.Viewport.Height / 2;

                    if (!firstMouseSample)
                    {
                        // Convert the amount the mouse was moved into camera rotation.
                        viewRotation.X += MathHelper.ToRadians((float)(halfx - mouse.X) * rotatescale * 1.5f);
                        viewRotation.Y -= MathHelper.ToRadians((float)(halfy - mouse.Y) * rotatescale * 1.5f);
                    }
                    else
                        firstMouseSample = false;

                    //Mouse.SetPosition(halfx, halfy);
                }

                if (viewRotation.Y > MathHelper.PiOver2 - 0.01f)
                    viewRotation.Y = MathHelper.PiOver2 - 0.01f;
                else if (viewRotation.Y < -MathHelper.PiOver2 + 0.01f)
                    viewRotation.Y = -MathHelper.PiOver2 + 0.01f;

                Quaternion rot = Quaternion.CreateFromYawPitchRoll(viewRotation.X, viewRotation.Y, viewRotation.Z);

                // Now apply the camera movement based on either the gamepad or keyboard input.
                if (gamepad.IsConnected)
                {
                    viewPosition += Vector3.Transform(new Vector3(movescale, 0, movescale) * new Vector3(
                        -gamepad.ThumbSticks.Left.X, 0,
                        gamepad.ThumbSticks.Left.Y), rot);
                }
                else
                {
                    Vector3 move = new Vector3();
                    if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
                        move.Z += 1.0f;
                    if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
                        move.Z -= 1.0f;
                    if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
                        move.X += 1.0f;
                    if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
                        move.X -= 1.0f;
                    viewPosition += Vector3.Transform(new Vector3(movescale, 0, movescale) * move, rot);
                }
            }

            // mouse visibility...
            if (!IsActive || GamePad.GetState(PlayerIndex.One).IsConnected)
                IsMouseVisible = true;
            else
                IsMouseVisible = false;

            // Convert the camera rotation and movement into a view transform.
            return GetViewMatrix();
        }
#endif

        /// <summary>
        /// Convert the camera rotation and movement into a view transform.
        /// </summary>
        /// <returns></returns>
        private Matrix GetViewMatrix()
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(viewRotation.X, viewRotation.Y, viewRotation.Z);
            Vector3 target = viewPosition + Vector3.Transform(Vector3.Backward, rotation);
            return Matrix.CreateLookAt(viewPosition, target, Vector3.Up);
        }

        #endregion

        #region Main entry point
#if !WINDOWS_PHONE
        static class Program
        {
            /// <summary>
            /// The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main(string[] args)
            {
#if WINDOWS
                // Improved ui.
                System.Windows.Forms.Application.EnableVisualStyles();
#endif

                using (TimeSinkGame game = new TimeSinkGame())
                    game.Run();
            }
        }
#endif
        #endregion

        //  public SpriteContainer staticSceneSprites { get; set; }
    }
}
