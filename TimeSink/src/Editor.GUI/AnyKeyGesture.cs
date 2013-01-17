using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TimeSink.Editor.GUI
{
    public class AnyKeyGesture : KeyGesture
    {
        public AnyKeyGesture(Key key)
            :base(key)
        {
        }

        public Key Key
        {
            get;
            protected set;
        }

        ///
        /// When overridden in a derived class, determines whether the specified matches the input associated with the specified object.
        ///
        /// The target of the command.
        /// The input event data to compare this gesture to.
        ///
        /// true if the gesture matches the input; otherwise, false.
        ///
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            KeyEventArgs args = inputEventArgs as KeyEventArgs;
            return (Key == args.Key);
        }

    }


}
