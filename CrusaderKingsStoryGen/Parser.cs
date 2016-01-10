using System;
using System.Collections.Generic;
using System.Drawing;

namespace CrusaderKingsStoryGen
{
    abstract class Parser
    {
        public string Name { get; set; }
        public Parser(ScriptScope scope)
        {
            this.Scope = scope;
            int line = 0;
            foreach (var child in scope.Children)
            {
                if (child is ScriptCommand)
                {
                    string name = (child as ScriptCommand).Name;
                    RegisterProperty(line, name, child);

                }
               
                line++;
            }
        }

        public abstract ScriptScope CreateScope();
        public ScriptScope Scope { get; set; }

        public Dictionary<string, int> PropertyMap = new Dictionary<string, int>();
        public void RegisterProperty(int line, string property, object child)
        {
            ScriptCommand command = child as ScriptCommand;
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                if (propertyInfo.Name == command.Name)
                {
                    if (command.Value.GetType() == propertyInfo.PropertyType)
                    {
                        propertyInfo.SetValue(this, command.Value);    
                    }
                    else
                    {
                        if (command.Value is ScriptReference)
                        {
                            var scriptReference = command.Value as ScriptReference;
                            if (propertyInfo.PropertyType == typeof(int))
                            {
                                propertyInfo.SetValue(this, Convert.ToInt32(scriptReference.Referenced));
                            }
                            else if (propertyInfo.PropertyType == typeof(float))
                            {
                                propertyInfo.SetValue(this, Convert.ToSingle(scriptReference.Referenced));
                            }
                            else if (propertyInfo.PropertyType == typeof(Color))
                            {
                                int r = 0;
                                int g = 0;
                                int b = 0;

                                String[] str = scriptReference.Referenced.Split(' ');
                                int[] rgb = new int[3];
                                int c = 0;

                                foreach (var s in str)
                                {
                                    if (s.Trim().Length > 0)
                                    {
                                        rgb[c] = Convert.ToInt32(s.Trim());
                                        c++;
                                        if (c >= 3)
                                            break;
                                    }
                                }

                                if (c == 0)
                                    propertyInfo.SetValue(this, Color.Black);
                                else if (c < 3)
                                {
                                    for (int n = c; n < 3; n++)
                                        rgb[n] = rgb[n - 1];
                                }

                                for (int n = 0; n < 3; n++)
                                {
                                    if (rgb[n] > 255)
                                        rgb[n] = 255;
                                    if (rgb[n] < 0)
                                        rgb[n] = 0;
                                }
                                propertyInfo.SetValue(this, Color.FromArgb(255, rgb[0], rgb[1], rgb[2]));
                            }
                            else propertyInfo.SetValue(this, scriptReference.Referenced); 
                        }
                        else
                        {
                          
                        }
                      
                    }
                    
                }
            }

            PropertyMap[property] = line;
        }


        public ScriptCommand GetProperty(string property)
        {
            if (!PropertyMap.ContainsKey(property))
                return null;
            return Scope.Children[PropertyMap[property]] as ScriptCommand;
        }

        public void DeleteProperty(string property)
        {
            if (!PropertyMap.ContainsKey(property))
                return;

            Scope.RemoveAt(PropertyMap[property]);
            
        }
        
        public void SetProperty(String property, object value)
        {
            if (!PropertyMap.ContainsKey(property))
            {
                Scope.Delete(property);
                Scope.Add(new ScriptCommand() { Name = property, Value = value });
             //   RegisterProperty(Scope.Children.Count-1, property, value);
                return;
            }

            Scope.Children[PropertyMap[property]] = new ScriptCommand() {Name = property, Value = value};
            Scope.ChildrenMap[property] = value;
        }
    }
}