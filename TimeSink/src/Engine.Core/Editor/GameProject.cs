using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core.Editor
{
    public class GameProject
    {
        public GameProject(string projectName)
        {
            ProjectName = projectName;
        }

        public string ProjectName { get; set; }
    }
}
