using System;
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

namespace TimeSink.Entities
{
    [EditorEnabled]
    [SerializableEntity("57eb5766-5ce2-4694-ad4b-e019d4817985")]
    public class NonPlayerCharacter : Entity
    {
        const string EDITOR_NAME = "NPC";
        const string DEFAULT_TEXTURE = "Textures/Enemies/Dummy";

        private static readonly Guid GUID = new Guid("57eb5766-5ce2-4694-ad4b-e019d4817985");

        protected int textureHeight;
        protected int textureWidth;

        private bool collided;
        private EngineGame game;

        public NonPlayerCharacter() : this(Vector2.Zero, DEFAULT_TEXTURE) { }

        public NonPlayerCharacter(Vector2 position, String textureName)
        {
            DialogueState = 0;
            Position = position;
            TextureName = textureName;

            DialogueRoots = new List<String>();
            DialogueRoots.Add("4cf17838-279c-11e2-b64d-109adda800ea");
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }
        
        [SerializableField]
        [EditableField ("Texture Name")]
        public string TextureName { get; set; }

        [SerializableField]
        [EditableField("DialogueState")]
        public int DialogueState { get; set; }

        [SerializableField]
        [EditableField("DialogueRoots")]
        public List<String> DialogueRoots { get; set; }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override IRendering Preview
        {
            get { return Rendering; }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            if (collided)
            {
                if (InputManager.Instance.IsNewKey(Keys.X) && !game.ScreenManager.IsInDialogueState())
                {
                    game.ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid(DialogueRoots[DialogueState])), null);
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
            return true;
        }

        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            collided = false;
        }

        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var texture = engineRegistrations.Resolve<IResourceCache<Texture2D>>().GetResource(TextureName);
                var world = engineRegistrations.Resolve<World>();
                game = engineRegistrations.ResolveOptional<EngineGame>();

                Width = texture.Width;
                Height = texture.Height;
                Physics = BodyFactory.CreateRectangle(
                    world,
                    PhysicsConstants.PixelsToMeters(Width),
                    PhysicsConstants.PixelsToMeters(Height),
                    1,
                    Position);
                Physics.FixedRotation = true;
                Physics.BodyType = BodyType.Dynamic;
                Physics.UserData = this;

                var fix = Physics.FixtureList[0];
                fix.CollisionCategories = Category.Cat3;
                fix.CollidesWith = Category.Cat1;

                var hitsensor = fix.Clone(Physics);
                hitsensor.IsSensor = true;
                hitsensor.CollisionCategories = Category.Cat2;
                hitsensor.CollidesWith = Category.Cat2;

                Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
                Physics.RegisterOnSeparatedListener<UserControlledCharacter>(OnSeparation);

                initialized = true;
            }
        }

        public override List<Fixture> CollisionGeometry
        {
            get
            {
                return Physics.FixtureList;
            }
        }

        public override IRendering Rendering
        {
            get
            {
                return new BasicRendering(
                  TextureName,
                  PhysicsConstants.MetersToPixels(Physics.Position),
                  0f,
                  Vector2.One);
            }
        }
    }
}
