using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Autofac;
using TimeSink.Engine.Core.Physics;
using FarseerPhysics.Factories;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("a0a2ba1a-0692-4f49-adb5-1f333e462649")]
    public class VerticalTracker : Enemy
    {
        private const string TEXTURE = "Textures/giroux";
        private float DEPTH = -50f;


        private UserControlledCharacter target;

        private static readonly Guid guid = new Guid("a0a2ba1a-0692-4f49-adb5-1f333e462649");

        public VerticalTracker()
            : this(Vector2.Zero)
        {
        }


        public VerticalTracker(Vector2 position)
            :base(position)
        {
        }

        public override Guid Id { get { return guid; } set{} }

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);
            var width = PhysicsConstants.PixelsToMeters(Width);
            var height = PhysicsConstants.PixelsToMeters(Height);
            var world = engineRegistrations.Resolve<PhysicsManager>().World;

            FixtureFactory.AttachRectangle(width, height, .1f, Vector2.Zero, Physics);
            Physics.Mass = 10f;

            
                    
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() 
                {
                    new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position)

                }
                };
            }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(TEXTURE);
            }
        }

        public void Jump()
        {
            Physics.ApplyForce(new Vector2(0, -25f));
        }

        public void Descend()
        {
        }

        public void Shoot()
        {
        }
    }
}
