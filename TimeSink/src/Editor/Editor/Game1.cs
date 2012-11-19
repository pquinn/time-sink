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
using Editor.States;
using Autofac;
using System.Xml.Serialization;
using System.Xml;

namespace Editor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : XNAControl.XNAControlGame
    {
        SpriteBatch spriteBatch;

        RenderManager renderManager;

        LevelManager leveManager;

        Camera camera;

        StateMachine<LevelManager> stateMachine;
        State<LevelManager> initState;
        private bool showCollisionGeometry;

        public Game1(IntPtr handle, int width, int height)
            : base(handle, "Content", width, height)
        {
        }

        public InMemoryResourceCache<Texture2D> TextureCache { get; private set; }

        public InMemoryResourceCache<SoundEffect> SoundCache { get; private set; }

        public IContainer Container { get; set; }

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

            Constants.SCREEN_X = GraphicsDevice.Viewport.Width;
            Constants.SCREEN_Y = GraphicsDevice.Viewport.Height;

            EditorProperties.Instance = new EditorProperties()
            {
                ShowGridLines = false,
                GridLineSpacing = 16,
                EnableSnapping = false,
                ResolutionX = ResolutionWidth,
                ResolutionY = ResolutionHeight
            };

            camera = Camera.ZeroedCamera;

            //set up managers
            renderManager = new RenderManager(TextureCache);

            // create default level
            leveManager = new LevelManager(new CollisionManager(), new PhysicsManager(), renderManager, new Level());
            leveManager.RegisterTiles(new List<Tile>()
                {
                    new Tile("Textures/Ground_Tile1", new Vector2(187, 361.5f), 0, Vector2.One, TextureCache),
                    new Tile("Textures/Ground_Tile1", new Vector2(461, 361.5f), 0, Vector2.One, TextureCache),
                    new Tile("Textures/Ground_Tile1", new Vector2(735, 361.5f), 0, Vector2.One, TextureCache),
                    new Tile("Textures/Side_Tile01", new Vector2(1009, 361.5f), 0, Vector2.One, TextureCache),
                    new Tile("Textures/Top_Tile01", new Vector2(187, 293.5f), 0, Vector2.One, TextureCache),
                    new Tile("Textures/Top_Tile01", new Vector2(461, 293.5f), 0, Vector2.One, TextureCache),
                    new Tile("Textures/Top_Tile01", new Vector2(735, 293.5f), 0, Vector2.One, TextureCache),
                });

            // set up state machine
            initState = new DefaultEditorState(camera, TextureCache);
            stateMachine = new StateMachine<LevelManager>(initState, leveManager);
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

            TextureCache.LoadResources(
                new List<string> 
                {
                    "Textures/Ground_Tile1",
                    "Textures/Top_Tile01",
                    "Textures/Top_Tile02",
                    "Textures/Top_Tile03",
                    "Textures/Side_Tile01"
                });
            var blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            TextureCache.AddResource("blank", blank);
            SoundCache.LoadResource("Audio/Sounds/Hop");
            SoundCache.LoadResource("Audio/Music/Four");


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);

            // instantiate the container
            var builder = new ContainerBuilder();
            builder.RegisterModule<EntityBootstrapper>();
            Container = builder.Build();
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

            if (InputManager.Instance.IsNewKey(Keys.C))
            {
                showCollisionGeometry = !showCollisionGeometry;
            }

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

            spriteBatch.Begin();

            if (EditorProperties.Instance.ShowGridLines)
            {
                for (var x = 2; x < ResolutionWidth; x += EditorProperties.Instance.GridLineSpacing)
                {
                    spriteBatch.DrawLine(
                        TextureCache.GetResource("blank"),
                        new Vector2(x, 0), new Vector2(x, ResolutionHeight),
                        1, new Color(0, 0, 0, 50));
                }
                for (var y = 2; y < ResolutionHeight; y += EditorProperties.Instance.GridLineSpacing)
                {
                    spriteBatch.DrawLine(
                        TextureCache.GetResource("blank"),
                        new Vector2(0, y), new Vector2(ResolutionWidth, y),
                        1, new Color(0, 0, 0, 50));
                }
            }

            spriteBatch.End();

            stateMachine.Draw(spriteBatch);

            if (showCollisionGeometry)
            {
                //level.CollisionManager.Draw(spriteBatch, TextureCache, Matrix.Identity);
            }

            base.Draw(gameTime);
        }

        public void PanSelected()
        {
            stateMachine.ChangeState(
                new CameraTranslateState(camera, TextureCache),
                true, true);
        }

        public void ZoomSelected()
        {
            stateMachine.ChangeState(
                new CameraZoomState(camera, TextureCache),
                true, true);
        }

        public void StaticMeshSelected(string textureKey)
        {
            stateMachine.ChangeState(
                new StaticMeshPlacementEditorState(camera, TextureCache, textureKey),
                true, true);
        }

        public void EntitySelected(Entity entity)
        {
            stateMachine.ChangeState(
                new EntityPlacementState(camera, TextureCache, entity),
                true, true);
        }

        public void SelectionSelected()
        {
            stateMachine.ChangeState(
                new SelectionEditorState(camera, TextureCache),
                true, true);
        }

        public void RotationSelected()
        {
            stateMachine.ChangeState(
                new RotationEditorState(camera, TextureCache),
                true, true);
        }

        public void ScalingSelected()
        {
            stateMachine.ChangeState(
                new ScalingEditorState(camera, TextureCache),
                true, true);
        }

        public void GeometrySelected()
        {
            stateMachine.ChangeState(
                new GeometryPlacementState(camera, TextureCache),
                true, true);
        }

        public void SaveAs()
        {
            leveManager.SerializeLevel();
        }
    }
}
