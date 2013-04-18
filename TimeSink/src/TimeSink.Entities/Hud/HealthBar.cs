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
        const string TEXTURE = "Textures/HUD/HealthBar_overlay";
        const string OUTLINE = "Textures/HUD/HealthBar_outline";
        const string SPLATTER = "Textures/HUD/HealthBar_splatter";
        const string EDITOR_NAME = "Health Bar";
        const int WIDTH = 300;
        const int HEIGHT = 100;
        private float scale = 1.0f;

        public HealthBar()
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
                        Position = new Vector2(WIDTH * width *.5f, HEIGHT * .2f),
                        Scale = new Vector2(width, .4f),
                        RenderLayer = RenderLayer.UI
                    },
                    new BasicRendering(SPLATTER)
                    {
                        Position = new Vector2(WIDTH / 2, HEIGHT / 5),
                        Scale = new Vector2(.65f, 4f),
                        RenderLayer = RenderLayer.UI
                    },
                    new BasicRendering(OUTLINE)
                    {
                        Position = new Vector2(WIDTH / 2, HEIGHT / 5),
                        Scale = new Vector2(.65f, 4f),
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
