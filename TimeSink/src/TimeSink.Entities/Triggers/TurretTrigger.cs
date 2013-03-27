using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Defaults;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Collisions;
using Autofac;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Entities.Enemies;

namespace TimeSink.Entities.Triggers
{
    [SerializableEntity("77913887-7ca6-4c45-a0c1-4f4c2d68f01b")]
    [EditorEnabled]
    public class TurretTrigger : Trigger
    {
        const string EDITOR_NAME = "Turret Trigger";
        private static readonly Guid GUID = new Guid("77913887-7ca6-4c45-a0c1-4f4c2d68f01b");
        private static readonly int TURRET_SIZE = 50;
        private static readonly float DAMAGE = 1.5f;
        public static readonly int TIME_BETWEEN_SHOTS = 100;

        private UserControlledCharacter character;
        private float timeSinceLastShot;

        public TurretTrigger() : base() { }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id
        {
            get { return GUID; }
            set { }
        }

        [SerializableField]
        [EditableField("Turret Id")]
        public string TurretId { get; set; }

        private LevelManager levelManager;
        private Turret turret;
        public Turret Turret
        {
            get
            {
                if (turret == null)
                {
                     turret = (Turret)levelManager.Level.Entities.First(x => x.InstanceId == TurretId);
                }
                return turret;
            }
            set
            {
                turret = value;
            }
        }
        public Body TurretPhysics { get; set; }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (character != null)
            {
                timeSinceLastShot += time.ElapsedGameTime.Milliseconds;

                if (timeSinceLastShot >= TIME_BETWEEN_SHOTS)
                {
                    if (Turret.HittingPlayer)
                    {
                        character.TakeDamage(DAMAGE, false);
                        timeSinceLastShot = 0;
                    }
                }
            }
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            if (Turret.Enabled)
            {
                if (character == null)
                    character = c;

                Turret.IsTargeting = true;
            }
            
            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            character = null;
            Turret.IsTargeting = false;
        }

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
            Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                TurretPhysics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(TURRET_SIZE),
                    PhysicsConstants.PixelsToMeters(TURRET_SIZE),
                    1);

                levelManager = engineRegistrations.Resolve<LevelManager>();
                    
            }


            base.InitializePhysics(force, engineRegistrations);
        }

        public override void DestroyPhysics()
        {
            base.DestroyPhysics();

            if (!initialized) return;
                TurretPhysics.Dispose();
        }
    }
}
