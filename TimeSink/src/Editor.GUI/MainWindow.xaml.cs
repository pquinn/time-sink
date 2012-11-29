﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();
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

        private void Save_As_Click(object sender, RoutedEventArgs e)
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

                if (LevelChanged != null)
                    LevelChanged();
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            editor.New();

            if (LevelChanged != null)
                LevelChanged();
        }
    }
}
