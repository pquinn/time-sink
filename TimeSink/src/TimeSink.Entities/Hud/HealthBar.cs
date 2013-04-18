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
        const int HEIGHT = 50;
        private float scale = 1.0f;
        private float SCALE_DOWN = .5f;

        public HealthBar()
        {
        }

        public List<IRendering> Renderings
        {
            get
            {
                var width = scale * SCALE_DOWN;
                return new List<IRendering>()
                {
                    new BasicRendering(TEXTURE)
                    {
                        Position = new Vector2((WIDTH / 2) * width, (HEIGHT * SCALE_DOWN) / 2 ),
                        Scale = new Vector2(width, SCALE_DOWN),
                        RenderLayer = RenderLayer.UI
                    },
                    new BasicRendering(SPLATTER)
                    {
                        Position = new Vector2((WIDTH / 2) * SCALE_DOWN, (HEIGHT * SCALE_DOWN) / 2 ),
                        Scale = new Vector2(SCALE_DOWN, SCALE_DOWN),
                        RenderLayer = RenderLayer.UI
                    },
                    new BasicRendering(OUTLINE)
                    {
                        Position = new Vector2((WIDTH / 2) * SCALE_DOWN, (HEIGHT * SCALE_DOWN) / 2 ),
                        Scale = new Vector2(SCALE_DOWN, SCALE_DOWN),
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
