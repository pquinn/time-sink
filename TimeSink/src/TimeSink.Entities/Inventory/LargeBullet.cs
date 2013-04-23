using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Autofac;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Entities.Objects;
using TimeSink.Entities.Triggers;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Enemies
{
    public class LargeBullet : Entity
    {
        const string TEXTURE = "Textures/Objects/ice beam";
        const float DEPTH = -50f;
        public LargeBullet(Vector2 position, int width, int height, Vector2 velocity)
        {
            Position = position;
            Width = width;
            Height = height;
            Velocity = velocity;
        }

        public Vector2 Velocity { get; set; }

        public override string EditorName
        {
            get { throw new NotImplementedException(); }
        }

        public override Guid Id
        {
            get
            {
                
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override IRendering Preview
        {
            get { throw new NotImplementedException(); }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() { new BasicRendering(TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache),
                        DepthWithinLayer = DEPTH
                    }};
            }
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (!initialized || force)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var level = engineRegistrations.Resolve<LevelManager>().Level;

                Physics = BodyFactory.CreateBody(world, Position, this);

                foreach (Entity e in level.Entities)
                {
                    if (e is TutorialTrigger)
                    {
                        var trigger = (TutorialTrigger)e;
                        trigger.Active = true;
                        trigger.RecheckCollision();
                    }
                }

                var body = FixtureFactory.AttachRectangle(
                    PhysicsConstants.PixelsToMeters(Width), 
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Vector2.Zero,
                    Physics);
                Physics.BodyType = BodyType.Dynamic;
                Physics.FixedRotation = true;
                Physics.IsSensor = true;
                Physics.IsBullet = true;
                Physics.IgnoreGravity = true;
                Physics.Mass = 5;

                var fix = Physics.FixtureList[0];

                Physics.CollidesWith = ~Category.Cat31;

                Physics.ApplyLinearImpulse(Velocity);

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<BreakableWall>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Entity>(OnCollidedWith);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        private bool OnCollidedWith(Fixture f1, BreakableWall wall, Fixture f2, Contact contact)
        {
            wall.BulletHit();
            Engine.LevelManager.RenderManager.UnregisterRenderable(this);
            Dead = true;

            return true;
        }

        public virtual bool OnCollidedWith(Fixture f1, UserControlledCharacter character, Fixture f2, Contact contact)
        {
            if (!character.Invulnerable)
            {
                character.TakeDamage(30, true, true);
                Dead = true;
            }


            return false;
        }

        public virtual bool OnCollidedWith(Fixture f1, Entity entity, Fixture f2, Contact contact)
        {
            return false;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);
            if (Dead)
            {
                world.LevelManager.RenderManager.UnregisterRenderable(this);
                world.LevelManager.PhysicsManager.UnregisterPhysicsBody(this);
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }
    }
}
