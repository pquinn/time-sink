using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace Engine.Defaults
{
    public class ItemPopup : IRenderable
    {
        const int ITEM_SIZE = 40;
        private Vector2 position;
        private string texture;
        public Vector2 OffSet { get; set; }
        private float DEPTH = -200;
        private IResourceCache<Texture2D> textureCache;

        public ItemPopup(string texture, Vector2 position, IResourceCache<Texture2D> textureCache)
        {
            this.texture = texture;
            this.position = position;
            this.textureCache = textureCache;            
            OffSet = Vector2.Zero;
        }

        public ItemPopup(string texture, Vector2 position, IResourceCache<Texture2D> textureCache, Vector2 offSet)
        {
            this.texture = texture;
            this.position = position;            
            this.textureCache = textureCache;    
            OffSet = offSet;
        }

        public List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>() 
                {
                    new BasicRendering(texture)
                    {
                        Position = PhysicsConstants.MetersToPixels(position) + OffSet,
                        Scale = BasicRendering.CreateScaleFromSize(ITEM_SIZE, ITEM_SIZE, texture, textureCache),
                        DepthWithinLayer = DEPTH
                    }
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
