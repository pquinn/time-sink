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
    public class NormalCentipede : Enemy
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Centipede/Neutral";
        const string CENTIPEDE_WALK_LEFT = "Textures/Enemies/Centipede/CentipedeWalk_Left";
        const string EDITOR_NAME = "Normal Centipede";

        private static readonly Guid GUID = new Guid("849aaec2-7155-4c37-aa71-42d0c1611881");

        new private static int textureHeight;
        new private static int textureWidth;

        private int xDirection;

        public Func<float, Vector2> PatrolFunction { get; private set; }

        public NormalCentipede()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public NormalCentipede(Vector2 position, Vector2 direction)
            : base(position)
        {
            health = 150;
            PatrolDirection = direction;
        }

        [EditableField("Patrol Direction")]
        [SerializableField]
        public Vector2 PatrolDirection { get; set; }

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
                  Physics.Rotation,
                  Vector2.One,
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

        bool OnCollidedWith(Fixture f, WorldGeometry2 wg, Fixture wF, Contact info)
        {
            Physics.IgnoreGravity = true;
            return true;
        }

        private bool collidedAhead = false;
        private bool collidedDownward = false;
        private bool needToTurn = false;
        private Vector2 rayTopOffset = Vector2.Zero;
        private Vector2 rayBottomOffset = Vector2.Zero;
        private Vector2 rayTurnTopOffset = Vector2.Zero;
        private Vector2 rayTurnBottomOffset = Vector2.Zero;
        private Vector2 xDirectionCast = new Vector2(.1f, 0);
        private Vector2 yDirectionCast = new Vector2(0, .25f);
        private bool initialized = false;
        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            Physics.Position += PatrolDirection * (float)time.ElapsedGameTime.TotalSeconds;

            var totalRotationMatrix = Matrix.CreateRotationZ(Physics.Rotation);
            var quarterTurnCWMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2);
            var quarterTurnCCWMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2);

            if (!initialized)
            {
                xDirection = (int)PatrolDirection.X >= 0 ? 1 : -1;

                rayTopOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    -PhysicsConstants.PixelsToMeters(Height) / 2);
                rayBottomOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    PhysicsConstants.PixelsToMeters(Height) / 2);

                initialized = true;
            }

            if (needToTurn)
            {
                rayTopOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 4,
                    -PhysicsConstants.PixelsToMeters(Height) / 2);

                rayBottomOffset = new Vector2(
                    -1 * xDirection * PhysicsConstants.PixelsToMeters(Width) / 4,
                    PhysicsConstants.PixelsToMeters(Height) / 2);
            }
            else
            {
                rayTopOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    -PhysicsConstants.PixelsToMeters(Height) / 2);

                rayBottomOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    PhysicsConstants.PixelsToMeters(Height) / 2);
            }

            //cast a ray forward from top front corner
            world.LevelManager.PhysicsManager.World.RayCast(
                delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    // do what it do here
                    collidedAhead = true;
                    if (fixture.UserData != null && fixture.UserData is UserControlledCharacter)
                        return -1;
                    else
                        return 0;
                },
                Physics.Position + Vector2.Transform(rayTopOffset, totalRotationMatrix),
                Physics.Position + Vector2.Transform(rayTopOffset + xDirectionCast, totalRotationMatrix));


            //cast a ray downward from bottom front 
            world.LevelManager.PhysicsManager.World.RayCast(
                delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    // do what it do here
                    collidedDownward = true;
                    if (fixture.UserData != null && fixture.UserData is UserControlledCharacter)
                        return -1;
                    else
                        return 0;
                },
                Physics.Position + Vector2.Transform(rayBottomOffset, totalRotationMatrix),
                Physics.Position + Vector2.Transform(rayBottomOffset + yDirectionCast, totalRotationMatrix));

            if (collidedAhead)
            {
                if (needToTurn)
                {
                    var rotation = MathHelper.PiOver2 * -xDirection;
                    Physics.Rotation += rotation;
                    PatrolDirection = Vector2.Transform(PatrolDirection, Matrix.CreateRotationZ(rotation));
                    needToTurn = false;
                }
                else
                {
                    needToTurn = true;
                }

                collidedAhead = false;
            }

            if (!collidedDownward && Physics.IgnoreGravity == true)
            {
                if (needToTurn)
                {
                    Physics.Rotation -= MathHelper.PiOver2 * -xDirection;
                    PatrolDirection = Vector2.Transform(PatrolDirection, quarterTurnCCWMatrix);
                    needToTurn = false;
                }
                else
                {
                    needToTurn = true;
                }
            }
            collidedDownward = false;
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var texture = textureCache.LoadResource(CENTIPEDE_TEXTURE);
        }

        protected override Texture2D GetTexture(IResourceCache<Texture2D> textureCache)
        {
            return textureCache.GetResource(CENTIPEDE_TEXTURE);
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);
            //Physics.IgnoreGravity = true;
            Physics.RegisterOnCollidedListener<WorldGeometry2>(OnCollidedWith);
        }
    }
}
