using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Hud
{
    public class HealthBar : IRenderable
    {
        const string TEXTURE = "Textures/HUD/HealthBarTemp";
        const string EDITOR_NAME = "Health Bar";
        private float scale = 1.0f;

        public HealthBar()
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
                    Position = new Vector2(0, 0),
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
            scale = c.Health / 100;
        }

    }
}
