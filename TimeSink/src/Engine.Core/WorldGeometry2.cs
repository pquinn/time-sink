using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Linq;
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
using TimeSink.Entities;

namespace TimeSink.Engine.Core
{
    [SerializableEntity("23a14bb4-caab-4b6b-9ec5-62dfc1561cf9")]
    public class WorldGeometry2 : Entity
    {
        const string WORLD_TEXTURE_NAME = "Textures/giroux";
        const string EDITOR_NAME = "World Geometry";

        private static readonly Guid GUID = new Guid("23a14bb4-caab-4b6b-9ec5-62dfc1561cf9");

        public WorldGeometry2() { }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public float Friction { get; set; }

        [SerializableField]
        public float Sticktion { get; set; }

        public List<List<WorldCollisionGeometrySegment>> GeoChains { get; set; }

        private Texture2D geoTexture;

        private List<Fixture> collisionGeometry;
        public override List<Fixture> CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override IRendering Rendering
        {
            get
            {
                return new NullRendering();
            }
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<World>();

                Physics = BodyFactory.CreateBody(world, this);
                Physics.BodyType = BodyType.Static;
                Physics.Friction = 1.0f;
                collisionGeometry = Physics.FixtureList;

                Physics.CollidesWith = Category.All;
                Physics.CollisionCategories = Category.Cat1;

                if (GeoChains != null)
                {
                    GeoChains.ForEach(
                    x =>
                    {
                        //Fixture old = null;
                        foreach (var pair in x.Take(x.Count - 1).Zip(x.Skip(1), Tuple.Create))
                        {
                            var f = FixtureFactory.AttachEdge(pair.Item1.EndPoint, pair.Item2.EndPoint, Physics);
                            //if (old != null)
                            //{
                            //    var oldShape = old.Shape as EdgeShape;
                            //    var fShape = f.Shape as EdgeShape;
                            //    //fShape.Vertex0 = oldShape.Vertex2;
                            //    //fShape.HasVertex0 = true;
                            //    //oldShape.Vertex3 = fShape.Vertex1;
                            //    //oldShape.HasVertex3 = true;
                            //}
                           if (pair.Item2.IsOneWay)
                                new OneWayPlatform(f);
                           //old = f;
                        }
                    });
                }

                initialized = true;
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(IComponentContext engineRegistrations)
        {
        }
    }
}