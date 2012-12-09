using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.StateManagement
{
    public interface IDialogueAction
    {
        /// <summary>
        /// Repsonsible for handling the action.
        /// </summary>
        void ExecuteAction();
    }
}
