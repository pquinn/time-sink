﻿using FarseerPhysics.Dynamics;
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

namespace TimeSink.Entities.Enemies
{
    public class LargeBullet : Entity
    {
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

        public override Engine.Core.Rendering.IRendering Preview
        {
            get { throw new NotImplementedException(); }
        }

        public override List<FarseerPhysics.Dynamics.Fixture> CollisionGeometry
        {
            get { throw new NotImplementedException(); }
        }

        public override Engine.Core.Rendering.IRendering Rendering
        {
            get { return new SizedRendering("Textures/giroux", Position, 0, Width, Height); }
        }

        private bool initialized;
        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            if (!initialized || force)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;

                Physics = BodyFactory.CreateRectangle(
                    world, 
                    PhysicsConstants.PixelsToMeters(Width), 
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.BodyType = BodyType.Dynamic;
                Physics.FixedRotation = true;
                Physics.IsSensor = true;
                Physics.IgnoreGravity = true;
                Physics.Mass = 5;

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat2;
                fix.CollidesWith = Category.Cat2;

                Physics.ApplyLinearImpulse(Velocity);

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<BreakableWall>(OnCollidedWith);
            }
        }

        private bool OnCollidedWith(Fixture f1, BreakableWall wall, Fixture f2, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            wall.BulletHit();
            Dead = true;

            return true;
        }

        private bool OnCollidedWith(Fixture f1, UserControlledCharacter character, Fixture f2, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (!character.Invulnerable)
                character.TakeDamage(30);

            return true;
        }
    }
}
