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
using WPFFolderBrowser;
using Editor;
using System.IO;
using TimeSink.Engine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Level.xaml
    /// </summary>
    public partial class Level : UserControl
    {
        private bool isLoaded;

        public Level()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Level_Loaded);
        }

        void Level_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                var mainWindow = this.TryFindParent<MainWindow>();
                Game = mainWindow.editor.Game;

                isLoaded = true;
            }
        }

        public EditorGame Game { get; set; }

        private void Background_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Midground_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new WPFFolderBrowserDialog();
            
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                var relativePath = dlg.FileName.Substring(dlg.FileName.LastIndexOf("Content\\") + 8);
                midground_txt.Text = relativePath;
                LoadMidground(relativePath);
            }
        }

        private void Foreground_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadMidground(string path)
        {
            var textures = Game.Content.LoadFolder<Texture2D>(path);

            if (textures.Any())
            {
                var first = textures.First();
                var height = first.Value.Height;
                var width = first.Value.Width;

                foreach (var texture in textures)
                {
                    var location = texture.Key.Split('@')[1];
                    var x = Int32.Parse(location[1].ToString());
                    var y = Int32.Parse(location[0].ToString());

                    Game.LevelManager.RegisterMidground(
                        new Tile(
                            texture.Key,
                            new Vector2(x * width, y * height),
                            0, Vector2.One));
                }
            }
        }
    }
}
