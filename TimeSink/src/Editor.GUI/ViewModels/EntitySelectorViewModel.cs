using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core;

namespace TimeSink.Editor.GUI.ViewModels
{
    public class EntitySelectorViewModel : PopUpViewModel 
    {
        #region Fields

        InMemoryResourceCache<Texture2D> cache;
        List<string> entityKeys;
        int selectedEntity = -1;

        #endregion // Fields

        #region Constructor

        public EntitySelectorViewModel(IEnumerable<Entity> entities, InMemoryResourceCache<Texture2D> cache, Action<string, bool> invokeCancel)
            : base (invokeCancel)
        {
            this.cache = cache;
            Entities = entities.ToList();
            this.entityKeys = Entities.Select(e => e.EditorName).ToList();
        }

        public List<string> EntityKeys
        {
            get { return entityKeys; }
            set
            {
                if (value != entityKeys)
                {
                    entityKeys = value;
                    base.OnPropertyChanged("EntityKeys");
                }
            }
        }

        public List<Entity> Entities { get; set; }

        public int SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (value != selectedEntity)
                {
                    selectedEntity = value;
                    SaveCommand.CanExecute(null);
                    base.OnPropertyChanged("SelectedEntity");
                }
            }
        }

        #endregion // Constructor

        #region Commands

        protected override void Close(bool ok)
        {
            invokeCancel(
                SelectedEntity < 0 ? null : EntityKeys[SelectedEntity], 
                ok);
        }

        protected override bool CanSave
        {
            get { return SelectedEntity >= 0; }
        }

        #endregion
    }
}
