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
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;

namespace TimeSink.Engine.Core
{
    public abstract class Entity
        : ICollideable, IRenderable, IEditorPreviewable, IKeyboardControllable
    {
        public virtual void Update(GameTime time, EngineGame world) { }

        [SerializableField]
        public bool Dead { get; set; }

        public abstract void HandleKeyboardInput(GameTime gameTime, EngineGame world);

        public abstract void Load(IComponentContext engineRegistrations);

        public abstract string EditorName { get; }

        public abstract void InitializePhysics(bool force, IComponentContext engineRegistrations);

        public abstract Guid Id { get; set; }

        [EditableField("Instance Id")]
        [SerializableField]
        public string InstanceId { get; set; }

        [EditableField("Position")]
        [SerializableField]
        public Vector2 Position
        {
            get { return Physics.Position; }
            set { Physics.Position = value; }
        }
        
        [XmlIgnore]
        public Body Physics { get; protected set; }

        [XmlIgnore]
        public abstract IRendering Preview
        {
            get;
        }

        [XmlIgnore]
        public abstract List<Fixture> CollisionGeometry
        {
            get;
        }

        [XmlIgnore]
        public abstract IRendering Rendering
        {
            get;
        }
    }
}