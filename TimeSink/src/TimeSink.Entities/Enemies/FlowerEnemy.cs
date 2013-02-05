using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics.Joints;
using TimeSink.Engine.Core;
using Engine.Defaults;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("c37c1bc9-b0c5-401d-a1e0-5d427628ff12")]
    public class FlowerEnemy : Enemy
    {
        const float MASS = 100f;
        const string ENEMY_TEXTURE = "Textures/Enemies/Flower_Neutral";
        const string EDITOR_NAME = "Flower Enemy";

        const float DEPTH = -50f;

        private static readonly Guid GUID = new Guid("c37c1bc9-b0c5-401d-a1e0-5d427628ff12");

        public FlowerEnemy()
            : base(Vector2.Zero)
        {
        }

        public FlowerEnemy(Vector2 position)
            : base(position)
        {
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                // Get variables
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                Width = texture.Width;
                Height = texture.Height;

                var width = PhysicsConstants.PixelsToMeters(Width);
                var height = PhysicsConstants.PixelsToMeters(Height);

                // Create a rectangular body for hit detection.
                Physics = BodyFactory.CreateRectangle(
                    world,
                    width,
                    height,
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;
                Physics.Mass = 0f;
                Physics.CollidesWith = Category.Cat1;
                Physics.CollisionCategories = Category.Cat3;
                
                // Register hit detection callbacks.
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * health);
                return new BasicRendering(ENEMY_TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    TintColor = new Color(255f, tint, tint, 255f),
                    DepthWithinLayer = DEPTH
                };
            }
        }

        public override IRendering Preview
        {
            get
            {
                return Rendering;
            }
        }
    }
}
