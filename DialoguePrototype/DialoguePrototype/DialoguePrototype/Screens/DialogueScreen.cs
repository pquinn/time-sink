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

        /// <summary>
        /// Horizontal and Vertical padding
        /// </summary>
        const int hPad = 32;
        const int vPad = 16;

        /// <summary>
        /// The scale of the text in the prompt.
        /// </summary>
        float scale;

        /// <summary>
        /// The index of the selected <see cref="Response"/>Response</see>.
        /// </summary>
        int selectedEntry = 0;

        /// <summary>
        /// Texture for the background rectangle of the <see cref="NPCPrompt"/>NPCPrompt</see>.
        /// </summary>
        Texture2D gradientTexture;

        /// <summary>
        /// The current <see cref="NPCPrompt"/>NPCPrompt</see> that will be displayed.
        /// </summary>
        NPCPrompt prompt;

        /// <summary>
        /// The list of possible player <see cref="Response"/>Responses</see>.
        /// </summary>
        List<Response> responses;

        /// <summary>
        /// The actions that are available to the user while this screen is active.
        /// </summary>
        InputAction upAction;
        InputAction downAction;
        InputAction selectAction;
        InputAction pauseAction;

        #endregion


        #region Events

        /// <summary>
        /// The event representing the end of a dialogue sequence when the NPCPrompt
        /// has no more responses.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Finished;

        #endregion


        #region Properties

        /// <summary>
        /// Gets the <see cref="NPCPrompt"/>NPCPrompt</see> contained in this DialogueScreen.
        /// </summary>
        public NPCPrompt Prompt
        {
            get { return prompt; }
        }

        /// <summary>
        /// Gets or Sets the list of possible player <see cref="Response"/>Responses</see>.
        /// </summary>
        public List<Response> Responses
        {
            get { return responses; }
            set { responses = value; }
        }

        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The GUID of the <see cref="NPCPrompt"/>NPCPrompt</see> in the database.</param>
        public DialogueScreen(Guid id)
            : base()
        {
            this.scale = 0.75f;
            this.prompt = FindPrompt(id);
            this.responses = new List<Response>();
            IsPopup = false;
            InitializeActions();
        }

        /// <summary>
        /// Adds the appropriate input to the corresponding user actions.
        /// </summary>
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

        /// <summary>
        /// Static method for creating a DialogueScreen from the gameplay screen.
        /// </summary>
        /// <param name="id">The GUID of the <see cref="NPCPrompt"/>NPCPrompt</see> in the database.</param>
        /// <returns>A new instance of a DialogueScreen</returns>
        public static DialogueScreen InitializeDialogueBox(Guid id)
        {
            DialogueScreen openingPrompt = new DialogueScreen(id);
            if (openingPrompt.Prompt.ResponseRequired)
            {
                openingPrompt.Responses = openingPrompt.FindResponses(id);
            }
            else
            {
                openingPrompt.Prompt.IncludeUsageText();
            }

            foreach (IDialogueAction action in openingPrompt.Prompt.PromptActions)
            {
                action.ExecuteAction();
            }
            return openingPrompt;
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent DialogueScreen tries to load this same content,
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

        /// <summary>
        /// Retrieves the appropriate <see cref="NPCPrompt"/>NPCPrompt</see> from the database.
        /// </summary>
        /// <param name="id">The GUID of the <see cref="NPCPrompt"/>NPCPrompt</see></param>
        /// <returns>The <see cref="NPCPrompt"/>NPCPrompt</see></returns>
        private NPCPrompt FindPrompt(Guid id)
        {
            List<IDialogueAction> promptActions = new List<IDialogueAction>();
            try
            {
                DataTable entry;
                String query = "select speaker \"speaker\", entry \"entry\", ";
                query += "animation \"animation\", sound \"sound\", quest \"quest\", ";
                query += "response_required \"response\" ";
                query += "from Prompt where id = \"" + id.ToString() + "\";";
                entry = StarterGame.Instance.database.GetDataTable(query);
                // only take the first result (there should only be one anyway)
                DataRow result = entry.Rows[0];
                String speaker = (String)result["speaker"];
                String body = (String)result["entry"];

                if (!DBNull.Value.Equals(result["animation"]))
                {
                    promptActions.Add(new AnimationAction((String)result["animation"]));
                }

                if (!DBNull.Value.Equals(result["sound"]))
                {
                    promptActions.Add(new SoundAction((String)result["sound"]));
                }

                if (!DBNull.Value.Equals(result["quest"]))
                {
                    promptActions.Add(new QuestAction((String)result["quest"]));
                }

                Boolean responseRequired = (Boolean)result["response"];
                NPCPrompt prompt = new NPCPrompt(id, speaker, body, promptActions, responseRequired);
                return prompt;

            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new NPCPrompt(id, "error", error, promptActions, false);
            }
        }

        /// <summary>
        /// Finds the appropriate <see cref="Response"/>Response</see> in the database. 
        /// </summary>
        /// <param name="id">the GUID of the <see cref="Response"/>Response</see></param>
        /// <returns>the <see cref="Response"/>Response</see> object</returns>
        private Response FindResponse(Guid id)
        {
            try
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
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new Response("error: " + error, new Guid());
            }
        }

        /// <summary>
        /// Finds the <see cref="Response"/>Responses</see> to the <see cref="NPCPrompt"/>Prompt</see>
        /// based on the mapping in the Response_Map table
        /// </summary>
        /// <param name="id">the GUID of the <see cref="NPCPrompt"/>Prompt</see></param>
        /// <returns>the list of <see cref="Response"/>Responses</see></returns>
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
        /// or pausing the <see cref="DialogueScreen"/>DialogueScreen</see>.
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
        /// Handler for when the user has chosen a response.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            responses[entryIndex].OnSelectEntry();
        }

        /// <summary>
        /// Event handler for when a <see cref="Response"/>Response</see> is selected.
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
        /// Updates the screen.
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
        /// Allows the screen the chance to position the responses. By default
        /// all repsonses are lined up in a vertical list, on the left side of the screen.
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
        /// Draws the screen.
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
                        prompt.ToString(),
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


        /// <summary>
        /// Wraps the text based on the line length.
        /// </summary>
        /// <param name="spriteFont">the font</param>
        /// <param name="text">the text to be wrapped</param>
        /// <param name="maxLineWidth">the max width of the line</param>
        /// <returns>the text with the line breaks in it</returns>
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
