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
using Microsoft.Xna.Framework;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for StaticMeshSelector.xaml
    /// </summary>
    public partial class StaticMeshSelector : Window
    {
        public StaticMeshSelector(IEnumerable<string> tiles, InMemoryResourceCache<Texture2D> cache)
        {
            InitializeComponent();
            DataContext = new TileSelectorViewModel(
                tiles,
                (string s, bool b) =>
                {
                    this.SelectedKey = s;
                    this.DialogResult = b;
                });
        }

        public string SelectedKey { get; set; }

        private void Mesh_Changed(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TileSelectorViewModel;
            var textureKey = viewModel.TextureKeys[meshList.SelectedIndex];
            preview.ChangePreview(new Tile(textureKey, Vector2.Zero, 0, Vector2.One));
        }
    }
}
