﻿using Microsoft.Xna.Framework;
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
using Microsoft.Xna.Framework.Input;

namespace TimeSink.Engine.Core
{
    public class EngineGame : Microsoft.Xna.Framework.Game
    {
        // Components
        public Camera Camera { get; set; }
        public PhysicsManager PhysicsManager { get; private set; }
        public CollisionManager CollisionManager { get; private set; }
        public RenderManager RenderManager { get; private set; }

        public InMemoryResourceCache<Texture2D> TextureCache { get; private set; }
        public InMemoryResourceCache<SoundEffect> SoundCache { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public HashSet<Entity> Entities { get; private set; }

        public EngineGame()
            : base()
        {
            Entities = new HashSet<Entity>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            Camera = Camera.ZeroedCamera;

            PhysicsManager = new PhysicsManager();
            CollisionManager = new CollisionManager();
            RenderManager = new RenderManager(TextureCache);

            CollisionManager.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // setup caches            
            TextureCache = new InMemoryResourceCache<Texture2D>(
                new ContentManagerProvider<Texture2D>(Content));
            SoundCache = new InMemoryResourceCache<SoundEffect>(
                new ContentManagerProvider<SoundEffect>(Content));

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            TextureCache.LoadResource("Textures/circle");
            var blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
            TextureCache.AddResource("blank", blank);

            foreach (var entity in Entities)
                entity.Load(this);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InputManager.Instance.Update();

            PhysicsManager.Update(gameTime);
            CollisionManager.Update(gameTime);

            foreach (var entity in Entities)
                entity.Update(gameTime, this);

            Entities.RemoveWhere(e => e.Dead);
        }

        protected override void Draw(GameTime gameTime)
        {
            RenderManager.Draw(SpriteBatch, Camera);

            base.Draw(gameTime);
        }
    }
}
