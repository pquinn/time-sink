using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TimeSink.Editor.GUI.ViewModels
{
    public class PopUpViewModel : ViewModelBase
    {
        #region Fields

        protected RelayCommand _closeCommand;
        protected RelayCommand _saveCommand;

        protected Action<string, bool> invokeCancel;

        #endregion

        public PopUpViewModel(Action<string, bool> invokeCancel)
        {
            this.invokeCancel = invokeCancel;
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => this.Close(true),
                        param => this.CanSave
                        );
                }
                return _saveCommand;
            }
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(
                        param => this.Close(false));
                }
                return _closeCommand;
            }
        }

        protected virtual void Close(bool ok)
        {
            invokeCancel(string.Empty, ok);
        }

        protected virtual bool CanSave
        {
            get { return true; }
        }
    }
}
