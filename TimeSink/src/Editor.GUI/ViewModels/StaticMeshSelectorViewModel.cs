using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemoApp.ViewModel;
using System.Windows.Input;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Editor.GUI.ViewModels
{
    public class StaticMeshSelectorViewModel : ViewModelBase 
    {
        #region Fields

        RelayCommand _closeCommand;
        InMemoryResourceCache<Texture2D> cache;

        #endregion // Fields

        #region Constructor

        public StaticMeshSelectorViewModel(InMemoryResourceCache<Texture2D> cache)
        {
            this.cache = cache; 
        }

        #endregion // Constructor

        #region CloseCommand

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.OnRequestClose());

                return _closeCommand;
            }
        }

        #endregion // CloseCommand

        #region RequestClose [event]

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
