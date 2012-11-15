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

namespace TimeSink.Engine.Game.Entities
{
    public class WorldGeometry : Entity
    {
        const string WORLD_TEXTURE_NAME = "Textures/giroux";

        private Texture2D geoTexture;

        private List<Fixture> collisionGeometry;
        public override List<Fixture> CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public WorldGeometry() { }

        public Body PhysicsBody { get; private set; }

        public override void Load(EngineGame game)
        {
            geoTexture = game.TextureCache.LoadResource(WORLD_TEXTURE_NAME);
        }

        //[OnCollidedWith.Overload]
        //public void OnCollidedWith(ICollideable body, CollisionInfo info)
        //{
        //    if (body is IPhysicsEnabledBody)
        //    {
        //        var phys = (body as IPhysicsEnabledBody).PhysicsController;
        //        if (phys != null)
        //            phys.Position -= info.MinimumTranslationVector + new Vector2(0, -1);
        //    }
        //}

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

        public override void InitializePhysics(World world)
        {
            PhysicsBody = BodyFactory.CreateBody(world, this);
            PhysicsBody.BodyType = BodyType.Static;
            PhysicsBody.Friction = .5f;
            collisionGeometry = PhysicsBody.FixtureList;

            PhysicsBody.CollidesWith = Category.All;
            PhysicsBody.CollisionCategories = Category.Cat1;
        }
    }
}