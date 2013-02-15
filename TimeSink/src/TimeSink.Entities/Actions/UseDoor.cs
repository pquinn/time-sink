using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using Autofac;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core.Editor;
using FarseerPhysics.Dynamics.Contacts;
using System.IO;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using Engine.Defaults;

namespace TimeSink.Entities.Actons
{
    public enum DoorType { Up, Down, Side, None }

    [SerializableEntity("66c116cc-60bf-4808-a4c0-f5bb8cad053b")]
    [EditorEnabled]
    public class UseDoor : Entity
    {
        const string EDITOR_NAME = "Use Door";
        const string TEXTURE = "Materials/blank";
        const string EDITOR_PREVIEW_FORWARD = "Textures/Objects/Kyles_SpecialDoorForward";
        const string EDITOR_PREVIEW_BACKGROUND = "Textures/Objects/Kyles_SpecialDoorBackward";
        const string EDITOR_PREVIEW_SIDE = "Textures/Objects/Kyles_SpecialDoorForward";
        const string IN_OVERLAY = "Textures/Objects/Inner_Door_Overlay";
        const float DEPTH = 0;

        const string UP_POPUP = "Textures/Keys/w-key";
        const string DOWN_POPUP = "Textures/Keys/s-key";

        private static readonly Guid guid = new Guid("66c116cc-60bf-4808-a4c0-f5bb8cad053b");

        private bool collided;
        private EngineGame engine;
        private ItemPopup popup;

        public UseDoor()
            : this(Vector2.Zero, 50, 50, DoorType.Up, string.Empty, 0)
        {
        }

        public UseDoor(Vector2 position, int width, int height, DoorType doorType, string levelPath, int spawnPoint)
        {
            Position = position;
            Width = width;
            Height = height;
            DoorType = doorType;
            LevelPath = levelPath;
            SpawnPoint = spawnPoint;
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

        [SerializableField]
        [EditableField("DoorType")]
        public DoorType DoorType { get; set; }

        [SerializableField]
        [EditableField("LevelPath")]
        public string LevelPath { get; set; }

        [SerializableField]
        [EditableField("SpawnPoint")]
        public int SpawnPoint { get; set; }

        private bool registered = false;
        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {            
            if (DoorType == DoorType.Side)
                ChangeLevel();
            if (DoorType == DoorType.Up)
            {
                c.DoorType = DoorType.Up;
            }
            if (DoorType == DoorType.Down)
            {
                c.DoorType = DoorType.Down;
            }

            collided = true;

            if (popup != null && !registered)
            {
                engine.LevelManager.RenderManager.RegisterRenderable(popup);
                registered = true;
            }

            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            if (popup != null)
            {
                var temp = engine.LevelManager.RenderManager.UnregisterRenderable(popup);
                registered = false;
            }

            collided = false;
            c.DoorType = DoorType.None;
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

                if (DoorType == DoorType.Up)
                {
                    popup = new ItemPopup(
                        UP_POPUP, 
                        Physics.Position + new Vector2(0, -PhysicsConstants.PixelsToMeters(Height / 2)),
                        cache);
                }
                else if (DoorType == DoorType.Down)
                {
                    popup = new ItemPopup(
                        DOWN_POPUP, 
                        Physics.Position + new Vector2(0, -PhysicsConstants.PixelsToMeters(Height / 2)),
                        cache);
                }
                else
                {
                    popup = null;
                }

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

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            Physics.Friction = 0;
            if (collided)
            {
                if ((InputManager.Instance.IsNewKey(Keys.W) && DoorType == DoorType.Up) ||
                    (InputManager.Instance.IsNewKey(Keys.S) && DoorType != DoorType.Up))
                {
                    ChangeLevel();
                }
            }
        }

        private void ChangeLevel()
        {
            engine.MarkAsLoadLevel(LevelPath, SpawnPoint);
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var cache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            cache.GetResource(UP_POPUP);
            cache.GetResource(DOWN_POPUP);
        }

        public override IRendering Preview
        {
            get 
            {
                var tex = EDITOR_PREVIEW_SIDE;
                if (DoorType == DoorType.Up) tex = EDITOR_PREVIEW_FORWARD;
                if (DoorType == DoorType.Down) tex = EDITOR_PREVIEW_BACKGROUND;
                return new BasicRendering(tex)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, tex, TextureCache),
                    DepthWithinLayer = DEPTH,
                    TintColor = (DoorType == DoorType.Down) ? new Color(255, 255, 255, .5f) : Color.White
                };
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override List<IRendering> Renderings
        {
            get 
            {
                if (DoorType == DoorType.Down)
                {
                    return new List<IRendering>()
                    {
                        new BasicRendering(IN_OVERLAY)
                        {
                            Position = PhysicsConstants.MetersToPixels(Position),
                            Scale = BasicRendering.CreateScaleFromSize(Width, Height, IN_OVERLAY, TextureCache),
                            DepthWithinLayer = -200,
                            TintColor = Color.White
                        }
                    };
                }
                else
                    return new List<IRendering>() { new NullRendering() };
            }
        }
    }
}
