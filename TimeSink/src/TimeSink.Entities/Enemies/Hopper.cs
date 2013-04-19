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
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Caching;
using FarseerPhysics.Factories;
using Autofac;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Entities.Inventory;
using TimeSink.Engine.Core.Collisions;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("849aaec2-7155-4a37-aa71-42d0c1611881")]
    public class Hopper : Enemy
    {
        private const string EDITOR_NAME = "Hopper";
        private const string NORM_TEXTURE = "Textures/Enemies/Hopper_normal";
        private const string AGGRO_TEXTURE = "Textures/Enemies/Hopper_aggro";
        private const string AWESOME_TEXTURE = "Textures/Enemies/Hopper_aggro_awesome";
        private string currentTexture = NORM_TEXTURE;
        private const int MAX_JUMP_GUARD = 300;
        private const int JUMP_FORCE_Y = 3500;
        private const int JUMP_FORCE_X = 700;
        private const int CHAR_DISTANCE_THRESH = 4000;

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
                currentTexture = NORM_TEXTURE;
                jumpGuard += time.ElapsedGameTime.Milliseconds;
                var character = game.LevelManager.Level.Entities.First(x => x.Id == charGuid);
                var distToPlayer = Math.Abs(Position.X - character.Position.X);
                if (jumpGuard >= MAX_JUMP_GUARD && distToPlayer < CHAR_DISTANCE_THRESH)
                {
                    TouchingGround = false;
                    jumpGuard = 0;

                    currentTexture = AWESOME_TEXTURE;

                    Physics.ApplyForce(new Vector2(
                        JUMP_FORCE_X * (character.Position.X < Position.X ? -1 : 1),
                        -JUMP_FORCE_Y));
                }
            }
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = textureCache.LoadResource(NORM_TEXTURE);
                Width = texture.Width;
                Height = texture.Height;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1.2f,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;

                Physics.RegisterOnCollidedListener<Arrow>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Dart>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<EnergyBullet>(OnCollidedWith);

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                var hitsensor = fix.Clone(Physics);
                hitsensor.IsSensor = true;
                hitsensor.CollisionCategories = Category.Cat2;
                hitsensor.CollidesWith = Category.Cat2;

                initialized = true;
            }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>
                {
                    new BasicRendering(currentTexture)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position), 
                        Scale = new Vector2(.75f, .75f)
                    }
                };
            }
        }
    }
}
