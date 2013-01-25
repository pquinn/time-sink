﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Rendering;

namespace Engine.Defaults
{
   public class TutorialDisplay : IRenderable
    {
       private Vector2 position;
       private String text;

       public TutorialDisplay(String text, Vector2 position)
       {
           this.text = text;
           this.position = position;
       }

       public void FadeText()
       {
       }

        public IRendering Rendering
        {
            get { return new TextRendering(text, new Vector2(100, 100), 0, Vector2.One, Color.DarkGoldenrod); }
        }
    }
}
