﻿using Autofac;
using Engine.Defaults;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Entities.Enemies;

namespace TimeSink.Entities.Triggers
{
    [EditorEnabled]
    [SerializableEntity("2605fa3e-389a-4ee4-a0d0-d576ab189404")]
    public class CreateTutorialMonsterTrigger : Trigger
    {
        const string EDITOR_NAME = "Create Tutorial Monster Trigger";
        private static readonly Guid GUID = new Guid("2605fa3e-389a-4ee4-a0d0-d576ab189404");

        public CreateTutorialMonsterTrigger()
            : base()
        {
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }
       
        protected override void RegisterCollisions()
        {            
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
        }

        private bool used;
        public virtual bool OnCollidedWith(Fixture f, UserControlledCharacter obj, Fixture f2, Contact info)
        {
            if (!used)
            {
                var monster = new TutorialMonster(Position - PhysicsConstants.PixelsToMeters(new Vector2(300, 800)), Vector2.UnitX);
                levelManager.RegisterEntity(monster);
                monster.StartChase();
                Engine.CameraLock = true;
                Engine.Camera.MoveCameraTo(new Vector3(-PhysicsConstants.MetersToPixels(obj.Position.X) + 400,
                                          -PhysicsConstants.MetersToPixels(obj.Position.Y) + 400,
                                           0));
                used = true;
            }

            return true;
        }
    }
}
