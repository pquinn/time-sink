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
using Microsoft.Xna.Framework.Input.Touch;

// Include the necessary SunBurn namespaces.
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Effects;
using TimeSink.Engine.Core.Collisions;
#endregion


namespace TimeSink.Engine.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class StarterGame : Microsoft.Xna.Framework.Game
    {
        const float viewWidth = 2f;

        // The SunBurn lighting system.
        SunBurnCoreSystem sunBurnCoreSystem;
        FrameBuffers frameBuffers;
        public SpriteManager SpriteManager;
        SplashScreenGameComponent splashScreenGameComponent;

        // Scene related members.
        SceneState sceneState;
        public SceneInterface SceneInterface;
        ContentRepository contentRepository;
        SceneEnvironment environment;

        // Default XNA members.
        GraphicsDeviceManager graphics;

        // Controller related.
        const float moveScale = 100.0f;

        // Components
        TimeSink.Engine.Core.Physics.PhysicsManager physicsManager = new TimeSink.Engine.Core.Physics.PhysicsManager();
        CollisionManager collisionManager = new CollisionManager();
        UserControlledCharacter character = new UserControlledCharacter(Vector2.Zero);
        WorldGeometry world;

        public UserControlledCharacter Character
        {
            get { return character; }
        }

        public StarterGame()
        {
            // Default XNA setup.
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Required for lighting system.
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            // Required for lighting system.
            splashScreenGameComponent = new SplashScreenGameComponent(this);
            Components.Add(splashScreenGameComponent);

            // Create the lighting system.
            sunBurnCoreSystem = new SunBurnCoreSystem(Services, Content);
            sceneState = new SceneState();

            // Create the scene interface. Acts as a service provider containing all scene managers
            // and returning them by type (including custom managers). Also acts as a component
            // container where calls to manager methods on the SceneInterface (such as BeginFrameRendering,
            // Unload, ...) are automatically called on all contained managers.
            //
            // This design allows managers to be plugged-in like modular components and for managers
            // to easily be added, removed, or replaced with custom implementations.
            //
            SceneInterface = new SceneInterface();
            SceneInterface.CreateDefaultManagers(RenderingSystemType.Forward, CollisionSystemType.Physics, true);

            SpriteManager = new SpriteManager(SceneInterface);
            SceneInterface.AddManager(SpriteManager);

            // Create the frame buffers used for rendering (sized to the backbuffer) and
            // assign them to the ResourceManager so we don't have to worry about cleanup.
            frameBuffers = new FrameBuffers(DetailPreference.High, DetailPreference.Medium);
            SceneInterface.ResourceManager.AssignOwnership(frameBuffers);

            physicsManager.RegisterPhysicsBody(Character);
            collisionManager.RegisterCollisionBody(Character);

            // Post console messages letting the user know how to open the SunBurn Editor.
            SceneInterface.ShowConsole = true;
            SystemConsole.AddMessage("Welcome to the SunBurn Engine.", 4);
            SystemConsole.AddMessage("Use an Xbox controller or the W, A, S, D keys to navigate the scene.", 8);
            SystemConsole.AddMessage("Press F11 to open the SunBurn Editor.", 12);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            world = new WorldGeometry(
                new Rectangle(
                    0,
                    0,
                    GraphicsDevice.Viewport.Width, 
                    1));

            collisionManager.RegisterCollisionBody(world);

            // Load the content repository, which stores all assets imported via the editor.
            // This must be loaded before any other assets.
            contentRepository = Content.Load<ContentRepository>("Content");
            
            // Add objects and lights to the ObjectManager and LightManager. They accept
            // objects and lights in several forms:
            //
            //   -As scenes containing both dynamic (movable) and static objects and lights.
            //
            //   -As SceneObjects and lights, which can be dynamic or static, and
            //    (in the case of objects) are created from XNA Models or custom vertex / index buffers.
            //
            //   -As XNA Models, which can only be static.
            //

            // Load the scene and add it to the managers.
            Scene scene = Content.Load<Scene>("Scenes/Scene");

            SceneInterface.Submit(scene);

            // Load the scene environment settings.
            environment = Content.Load<SceneEnvironment>("Environment/Environment");

            // TODO: use this.Content to load your game content here
            character.Load(this);

            world.Load(this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            // Cleanup any used resources.
            SceneInterface.Unload();
            sunBurnCoreSystem.Unload();

            environment = null;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Enables game input / control when not editing the scene (the editor provides its own control).
            if (!SceneInterface.Editor.EditorAttached)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                // Calculate the view.
                view = ProcessCameraInput(gameTime);

                HandleInput(gameTime);
            }

            // Calculate the projection.
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70.0f),
            GraphicsDevice.Viewport.AspectRatio, 0.1f, environment.VisibleDistance);

            // Update all contained managers.
            SceneInterface.Update(gameTime);

            // TODO: Add your update logic here
            physicsManager.Update(gameTime);
            collisionManager.Update(gameTime);

            base.Update(gameTime);
        }


        /// <summary>
        /// Move player based on controller input.
        /// </summary>
        /// <param name="gametime"></param>
        private void HandleInput(GameTime gametime)
        {
            character.HandleKeyboardInput(gametime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Check to see if the splash screen is finished.
            if (!SplashScreenGameComponent.DisplayComplete)
            {
                base.Draw(gameTime);
                return;
            }

            character.Draw(gameTime);
            world.Draw(gameTime);

            // Render the scene.
            sceneState.BeginFrameRendering(Vector2.Zero, viewWidth, GraphicsDevice.Viewport.AspectRatio, gameTime, environment, frameBuffers, true);
            SceneInterface.BeginFrameRendering(sceneState);

            // Add custom rendering that should occur before the scene is rendered.

            SceneInterface.RenderManager.Render();

            // Add custom rendering that should occur after the scene is rendered.

            SceneInterface.EndFrameRendering();
            sceneState.EndFrameRendering();

            base.Draw(gameTime);
        }


        #region Controller code

        // Scene/camera supporting members.
        bool firstMouseSample = true;
        Vector3 viewPosition = new Vector3(86.5f, 11.2f, 57.0f);
        Vector3 viewRotation = new Vector3(-2.2f, 0.16f, 0.0f);
        Matrix view = Matrix.Identity;
        Matrix projection = Matrix.Identity;
        private SpriteBatch spriteBatch;

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

                    Mouse.SetPosition(halfx, halfy);
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

                using (StarterGame game = new StarterGame())
                    game.Run();
            }
        }
#endif
        #endregion

        public SpriteContainer staticSceneSprites { get; set; }
    }
}
