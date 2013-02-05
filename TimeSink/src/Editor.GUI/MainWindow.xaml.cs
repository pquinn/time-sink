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

namespace TimeSink.Editor.GUI
{
    public delegate void LevelChangedEventHandler();

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FileName { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => Save()), new KeyGesture(Key.S, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => Open()), new KeyGesture(Key.O, ModifierKeys.Control)));
            this.InputBindings.Add(
                new KeyBinding(new RelayCommand(a => New()), new KeyGesture(Key.N, ModifierKeys.Control)));
        }

        public LevelChangedEventHandler LevelChanged { get; set; }

        private void Show_Grid_Lines_Click(object sender, RoutedEventArgs e)
        {
            editor.ToggleGridLines();
        }

        private void Grid_Line_Size_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            editor.SetGridLineSize(Int32.Parse(item.Tag.ToString()));
        }

        private void Enable_Snapping_Click(object sender, RoutedEventArgs e)
        {
            editor.ToggleSnapping();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        private void Save()
        {
            if (string.IsNullOrEmpty(FileName))
                SaveAs();
            else
            {
                editor.SaveAs(FileName);
            }
        }

        private void Save_As_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }
        private void SaveAs()
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results 
            if (result == true)
            {
                // Save document 
                editor.SaveAs(dlg.FileName);
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Open();   
        }
        private void Open()
        {
            if (!PromptOk())
                return;

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                editor.Open(dlg.FileName);

                FileName = dlg.FileName;

                var databaseLocation = System.IO.Directory.GetParent(dlg.FileName).ToString() + @"\..\..\DialoguePrototypeTestDB.s3db";
                editor.Game.Database.SetDBConnectionPath(databaseLocation);

                if (LevelChanged != null)
                    LevelChanged();
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            New();
        }

        private void New()
        {
            if (!PromptOk())
                return;

            FileName = null;

            editor.New();

            if (LevelChanged != null)
                LevelChanged();
        }

        private bool PromptOk()
        {
            // Configure the message box to be displayed 
            string messageBoxText = "WARNING! Proceeding will discard all changes since your last save!";
            string caption = "Discard Changes";
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results 
            switch (result)
            {
                case MessageBoxResult.OK:
                    return true;
                case MessageBoxResult.Cancel:
                    return false;
            }

            return false;
        }
    }
}
