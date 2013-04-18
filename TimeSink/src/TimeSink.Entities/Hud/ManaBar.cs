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
        const string TEXTURE = "Textures/HUD/ManaBar_overlay";
        const string OUTLINE = "Textures/HUD/HealthBar_outline";
        const string SPLATTER = "Textures/HUD/ManaBar_splatter";
        const string EDITOR_NAME = "Mana Bar";
        const int WIDTH = 300;
        const int HEIGHT = 50;
        private float scale = 1.0f;
        private float SCALE_DOWN = .5f;

        public ManaBar()
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
                        Position = new Vector2((WIDTH / 2) * width, 1.5f * HEIGHT * SCALE_DOWN),
                        Scale = new Vector2(width, SCALE_DOWN),
                        RenderLayer = RenderLayer.UI
                    },
                    new BasicRendering(SPLATTER)
                    {
                        Position = new Vector2((WIDTH / 2) * SCALE_DOWN, 1.5f * HEIGHT * SCALE_DOWN),
                        Scale = new Vector2(SCALE_DOWN, SCALE_DOWN),
                        RenderLayer = RenderLayer.UI
                    },
                    new BasicRendering(OUTLINE)
                    {
                        Position = new Vector2((WIDTH / 2) * SCALE_DOWN, 1.5f * HEIGHT * SCALE_DOWN),
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

        public void UpdateMana(UserControlledCharacter c)
        {
            scale = c.Mana / 100;
        }
    }
}
