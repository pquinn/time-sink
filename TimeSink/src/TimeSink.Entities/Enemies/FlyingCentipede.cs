using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Editor;
using FarseerPhysics.Dynamics;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.States;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("bb7f91f9-af92-41cc-a985-bd1e85066403")]
    public class FlyingCentipede : Enemy, IHaveHealth
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Flying Centipede/Flying01"; //temporary
        const string EDITOR_NAME = "Flying Centipede";

        private static readonly Guid GUID = new Guid("bb7f91f9-af92-41cc-a985-bd1e85066403");

        private static int textureHeight;
        private static int textureWidth;

        public FlyingCentipede()
            : this(Vector2.Zero)
        {
        }

        public FlyingCentipede(Vector2 position)
            : base(position)
        {
            health = 150;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * health);
                return new TintedRendering(
                  CENTIPEDE_TEXTURE,
                  PhysicsConstants.MetersToPixels(Physics.Position),
                  0,
                  new Vector2(.5f,.5f),
                  new Color(255f, tint, tint, 255f));
            }
        }

        public override IRendering Preview
        {
            get
            {
                return Rendering;
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            throw new NotImplementedException();
        }


        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var texture = textureCache.LoadResource(CENTIPEDE_TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height; 
        }

        protected override Texture2D GetTexture(IResourceCache<Texture2D> textureCache)
        {
            return textureCache.GetResource(CENTIPEDE_TEXTURE);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                base.InitializePhysics(force, engineRegistrations);
                Physics.BodyType = BodyType.Static;

                initialized = true;
            }
        }
    }
}
