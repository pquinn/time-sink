using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using FarseerPhysics.Dynamics;
using System.Xml.Serialization;
using Autofac;

namespace TimeSink.Engine.Core
{
    public abstract class Entity
        : ICollideable, IRenderable, IKeyboardControllable
    {
        public virtual void Update(GameTime time, EngineGame world) { }

        public bool Dead { get; set; }

        public abstract void HandleKeyboardInput(GameTime gameTime, EngineGame world);

        public abstract void Load(IContainer engineRegistrations);

        public abstract string EditorName { get; }

        public virtual string EditorPreview { get { return null; } }

        public abstract void InitializePhysics(IContainer engineRegistrations);

        [XmlIgnore]
        public abstract IRendering Rendering
        {
            get;
        }

        [XmlIgnore]
        public abstract List<Fixture> CollisionGeometry
        {
            get;
        }
    }
}