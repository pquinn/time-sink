﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;
using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Joints;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("b330a111-f01f-4106-b4ce-b400ae2b2ef6")]
    [EditorEnabled]
    public class VineBridge : Entity
    {
        private const string editorName = "Vine Bridge";
        private static readonly Guid id = new Guid("b330a111-f01f-4106-b4ce-b400ae2b2ef6");
        const string EDITOR_PREVIEW = "Textures/Objects/HorizontalVine";

        public VineBridge()
            : this(600, 75)
        {
        }

        public VineBridge(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string EditorName
        {
            get { return editorName; }
        }

        [SerializableField]
        public override Guid Id
        {
            get
            {
                return id;
            }
            set
            {
            }
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("NumLinks")]
        public int NumLinks { get; set; }

        public bool Hanging { get; private set; }
        private PrismaticJoint joint;
        private float origLinearDamping;
        private World world;

        public bool OnCollidedWith(Fixture f, UserControlledCharacter character, Fixture charfix, Contact info)
        {
            // Todo:this method gets called twice for some reason and I don't know why.
            // This check is so the second call doesn't override things and create two joints.
            if (!Hanging && character.Physics.LinearVelocity.Y > 0)
            {
                joint = new PrismaticJoint(                    
                    Physics,
                    character.WheelBody,                    
                    Vector2.Zero,
                    character.Position +
                        new Vector2(0, -(PhysicsConstants.PixelsToMeters(character.Height) / 4)) -
                            character.WheelBody.Position,
                    new Vector2(1, 0));
                world.AddJoint(joint);

                origLinearDamping = character.Physics.LinearDamping;
                character.Physics.LinearDamping = 10;

                Hanging = true;
            }

            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter character, Fixture f2)
        {
            if (Hanging)
                ForceSeperation(character);
        }

        public void ForceSeperation(UserControlledCharacter character)
        {
            character.Physics.LinearDamping = origLinearDamping;
            world.RemoveJoint(joint);
            Hanging = false;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            if (!initialized || force)
            {
                world = engineRegistrations.Resolve<World>();
                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height / 2);

                Physics = BodyFactory.CreateRectangle(
                    world,
                    spriteWidthMeters, spriteHeightMeters,
                    0.5f, Position);
                Physics.Friction = 5f;
                Physics.Restitution = 1f;
                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;
                Physics.UserData = this;
                Physics.CollidesWith = Category.Cat4;
                Physics.CollisionCategories = Category.Cat4;
                Physics.CollisionGroup = 1;

                Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                initialized = true;
            }
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }

        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get { return new SizedRendering(EDITOR_PREVIEW, PhysicsConstants.MetersToPixels(Physics.Position), 0, Width, Height); }
        }
    }
}
