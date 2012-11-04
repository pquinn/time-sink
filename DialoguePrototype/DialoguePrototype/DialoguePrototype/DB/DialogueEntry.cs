using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialoguePrototype
{
    public class DialogueEntry
    {
        #region Fields
        public String id { get; set; }
        public String speaker { get; set; }
        public String body { get; set; }
        public Boolean responseRequired { get; set; }
        #endregion

        public DialogueEntry(String id, String speaker, String body, Boolean responseRequired)
        {
            this.id = id;
            this.speaker = speaker;
            this.body = body;
            this.responseRequired = responseRequired;
        }

        public override string ToString()
        {
            return this.speaker + "\n" + this.body;
        }
    }
}
