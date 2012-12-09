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
using System.Collections;

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

                    if (type.Equals(typeof(bool)))
                    {
                        elementToAdd = new CheckBox() { IsChecked = (bool)val };
                    }
                    else
                    {
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var builder = new StringBuilder();
                            var j = 0;
                            foreach (var v in (IEnumerable)val)
                            {
                                if (j > 0)
                                    builder.Append(";\n");
                                builder.Append(CreateString(v, type.GetGenericArguments()[0]));
                                j++;
                            }

                            elementToAdd = new TextBox() 
                            { 
                                AcceptsReturn = true, 
                                TextWrapping = TextWrapping.Wrap,
                                Height = double.NaN,
                                Text = builder.ToString()
                            };
                        }
                        else
                        {
                            elementToAdd = new TextBox() { Text = CreateString(val,type) };
                        }
                    }                    

                    dynamic.Children.Add(elementToAdd);
                    Grid.SetRow(elementToAdd, i);
                    Grid.SetColumn(elementToAdd, 1);            

                    i++;
                }
            }
        }

        private string CreateString(object val, Type type)
        {
            string s;
            if (type.IsEnum)
            {
                s = ((int)val).ToString();
            }
            else if (type.Equals(typeof(int)) || type.Equals(typeof(float)) ||
                type.Equals(typeof(string)) || type.Equals(typeof(Guid)))
            {
                s = val.ToString();
            }
            else if (type.Equals(typeof(Vector2)))
            {
                s = ((Vector2)val).ToDisplayString();
            }
            else
            {
                throw new InvalidOperationException("Unhandled property type in entity editor.");
            }

            return s;
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
                    var type = prop.PropertyType;

                    if (type.Equals(typeof(bool)))
                        prop.SetValue(entity, ((CheckBox)element).IsChecked, null);
                    else
                    {
                        var text = ((TextBox)element).Text;
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var vals = text
                                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim()).ToList();

                            vals.Select(x => ParseVal(x, type.GetGenericArguments()[0]));
                            prop.SetValue(entity, vals, null);
                        }
                        else
                        {
                            prop.SetValue(entity, ParseVal(text, type), null);
                        }
                    }

                    i++;
                }
            }
        }

        private object ParseVal(string x, Type type)
        {
            object valToSet = null;
            if (type.IsEnum)
                valToSet = Int32.Parse(x);
            else if (type.Equals(typeof(int)))
                valToSet = Int32.Parse(x);
            else if (type.Equals(typeof(float)))
                valToSet = Single.Parse(x);
            else if (type.Equals(typeof(string)))
                valToSet = x;
            else if (type.Equals(typeof(Guid)))
                valToSet = Guid.Parse(x);
            else if (type.Equals(typeof(Vector2)))
            {
                valToSet = x.ParseVector();
            }

            return valToSet;
        }
    }
}
