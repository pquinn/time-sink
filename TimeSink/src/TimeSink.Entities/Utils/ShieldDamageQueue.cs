using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Defaults;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Utils
{
    class ShieldDamageQueue
    {
        IHaveShield target;

        public ShieldDamageQueue(IHaveShield target)
        {
            this.target = target;
        }

        class ShieldDamage
        {
            private float RechargeRemaining;

            public ShieldDamage(float amt)
            {
                RechargeRemaining = amt;
            }

            private float rate = 5;
            private float delay = 3000;

            public float Recharge(float ms)
            {
                delay -= ms;
                if (delay < 0)
                {
                    var amount = Math.Min(-delay * rate, RechargeRemaining);
                    RechargeRemaining -= amount;
                    delay = 0;
                    return amount;
                }
                return 0;
            }

            public bool Done
            {
                get { return RechargeRemaining == 0; }
            }
        }

        HashSet<ShieldDamage> damage = new HashSet<ShieldDamage>();

        public void Update(GameTime time)
        {
            var tick = (float)time.ElapsedGameTime.TotalMilliseconds;
            foreach (var d in damage)
            {
                target.Shield += d.Recharge(tick);
            }
            damage.RemoveWhere(x => x.Done);
        }

        public void TakeDamage(float amt)
        {
            target.Shield -= amt;
            damage.Add(new ShieldDamage(amt));
        }
    }
}
