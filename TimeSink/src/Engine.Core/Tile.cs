using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Collisions;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Xml.Serialization;
using Autofac;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core;

namespace TimeSink.Engine.Core
{
    [SerializableEntity("50be45d6-51a2-462e-a8e5-0779832ffee3")]
    public class Tile : Entity
    {
        const string EDITOR_NAME = "Tile";
        private static readonly Guid GUID = new Guid("50be45d6-51a2-462e-a8e5-0779832ffee3");

        public Tile()
        {
        }

        public Tile(string texture, Vector2 position, float rotation, Vector2 scale)
        {
            this.Texture = texture;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        public string Texture { get; set; }

        [SerializableField]
        public float Rotation { get; set; }

        [SerializableField]
        public Vector2 Scale { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [XmlIgnore]
        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return new List<Fixture>();
            }
        }

        [XmlIgnore]
        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(Texture, PhysicsConstants.MetersToPixels(Position), Rotation, Scale);
            }
        }

        [XmlIgnore]
        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var t = textureCache.LoadResource(Texture);
        }

        public void Expand(IResourceCache<Texture2D> cache, Vector2 dragOffset, Vector2 origScale, Matrix transform)
        {
            var tex = cache.GetResource(Texture);

            var size = origScale * new Vector2(tex.Width / 2, tex.Height / 2) * 2;

            var relativeTransform =
                Matrix.CreateRotationZ(Rotation) *
                transform;

            var offsetInMeshFrame = Vector2.Transform(dragOffset, relativeTransform);

            Scale += (2 * offsetInMeshFrame) / size;
        }
    }
}
