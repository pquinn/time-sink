using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Objects
{
    [SerializableEntity("51609f7d-8f65-46af-9eba-786f48352463")]
    [EditorEnabled]
    class WireBridge : VineBridge
    {
        private const string editorName = "Wire Bridge";
        const string EDITOR_PREVIEW = "";
        private static readonly Guid guid = new Guid("51609f7d-8f65-46af-9eba-786f48352463");
        public bool Electrified { get; set; }
        private UserControlledCharacter attachedChar = null;

        public WireBridge()
            : this(650, 50)
        { }

        public WireBridge(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter character, Fixture charfix, Contact info)
        {
            if (!Hanging && character.Physics.LinearVelocity.Y > 0 && !Electrified)
            {
                attachedChar = character;
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

            if (attachedChar != null)
            {
                attachedChar.Electrocute();
                ForceSeperation(attachedChar);
            }
        }

        private void AnimateElectricity()
        {
        }
    }
}
