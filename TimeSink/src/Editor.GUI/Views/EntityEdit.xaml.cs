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
                    UIElement elementToAdd = null;
                    dynamic.Children.Add(textBlock);
                    Grid.SetRow(textBlock, i);
                    Grid.SetColumn(textBlock, 0);

                    var type = prop.PropertyType;
                    if (type.IsEnum)
                    {
                        elementToAdd = new TextBox() { Text = ((int)val).ToString() };
                    }
                    else if (type.Equals(typeof(int)) || type.Equals(typeof(float)) || 
                        type.Equals(typeof(string)) || type.Equals(typeof(Guid)))
                    {
                        elementToAdd = new TextBox() { Text = val.ToString() };
                    }
                    else if (type.Equals(typeof(bool)))
                    {
                        elementToAdd = new CheckBox() { IsChecked = (bool)val };
                    }
                    else if (type.Equals(typeof(Vector2)))
                    {
                        var vec = (Vector2)val;
                        elementToAdd = new TextBox() { Text = vec.ToDisplayString() };
                    }
                    else
                    {
                        throw new InvalidOperationException("Unhandled property type in entity editor.");
                    }

                    dynamic.Children.Add(elementToAdd);
                    Grid.SetRow(elementToAdd, i);
                    Grid.SetColumn(elementToAdd, 1);            

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
                    var element = dynamic.Children
                      .Cast<UIElement>()
                      .First(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == 1);

                    object valToSet = null;
                    var type = prop.PropertyType;
                    if (type.IsEnum)
                        valToSet = Int32.Parse(((TextBox)element).Text);
                    else if (type.Equals(typeof(int)))
                        valToSet = Int32.Parse(((TextBox)element).Text);
                    else if (type.Equals(typeof(float)))
                        valToSet = Single.Parse(((TextBox)element).Text);
                    else if (type.Equals(typeof(string)))
                        valToSet = ((TextBox)element).Text;
                    else if (type.Equals(typeof(Guid)))
                        valToSet = Guid.Parse(((TextBox)element).Text);
                    else if (type.Equals(typeof(bool)))
                    {
                        valToSet = ((CheckBox)element).IsChecked;
                    }
                    else if (type.Equals(typeof(Vector2)))
                    {
                        valToSet = ((TextBox)element).Text.ParseVector();
                    }

                    prop.SetValue(entity, valToSet, null);

                    i++;
                }
            }
        }
    }
}
