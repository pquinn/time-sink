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

namespace TimeSink.Engine.Core
{
    public abstract class Entity
        : ICollideable, IRenderable, IKeyboardControllable
    {
        public virtual void Update(GameTime time, EngineGame world) { }

        public abstract IRendering Rendering
        {
            get;
        }

        public bool Dead { get; set; }

        public abstract void HandleKeyboardInput(GameTime gameTime, EngineGame world);

        public abstract void Load(EngineGame engineGame);

        public abstract string EditorName { get; }

        public virtual string EditorPreview { get { return null; } }

        public abstract void InitializePhysics(World world);

        public abstract List<Fixture> CollisionGeometry
        {
            get;
        }
    }
}