using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    internal class RegionManager
    {
        public static RegionManager instance = new RegionManager();

        internal class Region
        {
            public String name;
            public List<TitleParser> duchies = new List<TitleParser>();
        }
        public List<TitleParser> duchiesAssigned = new List<TitleParser>();
        
        public List<Region> regions = new List<Region>();
        public void AddRegion(String name, List<TitleParser> duchies)
        {
            String safeName = StarNames.SafeName(name);

            LanguageManager.instance.Add(safeName, name);
            Region r = new Region();
            for (int index = 0; index < duchies.Count; index++)
            {
                var titleParser = duchies[index];
                if (duchiesAssigned.Contains(titleParser))
                {
                    duchies.Remove(titleParser);
                    index--;
                }
            }
            r.name = name;
            r.duchies.AddRange(duchies);
            duchiesAssigned.AddRange(duchies);
            regions.Add(r);
        }

        public void Save()
        {
            Script s = new Script();
            s.Name = Globals.GameDir + "map\\geographical_region.txt";
            s.Root = new ScriptScope();
            foreach (var region in regions)
            {
                ScriptScope ss = new ScriptScope();

                String duchieList = "";

                foreach (var titleParser in region.duchies)
                {
                    duchieList = duchieList + " " + titleParser.Name;
                }
                ss.Name = StarNames.SafeName(region.name);
                ss.Do(@"
                    duchies = {
                        " + duchieList + @"
                    }
");
                s.Root.Add(ss);
           }

            s.Save();
        }
    }
}
