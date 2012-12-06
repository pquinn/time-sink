using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using TimeSink.Entities.Objects;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Entities
{
    public class VineBridgeInteraction
    {
        private World world;

        private Joint joint;
        private Joint joint2;

        public void CreateFixtures(World world, UserControlledCharacter character)
        {
            this.world = world;
            Fixture = FixtureFactory.AttachCircle(
                .1f, 0, character.Physics, new Vector2(0, -(PhysicsConstants.PixelsToMeters(character.Height) / 4)));
            Fixture.UserData = character;
            Fixture.IsSensor = true;
            Fixture.CollidesWith = Category.Cat4;
            Fixture.CollisionCategories = Category.Cat4;
            Fixture.CollisionGroup = 1;
        }

        public Fixture Fixture { get; set; }
        public bool Hanging { get; private set; }
        
        public void Update()
        {
        }

        public bool OnCollidedWith(UserControlledCharacter character, VineBridge bridge, Contact info)
        {
            // Todo:this method gets called twice for some reason and I don't know why.
            // This check is so the second call doesn't override things and create two joints.
            if (!Hanging && character.Physics.LinearVelocity.Y > 0)
            {
                joint = new PrismaticJoint(
                    character.WheelBody,
                    bridge.Physics,
                    character.Position +
                        new Vector2(0, -(PhysicsConstants.PixelsToMeters(character.Height) / 4)) -
                            character.WheelBody.Position, 
                    Vector2.Zero,
                    new Vector2(1, 0));
                world.AddJoint(joint);

                //joint2 = new PrismaticJoint(
                //    character.Physics,
                //    bridge.Physics,
                //    Vector2.Zero,
                //    character.Physics.Position - character.WheelBody.Position,
                //    new Vector2(1, 0));
                //world.AddJoint(joint2);

                Hanging = true;
            }

            return true;
        }

        public void OnSeperation(UserControlledCharacter character, VineBridge bridge)
        {
            if (Hanging)
                ForceSeperation(character);
        }

        public void ForceSeperation(UserControlledCharacter character)
        {
            world.RemoveJoint(joint);
            //world.RemoveJoint(joint2);
            Hanging = false;
        }
    }
}
