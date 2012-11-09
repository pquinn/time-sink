﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeSink.Engine.Core
{
    /// <summary>
    /// All constants related to the cache can be found here.  All caching keys are set
    /// to constant variables.  This greatly increasing the ability to refactor the application
    /// should a key change.
    /// </summary>
    public static class Constants
    {
        public const int SCREEN_X = 800;
        public const int SCREEN_Y = 480;
    }
}