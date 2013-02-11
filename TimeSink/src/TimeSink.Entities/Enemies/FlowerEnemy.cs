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
using TimeSink.Entities.Inventory;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("c37c1bc9-b0c5-401d-a1e0-5d427628ff12")]
    public class FlowerEnemy : Enemy
    {
        const float MASS = 100f;
        const string ENEMY_TEXTURE = "Textures/Enemies/Flower_Neutral";
        const string EDITOR_NAME = "Flower Enemy";

        private static int textureHeight;
        private static int textureWidth;

        const float DEPTH = -50f;

        private static readonly Guid GUID = new Guid("c37c1bc9-b0c5-401d-a1e0-5d427628ff12");

        public FlowerEnemy()
            : base(Vector2.Zero)
        {
        }

        public FlowerEnemy(Vector2 position)
            : this (128, 128, position)
        {
        }

        public FlowerEnemy(int width, int height, Vector2 position)
        {
            Width = width;
            Height = height;
            Position = position;
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var cache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var texture = cache.LoadResource(ENEMY_TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height;
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
                    DepthWithinLayer = DEPTH,
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, ENEMY_TEXTURE, TextureCache)
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

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
         {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                Width = texture.Width;
                Height = texture.Height;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;

                Physics.RegisterOnCollidedListener<Arrow>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Dart>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                var hitsensor = fix.Clone(Physics);
                hitsensor.IsSensor = true;
                hitsensor.CollisionCategories = Category.Cat2;
                hitsensor.CollidesWith = Category.Cat2;

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
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
