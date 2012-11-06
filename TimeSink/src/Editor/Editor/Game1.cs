using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Editor.Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : XNAControl.XNAControlGame
    {
        SpriteBatch spriteBatch;

        RenderManager renderManager;

        Level level;

        Camera camera;
        const int cameraTolerance = 10;
        const int cameraMoveSpeed = 5;

        StateMachine<Level> stateMachine;
        State<Level> initState;

        public Game1(IntPtr handle, int width, int height)
            : base(handle, "Content", width, height)
        {
        }

        public InMemoryResourceCache<Texture2D> TextureCache { get; private set; }

        public InMemoryResourceCache<SoundEffect> SoundCache { get; private set; }

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

            camera = new Camera();

            //set up managers
            renderManager = new RenderManager(TextureCache);

            // create default level
            level = new Level(new CollisionManager(), new PhysicsManager(), renderManager);
            level.RegisterStaticMeshes(new List<StaticMesh>()
                {
                    new StaticMesh(new Vector2(20, 20)),
                    new StaticMesh(new Vector2(294, 20)),
                    new StaticMesh(new Vector2(566, 20))
                });

            // set up state machine
            initState = new DefaultEditorState();
            stateMachine = new StateMachine<Level>(initState, level);
            initState.StateMachine = stateMachine;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // setup caches            
            TextureCache = new InMemoryResourceCache<Texture2D>(
                new ContentManagerProvider<Texture2D>(Content));
            SoundCache = new InMemoryResourceCache<SoundEffect>(
                new ContentManagerProvider<SoundEffect>(Content));


            TextureCache.LoadResource("Textures/Ground_Tile1");
            SoundCache.LoadResource("Audio/Sounds/Hop");
            SoundCache.LoadResource("Audio/Music/Four");


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            InputManager.Instance.Update();

            Point mouse_loc = new Point(InputManager.Instance.CurrentMouseState.X, InputManager.Instance.CurrentMouseState.Y);
            if (mouse_loc.X < cameraTolerance && mouse_loc.X > 0)
                camera.PanCamera(-Vector2.UnitX * cameraMoveSpeed);
            if (mouse_loc.X > Constants.SCREEN_X - cameraTolerance && mouse_loc.X < Constants.SCREEN_X)
                camera.PanCamera(Vector2.UnitX * cameraMoveSpeed);
            if (mouse_loc.Y < cameraTolerance && mouse_loc.Y > 0)
                camera.PanCamera(-Vector2.UnitY * cameraMoveSpeed);
            if (mouse_loc.Y > Constants.SCREEN_Y - cameraTolerance && mouse_loc.Y < Constants.SCREEN_Y)
                camera.PanCamera(Vector2.UnitY * cameraMoveSpeed);

            stateMachine.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            stateMachine.Draw(spriteBatch, camera);

            base.Draw(gameTime);
        }

        public void StaticMeshSelected(string textureKey)
        {
            stateMachine.ChangeState(
                new StaticMeshPlacementEditorState(textureKey),
                true, true);
        }
    }
}
