using Engine.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Entities.Enemies;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Entities.Triggers
{
    [SerializableEntity("60f7d4d9-287c-4ade-98c0-1e6e9023a815")]
    [EditorEnabled]
    public class ZoomerSpawner : Trigger
    {
        const String EDITOR_NAME = "Zoomer Spawn Trigger";
        private static readonly Guid guid = new Guid("60f7d4d9-287c-4ade-98c0-1e6e9023a815");

        private UserControlledCharacter character;
        private float timeSinceLastSpawn;

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [EditableField("Time Between Spawns (mili)")]
        [SerializableField]
        public float TimeBetweenSpawns { get; set; }

        public override Guid Id
        {
            get
            {
                return guid;
            }
            set
            {
            }
        }

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeperatedWith);
        }

        private bool OnCollidedWith(Fixture f1, UserControlledCharacter collidedWith, Fixture f2, Contact contact)
        {
            character = collidedWith;

            return true;
        }

        private void OnSeperatedWith(Fixture f1, UserControlledCharacter e2, Fixture f2)
        {
            character = null;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            timeSinceLastSpawn += time.ElapsedGameTime.Milliseconds;
            if (character != null && timeSinceLastSpawn > TimeBetweenSpawns)
            {
                var zoomer = new Zoomer(character.Position + PhysicsConstants.PixelsToMeters(new Vector2(Engine.GraphicsDevice.Viewport.Width * .75f, 0)));
                zoomer.Facing = 4.71f;
                zoomer.Speed = 300;

                Engine.LevelManager.RegisterEntity(zoomer);

                zoomer.PerformZoom();

                timeSinceLastSpawn = 0;
            }
        }
    }
}
