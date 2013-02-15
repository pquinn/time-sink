using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Scripting
{
    public class ScriptManager 
    {
        public ScriptManager(ScriptEngine scriptEngine)
        {
            ScriptEngine = scriptEngine;
        }

        internal ScriptEngine ScriptEngine { get; set; }

        internal string ScriptToInvoke { get; set; }

        public Dictionary<string, Func<ScriptEngine>> Scripts { get; set; }

        public void InvokeScript(string scriptName)
        {
            ScriptToInvoke = scriptName;
        }
    }
}
