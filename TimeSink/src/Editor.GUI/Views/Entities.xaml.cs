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

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Entities.xaml
    /// </summary>
    public partial class Entities : UserControl
    {
        public Entities()
        {
            InitializeComponent();
        }

        private EditorGame game;
        public EditorGame Game 
        {
            get
            {
                return game;
            }
            set
            {
                game = value;
                Ids = game.LevelManager.Level.Entities.Select(x => x.InstanceId).ToList();
            }
        }

        public List<string> Ids { get; set; }

        private void Id_Selected(object sender, EventArgs args)
        {
            var entity = game.LevelManager.Level.Entities.First(
                x => x.InstanceId.Equals(entityIds.SelectedItem.ToString()));

            entityEdit.InitGrid(entity);
        }
    }
}
