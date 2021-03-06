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
using log4net;

namespace TimeSink.Engine.Core
{
    public class EngineGame : Game
    {
        #region fields
        public GraphicsDeviceManager graphics { get; set; }

        public Camera Camera { get; set; }
        public bool CameraLock { get; set; }

        public LevelManager LevelManager { get; set; }
        public IComponentContext Container { get; set; }

        public InMemoryResourceCache<Texture2D> TextureCache { get; private set; }
        public InMemoryResourceCache<SoundEffect> SoundCache { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public SQLiteDatabase Database { get; private set; }
        //public SQLiteDatabase database;

        public static readonly ILog Logger = LogManager.GetLogger(typeof(EngineGame));

        public ScreenManager ScreenManager { get; private set; }
        public ScreenFactory ScreenFactory { get; private set; }


        private bool musicEnabled = true;

        public bool MusicEnabled { get { return musicEnabled; } set { musicEnabled = value; } }
        public bool SoundsEnabled { get; set; }
        public bool GamepadEnabled { get; set; }

        public bool RenderDebugGeometry { get; set; }

        public static EngineGame Instance;

        private DebugViewXNA debugView;
        #endregion

        // todo: used for hacking door in the editor
        public EngineGame()
        {
        }

        public EngineGame(int width, int height)
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;

            Content.RootDirectory = "Content";

            ScreenManager = new ScreenManager(this);

            Instance = this;

            GamepadEnabled = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Camera = new Camera(Vector2.One, Vector3.Zero);

            // create default level
            LevelManager = Container.Resolve<LevelManager>();
            LevelManager.LevelLoaded += new LevelLoadedEventHandler(LevelLoaded);

            debugView = new DebugViewXNA(LevelManager.PhysicsManager.World);
            debugView.LoadContent(GraphicsDevice, Content);

            Database = Container.Resolve<SQLiteDatabase>();

            log4net.Config.XmlConfigurator.Configure();

            Database = Container.Resolve<SQLiteDatabase>();

            Logger.Info("EngineGame initialized");
            
            ScreenManager.Initialize();
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

            builder.RegisterType<PhysicsManager>().AsSelf().SingleInstance();
            builder.RegisterType<RenderManager>().AsSelf().SingleInstance();
            builder.RegisterType<LevelManager>().AsSelf().SingleInstance();
            builder.RegisterType<ScreenManager>().AsSelf().SingleInstance();

            builder.RegisterType<SQLiteDatabase>().AsSelf().SingleInstance();

            builder.RegisterInstance(this).As<EngineGame>();

            Container = builder.Build();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InputManager.Instance.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();

            if (RenderDebugGeometry)
            {
                Vector3 scale;
                Quaternion rot;
                Vector3 tran;
                Camera.Transform.Decompose(out scale, out rot, out tran);
                var projection =
                    Matrix.CreateScale(scale) *
                    Matrix.CreateTranslation(tran / 64f) *
                    Matrix.CreateOrthographicOffCenter(
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

        public virtual void LoadLevel(string levelName)
        {
        }

        protected virtual void LevelLoaded() 
        {
            debugView = new DebugViewXNA(LevelManager.PhysicsManager.World);
            debugView.LoadContent(GraphicsDevice, Content);
        }

        public virtual void MarkAsLoadLevel(string levelPath, int spawnPoint, object loadData) { }

        public virtual void UpdateHealth() { }
    }
}