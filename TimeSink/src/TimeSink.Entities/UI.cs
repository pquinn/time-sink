using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using FarseerPhysics.Dynamics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities
{
    public class UI : Entity
    {
        private static readonly Guid guid = new Guid("00000000-0000-0000-0000-000000000000");
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

        public override IRendering Preview
        {
            get { return new NullRendering(); }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return new List<Fixture>(); }
        }

        public override IRendering Rendering
        {
            get { return new NullRendering(); }
        }
    }
}
