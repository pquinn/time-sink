using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.DB;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.DebugViews;
using Autofac;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.StateManagement;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;

namespace TimeSink.Engine.Core
{
    public class EngineGame : Game
    {
        // Components
        public GraphicsDeviceManager graphics { get; set; }

        public Camera Camera { get; set; }

        public LevelManager LevelManager { get; set; }
        public IComponentContext Container { get; set; }

        public InMemoryResourceCache<Texture2D> TextureCache { get; private set; }
        public InMemoryResourceCache<SoundEffect> SoundCache { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public SQLiteDatabase database;

        public ScreenManager ScreenManager { get; private set; }
        public ScreenFactory ScreenFactory { get; private set; }

        public bool RenderDebugGeometry { get; set; }

        public static EngineGame Instance;

        private DebugViewXNA debugView;

        public EngineGame(int width, int height)
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;

            Content.RootDirectory = "Content";

            ScreenManager = new ScreenManager(this);
            this.database = new SQLiteDatabase();

            Instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // todo: this is horrible
            Constants.SCREEN_X = GraphicsDevice.Viewport.Width;
            Constants.SCREEN_Y = GraphicsDevice.Viewport.Height;

            Camera = Camera.ZeroedCamera;

            // create default level
            LevelManager = new LevelManager(
                new CollisionManager(),
                new PhysicsManager(Container),
                new RenderManager(TextureCache),
                new EditorRenderManager(TextureCache),
                Container);

            // create default level
            LevelManager = Container.Resolve<LevelManager>();

            debugView = new DebugViewXNA(LevelManager.PhysicsManager.World);
            debugView.LoadContent(GraphicsDevice, Content);
            
            ScreenManager.Initialize();

            LevelManager.CollisionManager.Initialize();
        }

        protected override void LoadContent()
        {
            // instantiate the container
            var builder = new ContainerBuilder();

            // setup caches            
            TextureCache = new InMemoryResourceCache<Texture2D>(
                new ContentManagerProvider<Texture2D>(Content));
            SoundCache = new InMemoryResourceCache<SoundEffect>(
                new ContentManagerProvider<SoundEffect>(Content));
            builder.RegisterInstance(TextureCache).As<IResourceCache<Texture2D>>();
            builder.RegisterInstance(SoundCache).As<IResourceCache<SoundEffect>>();

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            TextureCache.LoadResource("Textures/circle");
            var blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            TextureCache.AddResource("blank", blank);

            builder.RegisterInstance(new World(PhysicsConstants.Gravity)).AsSelf();

            builder.RegisterType<CollisionManager>().AsSelf().SingleInstance();
            builder.RegisterType<PhysicsManager>().AsSelf().SingleInstance();
            builder.RegisterType<RenderManager>().AsSelf().SingleInstance();
            builder.RegisterType<EditorRenderManager>().AsSelf().SingleInstance();
            builder.RegisterType<LevelManager>().AsSelf().SingleInstance();

            Container = builder.Build();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InputManager.Instance.Update();

         /* Gameplayscreen handles updating the Level 
          * 
          * LevelManager.PhysicsManager.Update(gameTime);

            LevelManager.Level.Entities.ForEach(x => x.Update(gameTime, this));*/
        }

        protected override void Draw(GameTime gameTime)
        {
           // LevelManager.RenderManager.Draw(SpriteBatch, Camera);

            SpriteBatch.Begin();

            if (RenderDebugGeometry)
            {
                var projection = Matrix.CreateOrthographicOffCenter(
                    0,
                    PhysicsConstants.PixelsToMeters(GraphicsDevice.Viewport.Width),
                    PhysicsConstants.PixelsToMeters(GraphicsDevice.Viewport.Height),
                    0,
                    0,
                    1);
                debugView.RenderDebugData(ref projection);
            }

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}