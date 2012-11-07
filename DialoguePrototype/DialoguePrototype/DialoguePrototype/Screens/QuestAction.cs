using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialoguePrototype
{
    public class QuestAction : IDialogueAction
    {

        bool executed { get; set; }
        string path { get; set; }

        public QuestAction(String path)
        {
            this.executed = false;
            this.path = path;
        }

        public void ExecuteAction()
        {
            if (path == null) return;

            if (!executed)
            {
                Console.WriteLine("Added Quest!: " + path);
            }
        }
    }
}
