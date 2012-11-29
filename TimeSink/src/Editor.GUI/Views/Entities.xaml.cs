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
using Editor;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core;
using System.Collections.ObjectModel;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Entities.xaml
    /// </summary>
    public partial class Entities : UserControl
    {
        private bool isLoaded;

        public Entities()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Entities_Loaded);
        }
        public EditorGame Game { get; set; }

        public ObservableCollection<string> Ids { get; set; }

        void Entities_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                var mainWindow = this.TryFindParent<MainWindow>();
                Game = mainWindow.editor.Game;
                mainWindow.editor.EntityAdded += new EntityAddedEventHandler(Entity_Added);
                mainWindow.LevelChanged += new LevelChangedEventHandler(Level_Changed);

                Level_Changed();

                isLoaded = true;
            }
        }

        private void Entity_Added(Entity entity)
        {
            Ids.Add(entity.InstanceId);
        }

        private void Level_Changed()
        {
            Ids = new ObservableCollection<string>(
                Game.LevelManager.Level.Entities.Select(x => x.InstanceId));
            entityIds.ItemsSource = Ids;
        }

        private void Id_Selected(object sender, EventArgs args)
        {
            if (entityIds.SelectedItem != null)
            {
                var entity = Game.LevelManager.Level.Entities.First(
                    x => x.InstanceId.Equals(entityIds.SelectedItem.ToString()));

                entityEdit.InitGrid(entity);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var entity = Game.LevelManager.Level.Entities.First(
                x => x.InstanceId.Equals(entityIds.SelectedItem.ToString()));
            entityEdit.PopulateEntity(entity);
        }
    }
}
