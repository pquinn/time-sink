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
using TimeSink.Entities;

namespace TimeSink.Engine.Core
{
    public abstract class Entity
        : IPhysicsEnabledBody, IRenderable, IEditorPreviewable, IKeyboardControllable
    {
        internal void Update(GameTime time, EngineGame world)
        {
            OnUpdate(time, world);
            
            if (Physics != null)
            {
                PreviousPosition = new Vector2(Position.X, Position.Y);
            }
        }

        public virtual void OnUpdate(GameTime time, EngineGame world) { }

        [SerializableField]
        public bool Dead { get; set; }

        public virtual void HandleKeyboardInput(GameTime gameTime, EngineGame world) { }

        public virtual void Load(IComponentContext engineRegistrations) { }

        public abstract string EditorName { get; }

        public virtual void InitializePhysics(bool force, IComponentContext engineRegistrations) { }

        public abstract Guid Id { get; set; }

        [EditableField("Instance Id")]
        [SerializableField]
        public string InstanceId { get; set; }

        protected Vector2 position;

        [EditableField("Position")]
        [SerializableField]
        public Vector2 Position
        {
            get 
            {
                if (Physics != null)
                    return Physics.Position;
                return position; 
            }
            set 
            { 
                position = value;
                if (Physics != null)
                    Physics.Position = position;
            }
        }

        [XmlIgnore]
        public bool TouchingGround { get; set; }

        [XmlIgnore]
        public virtual IMenuItem InventoryItem { get; set; }

        [XmlIgnore]
        public Body Physics { get; protected set; }

        [XmlIgnore]
        public virtual int Width { get; set; }

        [XmlIgnore]
        public virtual int Height { get; set; }

      //  [XmlIgnore]
      ////  public virtual bool InHold { get; set; }

      //  [XmlIgnore]
      //  public virtual Vector2 Direction { get; set; }

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

        [XmlIgnore]
        public Vector2? PreviousPosition { get; private set; }

        public virtual void DestroyPhysics() { }
    }
}