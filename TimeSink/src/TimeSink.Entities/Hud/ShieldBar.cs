using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Hud
{
    public class ShieldBar : IRenderable
    {
        const string TEXTURE = "Textures/giroux";
        const string EDITOR_NAME = "Shield Bar";
        private float scale = 1.0f;

        public ShieldBar()
        {
        }

        public List<IRendering> Renderings
        {
            get
            {
                return new List<IRendering>()
            {
                new BasicRendering(TEXTURE)
                {
                    Position = new Vector2(0, -10),
                    Scale = new Vector2(scale, 1),
                    RenderLayer = RenderLayer.UI
                }
            };
            }
        }

        public string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public IRendering Preview
        {
            get { return new BasicRendering(TEXTURE); }
        }

        public void UpdateHealth(UserControlledCharacter c)
        {
            scale = c.Shield / 100;
        }
    }
}
