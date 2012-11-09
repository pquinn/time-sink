using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Sound
{
    public class SoundObject
    {
        #region Fields
        //Location of the object
         Vector2 position = Vector2.Zero;

        //soundEffect to be played
         SoundEffect sound;

        //Does this sound change based off distance?
         bool isModular;

        //Dynamic sound object for modular panning and volume
         SoundEffectInstance dynamic;

        #endregion

        #region Initialization

        //Basic Constructor
        public SoundObject(SoundEffect target)
        {
            this.sound = target;
            this.isModular = false;
            this.dynamic = sound.CreateInstance();
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

        //Update the sound object based off a given position
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

        //Play the sound for a modular SoundObject
        public void PlaySound(Vector2 targetPos)
        {
            Update(targetPos);
            dynamic.Play();
        }

        //Play the sound for a normal SoundObject
        public void PlaySound()
        {
            dynamic.Volume = 1.0f;
            dynamic.Pan = 0;
            dynamic.Play();
        }

        //Stop the sound playing
        public void StopSound()
        {
            dynamic.Stop();
        }

        //Toggle the state of the current sound playing and paused
        public void TogglePauseSound()
        {
            if (dynamic.State.Equals(SoundState.Playing))
            {
                dynamic.Pause();
            }
            else if (dynamic.State.Equals(SoundState.Paused))
            {
                dynamic.Resume();
            }
        }

        //Pan the sound to the left speaker
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

        //Pan the sound to the right speaker
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
