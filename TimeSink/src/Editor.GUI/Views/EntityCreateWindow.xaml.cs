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
using System.Windows.Shapes;
using TimeSink.Engine.Core;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for EntityCreateWindow.xaml
    /// </summary>
    public partial class EntityCreateWindow : Window
    {
        private Entity entity;

        public EntityCreateWindow(Entity entity)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(EntityCreateWindow_Loaded);
            this.entity = entity;
        }

        void EntityCreateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            entityEdit.InitGrid(entity);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            entityEdit.PopulateEntity(entity);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
