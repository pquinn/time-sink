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
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Editor.GUI.ViewModels;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for StaticMeshSelector.xaml
    /// </summary>
    public partial class StaticMeshSelector : UserControl
    {
        InMemoryResourceCache<Texture2D> cache;

        public StaticMeshSelector()
        {
            InitializeComponent();
            DataContext = new StaticMeshSelectorViewModel(cache);
        }

        public InMemoryResourceCache<Texture2D> Cache
        {
            get
            {
                return cache;
            }

            set
            {
                DataContext = value;
                cache = value;
            }
        }
    }
}
