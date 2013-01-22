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

namespace TimeSink.Entities.Objects
{

    [SerializableEntity("75522a0f-66c2-444e-90bb-88df79c36c29")]
    [EditorEnabled]
    public class TutorialTrigger : Entity
    {
        const string EDITOR_NAME = "TutorialTrigger";
        const string TEXTURE = "Materials/blank";
        const string EDITOR_PREVIEW = "Textures/Objects/tutorial";

        private static readonly Guid guid = new Guid("75522a0f-66c2-444e-90bb-88df79c36c29");

        private String tutorialText;
        private EngineGame engine;
        private TutorialDisplay display;

        public TutorialTrigger()
            : this(Vector2.Zero, 50, 50, "Enter Text Here")
        {
        }
        public TutorialTrigger(Vector2 position, int width, int height, String text)
        {
            Position = position;
            Width = width;
            Height = height;
            TutorialText = text;
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
        public override Engine.Core.Rendering.IRendering Preview
        {
            get
            {
                return new SizedRendering(
                    EDITOR_PREVIEW,
                    PhysicsConstants.MetersToPixels(Position),
                    0, Width, Height);
            }
        }
        public override Engine.Core.Rendering.IRendering Rendering
        {
            get
            {
                return new SizedRendering(
                    TEXTURE,
                    PhysicsConstants.MetersToPixels(Position),
                    0, Width, Height);
            }
        }

        [SerializableField]
        [EditableField("Width")]
        public override int Width { get; set; }

        [SerializableField]
        [EditableField("Height")]
        public override int Height { get; set; }

        [SerializableField]
        [EditableField("TutorialText")]
        public String TutorialText { get; set; }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }
        private bool initialized;
        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                engine = engineRegistrations.ResolveOptional<EngineGame>();
                Physics = BodyFactory.CreateBody(world, Position, this);
                display = new TutorialDisplay(TutorialText, PhysicsConstants.MetersToPixels(Position));

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

                initialized = true;
            }
        }


        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            engine.LevelManager.RenderManager.RegisterRenderable(display);

            return true;
        }
        public void OnSeparation(Fixture f1, UserControlledCharacter c, Fixture f2)
        {
            engine.LevelManager.RenderManager.UnregisterRenderable(display);
        }
    }
}
