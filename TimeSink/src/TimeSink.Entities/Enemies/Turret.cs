﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Physics;
using TimeSink.Entities.Triggers;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Entities.Objects;
using TimeSink.Entities.Utils;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics.Contacts;

namespace TimeSink.Entities.Enemies
{
    [SerializableEntity("5774325e-ce5e-4db6-a036-4ed8e85a36d4")]
    [EditorEnabled]
    public class Turret : Enemy, ISwitchable
    {
        const string EDITOR_NAME = "Turret";
        private static readonly Guid GUID = new Guid("5774325e-ce5e-4db6-a036-4ed8e85a36d4");
        private static readonly string BASE_ON = "Textures/Objects/MG-Turret-Base_On";
        private static readonly string BASE_OFF = "Textures/Objects/MG-Turret-Base_Off";
        private static readonly string GUN = "Textures/Objects/MG-Turret-gun";
        private static readonly string GUN_FIRING = "Textures/Objects/MG-Turret-gun-fire";
        private static readonly string IMPACT = "Textures/Objects/MG-Turret-impact";
        private static readonly float IMPACT_RANDOMNESS = PhysicsConstants.PixelsToMeters(15);

        private float gunRotation = 0f;
        private Guid charGuid = new Guid("defb4f64-1021-420d-8069-e24acebf70bb");
        private Vector2 hitPosition;
        public bool HittingPlayer { get; set; }

        private BasicRendering impact;
        private BasicRendering laser;
        private bool firing;
        private int timeSinceLastFire;

        private Random random;

        public Turret() : base() { random = new Random(); }

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
        [EditableField("Enabled")]
        public bool Enabled { get; set; }

        [SerializableField]
        [EditableField("Rotation")]
        public float Rotation { get; set; }

        public bool IsTargeting { get; set; }

        public override void OnUpdate(GameTime time, EngineGame game)
        {
            base.OnUpdate(time, game);

            if (Enabled && IsTargeting  && !Dead)
            {
                var mat = Matrix.CreateTranslation(new Vector3(.1f + Width / 2, 0, 0)) *
                    Matrix.CreateRotationX(gunRotation);
                var character = game.LevelManager.Level.Entities.First(x => x.Id == charGuid);
                game.LevelManager.PhysicsManager.World.RayCast(
                    RayCastCallback,
                    new Vector2(Physics.Position.X + mat.Translation.X, Physics.Position.Y + mat.Translation.Y),
                    character.Physics.Position);
                var offset = character.Position - Position;
                gunRotation = (float)Math.Atan2(offset.Y, offset.X);

                var width = PhysicsConstants.MetersToPixels(Vector2.Distance(Position, hitPosition));
                var off = hitPosition - Position;
                laser = new BasicRendering("blank")
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    DepthWithinLayer = -200,
                    Rotation = (float)Math.Atan2(off.Y, off.X),
                    TintColor = Color.Red,
                    Scale = BasicRendering.CreateScaleFromSize((int)width, 3, "blank", TextureCache)
                };

                timeSinceLastFire += time.ElapsedGameTime.Milliseconds;

                if (timeSinceLastFire >= TurretTrigger.TIME_BETWEEN_SHOTS)
                {
                    firing = true;
                    impact = new BasicRendering(IMPACT)
                    {
                        Position = new Vector2(
                            PhysicsConstants.MetersToPixels(hitPosition.X),
                            random.Next(
                                PhysicsConstants.MetersToPixels(hitPosition.Y - IMPACT_RANDOMNESS),
                                PhysicsConstants.MetersToPixels(hitPosition.Y + IMPACT_RANDOMNESS))),
                        Scale = Vector2.One * .09f,
                        DepthWithinLayer = -200,
                        Rotation = (float)Math.Atan2(-hitNormal.Y, -hitNormal.X)
                    };
                }
                if (timeSinceLastFire >= TurretTrigger.TIME_BETWEEN_SHOTS * 2)
                {
                    timeSinceLastFire = 0;
                    firing = false;
                }
            }
            else
            {
                firing = false;
                timeSinceLastFire = 0;
                impact = null;
            }
        }

        private float RayCastCallback(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            if (fixture.IsSensor == true || fixture.Body.UserData is SwitchableWall)
            {
                return -1;
            }
            else
            {
                HittingPlayer = fixture.Body.UserData is UserControlledCharacter;
                hitPosition = point;
                hitNormal = normal;
                return 0;
            }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                var renderList = new List<IRendering>();
                var tint = Math.Min(255, 2.55f * health);
                var tintCol = new Color((int)255f, (int) tint, (int) tint, (int) 255f);
                renderList.Add(
                    new BasicRendering(Enabled ? BASE_ON : BASE_OFF)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Rotation = Rotation,
                        TintColor = tintCol
                    });
                renderList.Add(
                    new BasicRendering(firing ? GUN_FIRING : GUN)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Rotation = gunRotation,
                        TintColor = tintCol
                    });
                if (IsTargeting)
                {
                    renderList.Add(laser);
                }
                if (firing)
                {
                    renderList.Add(impact);
                }

                return renderList;
            }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(Enabled ? BASE_ON : BASE_OFF)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Rotation = Rotation
                };
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        private Vector2 hitNormal;
        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var texture = engineRegistrations.Resolve<IResourceCache<Texture2D>>().GetResource(BASE_OFF);
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(texture.Width / 2),
                    PhysicsConstants.PixelsToMeters(texture.Height), 
                    0,
                    Position,
                    this);
                Physics.Friction = .2f;
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Static;
                Physics.IgnoreGravity = true;

                Physics.RegisterOnCollidedListener<EnergyBullet>(OnCollidedWith);

                gunRotation = Rotation;

                initialized = true;
            }

            base.InitializePhysics(force, engineRegistrations);
        }

        protected override bool OnCollidedWith(Fixture f, EnergyBullet bullet, Fixture df, Contact info)
        {
            Health -= 10f;
            bullet.Dead = true;

            return true;
        }

        public void OnSwitch()
        {
        }
    }
}
