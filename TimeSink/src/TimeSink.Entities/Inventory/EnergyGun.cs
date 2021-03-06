﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core;
using FarseerPhysics.Dynamics.Contacts;
using Engine.Defaults;
using TimeSink.Entities.Objects;
using TimeSink.Engine.Core.Collisions;
using Autofac;
using FarseerPhysics.Factories;
using TimeSink.Entities.Triggers;
using TimeSink.Entities.Enemies;
using TimeSink.Entities.Hud;

namespace TimeSink.Entities.Inventory
{
    [SerializableEntity("16b8d25a-25f1-4b0b-acae-c60114aade0e")]
    public class EnergyGun : Entity, IWeapon
    {
        private const string BULLET_TEXTURE = "";
        private const int BULLET_SPEED = 3000;
        private const int MAX_AMMO = 20;
        private const int RADIUS = 15;
        private const int TIME_BETWEEN_SHOTS = 300;
        private const int RELOAD_TIME = 7500;
        private const float MANA_DRAIN_PER_MILLI = .0025f;
        private const float TIME_SCALE = 2.5f;
        private const string TEXTURE = "Textures/Weapons/EnergyGun";
        private const int Y_OFFSET = 0;

        private static readonly Guid GUID = new Guid("16b8d25a-25f1-4b0b-acae-c60114aade0e");

        private int timeSinceLastShot;
        private int ammo = MAX_AMMO;
        private  bool reloading;
        private int reload_count;
        private bool charged;
        private UserControlledCharacter character;
        private ChargeBar chargeBar;

        public bool Render { get; set; }

        public EnergyGun()
            : this(Vector2.Zero)
        {
        }

        public EnergyGun(Vector2 position)
        {
            Position = position;
        }

        public UserControlledCharacter Character { get; set; }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override List<Fixture> CollisionGeometry
        {
            get { return new List<Fixture>(); }
        }

        public override List<IRendering> Renderings
        {
            get
            {
                if (Character != null)
                {
                    return new List<IRendering>() { 
                    new BasicRendering(TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Character.Position) - new Vector2(0, Y_OFFSET),
                        Scale = new Vector2(.3f * Character.Facing, .3f),
                        Rotation = Character.Direction.Y * Character.Facing,
                        DepthWithinLayer = -220f
                    }
                };
                }
                else
                {
                    return new List<IRendering>();
                }
            }
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            if (charged && character.Mana > 0)
            {
                character.Mana = Math.Max(0, character.Mana - time.ElapsedGameTime.Milliseconds * MANA_DRAIN_PER_MILLI);
                Engine.UpdateHealth();
            }
            else
            {
                charged = false;
                Engine.LevelManager.PhysicsManager.GlobalReferenceScale = 1;
            }

            timeSinceLastShot += time.ElapsedGameTime.Milliseconds;
            if (reloading)
            {
                reload_count += time.ElapsedGameTime.Milliseconds;
                chargeBar.UpdateProgress(reload_count, RELOAD_TIME);
                chargeBar.Position = Character.Position;

                if (reload_count > RELOAD_TIME * Engine.LevelManager.PhysicsManager.GlobalReferenceScale)
                {
                    Engine.LevelManager.RenderManager.UnregisterRenderable(chargeBar);
                    reload_count = 0;
                    reloading = false;
                    ammo = MAX_AMMO;
                }
            }

            base.OnUpdate(time, world);
        }

        public void Use(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            Fire(character, world, gameTime, holdTime, charged);
        }

        public void Fire(UserControlledCharacter character, EngineGame world, GameTime gameTime, double holdTime, bool charged)
        {
            Character = character;
            if (timeSinceLastShot >= TIME_BETWEEN_SHOTS * Engine.LevelManager.PhysicsManager.GlobalReferenceScale && !reloading && ammo > 0)
            {
                timeSinceLastShot = 0;
                EnergyBullet bullet = new EnergyBullet(
                    new Vector2(character.Physics.Position.X,// + UserControlledCharacter.X_OFFSET,
                                character.Physics.Position.Y + Y_OFFSET),
                                20, 20,
                                PhysicsConstants.PixelsToMeters(BULLET_SPEED * character.Direction));

                ammo -= 1;

                if (ammo <= 0)
                {
                    Reload();
                }

                world.LevelManager.RegisterEntity(bullet);
            }
        }

        public void Reload()
        {
            if (!reloading)
            {
                chargeBar = new ChargeBar(Character.Position);
                Engine.LevelManager.RenderManager.RegisterRenderable(chargeBar);
                chargeBar.UpdateProgress(reload_count, RELOAD_TIME);
                reloading = true;
                reload_count = 0;
                //add reload animations
            }
        }

        public void ChargeInitiated(UserControlledCharacter character, GameTime gameTime)
        {
            charged = character.Mana > 0;
            this.character = character;
            
            if (charged)
                Engine.LevelManager.PhysicsManager.GlobalReferenceScale = TIME_SCALE;
        }

        public void ChargeReleased(UserControlledCharacter character, GameTime gameTime)
        {
            charged = false;
            Engine.LevelManager.PhysicsManager.GlobalReferenceScale = 1;
        }

        public override string EditorName
        {
            get { throw new NotImplementedException(); }
        }

        public override IRendering Preview
        {
            get { throw new NotImplementedException(); }
        }

        public string Texture
        {
            get { return "Textures\\giroux"; }
        }
    }
}
