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
using TimeSink.Editor.Game;
using TimeSink.Engine.Core.Caching;
using TimeSink.Editor.GUI.ViewModels;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        Game1 m_game;
        bool meshButtonPressed;

        public Editor()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Editor_Loaded);
        }

        void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            m_game = new Game1(xnaControl.Handle, (int)xnaControl.ActualWidth, (int)xnaControl.ActualHeight);
        }

        public IEnumerable<string> Textures { get; set; }

        public IEnumerable<string> SelectedTexture { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!meshButtonPressed)
            {
                var selectorWindow = new StaticMeshSelector(m_game.TextureCache);

                selectorWindow.ShowDialog();

                var viewModel = selectorWindow.DataContext as StaticMeshSelectorViewModel;
                if ((bool)selectorWindow.DialogResult)
                {
                    m_game.StaticMeshSelected(selectorWindow.SelectedTexture);
                }
            }
            else
            {
                ClearButtons();
            }
        }

        private void ClearButtons()
        {
            meshButtonPressed = false;
        }
    }
}
