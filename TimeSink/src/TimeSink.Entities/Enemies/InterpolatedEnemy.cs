using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Autofac;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using TimeSink.Entities.Inventory;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Enemies
{
    public class InterpolatedEnemy : Enemy
    {
        private int elapsedTime;

        protected Texture2D texture;

        public InterpolatedEnemy()
            : this(Vector2.Zero)
        {
        }

        public InterpolatedEnemy(Vector2 position)
            : base(position)
        {
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("Texture Path")]
        public string TexturePath { get; set; }

        public Vector2 StartingPoint { get; set; }
        public bool Activated { get; set; }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (Activated)
            {
                elapsedTime += time.ElapsedGameTime.Milliseconds;
                UpdatePostion(elapsedTime);
            }
        }

        public virtual void UpdatePostion(int elapsedTime)
        {
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
                    {
                        new BasicRendering(TexturePath)
                        {
                            Position = Position,
                            Scale = BasicRendering.CreateScaleFromSize(Width, Height, TexturePath, TextureCache)
                        }
                    };
            }
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                StartingPoint = Position;

                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                texture = textureCache.GetResource(TexturePath);

                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);
                Physics = BodyFactory.CreateRectangle(
                    world,
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;
                Physics.IgnoreGravity = true;
                Physics.IsSensor = true;

                Physics.RegisterOnCollidedListener<Arrow>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<Dart>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnCollidedListener<EnergyBullet>(OnCollidedWith);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }
    }
}
