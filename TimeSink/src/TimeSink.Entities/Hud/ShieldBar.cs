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
        const string TEXTURE = "Textures/HUD/ShieldBar";
        const string EDITOR_NAME = "Shield Bar";
        const int WIDTH = 300;
        const int HEIGHT = 100;
        private float scale = 1.0f;

        public ShieldBar()
        {
        }

        public List<IRendering> Renderings
        {
            get
            {
                var width = scale * .65f;
                return new List<IRendering>()
                {
                    new BasicRendering(TEXTURE)
                    {
                        Position = new Vector2(WIDTH * width *.5f, HEIGHT * .6f),
                        Scale = new Vector2(width, .4f),
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
            scale = c.Shield / 50;
        }
    }
}
