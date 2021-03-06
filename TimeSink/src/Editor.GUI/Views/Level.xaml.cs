﻿using System;
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
using XNAControl;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Level.xaml
    /// </summary>
    public partial class LevelProperties : UserControl
    {
        private bool isLoaded;
        private MainWindow mainWindow;

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
                mainWindow = this.TryFindParent<MainWindow>();
                Game = mainWindow.editor.Game;

                Game.LevelManager.LevelLoaded += new LevelLoadedEventHandler(Level_Loaded);

                isLoaded = true;
            }
        }

        private void Level_Loaded()
        {
            background_txt.Text = Game.LevelManager.Level.BackgroundPath;
            midground_txt.Text = Game.LevelManager.Level.MidgroundPath;
            camera_txt.Text = Game.LevelManager.Level.CameraMax.ToDisplayString();
            spawn_txt.Text = Game.LevelManager.Level.DefaultStart.ToDisplayString();
            spawns_txt.Text = string.Join(";", Game.LevelManager.Level.SpawnPoints.Select(x => x.ToDisplayString()));
        }

        private void Background_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new WPFFolderBrowserDialog();

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                var relativePath = dlg.FileName.Substring(dlg.FileName.LastIndexOf("Content\\") + 8);
                background_txt.Text = relativePath;
                LoadBackground(relativePath);
            }
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
            Game.LevelManager.Level.CameraMax = camera_txt.Text.ParseVector();
            Game.LevelManager.Level.DefaultStart = spawn_txt.Text.ParseVector();
            Game.LevelManager.Level.SpawnPoints = ExtractSpawnPoints();
        }

        private List<Vector2> ExtractSpawnPoints()
        {
            var text = spawns_txt.Text;
            var points = text.Split(';');

            return points.Select(x => x.ParseVector()).ToList();
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

                    Game.LevelManager.Level.MidgroundPath = path;
                    Game.LevelManager.RegisterMidground(
                        new Tile(
                            texture.Key,
                            PhysicsConstants.PixelsToMeters(new Vector2((x * width) + (width / 2), (y * height) + (height / 2))),
                            0, Vector2.One, RenderLayer.Midground, 1));
                }
            }
        }

        private void LoadBackground(string path)
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

                    Game.LevelManager.Level.BackgroundPath = path;
                    Game.LevelManager.RegisterBackground(
                        new Tile(
                            texture.Key,
                            PhysicsConstants.PixelsToMeters(new Vector2((x * width) + (width / 2), (y * height) + (height / 2))),
                            0, Vector2.One, RenderLayer.Background, 1));
                }
            }
        }
    }
}
