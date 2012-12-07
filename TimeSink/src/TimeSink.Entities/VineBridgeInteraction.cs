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

        private PrismaticJoint joint;
        private Joint joint2;
        private float origLinearDamping;

        public void CreateFixtures(World world, UserControlledCharacter character)
        {
            this.world = world;
            Fixture = FixtureFactory.AttachCircle(
                .1f, 5, character.Physics, new Vector2(0, -(PhysicsConstants.PixelsToMeters(character.Height) / 4)));
            Fixture.Friction = 5f;
            Fixture.Restitution = 1f;
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
                //joint.MotorEnabled = true;
                //joint.MaxMotorForce = 50;
                //joint.MotorSpeed = 0;
                world.AddJoint(joint);

                origLinearDamping = character.Physics.LinearDamping;
                character.Physics.LinearDamping = 10;

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
            character.Physics.LinearDamping = origLinearDamping;
            world.RemoveJoint(joint);
            Hanging = false;
        }
    }
}
