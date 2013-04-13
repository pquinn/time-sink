using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Rendering;
using Engine.Defaults;
using TimeSink.Entities.Inventory;

namespace TimeSink.Entities.Actions
{
    [EditorEnabled]
    [SerializableEntity("edc87822-ee90-4826-84af-d0eeba3c13fe")]
    public class InteractableItem : Entity
    {
        const string EDITOR_NAME = "Interactable Item";
        const string TEXTURE = "Materials/blank";
        const string EDITOR_PREVIEW = "Textures/Objects/ladder";

        string ACTION_POPUP = "Textures/Keys/x-Key";
        const float DEPTH = 0;
        const int POPUP_OFFSET = 20;

        private static readonly Guid guid = new Guid("edc87822-ee90-4826-84af-d0eeba3c13fe");

        private bool collided;
        protected EngineGame engine;
        private ItemPopup popup;
        protected bool used;
        protected bool heldUsed;
        private UserControlledCharacter character;

        public UserControlledCharacter Character { get { return character; } }

        public InteractableItem()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public InteractableItem(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            used = false;
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        private bool registered = false;
        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            character = c;
            if (popup != null && !registered && !used)
            {
                engine.LevelManager.RenderManager.RegisterRenderable(popup);
                registered = true;
            }

            collided = true;

            return true;
        }

        public virtual bool OnCollidedWith(Fixture f, Projectile p, Fixture cf, Contact info)
        {
            return false;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            character = null;
            if (popup != null)
            {
                engine.LevelManager.RenderManager.UnregisterRenderable(popup);
                registered = false;
            }

            collided = false;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var cache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                engine = engineRegistrations.ResolveOptional<EngineGame>();
                Physics = BodyFactory.CreateBody(world, Position, this);

                if (engine != null && engine.GamepadEnabled)
                {
                    ACTION_POPUP = InputManager.Instance.GamepadTextures[InputManager.ButtonActions.Pickup];
                }

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

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);
                Physics.RegisterOnCollidedListener<Projectile>(OnCollidedWith);

                popup = new ItemPopup(ACTION_POPUP, 
                    Physics.Position + new Vector2(0, -PhysicsConstants.PixelsToMeters(Height / 2 + POPUP_OFFSET)),
                    cache);

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;

            Physics.Dispose();
            //TODO: destroy item popup too?
        }

        private bool usedGuard;
        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            if (collided && !used)
            {
                if (InputManager.Instance.ActionPressed(InputManager.ButtonActions.Pickup))
                {
                    ExecuteAction();
                }
                if (InputManager.Instance.ActionHeld(InputManager.ButtonActions.Pickup))
                {
                    ExecuteHeldAction(gameTime);
                }
            }
            if (used && !usedGuard)
            {
                engine.LevelManager.RenderManager.UnregisterRenderable(popup);
                registered = false;
                usedGuard = true;
            }
        }

        protected virtual void ExecuteHeldAction(GameTime gameTime)
        {
        }

        protected virtual void ExecuteAction()
        {
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var cache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            cache.GetResource(ACTION_POPUP);
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache),
                    DepthWithinLayer = DEPTH
                };
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override List<IRendering> Renderings
        {
            get { return new List<IRendering>(){ new NullRendering() }; }
        }
    }
}
