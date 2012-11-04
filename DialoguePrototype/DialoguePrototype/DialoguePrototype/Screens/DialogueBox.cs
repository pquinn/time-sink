using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DialoguePrototype
{
    class DialogueBox : MessageBoxScreen
    {
        #region Fields
        public Boolean responseRequired { get; set; }
        DialogueEntry dialogueEntry { get; set; }
        #endregion

        public DialogueBox(String id)
            : base(null, false, true)
        {
            this.dialogueEntry = FindDialogue(id);
            this.message = FindDialogue(id).ToString();
            this.responseRequired = dialogueEntry.responseRequired;
            IsPopup = false;
        }

        public DialogueBox(Guid id)
            : base(null, false, true)
        {
            this.dialogueEntry = FindDialogue(id.ToString());
            this.message = FindDialogue(id.ToString()).ToString();
            this.responseRequired = dialogueEntry.responseRequired;
            IsPopup = false;
        }

        public DialogueBox(DialogueEntry entry) : base(null, false, true)
        {
            this.dialogueEntry = entry;
            this.message = entry.ToString();
            this.responseRequired = entry.responseRequired;
        }

        public DialogueEntry FindDialogue(String id)
        {
            try
            {
                DataTable entry;
                String query = "select speaker \"speaker\", entry \"entry\", ";
                query += "response_required \"response\" ";
                query += "from Prompt where id = " + id + ";";
                entry = StarterGame.Instance.database.GetDataTable(query);
                DataRow result = entry.Rows[0];
                String speaker = (String)result["speaker"];
                String body = (String)result["entry"];
                Boolean responseRequired = (Boolean)result["response"];
                DialogueEntry dialogueEntry = new DialogueEntry(id, speaker, body, responseRequired);
                return dialogueEntry;

            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new DialogueEntry(id, "error", error, false);
            }
        }

        public static DialogueBox InitializeDialogueBox()
        {
            DialogueBox openingPrompt = new DialogueBox("12345678900");
            if (openingPrompt.responseRequired)
            {
                openingPrompt.Accepted += openingPrompt.AdvanceDialogueBox;
            }
            return openingPrompt;
        }

        public void AdvanceDialogueBox(object sender, PlayerIndexEventArgs e)
        {
            DialogueEntry nextEntry = FindResponses(this.dialogueEntry.id)[0];
            this.dialogueEntry = nextEntry;
            DialogueBox nextDialogueBox = new DialogueBox(nextEntry);
            if (nextDialogueBox.responseRequired)
            {
                nextDialogueBox.Accepted += AdvanceDialogueBox;
            }
            ScreenManager.AddScreen(nextDialogueBox, e.PlayerIndex);
        }

        public List<DialogueEntry> FindResponses(String from)
        {
            List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();
            try
            {
                DataTable entry;
                String query = "select toID \"to\" ";
                query += "from Response_Map where fromID = " + from + ";";
                entry = StarterGame.Instance.database.GetDataTable(query);
                Console.WriteLine("executed query: " + query);
                Console.WriteLine("found results: " + entry.Rows.Count);
                foreach (DataRow r in entry.Rows)
                {
                    dialogueEntries.Add(FindDialogue(r["to"].ToString()));
                }
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                dialogueEntries.Add(new DialogueEntry(from, "error", error, false));
            }

            return dialogueEntries;
        }
    }
}
