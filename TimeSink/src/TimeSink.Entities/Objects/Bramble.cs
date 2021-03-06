﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using Autofac;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics.Contacts;
using Engine.Defaults;

namespace TimeSink.Entities.Objects
{
    [EditorEnabled]
    [SerializableEntity("9bad74e2-3c00-443b-a461-26f625d32124")]
    public class Bramble : Entity
    {
        const string EDITOR_NAME = "Bramble";
        const string TEXTURE = "Textures/Objects/Bramble_Tileable";
        const string EDITOR_PREVIEW = "Textures/Objects/ladder";

        private static int textureHeight;
        private static int textureWidth;

        // lasts 1000 seconds, should probably use positive infinity
        public DamageOverTimeEffect dot = new DamageOverTimeEffect(10f, false);

        private static readonly Guid guid = new Guid("9bad74e2-3c00-443b-a461-26f625d32124");

        public Bramble()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public Bramble(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var texture = textureCache.LoadResource(TEXTURE);
            textureWidth = texture.Width;
            textureHeight = texture.Height; 
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("Invisible")]
        public bool Invisible { get; set; }

        private bool initialized;
        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                Physics = BodyFactory.CreateBody(world, Position, this);

                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                var rect = FixtureFactory.AttachRectangle(
                    spriteWidthMeters,
                    spriteHeightMeters,
                    1.4f,
                    Vector2.Zero,
                    Physics);

                Physics.BodyType = BodyType.Static;
                Physics.IsSensor = true;

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
        }

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

        public override IRendering Preview
        {
            get 
            {
                return new BasicRendering(TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache)
                    };
            }
        }

        public override List<IRendering> Renderings
        {
            get 
            {
                var list = new List<IRendering>();
                if (!Invisible)
                {
                    list.Add(
                        new BasicRendering(TEXTURE)
                        {
                            Position = PhysicsConstants.MetersToPixels(Position),
                            Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache)
                        });
                }

                return list;
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }
    }
}
