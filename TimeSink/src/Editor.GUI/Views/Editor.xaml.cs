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
using Autofac;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        private bool isLoaded;
        EditorGame m_game;

        bool panButtonPressed;
        bool zoomButtonPressed;
        bool selectionButtonPressed;
        bool rotationButtonPressed;
        bool scalingButtonPressed;
        bool geomButtonPressed;
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

        public void SaveAs(string fileName)
        {
            m_game.SaveAs(fileName);
        }

        void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                m_game = new EditorGame(xnaControl.Handle, (int)xnaControl.ActualWidth, (int)xnaControl.ActualHeight);
                this.TryFindParent<MainWindow>().levelControl.Game = m_game;
                isLoaded = true;
            }
        }

        private void Pan_Click(object sender, RoutedEventArgs e)
        {
            if (!selectionButtonPressed)
            {
                m_game.PanSelected();
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
                m_game.ZoomSelected();
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

        private void Entities_Click(object sender, RoutedEventArgs e)
        {
            if (!entitiesButtonPressed)
            {
                var entities = m_game.Container.Resolve<IEnumerable<Entity>>();
                entities.ForEach(x => x.InitializePhysics(m_game.Container));
                var entityWindow = new EntitySelector(entities, m_game.TextureCache);

                entityWindow.ShowDialog();

                ResetHandle();

                var viewModel = entityWindow.DataContext as EntitySelectorViewModel;
                if ((bool)entityWindow.DialogResult)
                {
                    m_game.EntitySelected(entityWindow.SelectedEntity);
                }
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
                var selectorWindow = new StaticMeshSelector(m_game.Tiles, m_game.TextureCache);

                selectorWindow.ShowDialog();

                ResetHandle();

                var viewModel = selectorWindow.DataContext as TileSelectorViewModel;
                if ((bool)selectorWindow.DialogResult)
                {
                    m_game.StaticMeshSelected(selectorWindow.SelectedKey);
                }
            }
            else
            {
                ClearButtons();
            }
        }

        private void ResetHandle()
        {
            Microsoft.Xna.Framework.Input.Mouse.WindowHandle = xnaControl.Handle;
        }

        private void Geometry_Click(object sender, RoutedEventArgs e)
        {
            if (!geomButtonPressed)
            {
                m_game.GeometrySelected();
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
            geomButtonPressed = false;
            entitiesButtonPressed = false;
            meshButtonPressed = false;
        }

        internal void Open(string fileName)
        {
            m_game.Open(fileName);
        }

        internal void New()
        {
            m_game.New();
        }
    }
}
