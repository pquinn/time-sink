using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Triggers;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("849aaec2-7155-4a37-ab71-42d0c2611881")]
    public class Waver : Enemy
    {
        private const string EDITOR_NAME = "Zoomer";
        private const string TEXTURE = "Textures/Enemies/Hanger_launched_vertical";
        private const int ZOOM_SPEED = 6;
        private const int CYCLE_TIME = 3000;

        private static readonly Guid GUID = new Guid("849aaec2-7155-4a37-ab71-42d0c2611881");
        private static readonly float maxOffset = PhysicsConstants.PixelsToMeters(200);

        private float startingX;
        private float elapsedCycle;
private  float lastGlobalReference;

         public Waver()
            : this(Vector2.Zero)
        {
        }

         public Waver(Vector2 position)
            : base(position)
        {
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                var slowDown = Engine == null ? 1 : Engine.LevelManager.PhysicsManager.GlobalReferenceScale;
                if (lastGlobalReference != slowDown)
                {
                    elapsedCycle *= slowDown / lastGlobalReference;
                    lastGlobalReference = slowDown;
                }

                return new List<IRendering>() 
                { 
                    new BasicRendering(TEXTURE)
                    { 
                        Position = PhysicsConstants.MetersToPixels(Position), 
                        Scale = new Vector2(.75f, .75f),
                        Rotation = (float)(-Math.Atan((float)Math.Cos(((elapsedCycle % (slowDown * CYCLE_TIME)) / (slowDown * CYCLE_TIME)) * 2 * Math.PI)) + Math.PI)
                    }
                };
            }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);
            if (!Dead)
            {
                elapsedCycle += time.ElapsedGameTime.Milliseconds;

                var slowDown = Engine.LevelManager.PhysicsManager.GlobalReferenceScale;
                if (lastGlobalReference != slowDown)
                {
                    elapsedCycle *= slowDown / lastGlobalReference;
                    lastGlobalReference = slowDown;
                }

                Physics.Rotation = (float)(-Math.Atan((float)Math.Cos(((elapsedCycle % (slowDown * CYCLE_TIME)) / (slowDown * CYCLE_TIME)) * 2 * Math.PI)) + Math.PI);
                Position = new Vector2(
                    startingX + (maxOffset * (float)Math.Sin(((elapsedCycle % (slowDown * CYCLE_TIME)) / (slowDown * CYCLE_TIME)) * 2 * Math.PI)),
                    Position.Y);
            }
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = textureCache.GetResource(TEXTURE);
                Width = (int)(texture.Width * .75f);
                Height = (int)(texture.Height * .75f);
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(25),
                    PhysicsConstants.PixelsToMeters(50),
                    1,
                    Position,
                    this);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.IgnoreGravity = true;
                Physics.IsSensor = true;
                Physics.CollisionCategories = Category.Cat3;
                Physics.CollidesWith = Category.Cat1;

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<EnergyBullet>(OnCollidedWith);

                startingX = Position.X;
                Physics.LinearVelocity = new Vector2(0, ZOOM_SPEED);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
            lastGlobalReference = Engine.LevelManager.PhysicsManager.GlobalReferenceScale;
        }

        protected override bool OnCollidedWith(Fixture f, EnergyBullet bullet, Fixture df, Contact info)
        {
            bullet.Dead = true;
            Health -= 50;
            return true;
        }

        protected override void OnDeath()
        {
            var pickup = new Pickup(Position, DropType.Mana, 35);
            Engine.LevelManager.RegisterEntity(pickup);
        }

        public override void DestroyPhysics()
        {
            Physics.Dispose();
        }
    }
}
