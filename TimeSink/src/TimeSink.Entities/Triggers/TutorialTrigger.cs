using Autofac;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Engine.Defaults;
using TimeSink.Engine.Core.Caching;
using TimeSink.Entities.Enemies;

namespace TimeSink.Entities.Triggers
{

    [SerializableEntity("75522a0f-66c2-444e-90bb-88df79c36c29")]
    [EditorEnabled]
    public class TutorialTrigger : Entity
    {
        const string EDITOR_NAME = "Tutorial Trigger";
        const string TEXTURE = "Materials/blank";
        const string EDITOR_PREVIEW = "Textures/Objects/tutorial";

        private static readonly Guid guid = new Guid("75522a0f-66c2-444e-90bb-88df79c36c29");

        private Vector2 previewScale;
        private EngineGame engine;
        private ItemPopup display1;
        private ItemPopup display2;
        private ItemPopup plusCombine;
        private UserControlledCharacter character;

        public TutorialTrigger()
            : this(Vector2.Zero, 200, 150, 0, 0, true)
        {
        }
        public TutorialTrigger(Vector2 position, int width, int height, int key1, int key2, bool initiallyActive)
        {
            Position = position;
            Width = width;
            Height = height;
            //Ints representing the enum value from InputActions
            Key1 = key1;
            Key2 = key2;
            Active = initiallyActive;
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
        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(EDITOR_PREVIEW)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = previewScale, 
                    TintColor = new Color(255, 255, 255, .1f),
                    DepthWithinLayer = .625f
                };
            }
        }
        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() { new NullRendering() };
            }
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("Key1")]
        public int Key1 { get; set; }

        [SerializableField]
        [EditableField("Key2")]
        public int Key2 { get; set; }

        [SerializableField]
        [EditableField("Active")]
        public bool Active { get; set; }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }
        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var cache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                engine = engineRegistrations.ResolveOptional<EngineGame>();
                var texture = cache.GetResource(EDITOR_PREVIEW);
                previewScale = new Vector2(Width / texture.Width, Height / texture.Height);

                Physics = BodyFactory.CreateBody(world, Position, this);

                float spriteWidthMeters = PhysicsConstants.PixelsToMeters(Width);
                float spriteHeightMeters = PhysicsConstants.PixelsToMeters(Height);

                string texture1 = InputTextureLookup(Key1);

                if (Key2 != 999)
                {
                    string texture2 = InputTextureLookup(Key2);
                    display2 = new ItemPopup(texture2, Vector2.Zero, cache, new Vector2(40, -90));

                    plusCombine = new ItemPopup("Textures/Keys/plusCombine", Vector2.Zero, cache, new Vector2(0, -90));
                    
                    display1 = new ItemPopup(texture1, Vector2.Zero, cache, new Vector2(-40, -90));
                }
                else
                {
                    display2 = null;

                    display1 = new ItemPopup(texture1, Vector2.Zero, cache, new Vector2(0, -90));
                }


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

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            if (Active)
            {
                if (!c.Popups.Contains(display1))
                {
                    c.Popups.Add(display1);
                }
                //engine.LevelManager.RenderManager.RegisterRenderable(display1);

                if (display2 != null)
                {
                    if (!c.Popups.Contains(display2))
                    {
                        c.Popups.Add(display2);
                        c.Popups.Add(plusCombine);
                    }
                }
            }
            character = c;

            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            if (Active)
            {
                c.Popups.Remove(display1);
                if (display2 != null)
                {
                    c.Popups.Remove(display2);
                    c.Popups.Remove(plusCombine);
                }
            }
            character = null;
        }

        internal void RecheckCollision()
        {
            if (character != null)
            {

                if (!character.Popups.Contains(display1))
                {
                    character.Popups.Add(display1);
                }
                //engine.LevelManager.RenderManager.RegisterRenderable(display1);

                if (display2 != null)
                {
                    if (!character.Popups.Contains(display2))
                    {
                        character.Popups.Add(display2);
                        character.Popups.Add(plusCombine);
                    }
                }
            }
        }

        public string InputTextureLookup(int action)
        {
            if (Engine != null && Engine.GamepadEnabled)
            {
                return InputManager.Instance.GamepadTextures[(InputManager.ButtonActions)action];
            }
            else
                return InputManager.Instance.KeyboardTextures[(InputManager.ButtonActions)action];
        }
    }
}
