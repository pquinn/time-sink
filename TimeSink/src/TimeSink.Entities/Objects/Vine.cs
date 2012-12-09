using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.States;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    [SerializableEntity("b425aa27-bc56-4953-aa4c-be089fdc29c8")]
    public class Vine : Entity
    {
        const string EDITOR_NAME = "Vine";
        const string VINE_TEXTURE = "Textures/Objects/Vine";

        private static readonly Guid GUID = new Guid("b425aa27-bc56-4953-aa4c-be089fdc29c8");

        public float TextureHeight { get; set; }
        public float TextureWidth { get; set; }

        //private Func<float, float> PatrolFunction { get; set; }
        private bool first;
        private float tZero;

        public Body VineAnchor { get; set; }
        public Body VineEndAffector { get; set; }

        public Vine() : this(Vector2.Zero, 1f) { }

        public Vine(Vector2 position) : this(position, 1f) { }

        public Vine(Vector2 position, float timeSpan)
        {
            Position = position;
            Scale = .5f;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Scale")]
        public float Scale { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(IComponentContext container)
        {
            var texture = container.Resolve<IResourceCache<Texture2D>>().GetResource(VINE_TEXTURE);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();
                var texture = engineRegistrations.Resolve<IResourceCache<Texture2D>>().GetResource(VINE_TEXTURE);

                Width = (int)(texture.Width / 2 * Scale);
                Height = (int)(texture.Height * Scale);
                TextureWidth = PhysicsConstants.PixelsToMeters(Width);
                TextureHeight = PhysicsConstants.PixelsToMeters(Height);


                //anchor point
                Physics = BodyFactory.CreateBody(world, Position, this);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Static;

                VineAnchor = BodyFactory.CreateRectangle(
                    world,
                    TextureWidth,
                    TextureHeight,
                    1,
                    Position);
                VineAnchor.BodyType = BodyType.Dynamic;
                VineAnchor.UserData = this;
                VineAnchor.CollidesWith = Category.Cat5;
                VineAnchor.CollisionCategories = Category.Cat5;
                VineAnchor.CollisionGroup = 2;
                VineAnchor.LinearDamping = 2;
                
                RevJoint = JointFactory.CreateRevoluteJoint(world, Physics, VineAnchor, new Vector2(0, -TextureHeight / 2));

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                VineAnchor.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            }
        }

        bool OnCollidedWith(Fixture f, UserControlledCharacter character, Fixture cFix, Contact info)
        {
            return true;
        }

        public override IRendering Rendering
        {
            get 
            {
                return new PivotedRendering(
                    VINE_TEXTURE,
                    PhysicsConstants.MetersToPixels(Physics.Position),
                    //Need to translate rotation
                    VineAnchor == null ? 0 : VineAnchor.Rotation,
                    new Vector2(Scale, Scale));
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            //interpolate the rotation like a line
            base.OnUpdate(time, world);
        }

        public FarseerPhysics.Dynamics.Joints.RevoluteJoint RevJoint { get; set; }
    }
}
