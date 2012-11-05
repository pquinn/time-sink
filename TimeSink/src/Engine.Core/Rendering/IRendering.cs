using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Engine.Core.Rendering
{
    public interface IRendering
    {
        void Draw(SpriteBatch spriteBatch);
    }
}
