using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core;
using Engine.Defaults;
using TimeSink.Entities.Objects;
using TimeSink.Entities.Inventory;
using TimeSink.Entities.Triggers;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Entities.Enemies
{
    public class EnergyBullet : LargeBullet
    {
        protected const string TEXTURE = "Textures/Objects/Projectile_character";
        const float DEPTH = -50f;

        public EnergyBullet(Vector2 position, int width, int height, Vector2 velocity)
            : base(position, width, height, velocity)
        {
        }

        public override bool OnCollidedWith(Fixture f1, UserControlledCharacter character, Fixture f2, Contact contact)
        {
            return false;
        }

        public override bool OnCollidedWith(Fixture f, Entity entity, Fixture eFix, Contact info)
        {
            if (info.Enabled && !(entity is UserControlledCharacter || 
                entity is Trigger || entity is Ladder || entity is Torch || 
                entity is TutorialTrigger || entity is NonPlayerCharacter))
            {
                Dead = true;

                if (entity is Enemy)
                {
                    ((Enemy)entity).Health -= 7;
                }
            }
            else
            {
                return false;
            }
            return info.Enabled;
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
