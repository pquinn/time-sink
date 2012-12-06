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

        public bool Finished
        {
            get
            {
                return timeApplied >= timeSpan;
            }
        }

        public DamageOverTimeEffect(float timeSpan, float totalDamage)
        {
            this.timeSpan = timeSpan;
            this.totalDamage = totalDamage;
            this.timeApplied = 0f;
            this.Active = false;
        }

        public float Tick(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeApplied += time;
            float damage = (totalDamage * time / timeSpan);
           /* Console.WriteLine(String.Format("dot applied! time applied: {0}, total time: {1}, for damage: {2}", 
                timeApplied,
                timeSpan,
                damage));*/
            return damage;   
        }
    }
}
