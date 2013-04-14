using Engine.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Entities.Triggers
{
    public class Pickup : Trigger
    {
        const String EDITOR_NAME = "Camera_Unlock_Trigger";
        private static readonly Guid guid = new Guid("ae305111-8ab2-4e54-b737-b932a1d5d127");

        protected override void RegisterCollisions()
        {
            throw new NotImplementedException();
        }

        public override string EditorName
        {
            get { throw new NotImplementedException(); }
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
    }
}
