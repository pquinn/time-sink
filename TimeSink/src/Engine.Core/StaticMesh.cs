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

namespace TimeSink.Engine.Core
{
    public class StaticMesh : Entity
    {
        const string EDITOR_NAME = "Static Mesh";

        string texture;
        IResourceCache<Texture2D> cache;

        public StaticMesh(string texture, Vector2 position, float rotation, Vector2 scale, IResourceCache<Texture2D> cache)
        {
            this.texture = texture;
            _initialPosition = position;
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.cache = cache;
        }

        private Vector2 _initialPosition;

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        private Body _physics;
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return _physics.FixtureList;
            }
        }

        public override void InitializePhysics(World world)
        {
            _physics = BodyFactory.CreateRectangle(
                world,
                PhysicsConstants.PixelsToMeters(128),
                PhysicsConstants.PixelsToMeters(128),
                1,
                _initialPosition,
                this);
            _physics.BodyType = BodyType.Static;

            /*var tex = cache.GetResource(texture);
                var relativeTransform =
                    Matrix.CreateTranslation(new Vector3(-tex.Width / 2, -tex.Height / 2, 0)) *
                    Matrix.CreateScale(new Vector3(Scale.X, Scale.Y, 1)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateTranslation(new Vector3(tex.Width / 2, tex.Height / 2, 0)) *
                    Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0)); 
                return new CollisionRectangle(
                    Vector2.Transform(Vector2.Zero, relativeTransform),
                    Vector2.Transform(new Vector2(0, tex.Height), relativeTransform),
                    Vector2.Transform(new Vector2(tex.Width, tex.Height), relativeTransform),
                    Vector2.Transform(new Vector2(tex.Width, 0), relativeTransform));
            */
        }

        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(texture, Position, Rotation, Scale);
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(texture);
        }


        public void Expand(IResourceCache<Texture2D> cache, Vector2 dragOffset, Vector2 origScale, Matrix transform)
        {
            var tex = cache.GetResource(texture);
            var size = origScale * new Vector2(tex.Width, tex.Height);

            var relativeTransform =
                Matrix.CreateRotationZ(Rotation) *
                transform;

            var offsetInMeshFrame = Vector2.Transform(dragOffset, relativeTransform);

            Scale += (2 * offsetInMeshFrame) / size;
        }
    }
}
