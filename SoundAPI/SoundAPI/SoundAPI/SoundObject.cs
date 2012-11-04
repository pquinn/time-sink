using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundAPI
{
    class SoundObject
    {
        #region Fields
        //Location of the object
         Vector2 position = Vector2.Zero;

        //soundEffect to be played
         SoundEffect sound;

        //Does this sound change based off distance?
         bool isModular;

        //Is our sound playing
         bool isPlaying;

         SoundEffectInstance dynamic;


        #endregion

        #region Initialization

        //Basic Constructor
        public SoundObject(SoundEffect target)
        {
            this.sound = target;
            this.isModular = false;
        }

        //Modular Constructor
        public SoundObject(SoundEffect target, Vector2 posn)
        {
            this.sound = target;
            this.position = posn;
            this.isModular = true;
            this.dynamic = sound.CreateInstance();
        }

        //Accessors
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsModular
        {
            get { return isModular; }
        }

        public SoundEffect Sound
        {
            get { return sound; }
            set { sound = value; }
        }

        public SoundEffectInstance Dynamic
        {
            get { return dynamic; }
        }

        #endregion

        #region Methods

        public void Update(Vector2 targetPos)
        {
            float distance = Math.Abs(position.X - targetPos.X);

            if(isModular)
            {
               if (position.X < targetPos.X)
                {
                    dynamic.Volume = 1 / distance;
                    dynamic.Pan = 0 - (1 - (1 / distance));
               }
               if (position.X > targetPos.X)
               {
                   dynamic.Volume = 1 / distance;
                   dynamic.Pan = 0 + (1 - (1 / distance));
               }
            }
        }

        public void PlaySound(Vector2 targetPos)
        {
            if (isModular)
            {
                Update(targetPos);
                dynamic.Play();
            }
            else
            {
                sound.Play();
            }
         
        }

        public void PanLeft()
        {
            if (dynamic.Pan <= -0.9f)
            {
                dynamic.Pan = -1.0f;
            }
            else
            {
                dynamic.Pan -= .1f;
            }
        }

        public void PanRight()
        {
            if (dynamic.Pan >= 0.9f)
            {
                dynamic.Pan = 1.0f;
            }
            else
            {
                dynamic.Pan += .1f;
            }
        }

        #endregion

    }
}
