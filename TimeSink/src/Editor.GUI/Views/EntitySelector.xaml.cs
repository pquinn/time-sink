using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Editor.GUI.ViewModels;
using TimeSink.Engine.Core;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for StaticMeshSelector.xaml
    /// </summary>
    public partial class EntitySelector : Window
    {
        public EntitySelector(IEnumerable<Entity> entities, InMemoryResourceCache<Texture2D> cache)
        {
            InitializeComponent();
            DataContext = new EntitySelectorViewModel(
                entities,
                cache,
                (string s, bool b) =>
                {
                    this.SelectedEntity = s;
                    this.DialogResult = b;
                });
        }

        public string SelectedEntity { get; set; }

        private void Entity_Changed(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EntitySelectorViewModel;
            preview.ChangeTextures(
                viewModel.Entities[entityList.SelectedIndex].EditorPreview);
        }
    }
}
