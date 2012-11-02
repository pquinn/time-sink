using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using MechanicsTest.Collisions;

namespace MechanicsTest
{
    public class WorldGeometry : ICollideable
    {
        private CollisionSet collisionGeometry = new CollisionSet();
        public ICollisionGeometry CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public WorldGeometry(Rectangle r)
        {
            collisionGeometry.Geometry.Add(new CollisionRectangle(r));
        }

        public void Draw(GameTime gameTime)
        {
            
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(UserControlledCharacter character)
        {
            character.GravityEnabled = false;
        }
    }
}
