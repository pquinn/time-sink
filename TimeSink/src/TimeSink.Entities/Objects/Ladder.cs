
﻿using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using TimeSink.Engine.Core;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    [SerializableEntity("657b0660-5620-46da-bea4-499f95c658e8")]
    public class Ladder : Entity
    {
        const string EDITOR_NAME = "Ladder";
        const string TEXTURE = "Materials/blank";
        const string EDITOR_PREVIEW = "Textures/Objects/ladder";

        private bool feetTouching = false;
        private float linearDamping = 0;
        private bool rectExit = false;
        private bool wheelExit1 = false;
        private bool wheelExit = false;
        public float LinearDamping { get { return linearDamping; } set { linearDamping = value; } }

        private static readonly Guid GUID = new Guid("657b0660-5620-46da-bea4-499f95c658e8");

        public Ladder()
            : this(Vector2.Zero, 75, 200, true, false)
        {
        }

        public Ladder(Vector2 position, int width, int height, bool sideways, bool vinewall)
        {
            Position = position;
            this.Width = width;
            this.Height = height;
            this.Sideways = sideways;
            this.VineWall = vinewall;
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("Sideways")]
        public bool Sideways { get; set; }

        [SerializableField]
        [EditableField("VineWall")]
        public bool VineWall { get; set; }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(IComponentContext container)
        {
            var texture = container.Resolve<IResourceCache<Texture2D>>().GetResource(TEXTURE);
        }
        private bool initialized;

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {

            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();
                Physics = BodyFactory.CreateBody(world, Position, this);
                
                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                var rect = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1.4f,
                    Vector2.Zero,
                    Physics);

                Physics.Friction = .2f;
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Static;

                //TODO -- Figure out Steve's collison logic for passable collision detection

                // Possible logic for passthrough collision detection
                Physics.IsSensor = true;

                initialized = true;
            }
        }

        [OnCollidedWith.Overload]
        public bool OnCollidedWith(UserControlledCharacter c, Contact info)
        {
            rectExit = false;
            wheelExit = false;
            wheelExit1 = false;
            //Enable the character to enter a climbing state thus effecting her input handling
            if (info.FixtureA.UserData == null && info.FixtureB.UserData == null)
            {
                feetTouching = false;
            }
            else if((info.FixtureA.UserData != null && info.FixtureA.UserData.Equals(false)) || (info.FixtureB.UserData != null && info.FixtureB.UserData.Equals(false)))
            {
                Physics.IsSensor = true;
                feetTouching = true;
            }

            if (!c.Climbing)
            {
                linearDamping = c.Physics.LinearDamping;
            }
            c.CanClimb = this;
            return true;
        }

        [OnSeparation.Overload]
        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            if (f2.UserData != null)
            {
                if (f2.UserData.Equals(true))
                {
                    c.CanClimb = null;
                    c.DismountLadder();
                    c.Physics.IgnoreGravity = false;
                    c.Physics.LinearDamping = linearDamping;
                    if (wheelExit1)
                        wheelExit = true;
                    else
                        wheelExit1 = true;
                    c.Climbing = false;
                }
                else if (f2.UserData.Equals(false))
                {
                    rectExit = true;
                }
                if (rectExit && wheelExit)
                {
                    c.CanClimb = null;
                    c.DismountLadder();
                    c.Physics.IgnoreGravity = false;
                    c.Physics.LinearDamping = linearDamping;
                    c.Climbing = false;
                }
            }
            else if (feetTouching == false)
            {
                c.CanClimb = null;
                c.Physics.LinearDamping = linearDamping;
                c.Physics.IgnoreGravity = false;
            }

        }

        public override IRendering Preview
        {
            get { return new SizedRendering(EDITOR_PREVIEW, PhysicsConstants.MetersToPixels(Physics.Position), 0, Width, Height); }
        }

        public override IRendering Rendering
        {
            get
            {
                return new NullRendering();
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }
    }
}