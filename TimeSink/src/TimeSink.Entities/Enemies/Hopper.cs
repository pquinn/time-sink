using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("849aaec2-7155-4a37-aa71-42d0c1611881")]
    public class Hopper : Enemy
    {
        private const string EDITOR_NAME = "Hopper";
        private const int MAX_JUMP_GUARD = 300;
        private const int JUMP_FORCE_Y = 4000;
        private const int JUMP_FORCE_X = 700;

        private static readonly Guid GUID = new Guid("849aaec2-7155-4a37-aa71-42d0c1611881");
        private Guid charGuid = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");

        private int jumpGuard;

        public Hopper()
            : this(Vector2.Zero)
        {
        }

        public Hopper(Vector2 position)
            : base(position)
        {
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override void OnUpdate(GameTime time, EngineGame game)
        {
            base.OnUpdate(time, game);

            RayCastCallback cb = delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
            {
                if (fixture.Body.UserData is WorldGeometry2 || fixture.Body.UserData is MovingPlatform)
                {
                    TouchingGround = true;
                    return 0;
                }
                else
                {
                    return -1;
                }
            };

            var startMid = Physics.Position + new Vector2(0, PhysicsConstants.PixelsToMeters(Height) / 2);
            var distMid = new Vector2(0, .1f);
            game.LevelManager.PhysicsManager.World.RayCast(cb, startMid, startMid + distMid);

            if (TouchingGround)
            {
                jumpGuard += time.ElapsedGameTime.Milliseconds;
                if (jumpGuard >= MAX_JUMP_GUARD)
                {
                    TouchingGround = false;
                    jumpGuard = 0;  
                 
                    PerformJump(game.LevelManager.Level.Entities.First(x => x.Id == charGuid));
                }
            }
        }

        private void PerformJump(Entity character)
        {
            Physics.ApplyForce(
                new Vector2(
                    JUMP_FORCE_X * (character.Position.X < Position.X ? -1 : 1), 
                    -JUMP_FORCE_Y));
        }
    }
}
