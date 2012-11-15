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

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : UserControl
    {
        PreviewGame m_game;

        public PreviewWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Editor_Loaded);
        }

        void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            m_game = new PreviewGame(xnaControl.Handle, (int)xnaControl.ActualWidth, (int)xnaControl.ActualHeight);
        }

        public void ChangeTextures(string texturePathToDisplay)
        {
            m_game.ChangeTexture(texturePathToDisplay);
        }
    }
}
