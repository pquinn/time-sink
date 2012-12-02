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
using TimeSink.Engine.Core.States;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Level.xaml
    /// </summary>
    public partial class LevelProperties : UserControl
    {
        private bool isLoaded;

        public LevelProperties()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(LevelProperties_Loaded);
        }

        public EditorGame Game { get; set; }

        void LevelProperties_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                var mainWindow = this.TryFindParent<MainWindow>();
                Game = mainWindow.editor.Game;

                Game.LevelManager.LevelLoaded += new LevelLoadedEventHandler(Level_Loaded);

                isLoaded = true;
            }
        }

        private void Level_Loaded()
        {
            spawn_txt.Text = Game.LevelManager.Level.PlayerStart.ToDisplayString();
        }

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

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Game.LevelManager.Level.PlayerStart = spawn_txt.Text.ParseVector();
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
