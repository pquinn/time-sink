using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("023bb987-a342-4654-a8c1-b7e4f1c0f373")]
    public class CyberCentipede : NormalCentipede
    {
        const string CENTIPEDE_WALK_LEFT = "Textures/Enemies/Centipede/WalkingCyber";
        const string EDITOR_NAME = "Cyber Centipede";
        const float DEPTH = -50f;
        public static readonly Guid GUID = new Guid("023bb987-a342-4654-a8c1-b7e4f1c0f373");

        NewAnimationRendering anim =
            new NewAnimationRendering(CENTIPEDE_WALK_LEFT, new Vector2(256f, 128f), 2, Vector2.Zero, 0, new Vector2(.5f, .5f), Color.White)
            {
                DepthWithinLayer = DEPTH
            };
        float animTimer = 0f;
        float animInterval = 100f;
        private HashSet<Tuple<Fixture, Vector2>> collided = new HashSet<Tuple<Fixture, Vector2>>();
        private List<Body> anchors = new List<Body>();
        private float angle;

        public CyberCentipede()
            : base(Vector2.Zero, Vector2.Zero) { }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get
            {
                return EDITOR_NAME;
            }
        }

        int generalDirection
        {
            get
            {
                return WheelSpeed > 0 ? 1 : -1;
            }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {

            animTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            base.OnUpdate(time, world);

            if (!collided.Any())
                anchors.ForEach(x => x.IgnoreGravity = false);

            foreach (var x in collided)
                anchors.ForEach(b => b.ApplyForce(-x.Item2 * 10));


            if (PreviousPosition != null)
            {
                Vector2 move = Position - (PreviousPosition ?? Position);
                move.Normalize();

                angle = (float)Math.Atan2(move.Y, move.X);

                if (generalDirection == -1)
                    angle += (float)Math.PI;
            }
            if (animTimer >= animInterval)
            {
                anim.CurrentFrame = (anim.CurrentFrame + 1) % anim.NumFrames;
                animTimer = 0f;
            }
        }


        public override List<IRendering> Renderings
        {
            get
            {
                anim.Position = PhysicsConstants.MetersToPixels(Physics.Position);
                anim.Rotation = angle;
                return new List<IRendering>() { anim };
            }
        }
    }
}
