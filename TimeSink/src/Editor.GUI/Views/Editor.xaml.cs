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
    public delegate void EntityAddedEventHandler(Entity entity);
    public delegate void EntityRemovedEventHandler(Entity entity);
    public delegate void TileAddedEventHandler(Tile tile);
    public delegate void TileRemovedEventHandler(Tile tile);

    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        private bool isLoaded;

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

        public EditorGame Game { get; set; }

        public IEnumerable<string> Textures { get; set; }

        public IEnumerable<string> SelectedTexture { get; set; }

        public EntityAddedEventHandler EntityAdded { get; set; }

        public EntityRemovedEventHandler EntityRemoved { get; set; }

        public TileAddedEventHandler TileAdded { get; set; }

        public TileRemovedEventHandler TileRemoved { get; set; }

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
            Game.SaveAs(fileName);
        }

        void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                Game = new EditorGame(xnaControl.Handle, (int)xnaControl.ActualWidth, (int)xnaControl.ActualHeight);

                isLoaded = true;
            }
        }

        private void Pan_Click(object sender, RoutedEventArgs e)
        {
            if (!selectionButtonPressed)
            {
                Game.PanSelected();
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
                Game.ZoomSelected();
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
                Game.SelectionSelected();
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
                Game.RotationSelected();
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
                Game.ScalingSelected();
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
                var entities = Game.Container.Resolve<IEnumerable<Entity>>().ToList();
                entities.ForEach(
                    x =>
                    {
                        x.Load(Game.Container);
                        Game.LevelManager.RegisterEntity(x);
                    });
                var entityWindow = new EntitySelector(entities, Game.TextureCache);

                entityWindow.ShowDialog();

                if (entityWindow.SelectedEntity != null)
                {
                    entities.Remove(entityWindow.SelectedEntity);
                }
                Game.LevelManager.UnregisterEntities(entities);

                ResetHandle();

                var viewModel = entityWindow.DataContext as EntitySelectorViewModel;
                if ((bool)entityWindow.DialogResult)
                {
                    Game.EntitySelected(
                        entityWindow.SelectedEntity,
                        (entity) =>
                        {
                            var dlg = new EntityCreateWindow(entity);
                            Nullable<bool> result = dlg.ShowDialog();

                            if (result.Value && EntityAdded != null)
                            {
                                EntityAdded(entity);
                            }

                            return result.Value;
                        });
                }
                else if (entityWindow.SelectedEntity != null)
                {
                    Game.LevelManager.UnregisterEntity(entityWindow.SelectedEntity);
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
                var selectorWindow = new StaticMeshSelector(Game.Tiles, Game.TextureCache);

                selectorWindow.ShowDialog();

                ResetHandle();

                var viewModel = selectorWindow.DataContext as TileSelectorViewModel;
                if ((bool)selectorWindow.DialogResult)
                {
                    Game.StaticMeshSelected(selectorWindow.SelectedKey);
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
                var state = Game.GeometrySelected();
                standardCollisionButton.Checked += delegate
                {
                    if (standardCollisionButton.IsChecked == true)
                        state.OneWay = false;
                };
                oneWayCollisionButton.Checked += delegate
                {
                    if (oneWayCollisionButton.IsChecked == true)
                        state.OneWay = true;
                };

                //TODO: fixme
                xnaControl.GotFocus += delegate
                {
                    state.IsMouseInteractionEnabled = true;
                };
                xnaControl.LostFocus += delegate
                {
                    state.IsMouseInteractionEnabled = false;
                };

                collisionType.Visibility = Visibility.Visible;
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
            collisionType.Visibility = Visibility.Collapsed;
        }

        internal void Open(string fileName)
        {
            Game.Open(fileName);
        }

        internal void New()
        {
            Game.New();
        }
    }
}
