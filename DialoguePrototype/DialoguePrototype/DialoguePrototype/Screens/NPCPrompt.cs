using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialoguePrototype
{
    class NPCPrompt
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
        /// The path of the animation that's associated with this prompt.
        /// </summary>
        AnimationAction animationPath { get; set; }

        /// <summary>
        /// The path of the sound that's associated with this prompt.
        /// </summary>
        SoundAction soundPath { get; set; }

        /// <summary>
        /// The path of the quest that's associated with this prompt.
        /// </summary>
        QuestAction questPath { get; set; }

        /// <summary>
        /// A boolean representing whether or not the prompt needs a response.
        /// </summary>
        bool responseRequired { get; set; }
        
        /// <summary>
        /// The text representing how the User should proceed, should there be no
        /// responses
        /// </summary>
        const string usageText = "\n{enter}...";

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">the GUID of this prompt in the database.</param>
        /// <param name="speaker">a String representing the name of the speaker</param>
        /// <param name="body">a String representing the body of this prompt</param>
        /// <param name="animationPath">a String representing the path of the animation for this prompt</param>
        /// <param name="sountPath">a String representing the path of the sound for this prompt</param>
        /// <param name="questPath">a String representing the path of the quest for this prompt</param>
        /// <param name="responseRequired">whether or not the prompt has responses</param>
        public NPCPrompt(Guid id, String speaker, String body, String animationPath, String soundPath, String questPath, Boolean responseRequired)
        {
            this.id = id;
            this.speaker = speaker;
            this.body = body;
            this.animationPath = animationPath;
            this.soundPath = soundPath;
            this.questPath = questPath;
            this.responseRequired = responseRequired;
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
