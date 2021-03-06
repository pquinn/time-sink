﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.StateManagement;
using TimeSink.Engine.Core.States;
using Engine.Defaults;

namespace TimeSink.Entities
{
    [EditorEnabled]
    [SerializableEntity("57eb5766-5ce2-4694-ad4b-e019d4817985")]
    public class NonPlayerCharacter : Entity
    {
        const string EDITOR_NAME = "NPC";
        const string DEFAULT_TEXTURE = "Textures/Enemies/Dummy";
        string ACTION_POPUP = "Textures/Keys/x-Key";
        const int POPUP_OFFSET = 20;
        const float DEPTH = .25f;

        private static readonly Guid GUID = new Guid("57eb5766-5ce2-4694-ad4b-e019d4817985");

        protected int textureHeight;
        protected int textureWidth;

        protected bool collided;
        protected EngineGame game;

        private ItemPopup popup;

        public NonPlayerCharacter() : this(Vector2.Zero,  DEFAULT_TEXTURE, 64, 128) { }

        public NonPlayerCharacter(Vector2 position, String textureName, int width, int height)
        {
            DialogueState = 0;
            Position = position;
            TextureName = textureName;
            Width = width;
            Height = height;
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField ("Texture Name")]
        public string TextureName { get; set; }

        [SerializableField]
        [EditableField("DialogueState")]
        public int DialogueState { get; set; }

        [SerializableField]
        [EditableField("DialogueRoots")]
        public string DialogueRoots { get; set; }

        public List<String> DialogueRootsList
        {
            get
            {
                return DialogueRoots.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        protected Body HitSensor { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get { return Renderings[0]; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            if (collided && DialogueRootsList.Count > 0)
            {
                if (InputManager.Instance.ActionPressed(InputManager.ButtonActions.Pickup) && !game.ScreenManager.IsInDialogueState())
                {
                    //game.
                    game.ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid(DialogueRootsList[DialogueState])), null);
                }
            }
        }

        public override void Load(IComponentContext container)
        {
            var texture = container.Resolve<IResourceCache<Texture2D>>().GetResource(TextureName);
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            collided = true;
            game.LevelManager.RenderManager.RegisterRenderable(popup);
            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            game.LevelManager.RenderManager.UnregisterRenderable(popup);
            collided = false;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var cache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = cache.GetResource(TextureName);
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                game = engineRegistrations.ResolveOptional<EngineGame>();

                if (game != null && game.GamepadEnabled)
                {
                    ACTION_POPUP = InputManager.Instance.GamepadTextures[InputManager.ButtonActions.Pickup];
                }

                //Width = texture.Width;
                //Height = texture.Height;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Static;
                Physics.UserData = this;

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                HitSensor = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width) * 2,
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                HitSensor.IsSensor = true;
                HitSensor.CollisionCategories = Category.Cat2;
                HitSensor.CollidesWith = Category.Cat2;

                HitSensor.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                HitSensor.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);

                popup = new ItemPopup(ACTION_POPUP, 
                    Physics.Position + new Vector2(0, -PhysicsConstants.PixelsToMeters(Height / 2 + POPUP_OFFSET)),
                    cache);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
            }
        }

        // fix NPC in editor
        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>(){
                    new BasicRendering(TextureName)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TextureName, TextureCache),
                        DepthWithinLayer = DEPTH
                    }
                };
            }
        }
    }
}
