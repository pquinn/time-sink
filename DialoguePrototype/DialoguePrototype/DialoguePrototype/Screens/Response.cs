#region File Description
//-----------------------------------------------------------------------------
// Response.cs
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
#endregion

namespace DialoguePrototype
{
    /// <summary>
    /// Holds the data for a player response in the Dialogue engine
    /// </summary>
    class Response
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        /// <summary>
        /// The GUID of the entry that this response leads to
        /// </summary>
        Guid nextEntry;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

        /// <summary>
        /// The text that will be appended to this response if it's selected.
        /// </summary>
        const string usageText = " {enter}";

        /// <summary>
        /// The text that will be displayed if the user has this entry currently selected.
        /// </summary>
        string selectedText;

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets or sets the selected version of the text of this response.
        /// </summary>
        public String SelectedText
        {
            get { return selectedText; }
            set { selectedText = value; }
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets the GUID of the following NPC prompt.
        /// </summary>
        public Guid NextEntry
        {
            get { return nextEntry; }
        }

        #endregion


        #region Events

        /// <summary>
        /// Event raised when this response is selected.
        /// </summary>
        public event EventHandler<ResponseEventArgs> Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, new ResponseEventArgs(nextEntry));
        }

        #endregion


        #region Initialization

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public Response(string text, Guid nextEntry)
        {
            this.text = text;
            this.nextEntry = nextEntry;
            this.selectedText = text + usageText;
        }


        #endregion


        #region Update and Draw

        /// <summary>
        /// Updates this Response.
        /// </summary>
        public virtual void Update(DialogueScreen screen, bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws this Response. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(DialogueScreen screen, bool isSelected, GameTime gameTime)
        {

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;
            String displayText = isSelected ? selectedText : text;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            // float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1; // +pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, displayText, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Queries how much space this response requires.
        /// </summary>
        public virtual int GetHeight(DialogueScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }


        /// <summary>
        /// Queries how wide the response is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(DialogueScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }


        #endregion
    }
}
