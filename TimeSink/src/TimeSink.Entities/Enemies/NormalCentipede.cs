using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Editor;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("849aaec2-7155-4c37-aa71-42d0c1611881")]
    public class NormalCentipede : Enemy, IHaveHealth
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Centipede/Neutral";
        const string CENTIPEDE_WALK_LEFT = "Textures/Enemies/Centipede/CentipedeWalk_Left";
        const string EDITOR_NAME = "Normal Centipede";

        private static readonly Guid GUID = new Guid("849aaec2-7155-4c37-aa71-42d0c1611881");

        new private static int textureHeight;
        new private static int textureWidth;

        private bool first;
        private float tZero;

        public Func<float, Vector2> PatrolFunction { get; private set; }

        [EditableField("Patrol Direction")]
        public Vector2 PatrolDirection { get; set; }

        public NormalCentipede()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public NormalCentipede(Vector2 position, Vector2 direction) : base(position)
        {
            health = 150;
            PatrolDirection = direction;
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

        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);

            Physics.Position += PatrolDirection * (float)time.ElapsedGameTime.TotalSeconds;

            var start = Physics.Position + new Vector2(
                PatrolDirection.X >= 0 ? PhysicsConstants.PixelsToMeters(textureWidth) / 2 : -PhysicsConstants.PixelsToMeters(textureWidth) / 2, 
                PhysicsConstants.PixelsToMeters(textureHeight) / 2);

            var collided = false;

            world.LevelManager.PhysicsManager.World.RayCast(
                delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    collided = true;
                    return 0;
                },
                start,
                start + new Vector2(0, .1f));

            if (!collided)
            {
                PatrolDirection *= -Vector2.UnitX;
            }
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
    }
}
