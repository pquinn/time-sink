using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities.Hud
{
    public class ManaBar : IRenderable
    {
        const string TEXTURE = "Textures/HUD/ManaBarTemp";
        const string EDITOR_NAME = "Mana Bar";
        const int WIDTH = 300;
        const int HEIGHT = 100;
        private float scale = 1.0f;

        public ManaBar()
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
                        Position = new Vector2(WIDTH * width *.5f, HEIGHT),
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

        public void UpdateMana(UserControlledCharacter c)
        {
            scale = c.Mana / 100;
        }
    }
}
