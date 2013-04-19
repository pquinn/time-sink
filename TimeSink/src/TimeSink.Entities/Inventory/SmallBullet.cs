using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Enemies
{
    public class SmallBullet : LargeBullet
    {
        protected const string TEXTURE = "Textures/Objects/Projectile_enemy";
        const float DEPTH = -50f;

        public SmallBullet(Vector2 position, int width, int height, Vector2 velocity)
            : base(position, width, height, velocity)
        {
        }
        
        public override bool OnCollidedWith(Fixture f1, UserControlledCharacter character, Fixture f2, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (!character.Invulnerable)
            {
                character.TakeDamage(25, true);
                Dead = true;
            }

            return false;
        }

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() { new BasicRendering(TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache),
                        DepthWithinLayer = DEPTH
                    }};
            }
        }
    }
}
