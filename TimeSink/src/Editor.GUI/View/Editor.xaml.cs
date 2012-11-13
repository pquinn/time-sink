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
using TimeSink.Engine.Core.Caching;
using TimeSink.Editor.GUI.ViewModels;
using Editor;
using TimeSink.Engine.Core;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        Game1 m_game;

        bool panButtonPressed;
        bool zoomButtonPressed;
        bool selectionButtonPressed;
        bool rotationButtonPressed;
        bool scalingButtonPressed;
        bool _ButtonPressed;
        bool entitiesButtonPressed;
        bool meshButtonPressed;

        public Editor()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Editor_Loaded);
        }

        public IEnumerable<string> Textures { get; set; }

        public IEnumerable<string> SelectedTexture { get; set; }

        public void ToggleGridLines()
        {
            EditorProperties.Instance.ShowGridLines = !EditorProperties.Instance.ShowGridLines;
        }

        public void ToggleSnapping()
        {
            EditorProperties.Instance.EnableSnapping = !EditorProperties.Instance.EnableSnapping;
        }

        public void SetGridLineSize(int size)
        {
            EditorProperties.Instance.GridLineSpacing = size;
        }

        void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            m_game = new Game1(xnaControl.Handle, (int)xnaControl.ActualWidth, (int)xnaControl.ActualHeight);
        }

        private void Pan_Click(object sender, RoutedEventArgs e)
        {
            if (!selectionButtonPressed)
            {
            }
            else
            {
                ClearButtons();
            }
        }

        private void Zoom_Click(object sender, RoutedEventArgs e)
        {
            if (!selectionButtonPressed)
            {
            }
            else
            {
                ClearButtons();
            }
        }

        private void Selection_Click(object sender, RoutedEventArgs e)
        {
            if (!selectionButtonPressed)
            {
                m_game.SelectionSelected();
            }
            else
            {
                ClearButtons();
            }
        }

        private void Rotation_Click(object sender, RoutedEventArgs e)
        {
            if (!rotationButtonPressed)
            {
                m_game.RotationSelected();
            }
            else
            {
                ClearButtons();
            }
        }

        private void Scaling_Click(object sender, RoutedEventArgs e)
        {
            if (!selectionButtonPressed)
            {
                m_game.ScalingSelected();
            }
            else
            {
                ClearButtons();
            }
        }

        private void _Click(object sender, RoutedEventArgs e)
        {
            if (!selectionButtonPressed)
            {
            }
            else
            {
                ClearButtons();
            }
        }

        private void Entities_Click(object sender, RoutedEventArgs e)
        {
            if (!entitiesButtonPressed)
            {
            }
            else
            {
                ClearButtons();
            }
        }

        private void Static_Click(object sender, RoutedEventArgs e)
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
            panButtonPressed = false;
            zoomButtonPressed = false;
            selectionButtonPressed = false;
            rotationButtonPressed = false;
            scalingButtonPressed = false;
            _ButtonPressed = false;
            entitiesButtonPressed = false;
            meshButtonPressed = false;
        }
    }
}
