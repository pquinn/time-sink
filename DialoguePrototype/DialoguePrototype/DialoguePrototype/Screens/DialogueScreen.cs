using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace DialoguePrototype
{
    class DialogueScreen : GameScreen
    {
        #region Fields

        float scale;
        int selectedEntry = 0;

        NPCPrompt prompt;
        List<Response> responses;

        InputAction upAction;
        InputAction downAction;
        InputAction selectAction;

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

        /*
        public DialogueScreen(String id)
            : base(null, false, true)
        {
            this.scale = 0.75f;
            this.prompt = FindPrompt(id);
            this.message = FindPrompt(id).ToString();
            IsPopup = false;
        }
         * */

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

        #endregion

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
            query += "from Response_Map where fromID = \"" + id.ToString() + "\";";
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
                query += "from Response_Map where fromID = \"" + id + "\";";
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

            if (selectAction.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry);
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
    }
}
