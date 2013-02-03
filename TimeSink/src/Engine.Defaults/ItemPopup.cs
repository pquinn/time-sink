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
        const int ITEM_SIZE = 30;
        private Vector2 position;
        private string texture;
        private float DEPTH = -200;

        public ItemPopup(string texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public IRendering Rendering
        {
            get
            {
                return new BasicRendering(texture)
                {
                    Position = PhysicsConstants.MetersToPixels(position),
                    Size = new Vector2(ITEM_SIZE, ITEM_SIZE),
                    DepthWithinLayer = DEPTH
                };
            }
        }


        public string EditorName
        {
            get { throw new NotImplementedException(); }
        }

        public IRendering Preview
        {
            get { throw new NotImplementedException(); }
        }
    }
}
