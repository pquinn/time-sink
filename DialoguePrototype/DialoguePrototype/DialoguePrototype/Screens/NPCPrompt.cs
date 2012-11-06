using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialoguePrototype
{
    public class NPCPrompt
    {
        #region Fields

        /// <summary>
        /// The GUID of the entry.
        /// </summary>
        Guid id;

        /// <summary>
        /// The string representing the character speaking.
        /// </summary>
        string speaker { get; set; }

        /// <summary>
        /// The text representing the body of the prompt.
        /// </summary>
        string body { get; set; }

        /// <summary>
        /// A boolean representing whether or not the prompt needs a response.
        /// </summary>
        bool responseRequired { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public Guid Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the speaker.
        /// </summary>
        public String Speaker
        {
            get { return speaker; }
        }


        /// <summary>
        /// Gets the Body.
        /// </summary>
        public String Body
        {
            get { return body; }
        }

        /// <summary>
        /// Gets the responseRequired flag.
        /// </summary>
        public Boolean ResponseRequired
        {
            get { return responseRequired; }
        }

        #endregion

        public NPCPrompt(Guid id, String speaker, String body, Boolean responseRequired)
        {
            this.id = id;
            this.speaker = speaker;
            this.body = body;
            this.responseRequired = responseRequired;
        }
    }
}
