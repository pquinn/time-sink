#region File Description
//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace DialoguePrototype
{
    /// <summary>
    /// Custom event argument which includes the GUID
    /// of the selected response
    /// </summary>
    class ResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ResponseEventArgs(Guid nextEntry)
        {
            this.nextEntry = nextEntry;
        }


        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public Guid NextEntry
        {
            get { return nextEntry; }
        }

        Guid nextEntry;
    }
}
