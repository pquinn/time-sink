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

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    public class NormalCentipede : Enemy, IHaveHealth
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Goomba";
        const string EDITOR_NAME = "Normal Centipede";

        private bool first;
        private float tZero;

        public Func<float, Vector2> PatrolFunction { get; private set; }

        public NormalCentipede()
            : this(Vector2.Zero, Vector2.Zero, Vector2.Zero)
        {
        }

        public NormalCentipede(Vector2 position, Vector2 start, Vector2 end) : base(position)
        {
            health = 150;
            physics = new GravityPhysics(position, CENTIPEDE_MASS)
            {
                GravityEnabled = true
            };

            first = true;
            PatrolFunction = (time) => start + time * (end - start);
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override string EditorPreview
        {
            get
            {
                return CENTIPEDE_TEXTURE;
            }
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
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));
            }
        }

        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);

            if (first)
            {
                tZero = time.TotalGameTime.Seconds;
                first = false;
            }

            var positionDelta = PatrolFunction.Invoke((float)time.TotalGameTime.TotalSeconds - tZero) - physics.Position;
            physics.Position += positionDelta;
        }

        public override void Load(EngineGame engineGame)
        {
            var texture = engineGame.TextureCache.LoadResource(CENTIPEDE_TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
        }
    }
}
