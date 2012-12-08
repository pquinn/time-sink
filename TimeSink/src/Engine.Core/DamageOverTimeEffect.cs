using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TimeSink.Engine.Core;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core
{
    public class DamageOverTimeEffect
    {
        private float timeSpan { get; set; }
        private float totalDamage { get; set; }
        private float timeApplied { get; set; }

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
            this.timeApplied = 0f;
            this.Active = false;
        }

        public DamageOverTimeEffect(float timeSpan, int damagePerTick)
        {
            this.timeSpan = timeSpan;
            this.totalDamage = timeSpan * damagePerTick;
            this.timeApplied = 0f;
            this.Active = false;
        }

        public float Tick(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeApplied += time;
            return totalDamage * time / timeSpan;
        }
    }
}
