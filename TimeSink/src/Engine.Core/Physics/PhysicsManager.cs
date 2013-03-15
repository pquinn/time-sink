using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.DebugViews;
using Autofac;
using FarseerPhysics.Collision.Shapes;

namespace TimeSink.Engine.Core.Physics
{
    public class PhysicsManager
    {
        private IComponentContext engineRegistrations;

        public World World { get; private set; }

        public PhysicsManager(IComponentContext engineRegistrations)
        {
            World = new World(PhysicsConstants.Gravity);
            this.engineRegistrations = engineRegistrations;
        }

        public void RegisterPhysicsBody(IPhysicsEnabledBody body)
        {
            body.InitializePhysics(false, engineRegistrations);
        }

        public Converter<Vector2, float> TimeScaleLookup = _ => 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="globalReferenceScale">Scale world is ticking at.</param>
        public void Update(GameTime gameTime, float globalReferenceScale=1)
        {
            //time step from player's perspective
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var scale = 1 / globalReferenceScale;

            //calculates time scale at specific positions
            Converter<Vector2, float> scaleLookup =
                body => TimeScaleLookup(body) * scale;

            //We need to base the timeScale off of the player's timeScale.
            //IE if the player's scale is low, it's adjusted to normal and
            //everything else is increased.


            //timescale = 0                 -> no time passes, objects are frozen
            //timescale = 1                 -> time passes normally
            //timescale approaches infinity -> objects speed up since they're passing through time faster

            //from the player's pespective (remember that the player always moves at normal speed)
            //0 < timescale_p < 1 ->  everything moves faster
            //timescale_p = 1     -> normal
            //timescale_p > 1     -> everything is slowed down

            World.Step(dt, scaleLookup);
        }

        internal void Clear()
        {
            World = new World(PhysicsConstants.Gravity);
        }

        public void UnregisterPhysicsBody(IPhysicsEnabledBody body)
        {
            body.DestroyPhysics();
        }
    }
}
