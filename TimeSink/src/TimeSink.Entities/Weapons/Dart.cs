using Microsoft.Xna.Framework;

using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

using System;
using TimeSink.Engine.Core.Editor;


namespace TimeSink.Entities.Weapons
{
    [EditorEnabled]
    public class Dart : Entity, IWeapon
    {
        const float DART_MASS = 1f;
        const float DART_SPEED = 2000f;
        const string DART_TEXTURE_NAME = "Textures/Weapons/Dart";
        const string EDITOR_NAME = "Dart";

        public GravityPhysics physics { get; private set; }
        public DamageOverTimeEffect dot { get; private set; }

        public Dart() 
            : this(Vector2.Zero)
        {
        }

        public Dart(Vector2 position)
        {
            physics = new GravityPhysics(position, DART_MASS)
            {
                GravityEnabled = true
            };

            dot = new DamageOverTimeEffect(4, 100);
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override string EditorPreview
        {
            get
            {
                return DART_TEXTURE_NAME;
            }
        }

        [EditableField("Position")]
        public Vector2 Position
        {
            get { return physics.Position; }
            set { physics.Position = value; }
        }   

        public override ICollisionGeometry CollisionGeometry
        {
            get
            {
                return new CollisionRectangle(new Rectangle(
                    (int)physics.Position.X,
                    (int)physics.Position.Y,
                    16,
                    8
                ));
            }
        }

        public override IPhysicsParticle PhysicsController
        {
            get { return physics; }
        }

        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(
                    DART_TEXTURE_NAME,
                    physics.Position,
                    (float)Math.Atan2(physics.Velocity.Y, physics.Velocity.X),
                    Vector2.One
                );
            }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(WorldGeometry entity, CollisionInfo info)
        {
            Dead = true;
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(Entity entity, CollisionInfo info)
        {
            if (!(entity is UserControlledCharacter))
            {
                Dead = true;
            }
        }

        public override void Load(EngineGame engineGame)
        {
            engineGame.TextureCache.LoadResource(DART_TEXTURE_NAME);
        }

        public override void Update(GameTime time, EngineGame world)
        {
            base.Update(time, world);

            if (Dead)
            {
                world.RenderManager.UnregisterRenderable(this);
                world.CollisionManager.UnregisterCollisionBody(this);
                world.PhysicsManager.UnregisterPhysicsBody(this);
            }
        }

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
            character.InHold = false;
            Dart dart = new Dart(
                            new Vector2(character.PhysicsController.Position.X + UserControlledCharacter.X_OFFSET,
                                        character.PhysicsController.Position.Y + UserControlledCharacter.Y_OFFSET));
            Vector2 initialVelocity = character.Direction * DART_SPEED;
            dart.physics.Velocity += initialVelocity;
            world.Entities.Add(dart);
            world.RenderManager.RegisterRenderable(dart);
            world.PhysicsManager.RegisterPhysicsBody(dart);
            world.CollisionManager.RegisterCollisionBody(dart);
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime)
        {
            Fire(character, world, gameTime, holdTime);
        }
    }
}