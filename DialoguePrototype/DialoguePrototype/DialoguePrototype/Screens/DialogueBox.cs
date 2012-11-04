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
        #endregion

        public DialogueBox(String id)
            : base()
        {
            this.message = ConstructMessage(id);
        }

        public DialogueBox(Guid id)
            : base()
        {
            this.message = ConstructMessage(id.ToString());
        }

        public String ConstructMessage(String id)
        {
            try
            {
                DataTable entry;
                String query = "select speaker \"speaker\", entry \"entry\", ";
                query += "response_required \"response\" ";
                query += "from Prompt where id = " + id + ";";
                entry = StarterGame.Instance.database.GetDataTable(query);
                DataRow result = entry.Rows[0];
                String message = result["speaker"].ToString() + "\n";
                message += result["entry"].ToString();
                Console.WriteLine(message);
                return message;

            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n\n";
                error += e.Message.ToString() + "\n\n";
                Console.WriteLine(error);
                return error;
            }



        }
    }
}
