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
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using Microsoft.Xna.Framework;

namespace TimeSink.Editor.GUI.Views
{
    /// <summary>
    /// Interaction logic for EntityEdit.xaml
    /// </summary>
    public partial class EntityEdit : UserControl
    {
        public EntityEdit()
        {
            InitializeComponent();
        }

        public void InitGrid(Entity entity)
        {
            dynamic.Children.Clear();

            int i = 0;
            foreach (var prop in entity.GetType().GetProperties())
            {
                var attr = prop.GetCustomAttributes(typeof(EditableFieldAttribute), false);
                if (attr.Any())
                {
                    dynamic.RowDefinitions.Add(new RowDefinition());

                    var val = prop.GetValue(entity, null) ?? string.Empty;
                    var textBlock = new TextBlock() { Text = ((EditableFieldAttribute)attr[0]).Display };
                    dynamic.Children.Add(textBlock);
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, 0);

                    var type = prop.PropertyType;
                    if (type.Equals(typeof(int)) || type.Equals(typeof(float)) || 
                        type.Equals(typeof(string)) || type.Equals(typeof(Guid)))
                    {
                        var textBox = new TextBox() { Text = val.ToString() };
                        dynamic.Children.Add(textBox);
                        Grid.SetRow(textBox, i);
                        Grid.SetColumn(textBox, 1);
                    }
                    else if (type.Equals(typeof(Vector2)))
                    {
                        var vec = (Vector2)val; 
                        var textBox = new TextBox() { Text = vec.ToDisplayString() };
                        dynamic.Children.Add(textBox);
                        Grid.SetRow(textBox, i);
                        Grid.SetColumn(textBox, 1);
                    }                    

                    i++;
                }
            }
        }

        public void PopulateEntity(Entity entity)
        {
            int i = 0;
            foreach (var prop in entity.GetType().GetProperties())
            {
                var attr = prop.GetCustomAttributes(typeof(EditableFieldAttribute), false);
                if (attr.Any())
                {
                    var textBox = dynamic.Children
                      .Cast<UIElement>()
                      .First(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == 1) as TextBox;

                    object valToSet = null;
                    var type = prop.PropertyType;
                    if (type.Equals(typeof(int)))
                        valToSet = Int32.Parse(textBox.Text);
                    else if (type.Equals(typeof(float)))
                        valToSet = Single.Parse(textBox.Text);
                    else if (type.Equals(typeof(string)))
                        valToSet = textBox.Text;
                    else if (type.Equals(typeof(Guid)))
                        valToSet = Guid.Parse(textBox.Text);
                    else if (type.Equals(typeof(Vector2)))
                    {
                        valToSet = textBox.Text.ParseVector();
                    }

                    prop.SetValue(entity, valToSet, null);

                    i++;
                }
            }
        }
    }
}
