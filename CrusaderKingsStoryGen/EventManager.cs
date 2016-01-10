using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class EventNamespace
    {
        public String name;
        public List<EventParser> events = new List<EventParser>();
        public Dictionary<String, EventParser> eventmap = new Dictionary<String, EventParser>();

        public EventNamespace(String name)
        {
            this.name = name;
        }
    }
    class EventManager
    {
        public static EventManager instance = new EventManager();
        public List<Script>  Scripts = new List<Script>();
        public void AddToNamespace(String names, EventParser ev)
        {
            EventNamespace e = null;
            if (!NamespaceMap.ContainsKey(names))
            {
                e = new EventNamespace(names);
                NamespaceMap[names] = e;
            }
            else
            {
                e = NamespaceMap[names];
            }

            e.events.Add(ev);
            e.eventmap[ev.GetProperty("id").Value.ToString().Replace(names + ".", "")] = ev;
        }
        public void Load()
        {
            var files = Directory.GetFiles(Globals.GameDir + "events");
            
            foreach (var file in files)
            {
                Script s = ScriptLoader.instance.Load(file);
                string names = null;
                foreach (var child in s.Root.Children)
                {
                    if(child is ScriptCommand)
                    {
                        if (child.ToString().Contains("namespace"))
                        {
                            names = (child as ScriptCommand).Value.ToString();
                        }
                        continue;
                    }
                    var sc = (child as ScriptScope);
                    EventParser ee = new EventParser(sc);
                    Events.Add(ee);

                    EventMap[(sc.Find("id") as ScriptCommand).Value.ToString()] = ee;
                    if(names != null)
                        AddToNamespace(names, ee);
                }
                Scripts.Add(s);
            }

            foreach (var scriptScope in Events)
            {
                scriptScope.FindLinks();
            }

            var ev = EventMap["TOG.1200"];

            var e = ev;


        }

        public void Save()
        {
            foreach (var script in Scripts)
            {
            //    RemoveAllReligionTests(script.Root);
        //        script.Save();
            }
        }

        public List<EventParser> Events = new List<EventParser>();
        public Dictionary<string, EventParser> EventMap = new Dictionary<string, EventParser>();
        public Dictionary<string, EventNamespace> NamespaceMap = new Dictionary<string, EventNamespace>();

        private void RemoveAllReligionTests(ScriptScope node)
        {
            for (int index = 0; index < node.Children.Count; index++)
            {
                var child = node.Children[index];
                if (child is ScriptScope)
                    RemoveAllReligionTests(child as ScriptScope);
                if (child is ScriptCommand)
                {
                    ScriptCommand c = (ScriptCommand)child;
                    if (c.Name == "religion" || c.Name == "religion_group")
                    {
                        if (c.Value.ToString().ToUpper() == "FROM" || c.Value.ToString().ToUpper() == "ROOT" || c.Value.ToString().ToUpper().Contains("PREV"))
                        {

                        }
                        else
                        {
                            node.Remove(child);
                            index--;

                        }
                        continue;
                    }
                    if (c.Name == "culture" || c.Name == "culture_group")
                    {
                        if (c.Value.ToString().ToUpper() == "FROM" || c.Value.ToString().ToUpper() == "ROOT" || c.Value.ToString().ToUpper().Contains("PREV"))
                        {

                        }
                        else
                        {
                            node.Remove(child);
                            index--;

                        }
                        continue;
                    }
                }
            }
        }

        public EventParser GetEvent(string id)
        {
            if (!EventMap.ContainsKey(id))
                return null;

            return EventMap[id];
        }
    }
}
