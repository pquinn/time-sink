﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using TimeSink.Entities.Inventory;
using Autofac;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Enemies
{
    public enum DropType { None, Health, Mana }

    [EditorEnabled]
    [SerializableEntity("849aaec2-7155-4a37-ab71-42d0c1611881")]
    public class Zoomer : Enemy
    {
        private const string EDITOR_NAME = "Zoomer";
        private const string DUMMY_TEXTURE = "Textures/Enemies/Dummy";
        private const int ZOOM_SPEED = 300;
        private const int RAY_LENGTH = 50;

        private static readonly Guid GUID = new Guid("849aaec2-7155-4a37-ab71-42d0c1611881");
        private Guid charGuid = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");

        private bool zoomed;

        public Zoomer()
            : this(Vector2.Zero)
        {
        }

        public Zoomer(Vector2 position)
            : base(position)
        {
        }

        [SerializableField]
        [EditableField("Facing (radians)")]
        public float Facing { get; set; }

        [SerializableField]
        [EditableField("Drop Type")]
        public DropType DropType 
        { 
            get; 
            set; 
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() 
                { 
                    new BasicRendering(DUMMY_TEXTURE)
                    { 
                        Position = PhysicsConstants.MetersToPixels(Position), 
                        Scale = new Vector2(.5f, .5f),
                        Rotation = Facing
                    }
                };
            }
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = textureCache.GetResource(DUMMY_TEXTURE);
                Width = texture.Width / 2;
                Height = texture.Height / 2;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
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

        public override void OnUpdate(GameTime time, EngineGame game)
        {
            base.OnUpdate(time, game);

            if (!zoomed)
            {
                RayCastCallback cb = delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    if (fixture.Body.UserData is UserControlledCharacter)
                    {
                        PerformZoom();
                        return 0;
                    }
                    else if (fixture.Body.UserData is WorldGeometry2)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                };

                game.LevelManager.PhysicsManager.World.RayCast(
                    cb,
                    Position,
                    Position + (new Vector2((float)Math.Sin(Facing), -(float)Math.Cos(Facing)) * RAY_LENGTH));
            }
        }

        private void PerformZoom()
        {
            var dir = new Vector2((float)Math.Sin(Facing), -(float)Math.Cos(Facing));
            Physics.ApplyForce(dir * ZOOM_SPEED);
            zoomed = true;
        }

        protected override void OnDeath()
        {
            if (zoomed)
            {
                switch (DropType)
                {
                    case DropType.Health:
                        //drop health pickup
                        break;
                    case DropType.Mana:
                        //drop mana pickup
                        break;
                    default:
                        break;
                }
            }    
        } 
    }
}
