using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using Autofac;
using TimeSink.Engine.Core.Caching;
using System.Xml.Serialization;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities
{
    [SerializableEntity("23a14bb4-caab-4b6b-9ec5-62dfc1561cf9")]
    public class WorldGeometry : Entity
    {
        const string WORLD_TEXTURE_NAME = "Textures/giroux";
        const string EDITOR_NAME = "World Geometry";

        private static readonly Guid GUID = new Guid("23a14bb4-caab-4b6b-9ec5-62dfc1561cf9");

        [SerializableField]
        public float Friction { get; set; }

        [SerializableField]
        public float Sticktion { get; set; }

        private Texture2D geoTexture;

        private List<Fixture> collisionGeometry;
        public override List<Fixture> CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public WorldGeometry() { }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }
        
        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            geoTexture = textureCache.LoadResource(WORLD_TEXTURE_NAME);
        }

        public override IRendering Rendering
        {
            get 
            {
                var renderStack = new Stack<IRendering>();

                foreach (var geo in collisionGeometry)
                {
                    if (geo.ShapeType == ShapeType.Polygon)
                    {
                        var shape = geo.Shape as PolygonShape;

                        renderStack.Push(MakeCollisionRendering(
                            new CollisionRectangle(
                                PhysicsConstants.MetersToPixels(shape.Vertices[0]),
                                PhysicsConstants.MetersToPixels(shape.Vertices[3]),
                                PhysicsConstants.MetersToPixels(shape.Vertices[2]),
                                PhysicsConstants.MetersToPixels(shape.Vertices[1]))));
                    }
                    //else
                    //{
                    //    AABB box;
                    //    geo.GetAABB(out box, 0);
                    //    var topLeft = PhysicsConstants.MetersToPixels(box.Vertices[0]);
                    //    var botLeft = PhysicsConstants.MetersToPixels(box.Vertices[1]);
                    //    var botRight = PhysicsConstants.MetersToPixels(box.Vertices[2]);
                    //    var r = new Rectangle(
                    //        (int)topLeft.X,
                    //        (int)topLeft.Y,
                    //        (int)(botRight.X - botLeft.X),
                    //        (int)(botLeft.Y - topLeft.Y));
                    //    renderStack.Push(MakeCollisionRendering(r));
                    //}
                }

                return new StackableRendering(
                    renderStack
                ); 
            }
        }

        private BasicRendering MakeCollisionRendering(CollisionRectangle r)
        {
            var top = r.TopRight - r.TopLeft;
            top.Normalize();

            float width = r.TopRight.X - r.TopLeft.X;
            float height = r.BottomLeft.Y - r.TopLeft.Y;

            return new BasicRendering(
                WORLD_TEXTURE_NAME,
                r.TopLeft + new Vector2(width / 2, height / 2),
                (float)-Math.Acos(Vector2.Dot(Vector2.UnitX, top)),
                new Vector2(
                    width / geoTexture.Width,
                    height / geoTexture.Height
                )
            );
        }

        private BasicRendering MakeCollisionRendering(Rectangle r)
        {
            float width = r.Right - r.Left;
            float height = r.Bottom - r.Top;

            return new BasicRendering(
                WORLD_TEXTURE_NAME,
                new Vector2(r.Left + width / 2, r.Top + height / 2),
                0,
                new Vector2(
                    width / geoTexture.Width,
                    height / geoTexture.Height
                )
            );
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();

                Physics = BodyFactory.CreateBody(world, this);
                Physics.BodyType = BodyType.Static;
                Physics.Friction = .5f;
                collisionGeometry = Physics.FixtureList;

                Physics.CollidesWith = Category.All;
                Physics.CollisionCategories = Category.Cat1;

                initialized = true;
            }
        }
    }
}