using FarseerPhysics.Factories;
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

        private static readonly Guid GUID = new Guid("657b0660-5620-46da-bea4-499f95c658e8");

        public Ladder()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public Ladder(Vector2 position, int width, int height)
        {
            Position= position;
            this.Width = width;
            this.Height = height;
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Width")]
        public int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public int Height { get; set; }

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

                var rect = FixtureFactory.AttachRectangle(
                    PhysicsConstants.PixelsToMeters(Width), 
                    PhysicsConstants.PixelsToMeters(Height),
                    1.4f,
                    Vector2.Zero,
                    Physics);

                Physics.Friction = 0;
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
            //Enable the character to enter a climbing state thus effecting her input handling
            c.CanClimb = true;
            return true;
        }

        [OnSeparation.Overload]
        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            c.CanClimb = false;
            c.Physics.IgnoreGravity = false;
        }

        public override IRendering Preview
        {
            get 
            {
                var scale = new Vector2(Width / 234f, Height / 596f);
                return new TintedRendering(EDITOR_PREVIEW, PhysicsConstants.MetersToPixels(Physics.Position), 0, scale, Color.White); 
            }
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
