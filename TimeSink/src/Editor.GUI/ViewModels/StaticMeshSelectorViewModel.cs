using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;

namespace TimeSink.Editor.GUI.ViewModels
{
    public class StaticMeshSelectorViewModel : PopUpViewModel 
    {
        #region Fields

        InMemoryResourceCache<Texture2D> cache;
        List<string> textureKeys;
        int selectedTextureKey = -1;

        Action<string, bool> invokeCancel;

        #endregion // Fields

        #region Constructor

        public StaticMeshSelectorViewModel(InMemoryResourceCache<Texture2D> cache, Action<string, bool> invokeCancel)
        {
            this.cache = cache; 
            this.textureKeys = cache.GetResources().Select(x => x.Item1).ToList();
            this.invokeCancel = invokeCancel;
        }

        public List<string> TextureKeys
        {
            get { return textureKeys; }
            set
            {
                if (value != textureKeys)
                {
                    textureKeys = value;
                    base.OnPropertyChanged("TextureKeys");
                }
            }
        }

        public int SelectedTextureKey
        {
            get { return selectedTextureKey; }
            set
            {
                if (value != selectedTextureKey)
                {
                    selectedTextureKey = value;
                    SaveCommand.CanExecute(null);
                    base.OnPropertyChanged("SelectedTextureKey");
                }
            }
        }

        #endregion // Constructor

        #region Commands

        protected override void Close(bool ok)
        {
            invokeCancel(
                SelectedTextureKey < 0 ? null : TextureKeys[SelectedTextureKey], 
                ok);
        }

        protected override bool CanSave
        {
            get { return SelectedTextureKey >= 0; }
        }

        #endregion
    }
}
