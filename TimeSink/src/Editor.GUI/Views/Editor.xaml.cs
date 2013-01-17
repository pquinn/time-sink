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
using System.Windows.Interop;

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
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => PanClick()), new KeyGesture(Key.P, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => PanRevertClick()), new KeyGesture(Key.P, ModifierKeys.Alt)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => ZoomClick()), new KeyGesture(Key.Z, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => ZoomRevertClick()), new KeyGesture(Key.Z, ModifierKeys.Alt)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => SelectionClick()), new KeyGesture(Key.M, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => RotationClick()), new KeyGesture(Key.R, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => ScalingClick()), new KeyGesture(Key.S, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => GeometryClick()), new KeyGesture(Key.G, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => EntitiesClick()), new KeyGesture(Key.E, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => StaticClick()), new KeyGesture(Key.T, ModifierKeys.Control)));
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
            PanClick();
        }
        private void PanClick()
        {
            ClearButtons();

            if (!selectionButtonPressed)
            {
                Game.PanSelected();
            }
        }

        private void Zoom_Click(object sender, RoutedEventArgs e)
        {
            ZoomClick();
        }
        private void ZoomClick()
        {
            ClearButtons();

            if (!selectionButtonPressed)
            {
                Game.ZoomSelected();
            }
        }

        private void Selection_Click(object sender, RoutedEventArgs e)
        {
            SelectionClick();
        }
        private void SelectionClick()
        {
            ClearButtons();

            if (!selectionButtonPressed)
            {
                Game.SelectionSelected();
            }
        }

        private void Rotation_Click(object sender, RoutedEventArgs e)
        {
            RotationClick();
        }
        private void RotationClick()
        {
            ClearButtons();

            if (!rotationButtonPressed)
            {
                Game.RotationSelected();
            }
        }

        private void Scaling_Click(object sender, RoutedEventArgs e)
        {
            ScalingClick();
        }
        private void ScalingClick()
        {
            ClearButtons();

            if (!selectionButtonPressed)
            {
                Game.ScalingSelected();
            }
        }

        private void Entities_Click(object sender, RoutedEventArgs e)
        {
            EntitiesClick();
        }
        private void EntitiesClick()
        {
            ClearButtons();

            if (!entitiesButtonPressed)
            {
                var entities = Game.Container.Resolve<IEnumerable<Entity>>().ToList();
                entities.ForEach(
                    x =>
                    {
                        x.Load(Game.Container);
                        Game.LevelManager.EditorRenderManager.RegisterPreviewable(x);
                        //Game.LevelManager.RegisterEntity(x);
                    });
                var entityWindow = new EntitySelector(entities, Game.TextureCache);

                entityWindow.ShowDialog();

                entities.ForEach(x => Game.LevelManager.EditorRenderManager.UnregisterPreviewable(x));

                ResetHandle();

                var viewModel = entityWindow.DataContext as EntitySelectorViewModel;
                if (entityWindow.DialogResult.Value)
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
        }

        private void Static_Click(object sender, RoutedEventArgs e)
        {
            StaticClick();
        }
        private void StaticClick()
        {
            ClearButtons();

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
        }

        private void ResetHandle()
        {
            Microsoft.Xna.Framework.Input.Mouse.WindowHandle = xnaControl.Handle;
        }

        private void Geometry_Click(object sender, RoutedEventArgs e)
        {
            GeometryClick();
        }
        private void GeometryClick()
        {
            ClearButtons();

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
        }

        private void ZoomRevertClick()
        {
            Game.ZoomRevertClick();
        }

        private void PanRevertClick()
        {
            Game.PanRevertClick();
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
