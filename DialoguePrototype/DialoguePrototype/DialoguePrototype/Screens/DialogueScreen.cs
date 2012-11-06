using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace DialoguePrototype
{
    class DialogueScreen : GameScreen
    {
        #region Fields

        const int hPad = 32;
        const int vPad = 16;

        float scale;
        int selectedEntry = 0;
        Texture2D gradientTexture;

        NPCPrompt prompt;
        List<Response> responses;

        InputAction upAction;
        InputAction downAction;
        InputAction selectAction;
        InputAction pauseAction;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Finished;

        #endregion

        #region Properties

        public NPCPrompt Prompt
        {
            get { return prompt; }
        }

        public List<Response> Responses
        {
            get { return responses; }
            set { responses = value; }
        }

        #endregion

        #region Initialization

        public DialogueScreen(Guid id)
            : base()
        {
            this.scale = 0.75f;
            this.prompt = FindPrompt(id);
            this.responses = new List<Response>();
            IsPopup = false;
            InitializeActions();
        }

        public void InitializeActions()
        {
            upAction = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up },
                true);

            downAction = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down },
                true);

            selectAction = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Enter, Keys.Space },
                true);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }

        public static DialogueScreen InitializeDialogueBox(Guid id)
        {
            DialogueScreen openingPrompt = new DialogueScreen(id);
            if (openingPrompt.Prompt.ResponseRequired)
            {
                openingPrompt.Responses = openingPrompt.FindResponses(id);
            }
            return openingPrompt;
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ContentManager content = ScreenManager.Game.Content;
                gradientTexture = content.Load<Texture2D>("textures/gradient");
            }
        }

        #endregion

        #region Database Interactions

        private NPCPrompt FindPrompt(Guid id)
        {
            try
            {
                DataTable entry;
                String query = "select speaker \"speaker\", entry \"entry\", ";
                query += "response_required \"response\" ";
                query += "from Prompt where id = \"" + id.ToString() + "\";";
                entry = StarterGame.Instance.database.GetDataTable(query);
                // only take the first result (there should only be one anyway)
                DataRow result = entry.Rows[0];
                String speaker = (String)result["speaker"];
                String body = (String)result["entry"];
                Boolean responseRequired = (Boolean)result["response"];
                NPCPrompt prompt = new NPCPrompt(id, speaker, body, responseRequired);
                return prompt;

            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new NPCPrompt(id, "error", error, false);
            }
        }

        private Response FindResponse(Guid id)
        {
            DataTable entry;
            String query = "select entry \"entry\", ";
            query += "next_entry \"next_entry\" ";
            query += "from Response where ID = \"" + id.ToString() + "\";";
            entry = StarterGame.Instance.database.GetDataTable(query);
            // again, there should only be one result
            DataRow result = entry.Rows[0];
            String entryText = (String)result["entry"];
            Guid nextEntry = new Guid((String)result["next_entry"]);
            return new Response(entryText, nextEntry);
        }

        private List<Response> FindResponses(Guid id)
        {
            List<Response> responses = new List<Response>();
            try
            {
                DataTable entry;
                String query = "select toID \"to\" ";
                query += "from Response_Map where fromID = \"" + id.ToString() + "\";";
                entry = StarterGame.Instance.database.GetDataTable(query);
                foreach (DataRow r in entry.Rows)
                {
                    responses.Add(FindResponse(new Guid((String)r["to"])));
                }
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                responses.Add(new Response("error: " + error, new Guid()));
            }

            // attach event handlers
            foreach (Response response in responses)
            {
                response.Selected += ResponseSelected;
            }

            return responses;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            // Move to the previous menu entry?
            if (upAction.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = responses.Count - 1;
            }

            // Move to the next menu entry?
            if (downAction.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;

                if (selectedEntry >= responses.Count)
                    selectedEntry = 0;
            }

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else if (selectAction.Evaluate(input, ControllingPlayer, out player))
            {
                if (responses.Count != 0)
                {
                    OnSelectEntry(selectedEntry);
                }
                else
                {
                    if (Finished != null)
                    {
                        Finished(this, new PlayerIndexEventArgs(playerIndex));
                    }

                    ExitScreen();
                }
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            responses[entryIndex].OnSelectEntry();
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void ResponseSelected(object sender, ResponseEventArgs e)
        {
            // close the current screen and
            // InitializeDialogueBox on e.NextEntry
            ExitScreen();
            ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(e.NextEntry), null);
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < responses.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                responses[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateResponseLocations(float startY)
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            //float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, startY);

            // update each menu entry's location in turn
            for (int i = 0; i < responses.Count; i++)
            {
                Response response = responses[i];

                // each entry is to be centered horizontally
                position.X = hPad;

                // set the entry's position
                response.Position = position;

                // move down for the next entry the size of this entry
                position.Y += response.GetHeight(this);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 origin = new Vector2(0, 0);
            float titleScale = 1.0f;

            //  + " This is extra text for padding shit. Let's see what happens!",
            String tempPrompt = WrapText(font,
                        prompt.Speaker + ":\n" + prompt.Body + " This is extra text for padding shit. Let's see what happens!",
                        viewportSize.X - (hPad * 3));

            Vector2 titleSize = font.MeasureString(tempPrompt) * titleScale;

            // Draw the menu title in the top left corner of the screen
            Vector2 titlePosition = new Vector2(hPad + titleSize.X / 2, vPad + titleSize.Y / 2);
            Vector2 titleOrigin = titleSize / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;

            Color color = Color.White * TransitionAlpha;

            //draw the rectangle the length of the screen starting at the origin
            Rectangle backgroundRectangle = new Rectangle(0, 0,
                                  (int)viewportSize.X,
                                  (int)titleSize.Y + vPad * 2);

            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            spriteBatch.DrawString(font, tempPrompt, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0.0f);

            UpdateResponseLocations(titleSize.Y + vPad * 4);

            // Draw each menu entry in turn.
            for (int i = 0; i < responses.Count; i++)
            {
                Response response = responses[i];

                bool isSelected = IsActive && (i == selectedEntry);

                response.Draw(this, isSelected, gameTime);
            }

            spriteBatch.End();
        }

        private String WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            String[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
