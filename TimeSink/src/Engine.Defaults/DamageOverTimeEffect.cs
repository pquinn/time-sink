using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Defaults
{
    public class DamageOverTimeEffect
    {
        private float timeSpan { get; set; }
        private float totalDamage { get; set; }
        private float timeApplied { get; set; }
        private float damagePerTick { get; set; }

        public bool Active { get; set; }

        private bool finished;
        public bool Finished
        {
            get
            {
                return timeApplied >= timeSpan || finished;
            }
            set
            {
                finished = value;
            }
        }

        public DamageOverTimeEffect(float timeSpan, float totalDamage)
        {
            this.timeSpan = timeSpan;
            this.totalDamage = totalDamage;
            this.damagePerTick = (int)(totalDamage / timeSpan);
            this.timeApplied = 0f;
            this.Active = false;
        }

        public DamageOverTimeEffect(float damagePerTick)
        {
            this.timeSpan = Single.PositiveInfinity;
            this.totalDamage = timeSpan * damagePerTick;
            this.damagePerTick = damagePerTick;
            this.timeApplied = 0f;
            this.Active = false;
        }

        public float Tick(GameTime gameTime)
        {
            if (Single.IsPositiveInfinity(timeSpan))
            {
                return damagePerTick;
            }
            else
            {
                float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeApplied += time;
                return totalDamage * time / timeSpan;
            }
        }
    }
}
