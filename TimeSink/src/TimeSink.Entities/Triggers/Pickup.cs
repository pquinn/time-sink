using Engine.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Entities.Actions;
using TimeSink.Entities.Enemies;
using TimeSink.Engine.Core.Collisions;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Triggers
{
    public class Pickup : Trigger
    {
        const String EDITOR_NAME = "Pickup";
        const String HEALTH_TEXTURE = "Textures/Objects/Health_Pickup";
        const String MANA_TEXTURE = "Textures/Objects/Mana_Pickup";
        private static readonly Guid guid = new Guid("ae305111-8ab2-4e54-b737-b932a1d5d127");

        public Pickup(Vector2 position, DropType dropType, int amount)
            :base(position, 30, 30)
        {
            Position = position;
            DropType = dropType;
            Amount = amount;
        }

        public DropType DropType { get; set; }
        public int Amount { get; set; }

        protected override void RegisterCollisions()
        {
            Physics.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);
        }

        public bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            switch (DropType)
            {
                case DropType.Health:
                    c.Health = Math.Min(UserControlledCharacter.MAX_HEALTH, c.Health + Amount);
                    break;
                case DropType.Mana:
                    c.Mana = Math.Min(UserControlledCharacter.MAX_MANA, c.Mana + Amount);
                    break;
            }

            Engine.LevelManager.UnregisterEntity(this);

            return true;
        }

        public override string EditorName
        {
            get { return EditorName; }
        }

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

        public override List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
                {
                    new BasicRendering(DropType == DropType.Health ? HEALTH_TEXTURE : MANA_TEXTURE)
                    {
                        Position = Position,
                    }
                };
            }
        }
    }
}
