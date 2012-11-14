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

namespace TimeSink.Engine.Core
{
    public class StaticMesh : Entity
    {
        string texture;
        public StaticMesh(string texture, Vector2 position)
        {
            this.texture = texture;
            _initialPosition = position;
        }

        private Vector2 _initialPosition;
        public Vector2 Position 
        {
            get
            {
                return _physics.Position;
            }
            set
            {
                _physics.Position = value;
            }
        }

        private Body _physics;
        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return _physics.FixtureList;
            }
        }

        public override IRendering Rendering
        {
            get 
            {
                return new BasicRendering(texture, Position, 0, Vector2.One);
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(texture);
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
        }
    }
}
