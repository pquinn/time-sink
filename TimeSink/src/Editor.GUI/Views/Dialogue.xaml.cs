using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using TimeSink.Engine.Core.StateManagement;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for Dialogue.xaml
    /// </summary>
    public partial class Dialogue : UserControl
    {
        private bool isLoaded;
        private List<NPCPrompt> prompts;

        public event PropertyChangedEventHandler PropertyChanged;

        public Dialogue()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Dialogue_Loaded);
        }

        public EditorGame Game { get; set; }

        private ObservableCollection<string> _dialogueIds;
        public ObservableCollection<string> DialogueIds
        {
            get { return _dialogueIds; }
            set
            {
                _dialogueIds = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DialogueIds");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        void Dialogue_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                var mainWindow = this.TryFindParent<MainWindow>();
                Game = mainWindow.editor.Game;
                //mainWindow.editor.EntityAdded += new EntityAddedEventHandler(Dialogue_Added);
                mainWindow.LevelChanged += new LevelChangedEventHandler(Level_Changed);
                prompts = new List<NPCPrompt>();

                DialogueIds = new ObservableCollection<string>(prompts.Select(x => x.Id.ToString()));
                dialogueIds.ItemsSource = DialogueIds;

                dialogueEdit.Game = Game;

                isLoaded = true;
            }
        }

        private void Dialogue_Added(Dialogue entity)
        {
        }

        private void Id_Selected(object sender, EventArgs args)
        {
            if (dialogueIds.SelectedItem != null)
            {
                var entry = prompts.First(x => x.Id != null && x.Id.ToString().Equals(dialogueIds.SelectedItem.ToString()));
            
                dialogueEdit.InitGrid(entry);
            }
        }

        private void Level_Changed()
        {
            prompts = Game.Database.FindAllPrompts();
            DialogueIds = new ObservableCollection<string>(
                prompts.Select(x => x.Id.ToString()));
            dialogueIds.ItemsSource = DialogueIds;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var entry = prompts.First(x => x.Id != null && x.Id.ToString().Equals(dialogueIds.SelectedItem.ToString()));
            dialogueEdit.UpdatePrompt(entry);
        }
    }
}
