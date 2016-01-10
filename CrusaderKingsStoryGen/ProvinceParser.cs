using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CrusaderKingsStoryGen.Simulation;

namespace CrusaderKingsStoryGen
{
    class ProvinceParser :  Parser
    {
        public String Name { get; set; }
        public int id;
        public int provinceRCode;
        public int provinceGCode;
        public int provinceBCode;
        public String title { get; set; }
        public int max_settlements { get; set; }
        public TitleParser ProvinceOwner { get; set; }
        public List<Barony> baronies = new List<Barony>(); 
        public void Save()
        {
            if (title == null)
                return;
            if (Title == null)
                return;

            string dir = Globals.ModDir + "history\\provinces\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            String provincesDir = Globals.MapDir + "history\\provinces\\" + id.ToString() + " - " + Name.Replace("c_", "") + ".txt";

            Script s = new Script();
            s.Root = new ScriptScope();
            s.Name = provincesDir;
            s.Root.Add(new ScriptCommand("title", Name, s.Root));
            s.Root.Add(new ScriptCommand("max_settlements", 3, s.Root));
            if (Title.Holder == null)
            {
                foreach (var provinceParser in Adjacent)
                {
                    if (provinceParser.title != null && provinceParser.Title.Holder != null)
                    {
                        s.Root.Add(new ScriptCommand("culture", provinceParser.Title.Holder.culture, s.Root));
                        s.Root.Add(new ScriptCommand("religion", provinceParser.Title.Holder.religion, s.Root));
                        break;
                    }
                }
            }
            else
            {
                s.Root.Add(new ScriptCommand("culture", Title.Holder.culture, s.Root));
                s.Root.Add(new ScriptCommand("religion", Title.Holder.religion, s.Root));
                
            }
            if (MapManager.instance.LoadedTerrain.ContainsKey(id))
                s.Root.Add(new ScriptCommand("terrain", MapManager.instance.LoadedTerrain[id], s.Root));
        
            foreach (var barony in baronies)
            {
                s.Root.Add(new ScriptCommand(barony.title, barony.type, s.Root));
            }
            s.Save();
        }

        public void Rename(string name)
        {
            String oldName = Name;
            Name = "c_" + StarNames.SafeName(name);
            LanguageManager.instance.Add(Name, name);
            LanguageManager.instance.Add("PROV" + id, name);
            MapManager.instance.ProvinceMap.Remove(oldName);
            MapManager.instance.ProvinceMap[Name] = this;
        }

        public void RenameForCulture(CultureParser culture)
        {
            LanguageManager.instance.Remove(Name);
            var name = culture.dna.GetPlaceName();

            if (MapManager.instance.ProvinceMap.ContainsKey("c_" + StarNames.SafeName(name)))
            {
                RenameForCulture(culture);
                return;
            }

            if (Title != null)
            {
                Title.Rename(name);
            }
          
             Rename(name);
           
            
        }
        public void AddTemple(CultureParser culture)
        {
            AddBarony("temple", culture);
        }

        public void CreateProvinceDetails(CultureParser culture)
        {
            if (culture.Governments.Count == 0)
            {
                AddBarony("tribal", culture);
                return;
            }
            foreach (var government in culture.Governments)
            {
                switch (government.type)
                {
                    case "tribal":
                    case "nomadic":
                        AddBarony("tribal", culture);
                    break;
                    case "republic":
                    AddBarony("town", culture);

                    break;
                    case "theocracy":
                    AddBarony("temple", culture);

                    break;
                    case "feudal":
                    AddBarony("castle", culture);

                    break;
                }
            }
          
        }

        public void AddBarony(CultureParser culture)
        {
            if (Rand.Next(4) != 0)
            {
       
                    AddBarony("castle", culture);
            }
            else
            {
                if (Rand.Next(2) == 0)
                {
                    AddBarony("temple", culture);
                }
                else
                {
                    AddBarony("city", culture);

                }
            }
        }

        public void AddBarony2(CultureParser culture)
        {
            if (Rand.Next(4) == 0)
            {
                if (Rand.Next(2) != 0)
                {
                    AddBarony("temple", culture);
                }
                else
                {
                    AddBarony("city", culture);

                } 
            }
            else
            {
                AddBarony("castle", culture);
            }
        }

        private void AddBarony(string type, CultureParser culture)
        {
            TitleParser title = TitleManager.instance.CreateBaronyScriptScope(this, culture);
            baronies.Add(new Barony() { province = this, title = title.Name, titleParser = title, type = type });
            this.Title.AddSub(title);
            SimulationManager.instance.AddCharacterForTitle(title);
        }

        public CharacterParser TotalLeader
        {
            get
            {
                var title = this.ProvinceOwner;
                if (title == null)
                    return null;
                while (title.Liege != null && title.Rank < title.Liege.Rank)
                {
                    title = title.Liege;
                }
                return title.Holder;
            }
        }

        public Color Color { get; set; }


        public TitleParser Title
        {
            get
            {
                if (title == null)
                    return null;
                if (!TitleManager.instance.TitleMap.ContainsKey(title))
                    return null;
                return TitleManager.instance.TitleMap[title]; }
        }

        public List<Point> Points = new List<Point>();
        public override string ToString()
        {
            return id + " - " + title;
        }

        public ProvinceParser(ScriptScope scope) : base(scope)
        {
            int line = 0;
            foreach (var child in scope.Children)
            {
                if (child is ScriptCommand)
                {
                    RegisterProperty(line, ((child as ScriptCommand).Name), child);

                }
                line++;
                if (child is ScriptScope)
                {
                    var subscope = (child as ScriptScope);
                   
                }
            }
           
        }

        public void DoTitleOwnership()
        {
            if (title != null && TitleManager.instance.TitleMap.ContainsKey(title))
            {
                TitleManager.instance.TitleMap[title].Owns.Add(this);
                ProvinceOwner = TitleManager.instance.TitleMap[title];
            }
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

        public List<ProvinceParser> Adjacent = new List<ProvinceParser>();
        public void AddAdjacent(ProvinceParser prov)
        {
            if (!Adjacent.Contains(prov))
                Adjacent.Add(prov);
            if (!prov.Adjacent.Contains(this))
                prov.Adjacent.Add(this);
        }

        public bool IsAdjacentToUnclaimed()
        {
            foreach (var provinceParser in Adjacent)
            {
                if (!provinceParser.Title.Claimed)
                    return true;
            }
            return false;
        }

        public class Barony
        {
            public String type;
            public String title;
            public ProvinceParser province;
            public TitleParser titleParser;
        }

        public List<Barony> Temples = new List<Barony>();
        public bool land = false;

        public void GatherBaronies()
        {
            foreach (var child in Scope.Children)
            {
                if (child is ScriptCommand)
                {
                    ScriptCommand c = (ScriptCommand) child;

                    if (c.Name.StartsWith("b_"))
                    {
                        String str = c.Value.ToString();
                    
                        if (c.Value.ToString() == "temple")
                        {
                            var t = new Barony() {type = c.Value.ToString(), title = c.Name, province = this};
                            Temples.Add(t);
                            MapManager.instance.Temples[c.Name] = t;
                        }
                    }
                }
            }
        }

        public TitleParser CreateTitle()
        {
            this.title = this.Name;
        //    string n = culture.dna.GetPlaceName();
         //   String sn = StarNames.SafeName(n);
          //  this.Name = "c_" + sn;
            var scope = new ScriptScope();
            scope.Name = this.title;
            var c = new TitleParser(scope);
            c.capital = this.id;
            c.CapitalProvince = this;
            TitleManager.instance.AddTitle(c);
            return c;
        }

        public CharacterParser GetCurrentHolder()
        {
            if (this.title == null)
                return null;
            if (this.Title.CurrentHolder != null)
                return this.Title.CurrentHolder;

            return this.Title.Holder;
        }

    }
}