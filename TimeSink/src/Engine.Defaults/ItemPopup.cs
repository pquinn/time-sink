using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace Engine.Defaults
{
    public class ItemPopup : IRenderable
    {
        private Vector2 position;
        private string texture;

        public ItemPopup(string texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public IRendering Rendering
        {
            get { return new SizedRendering(texture, PhysicsConstants.MetersToPixels(position), 0, 45, 45); }
        }
    }
}
