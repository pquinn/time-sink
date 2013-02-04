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
        const int ITEM_SIZE = 40;
        private Vector2 position;
        private string texture;
        public Vector2 OffSet { get; set; }
        private float DEPTH = -200;


        public ItemPopup(string texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            OffSet = Vector2.Zero;
        }

        public ItemPopup(string texture, Vector2 position, Vector2 offSet)
        {
            this.texture = texture;
            this.position = position;
            OffSet = offSet;
        }

        public IRendering Rendering
        {
            get
            {
                return new BasicRendering(texture)
                {
                    Position = PhysicsConstants.MetersToPixels(position) + OffSet,
                    Size = new Vector2(ITEM_SIZE, ITEM_SIZE),
                    DepthWithinLayer = DEPTH
                };
            }
        }

        public void SetPos(Vector2 newPos)
        {
            this.position = newPos;
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
