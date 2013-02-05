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
using DialoguePrototype;
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
                //mainWindow.LevelChanged += new LevelChangedEventHandler(Level_Changed);
                prompts = Dialogue.FindAllPrompts();

                DialogueIds = new ObservableCollection<string>(prompts.Select(x => x.Id.ToString()));
                dialogueIds.ItemsSource = DialogueIds;

                isLoaded = true;
            }
        }

        private void Dialogue_Added(Dialogue entity)
        {
            DialogueIds.Add("id");
        }

        private void Id_Selected(object sender, EventArgs args)
        {
            if (dialogueIds.SelectedItem != null)
            {
                var entry = prompts.First(x => x.Id != null && x.Id.ToString().Equals(dialogueIds.SelectedItem.ToString()));
            
                dialogueEdit.InitGrid(entry);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var entry = prompts.First(x => x.Id != null && x.Id.ToString().Equals(dialogueIds.SelectedItem.ToString()));
            dialogueEdit.UpdatePrompt(entry);
        }

        public static List<NPCPrompt> FindAllPrompts()
        {
            List<NPCPrompt> prompts = new List<NPCPrompt>();
            try
            {
                DataTable entry;
                String query = "select * from Prompt;";
                entry = EditorGame.Database.GetDataTable(query);
                foreach (DataRow r in entry.Rows)
                {
                    prompts.Add(FindPrompt(new Guid((String)r["id"])));
                }
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                //EngineGame.Logger.Error(error);
                prompts.Add(new NPCPrompt());
            }
            return prompts;
        }

        public static NPCPrompt FindPrompt(Guid id)
        {
            List<IDialogueAction> promptActions = new List<IDialogueAction>();
            try
            {
                DataTable entry;
                String query = "select speaker \"speaker\", entry \"entry\", ";
                query += "animation \"animation\", sound \"sound\", quest \"quest\", ";
                query += "response_required \"response\" ";
                query += "from Prompt where id = \"" + id.ToString() + "\";";
                entry = EditorGame.Database.GetDataTable(query);
                // only take the first result (there should only be one anyway)
                DataRow result = entry.Rows[0];
                String speaker = (String)result["speaker"];
                String body = (String)result["entry"];

                if (!DBNull.Value.Equals(result["animation"]))
                {
                    promptActions.Add(new AnimationAction((String)result["animation"]));
                }

                if (!DBNull.Value.Equals(result["sound"]))
                {
                    promptActions.Add(new SoundAction((String)result["sound"]));
                }

                if (!DBNull.Value.Equals(result["quest"]))
                {
                    promptActions.Add(new QuestAction((String)result["quest"]));
                }

                Boolean responseRequired = (Boolean)result["response"];

                NPCPrompt prompt = new NPCPrompt(id, speaker, body, promptActions, responseRequired);

                if (responseRequired)
                {
                    prompt.Responses = FindResponses(id);
                }
                return prompt;

            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new NPCPrompt(id, "error", error, promptActions, false);
            }
        }

        public static Response FindResponse(Guid id)
        {
            try
            {
                DataTable entry;
                String query = "select entry \"entry\", ";
                query += "next_entry \"next_entry\" ";
                query += "from Response where ID = \"" + id.ToString() + "\";";
                entry = EditorGame.Database.GetDataTable(query);
                // again, there should only be one result
                DataRow result = entry.Rows[0];
                String entryText = (String)result["entry"];
                Guid nextEntry = new Guid((String)result["next_entry"]);
                return new Response(entryText, nextEntry);
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                return new Response("error: " + error, new Guid());
            }
        }

        public static List<Response> FindResponses(Guid id)
        {
            List<Response> responses = new List<Response>();
            try
            {
                DataTable entry;
                String query = "select toID \"to\" ";
                query += "from Response_Map where fromID = \"" + id.ToString() + "\";";
                entry = EditorGame.Database.GetDataTable(query);
                foreach (DataRow r in entry.Rows)
                {
                    responses.Add(FindResponse(new Guid((String)r["to"])));
                }
            }
            catch (Exception e)
            {
                String error = "The following error has occurred:\n";
                error += e.Message.ToString() + "\n";
                Console.WriteLine(error);
                responses.Add(new Response("error: " + error, new Guid()));
            }
            return responses;
        }
    }
}
