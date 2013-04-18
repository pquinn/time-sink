using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("51609f7d-8f65-46af-9eba-786f48352463")]
    [EditorEnabled]
    class WireBridge : VineBridge
    {
        private const string editorName = "Wire Bridge";
        const string EDITOR_PREVIEW = "Textures/Objects/electric wireOn";
        const string TEXTURE = "Textures/Objects/electric wireOff";
        private static readonly Guid guid = new Guid("51609f7d-8f65-46af-9eba-786f48352463");
        private float electrifyLength = 3000f;
        private float electrifyTimer, electrifySwitchTimer = 0f;
        private float electifySwitchInterval = 200f;
        //private bool electrifying = false;
        private string currentTexture = TEXTURE;
        private UserControlledCharacter attachedChar = null;
        private BasicRendering anim;


        public WireBridge()
            : this(650, 50)
        { }

        public WireBridge(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string EditorName
        {
            get
            {
                return editorName;
            }
        }

        [SerializableField]
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

        [SerializableField]
        [EditableField("Electrified")]
        public bool Electrified 
        { 
            get; 
            set; 
        }

        public override bool OnCollidedWith(Fixture f, UserControlledCharacter character, Fixture charfix, Contact info)
        {
            attachedChar = character;

            if (!Hanging && character.Physics.LinearVelocity.Y > 0 && !Electrified)
            {
                character.Physics.ResetDynamics();
                character.WheelBody.ResetDynamics();

                joint = JointFactory.CreatePrismaticJoint(
                    world,
                    character.WheelBody,
                    Physics,
                    Vector2.Zero,
                    Vector2.UnitX);

                origLinearDamping = character.Physics.LinearDamping;
                character.Physics.LinearDamping = 10;

                Hanging = true;
            }
            else if (Electrified)
            {
                attachedChar.Electrocute();
            }

            return true;
        }

        public void ElectrifyWire()
        {
            AnimateElectricity();
            Electrified = true;

            if (attachedChar != null)
            {
                attachedChar.Electrocute();
                ForceSeperation(attachedChar);
            }
        }

        public override void OnUpdate(GameTime time, Engine.Core.EngineGame world)
        {
            base.OnUpdate(time, world);


            if (Electrified)
            {
                electrifyTimer += time.ElapsedGameTime.Milliseconds;
                electrifySwitchTimer += time.ElapsedGameTime.Milliseconds;

                if (electrifyTimer >= electrifyLength)
                {
                    Electrified = false;
                    electrifyTimer = 0f;
                }
                else if (electrifySwitchTimer >= electifySwitchInterval)
                {
                    if (currentTexture.Equals(TEXTURE))
                    {
                        currentTexture = EDITOR_PREVIEW;
                    }
                    else
                        currentTexture = TEXTURE;
                    electrifySwitchTimer = 0f;
                }

            }
        }

        private void AnimateElectricity()
        {
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
                {
                 
                    new BasicRendering(currentTexture)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, currentTexture, TextureCache)
                    }
                };
            }
        }
    }
}
