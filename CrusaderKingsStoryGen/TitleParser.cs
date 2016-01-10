using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrusaderKingsStoryGen.Simulation;

namespace CrusaderKingsStoryGen
{
    class TitleParser : Parser
    {
        public int Rank = 1;
        public Dictionary<String, TitleParser> SubTitles = new Dictionary<string, TitleParser>();

        public void SetCapital(ProvinceParser cap)
        {
            Scope.Add(new ScriptCommand() { Name = "capital", Value = cap.id });
           
        }
        public TitleParser Liege
        {
            get { return _liege; }
            set
            {
               
              

                _liege = value;

            }
        }

        public Government Government { get; set; }

        public bool rebel { get; set; }
        public bool landless { get; set; }
        public bool primary { get; set; }
        public string culture { get; set; }
        public bool tribe { get; set; }
        public Color color { get; set; }
        public Color color2 { get; set; }
        public int capital { get; set; }
        public int dignity { get; set; }
        public bool Active { get; set; }
        public List<ProvinceParser> Owns = new List<ProvinceParser>();
        private CharacterParser _holder;
        public List<TitleParser> AdjacentToTitle = new List<TitleParser>();
        public HashSet<TitleParser> AdjacentToTitleSet = new HashSet<TitleParser>();
        private TitleParser _liege;

        public void RenameForCulture(CultureParser culture)
        {
            var name = culture.dna.GetPlaceName();
            Rename(name);

            if (Rank == 1 && Owns.Count > 0)
            {
                Owns[0].Rename(name);
            }
        }

        public void Rename(String name)
        {
            String oldName = Name;
            LanguageManager.instance.Remove(Name);
            this.Name = StarNames.SafeName(name);
            if (Rank == 0)
                Name = "b_" + Name;
            if (Rank == 1)
                Name = "c_" + Name;
            if (Rank == 2)
                Name = "d_" + Name;
            if (Rank == 3)
                Name = "k_" + Name;
            if (Rank == 4)
                Name = "e_" + Name;

            LanguageManager.instance.Add(Name, name);
            TitleManager.instance.TitleMap.Remove(oldName);
            TitleManager.instance.TitleMap[name] = this;
            if (TitleManager.instance.TieredTitles.ContainsKey(oldName))
            {
                TitleManager.instance.TieredTitles.Remove(oldName);
                TitleManager.instance.TieredTitles[name] = this;

                if (Liege != null)
                {
                    Liege.SubTitles.Remove(oldName);
                    Liege.SubTitles[name] = this;

                }
            }

            Scope.Parent.ChildrenMap.Remove(oldName);
            Scope.Parent.ChildrenMap[name] = this;

        }


        public void AddSub(TitleParser sub)
        {
            if (sub.Rank >= this.Rank)
                return;
          //  if (SubTitles.ContainsKey(sub.Name))
           //     return;
            if (this == sub)
                return;

            var liege = sub;

            while (liege.Liege != null && liege.Liege.Rank > liege.Rank)
            {
                if (liege == this)
                {

                    return;
                }
                liege = liege.Liege;
                
            }
            if (liege != null)
            {
                liege.SubTitles.Remove(sub.Name);
            }
            SubTitles[sub.Name] = sub;
            Scope.SetChild(sub.Scope);
            sub.Liege = this;

        }
        public TitleParser(ScriptScope scope)
            : base(scope)
        {
            String newName = "";
            Name = scope.Name;
            if (Name.StartsWith("b_"))
            {
          //      newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture)); 
                Rank = 0;
            }
            if (Name.StartsWith("c_"))
            {
           //     newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture)); 
                Rank = 1;
            }
            if (Name.StartsWith("d_"))
            {
           //     newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture)); 
                Rank = 2;
            }
            if (Name.StartsWith("k_"))
            {
             //   newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture));
                Rank = 3;
            }
            if (Name.StartsWith("e_"))
            {
               // newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture) + " Empire");
                Rank = 4;
            }

            
            if (TitleManager.instance.TitleMap.ContainsKey(Name))
            {
                
            }
            TitleManager.instance.TitleMap[Name] = this;
            TitleManager.instance.Titles.Add(this);
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
                    if (subscope.Name == "OR" || subscope.Name == "NOT" || subscope.Name == "AND" ||
                        subscope.Name == "allow" ||
                        subscope.Name == "male_names" ||
                        subscope.Name == "coat_of_arms")
                        continue;
                    SubTitles[subscope.Name] = new TitleParser(subscope);
                  
                    SubTitles[subscope.Name].Liege = this;
                    if (subscope.Name.StartsWith("b_"))
                    {
                        MapManager.instance.RegisterBarony(subscope.Name, SubTitles[subscope.Name]);
                    }
                }
            }

            if (capital != 0)
            {
                if (MapManager.instance.ProvinceIDMap.ContainsKey(capital))
                {
                    ProvinceParser provinceParser = MapManager.instance.ProvinceIDMap[capital];
                    CapitalProvince = provinceParser;
                    if (Name.StartsWith("c_"))
                    {
                        Owns.Add(CapitalProvince);
                        CapitalProvince.title = this.Name;
                    }

                }


            }
            else if (MapManager.instance.ProvinceMap.ContainsKey(Name) && Rank == 1)
            {
                ProvinceParser provinceParser = MapManager.instance.ProvinceMap[Name];
                CapitalProvince = provinceParser;
                if (!Name.StartsWith("d_"))
                {
                    Owns.Add(CapitalProvince);
                    CapitalProvince.title = this.Name;
                }

            }
        }

        public void DoCapital()
        {
           
        }

        public override string ToString()
        {
            return Name;
        }
        
        public ProvinceParser CapitalProvince { get; set; }

        public CharacterParser Holder
        {
            get { return _holder; }
            set
            {
                _holder = value;

                if (value != null)
                {
                    culture = _holder.culture;
                    foreach (var provinceParser in Owns)
                    {
                        Color col = value.Color;
                        MapManager.instance.SetColour(MapManager.instance.globalLockBitmap, provinceParser.id, col);
                        SetProperty("color", col);
                        SetProperty("color2", col);
                    }
                    
                }
             
                //foreach (var provinceParser in CapitalProvince)
               
                
            }
        }

        public bool Claimed
        {
            get
            {
                var t = this;
                return !(!t.Active || t.Holder == null);
            }
        }

        public int LandedTitlesCount
        {
            get
            {
                int c = 0;
                if (Rank == 1)
                    return 1;

                foreach (var titleParser in SubTitles)
                {
                    c += titleParser.Value.LandedTitlesCount;
                }

                return c;
            }
        }
        public int DirectLandedTitlesCount
        {
            get
            {
                int c = 0;
                if (Rank == 1)
                    return 1;
            
                return c;
            }
        }
        public int DirectVassalLandedTitlesCount
        {
            get
            {
                int c = 0;
                if (Rank == 1)
                    return 1;

                foreach (var titleParser in SubTitles)
                {
                    c += titleParser.Value.DirectLandedTitlesCount;
                }

                return c;
            }
        }

        public TitleParser TopmostTitle
        {
            get
            {
                var liege = Liege;

                if (liege == null)
                    return this;

                while (liege.Liege != null && liege.Liege.Rank > liege.Rank)
                {
                    liege = liege.Liege;
                }

                return liege;
            }
        }

        public bool Religious { get; set; }
        public CharacterParser CurrentHolder { get; set; }

        public CultureParser Culture
        {
            get { return CultureManager.instance.CultureMap[culture]; }
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

        public void Remove()
        {
            Scope.Parent.Remove(Scope);
        }

        public bool SameRealm(TitleParser title)
        {          
            var liege = this;

            while (liege.Liege != null && liege.Rank < liege.Liege.Rank)
            {
                
                liege = liege.Liege;
            }
            var liege2 = title;

            while (liege2.Liege != null && liege2.Rank < liege2.Liege.Rank)
            {
                liege2 = liege2.Liege;
            }

            return liege == liege2;

        }
        
        public void AddChildProvinces(List<ProvinceParser> targets)
        {
      
            foreach (var subTitle in SubTitles)
            {
                targets.AddRange(subTitle.Value.Owns);
                subTitle.Value.AddChildProvinces(targets);
            }
        }

        public bool Adjacent(TitleParser other)
        {
        
            if (AdjacentToTitleSet.Contains(other))
                return other.AdjacentToTitle.Contains(this);
          
            if (Rank==1)
            {
                if (this.Owns.Count == 0)
                    return false;
                if (other.Rank == 1)
                    return (this.Owns[0].Adjacent.Contains(other.Owns[0]));
                if (other.Rank == 2)
                {
                    foreach (var titleParser in other.SubTitles)
                    {
                        if (this.Adjacent(titleParser.Value))
                        {
                            AddAdj(other);
                            return true;
                        }
                     
                    }
                }

            }
            if (Rank==2)
            {
                if (other.Rank==2)
                {
                    foreach (var titleParser in other.SubTitles)
                    {
                        if (Adjacent(titleParser.Value))
                        {
                            AddAdj(other);

                            return true;
                        }
                       
                    }
                    
                }
                else
                {
                    if (other.Adjacent(this))
                    {
                        AddAdj(other);
                        return true;
                    }
                  
                }
            }
            AddNotAdj(other);
            return false;
        }

        private void AddAdj(TitleParser other)
        {
            if (AdjacentToTitleSet.Contains(other))
                return;

            AdjacentToTitleSet.Add(other);
            AdjacentToTitle.Add(other);
            other.AdjacentToTitleSet.Add(this);
            other.AdjacentToTitle.Add(this);

        }
        private void AddNotAdj(TitleParser other)
        {
            if (AdjacentToTitleSet.Contains(other))
                return;

            AdjacentToTitleSet.Add(other);
            other.AdjacentToTitleSet.Add(this);
          
        }


        public void RemoveVassal(TitleParser titleParser)
        {
            this.SubTitles.Remove(titleParser.Name);
            if (titleParser.Liege == this)
                titleParser.Liege = null;

            Scope.Remove(titleParser.Scope);
        }

        public void AddVassals(ICollection<TitleParser> vassals)
        {
            var a = vassals.ToArray();
            foreach (var titleParser in a)
            {
                SubTitles[titleParser.Name] = titleParser;
                titleParser.Liege = this;
                Scope.Add(titleParser.Scope);
            }
        }

        public TitleParser GetRandomLowRankLandedTitle()
        {
            List<TitleParser> choices = new List<TitleParser>();

            GetRandomLowRankLandedTitle(choices);
            if (choices.Count == 0)
                return null;
            return choices[Rand.Next(choices.Count)];
     
        }
        
        public void GetRandomLowRankLandedTitle(List<TitleParser> choices)
        {
            if(this.Owns.Count > 0 && this.Holder.PrimaryTitle.Rank==1)
                choices.Add(this);

            foreach (var titleParser in SubTitles)
            {
                titleParser.Value.GetRandomLowRankLandedTitle(choices);
            }

            if (choices.Count == 0)
                return;

        }

        public void SplitLands()
        {
          
            if (Rank == 2)
            {
                List<ProvinceParser> titles = GetAllProvinces();
                List<ProvinceParser> half = GetAdjacentGroup(titles, titles.Count / 2);
                List<TitleParser> tits = new List<TitleParser>();
                foreach (var provinceParser in half)
                {
                    tits.Add(TitleManager.instance.TitleMap[provinceParser.title]);
                }
              //  TitleManager.instance.PromoteNewRuler(TitleManager.instance.CreateDuke(tits));
            }
        }

        private List<ProvinceParser> GetAdjacentGroup(List<ProvinceParser> provinces, int preferedSize)
        {
            List<ProvinceParser> split = new List<ProvinceParser>();

            var start = provinces[Rand.Next(provinces.Count)];

            foreach (var provinceParser in provinces)
            {
                if(start.Adjacent.Contains(provinceParser))
                    split.Add(provinceParser);

                if (split.Count >= preferedSize)
                    break;
            }

            if (split.Count < preferedSize)
            {
                foreach (var provinceParser in provinces)
                {
                    var a = split.ToArray();
                    foreach (var chosen in a)
                    {
                        if (start.Adjacent.Contains(provinceParser))
                            split.Add(provinceParser);

                        if (split.Count >= preferedSize)
                            break; 
                    }
                 
                }

            }

            return split;
        }

        public List<ProvinceParser> GetAllProvinces()
        {
            List<ProvinceParser> provinces = new List<ProvinceParser>();

            GetAllProvinces(provinces);

            return provinces;
        }

        public List<ProvinceParser> GetAllProvinces(List<ProvinceParser> provinces)
        {

            if(Owns.Count > 0 && !provinces.Contains(Owns[0]))
                provinces.Add(Owns[0]);
        
            foreach (var subTitle in SubTitles)
            {
                if (subTitle.Value.Rank >= Rank)
                    continue;

                subTitle.Value.GetAllProvinces(provinces);
            }

            return provinces;
        }

        public void Kill()
        {
            if (this.Liege != null)
            {
                this.Liege.SubTitles.Remove(this.Name);
            }

            this.Scope.Parent.Remove(this.Scope);
        }

        public bool AnyHolder()
        {
            return Holder != null || CurrentHolder != null;
        }
    }
}
