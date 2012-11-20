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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

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
    }
}
