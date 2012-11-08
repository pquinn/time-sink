using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialoguePrototype
{
    public class SoundAction : IDialogueAction
    {
        bool executed { get; set; }
        string path { get; set; }

        public SoundAction(String path)
        {
            this.executed = false;
            this.path = path;
        }

        public void ExecuteAction()
        {
            if (path == null) return;

            if (!executed)
            {
                Console.WriteLine("Played sound!: " + path);
            }
        }
    }
}
