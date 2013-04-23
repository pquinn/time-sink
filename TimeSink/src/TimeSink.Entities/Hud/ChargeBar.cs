using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Hud
{
    class ChargeBar : IRenderable
    {
        private const string OUTLINE = "Textures/HUD/ChargeBar";
        private const string COLOR = "Textures/HUD/ChargeBar_color";
        const string EDITOR_NAME = "Charge Bar";
        private const float SCALE_DOWN = 1.0f;
        private float scale = 1.0f;

        public Vector2 Position { get; set; }

        public ChargeBar(Vector2 position)
        {
            Position = position;
        }

        public List<IRendering> Renderings
        {
            get
            {
                var width = scale * SCALE_DOWN;
                return new List<IRendering>()
            {
                new BasicRendering(COLOR)
                {
                    Position = PhysicsConstants.MetersToPixels(Position) - new Vector2(0, 100),
                    Scale = new Vector2(width, SCALE_DOWN),
                    RenderLayer = RenderLayer.Foreground
                },
                new BasicRendering(OUTLINE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position) - new Vector2(0, 100),
                    Scale = new Vector2(SCALE_DOWN, SCALE_DOWN),
                    RenderLayer = RenderLayer.Foreground
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
            get { return new BasicRendering(COLOR); }
        }

        public void UpdateProgress(float currentProgress, float maxProgress)
        {
            scale = currentProgress / maxProgress;
        }
    }
}
