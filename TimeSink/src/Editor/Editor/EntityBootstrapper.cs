using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using TimeSink.Entities;
using TimeSink.Entities.Enemies;
using TimeSink.Entities.Weapons;
using TimeSink.Engine.Core;
using System.Reflection;
using TimeSink.Engine.Core.Editor;

namespace Editor
{
    public class EntityBootstrapper : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var assembly = typeof(WorldGeometry).Assembly;
            assembly.GetTypes().ForEach(
                t =>
                {
                    if (!t.IsAbstract && t.IsSubclassOf(typeof(Entity)) && 
                        t.GetCustomAttributes(typeof(EditorEnabledAttribute), false).Any())
                    {
                        builder.RegisterType(t).As<Entity>().WithMetadata<IEntityMetadata>(
                            m => m.For(tm => tm.Name, t.Name));
                    }
                });
        }
    }
}
