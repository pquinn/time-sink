using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.DB;
using TimeSink.Engine.Core.Editor;

namespace TimeSink.Engine.Core.StateManagement
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
        /// The list of actions that have to happen when this prompt is rendered.
        /// </summary>
        List<IDialogueAction> promptActions { get; set; }

        /// <summary>
        /// A boolean representing whether or not the prompt needs a response.
        /// </summary>
        bool responseRequired { get; set; }
        
        /// <summary>
        /// The text representing how the User should proceed, should there be no
        /// responses
        /// </summary>
        const string usageText = "\n{enter}...";

        public const string TABLE_NAME = "Prompt";

        List<Response> responses; 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        [EditableField("Id")]
        public Guid Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the speaker.
        /// </summary>
        [EditableField("Speaker")]
        public String Speaker
        {
            get { return speaker; }
            set { speaker = value; }
        }


        /// <summary>
        /// Gets the Body.
        /// </summary>
        [EditableField("Body Text")]
        public String Body
        {
            get { return body; }
            set { body = value; }
        }

        /// <summary>
        /// Gets the responseRequired flag.
        /// </summary>
        [EditableField("Response Required?")]
        public Boolean ResponseRequired
        {
            get { return responseRequired; }
            set { responseRequired = value; }
        }

        /// <summary>
        /// Gets the list of <see cref="IDialogueAction"/>Actions</see> associated
        /// with this prompt.
        /// </summary>
        public List<IDialogueAction> PromptActions
        {
            get { return promptActions; }
            set { promptActions = value; }
        }

        [EditableField("Responses")]
        public List<Response> Responses
        {
            get { return responses; }
            set { responses = value; }
        }

        #endregion

        public NPCPrompt() : this(Guid.Empty, String.Empty, String.Empty, new List<IDialogueAction>(), false) { }

        public NPCPrompt(Guid id, String speaker, String body, List<IDialogueAction> promptActions, Boolean responseRequired)
            : this(id, speaker, body, promptActions, responseRequired, new List<Response>())
        {
        }

        public NPCPrompt(Guid id, String speaker, String body, List<IDialogueAction> promptActions, Boolean responseRequired, List<Response> responses)
        {
            this.id = id;
            this.speaker = speaker;
            this.body = body;
            this.promptActions = promptActions;
            this.responseRequired = responseRequired;
            this.responses = responses;
        }

        public override string ToString()
        {
            return this.speaker + ":\n" + this.body;
        }

        /// <summary>
        /// Appends the usage text to the body. 
        /// Used for prompts that don't have any responses.
        /// </summary>
        internal void IncludeUsageText()
        {
            this.body = this.body + usageText;
        }
    }
}
