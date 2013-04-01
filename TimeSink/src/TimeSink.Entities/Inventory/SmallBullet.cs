﻿using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Entities.Enemies
{
    public class EnergyBullet : LargeBullet
    {
        protected const string TEXTURE = "Textures/giroux";

        public EnergyBullet(Vector2 position, int width, int height, Vector2 velocity)
            : base(position, width, height, velocity)
        {
        }
        
        public override bool OnCollidedWith(Fixture f1, UserControlledCharacter character, Fixture f2, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            return false;
        }
    }
}
