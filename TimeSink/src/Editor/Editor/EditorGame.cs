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
using System.Collections;
using FarseerPhysics.Dynamics;
using XNAControl;
using TimeSink.Engine.Core.Editor;

namespace Editor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class EditorGame : XNAControlGame
    {
        SpriteBatch spriteBatch;
        
        Camera camera;

        StateMachine<LevelManager> stateMachine;
        State<LevelManager> initState;
        private bool showCollisionGeometry;

        public EditorGame(IntPtr handle, int width, int height)
            : base(handle, "Content", width, height)
        {
        }

        public LevelManager LevelManager { get; private set; }

        public IEnumerable<string> Tiles { get; private set; }

        public InMemoryResourceCache<Texture2D> TextureCache { get; private set; }

        public InMemoryResourceCache<SoundEffect> SoundCache { get; private set; }

        public IComponentContext Container { get; set; }

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

            // todo: this is horrible
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

            // create default level
            LevelManager = Container.Resolve<LevelManager>();

            // set up state machine
            initState = new DefaultEditorState(camera, TextureCache);
            stateMachine = new StateMachine<LevelManager>(initState, LevelManager);
            initState.StateMachine = stateMachine;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // instantiate the container
            var builder = new ContainerBuilder();
            builder.RegisterModule<EntityBootstrapper>();

            Tiles = new TileBootstrapper().GetTiles(Content);

            // setup caches            
            TextureCache = new InMemoryResourceCache<Texture2D>(
                new ContentManagerProvider<Texture2D>(Content));
            SoundCache = new InMemoryResourceCache<SoundEffect>(
                new ContentManagerProvider<SoundEffect>(Content));
            builder.RegisterInstance(TextureCache).As<IResourceCache<Texture2D>>();
            builder.RegisterInstance(SoundCache).As<IResourceCache<SoundEffect>>();

            TextureCache.LoadResources(Tiles);
            var blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            TextureCache.AddResource("blank", blank);
            SoundCache.LoadResource("Audio/Sounds/Hop");
            SoundCache.LoadResource("Audio/Music/Four");

            builder.RegisterInstance(new World(PhysicsConstants.Gravity)).AsSelf();

            builder.RegisterType<CollisionManager>().AsSelf().SingleInstance();
            builder.RegisterType<PhysicsManager>().AsSelf().SingleInstance();
            builder.RegisterType<RenderManager>().AsSelf().SingleInstance();
            builder.RegisterType<EditorRenderManager>().AsSelf().SingleInstance();
            builder.RegisterType<LevelManager>().AsSelf().SingleInstance();
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDeviceManager.GraphicsDevice);

            Container = builder.Build();

            foreach (var entity in Container.Resolve<IEnumerable<Entity>>())
            {
                entity.Load(Container);
            }
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

        public void SaveAs(string fileName)
        {
            LevelManager.SerializeLevel(fileName);
        }

        public void Open(string fileName)
        {
            LevelManager.Clear();
            LevelManager.DeserializeLevel(fileName);
        }

        public void New()
        {
            LevelManager.Clear();
        }
    }
}
