using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CrusaderKingsStoryGen.Simulation
{
    class CharacterParser : Parser
    {
        public string religion = "pagan";
        public string culture = "norse";
        public List<TitleParser> Titles = new List<TitleParser>();
        private Color _color;
        private TitleParser _primaryTitle;

        public void DoDynasty()
        {
            var d = DynastyManager.instance.GetDynasty(CultureManager.instance.CultureMap[culture]);
            Scope.Delete("dynasty");
            Scope.Delete("father");
            Scope.Delete("mother");
            Scope.Add("dynasty", d.ID);
            if (Father != null)
                Scope.Add("father", Father.ID);
            if (Mother != null)
                Scope.Add("mother", Mother.ID);
            if (!d.Members.Contains(this))
                d.Members.Add(this);

            this.Dynasty = d;

        }

        public void SetupExistingDynasty()
        {
            var d = this.Dynasty;
            Scope.Delete("dynasty");
            Scope.Delete("father");
            Scope.Delete("mother");
            Scope.Add("dynasty", d.ID);
            if (Father != null)
                Scope.Add("father", Father.ID);
            if (Mother != null)
                Scope.Add("mother", Mother.ID);
            if (!d.Members.Contains(this))
                d.Members.Add(this);

            this.Dynasty = d;

        }

        public Dynasty Dynasty { get; set; }

        private static int ccc = 0;
        public void UpdateCultural()
        {
            ccc++;
            if (ccc > 10)
            {
                var sub = Titles[0].SubTitles;
            }
            var top = TopLiegeCharacter;
            this.religion = top.religion;
            if (top.culture == null)
            {
                top.culture = "norse";
            }
            this.culture = top.culture;
            if (religion != "pagan")
            {

            }
            SetProperty("religion", religion);

            if (isFemale)
            {
                Scope.Delete("female");
                Scope.Add(new ScriptCommand("female", true, Scope));
            }
            SetProperty("culture", culture);
            SetProperty("name", CultureManager.instance.CultureMap[culture].PickCharacterName(isFemale));
            foreach (var titleParser in Titles)
            {
                if (titleParser.Owns.Count > 0)
                {
                    titleParser.Owns[0].SetProperty("religion", religion);
                    titleParser.Owns[0].SetProperty("culture", culture);
                }
                else
                {
                    foreach (var subTitle in titleParser.SubTitles)
                    {
                        if (subTitle.Value.Holder != null)
                            subTitle.Value.Holder.UpdateCultural();
                    }
                }
            }
            if (!Scope.HasNamed("dynasty"))
                DoDynasty();
            else
                SetupExistingDynasty();

            ccc--;
        }

        public bool isFemale { get; set; }

        public CharacterParser Liege
        {
            get
            {
                if (PrimaryTitle == null)
                    return null;

                if (PrimaryTitle.Liege == null)
                    return null;

                if (PrimaryTitle.Liege.Holder == this)
                    return null;

                return PrimaryTitle.Liege.Holder;
            }
        }
        public int PreferedKingdomSize { get; set; }
        public static Random rand = new Random();
        public float Aggressiveness = 0;
        public int ID = 1000000;
        public static int IDMax = 1000000;
        public int ConquererTimer = 0;
        public int MaxDeminse = 4;
        public Color Color
        {
            get
            {
                var chr = TopLiegeCharacter;
                if (chr == this)
                    return _color;
                return chr._color;
            }
            set { _color = value; }
        }

        public override string ToString()
        {
            return ID.ToString();
        }

        public CharacterParser(ScriptScope scope)
            : base(scope)
        {
            ID = IDMax++;
            YearOfBirth = 769 - (Rand.Next(80));
            YearOfDeath = 769 + Rand.Next(30);
            Color = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            Aggressiveness = rand.Next(100) / 100.0f;
            MaxDeminse = rand.Next(4) + 3;
            PreferedKingdomSize = rand.Next(6) + 3;
        }

        public bool bKill = false;
        public CharacterParser()
            : base(CharacterManager.instance.GetNewCreatedCharacter())
        {
            ID = Convert.ToInt32(Scope.Name);
            IDMax = ID + 1;
            CharacterManager.instance.Unpicked.Remove(Scope);
            Color = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            Aggressiveness = rand.Next(100) / 100.0f;
            MaxDeminse = rand.Next(4) + 3;
            PreferedKingdomSize = rand.Next(6) + 3;
            int b = 769 - (16 + Rand.Next(50));
            YearOfBirth = 768 - (Rand.Next(80));
            YearOfDeath = 770 + Rand.Next(30);
         //   CharacterManager.instance.SetAllDates(YearOfBirth, YearOfDeath, Scope);
        }
        public CharacterParser(bool adult = false)
            : base(CharacterManager.instance.GetNewCreatedCharacter())
        {
            ID = Convert.ToInt32(Scope.Name);
            IDMax = ID + 1;
            CharacterManager.instance.Unpicked.Remove(Scope);
            Color = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            Aggressiveness = rand.Next(100) / 100.0f;
            MaxDeminse = rand.Next(4) + 3;
            PreferedKingdomSize = rand.Next(6) + 3;
            int b = 769 - (16 + Rand.Next(50));
            YearOfBirth = 768 - (Rand.Next(80));
            YearOfDeath = 770 + Rand.Next(30);
            if (adult && Age < 16)
            {
                YearOfBirth -= 16 - Age;
            }
        //    CharacterManager.instance.SetAllDates(YearOfBirth, YearOfDeath, Scope);
        }

        public void Tick()
        {
            int lands = 0;
            foreach (var titleParser in this.Titles)
            {
                lands += titleParser.LandedTitlesCount;
            }
            if (PrimaryTitle.Government == null)
            {
                PrimaryTitle.Government = GovernmentManager.instance.CreateNewGovernment(Culture);
            }
            if (lands == 0)
            {
                var title = PrimaryTitle.GetRandomLowRankLandedTitle();
                if (title != null)
                    GiveTitle(title);
                else
                {
                    bKill = true;
                }
            }
            //    ConvertCountTitlesToDuchies();
            /*     if (PrimaryTitle != null && PrimaryTitle.Rank >= 3)
                 {
                     if (PrimaryTitle.Rank == 2)
                     {
                         if (PrimaryTitle.SubTitles.Count < 3)
                         {
                        
                         }
                         else
                         {
                             return;
                         }
                     }
                     else
                     {
                         return;
                     }
                 }*/
            if (PrimaryTitle.DirectVassalLandedTitlesCount > 5)
            {
                PrimaryTitle.SplitLands();
            }
            ConquererTimer--;
            if (rand.Next(10000) == 0)
                ConquererTimer = 1000 + rand.Next(2000);
            if (Titles.Count == 0)
                return;

            if (rand.Next((int)(2 * (1.0f - Aggressiveness))) != 0)
                return;

            List<ProvinceParser> targets = new List<ProvinceParser>();
            List<ProvinceParser> hightargets = new List<ProvinceParser>();
            List<ProvinceParser> test = new List<ProvinceParser>();
            foreach (var titleParser in Titles)
            {
                foreach (var provinceParser in titleParser.Owns)
                {
                    if (provinceParser.land)
                        test.Add(provinceParser);

                }

                titleParser.AddChildProvinces(test);
            }


            foreach (var provinceParser in test)
            {
                foreach (var parser in provinceParser.Adjacent)
                {
                    if (!parser.land)
                        continue;

                    if (parser.title == null)
                    {
                        parser.RenameForCulture(Culture);
                        parser.CreateTitle();
                  //      if (Culture.dna.horde)
                    //        parser.Title.Scope.Do("historical_nomad = yes");
         
                        parser.CreateProvinceDetails(Culture);
                    }

                    if (parser.ProvinceOwner != null && parser.ProvinceOwner.Rank > TopLiegeCharacter.PrimaryTitle.Rank)
                        continue;
                    if (PrimaryTitle.Rank >= 2 && parser.Title.Rank >= 2 &&
                        parser.Title.LandedTitlesCount < 2 && PrimaryTitle.LandedTitlesCount < 5)
                    {
                        PrimaryTitle.AddVassals(parser.Title.SubTitles.Values);
                        continue;
                    }
                    if (parser.Title.Holder != null)
                    {
                        continue;
                        if (parser.Title.Holder == this || parser.Title.Holder.TopLiegeCharacter == this.TopLiegeCharacter)
                        {
                            {
                                continue;
                            }

                        }
                    }

                    if (!targets.Contains(parser))
                    {
                        TitleParser t = TitleManager.instance.TitleMap[parser.title];
                        if (!t.Claimed)
                            hightargets.Add(parser);
                        targets.Add(parser);
                    }
                }
            }

            if (targets.Count == 0)
                return;
            if (hightargets.Count > 0)
                targets = hightargets;

            int max = 1;
            if (ConquererTimer > 0)
                max = Math.Min(targets.Count, max);
            for (int i = 0; i < max; i++)
            {
                ProvinceParser t = targets[rand.Next(targets.Count)];
                TitleParser tit = TitleManager.instance.TitleMap[t.title];
                //    while (tit.Liege != null)
                //       tit = tit.Liege;


                GiveTitle(tit);
                targets.Remove(t);

            }

        }

        public CharacterParser TopLiegeCharacter
        {
            get
            {
                if (Liege == null)
                    return this;
                var liege = Liege;

                while (liege.Liege != null && liege.Liege.PrimaryTitle.Rank > liege.PrimaryTitle.Rank)
                {
                    liege = liege.Liege;
                }

                return liege;
            }
        }

        public TitleParser PrimaryTitle
        {
            get
            {
                if (_primaryTitle != null && _primaryTitle.Holder != this)
                    _primaryTitle = null;
                if (_primaryTitle != null)
                    return _primaryTitle;
                if (Titles.Count == 0)
                    return null;
                if (_primaryTitle == null)
                    _primaryTitle = Titles[0];

                return _primaryTitle;
            }
            set { _primaryTitle = value; }
        }

        //attack / Capture duchies
        public void GiveTitle(TitleParser t)
        {
            if (PrimaryTitle != null && t.Rank > PrimaryTitle.Rank)
            {
                return;
            }

            t.CurrentHolder = null;
            if (Titles.Contains(t))
                return;


            if (t.Holder != null)
            {
                if (t.Holder.PrimaryTitle == t)
                    t.Holder.PrimaryTitle = null;

                t.Holder.Titles.Remove(t);                
            }

            t.Active = true;
            t.Holder = this;
            _primaryTitle = null;
            if ((PrimaryTitle == null || t.Rank > PrimaryTitle.Rank))
            {
                Titles.Insert(0, t);
                PrimaryTitle = t;
            }
            else
            {
                Titles.Add(t);
            }
            foreach (var titleParser in t.SubTitles)
            {
                titleParser.Value.Liege = t;
            }

            if (t.Owns.Count > 0)
            {
                if (this.GetProperty("culture") != null)
                    t.Owns[0].SetProperty("culture", this.GetProperty("culture").Value);
                if (this.GetProperty("religion") != null)
                    t.Owns[0].SetProperty("religion", this.GetProperty("religion").Value);
                t.Owns[0].ProvinceOwner = t;

            }
            SetColorOfChildren(t);

        }

        private void SetColorOfChildren(TitleParser t)
        {
            if (t.Owns.Count > 0)
            {
                t.Owns[0].SetProperty("color", Color);
                t.Owns[0].SetProperty("color2", Color);
            }

            foreach (var titleParser in t.SubTitles)
            {
                if (titleParser.Value.Rank >= PrimaryTitle.Rank)
                {
                    continue;
                }
                SetColorOfChildren(titleParser.Value);
            }
        }

        public List<ProvinceParser> GetProvinceGroup(int i, CharacterParser chr)
        {
            List<ProvinceParser> list = new List<ProvinceParser>();
            List<ProvinceParser> list2 = new List<ProvinceParser>();
            foreach (var titleParser in Titles)
            {
                titleParser.GetAllProvinces(list);
            }

            foreach (var provinceParser in list)
            {
                if (!provinceParser.land)
                    continue;
                if (provinceParser.title == null)
                    continue;

                if (!(provinceParser.Title.Liege == null && (chr == provinceParser.Title.Holder || chr == null)))
                {
                    list2.Add(provinceParser);

                }
            }
            if (list2.Count == 0)
                return list2;
            ProvinceParser p = null;
            p = list2[Rand.Next(list2.Count)];

            List<ProvinceParser> provinces = new List<ProvinceParser>();
            provinces.Add(p);

            int last = provinces.Count;

            do
            {
                last = provinces.Count;
                MapManager.instance.FindAdjacent(provinces, i - provinces.Count, chr);

            }
            while (last != provinces.Count && provinces.Count < i);

            return provinces;
        }

        public List<ProvinceParser> GetProvinceGroupSameRealm(int i, CharacterParser chr)
        {
            List<ProvinceParser> list = new List<ProvinceParser>();
            List<ProvinceParser> list2 = new List<ProvinceParser>();
            foreach (var titleParser in Titles)
            {
                titleParser.GetAllProvinces(list);
            }

            foreach (var provinceParser in list)
            {
                //      if ((provinceParser.Title.SameRealm(chr.PrimaryTitle)))
                {
                    list2.Add(provinceParser);

                }
            }
            if (list2.Count == 0)
                return list2;
            ProvinceParser p = null;
            p = list2[Rand.Next(list2.Count)];

            List<ProvinceParser> provinces = new List<ProvinceParser>();
            provinces.Add(p);

            int last = provinces.Count;

            do
            {
                last = provinces.Count;
                MapManager.instance.FindAdjacentSameRealm(provinces, i - provinces.Count, chr);

            }
            while (last != provinces.Count && provinces.Count < i);

            return provinces;
        }
        static Stack<List<ProvinceParser>> resultsStack = new Stack<List<ProvinceParser>>();
        public void ConvertCountTitlesToDuchies()
        {
            if (Titles.Count == 0)
                return;

            int last = 0;
            int timeout = 30;
            List<ProvinceParser> homeless = new List<ProvinceParser>();
            while (this.NumberofCountTitles > 0 && homeless.Count < this.NumberofCountTitles && timeout > 0)
            {
                int nc = this.NumberofCountTitles;
                if (last == nc)
                    timeout--;
                else if (timeout <= 0)
                    break;
                else
                    timeout = 10;
                if (timeout == 1)
                {

                }
                last = nc;
                int duchySize = 3 + rand.Next(2);
                var results = this.GetProvinceGroupSameRealm(duchySize, this);
                if (results.Count < 2)
                {
                    
                }
                for (int index = 0; index < results.Count; index++)
                {
                    var provinceParser = results[index];
                    if (provinceParser.Title.Liege != null && provinceParser.Title.Liege != provinceParser.Title.TopmostTitle)
                    {
                        results.Remove(provinceParser);
                        index--;
                    }
                }
                if (results.Count == 1 && results[0].Title.Liege == null)
                {
                    if (!homeless.Contains(results[0]))
                    {
                        homeless.Add(results[0]);
                        timeout++;
                    }
                }

                CharacterManager.instance.Prune();
                if (results.Count <= 1)
                    continue;

                foreach (var provinceParser in results)
                {
                    this.RemoveTitle(provinceParser.Title);
                }
                ProvinceParser capital = results[rand.Next(results.Count)];

                var chr = SimulationManager.instance.AddCharacter(culture, religion);
                var title = TitleManager.instance.CreateDukeScriptScope(capital, chr);
                chr.GiveTitle(title);

                //    results.RemoveAt(0);
                int n = 0;
                foreach (var provinceParser in results)
                {
                    //   var ruler = TitleManager.instance.PromoteNewRuler(provinceParser.Title);
                    //  SimulationManager.instance.characters.Remove(ruler);
                    if (provinceParser.Title.Holder == null)
                        SimulationManager.instance.AddCharacterForTitle(provinceParser.Title);

                    title.AddSub(provinceParser.Title);
                    chr.GiveTitleAsHolder(provinceParser.Title);
                    if (Rand.Next(2) == 0 && n != 0)
                    {
                        SimulationManager.instance.AddCharacterForTitle(provinceParser.Title);
                    }
                    n++;
                    //  this.PrimaryTitle.RemoveVassal(provinceParser.Title);
                }


                //     PrimaryTitle.AddSub(title);
                CharacterManager.instance.Prune();
            }
            List<ProvinceParser> blankPlacesToSpreadTo = new List<ProvinceParser>();
            List<ProvinceParser> blankPlacesToSteal = new List<ProvinceParser>();
            while (homeless.Count > 0)
            {
                for (int index = 0; index < homeless.Count; index++)
                {
                    var homelessCounty = homeless[index];


                    {
                        TitleParser smallestDuchy = null;
                        int smallest = 10000;
                        foreach (var provinceParser in homelessCounty.Adjacent)
                        {
                            if (!provinceParser.land)
                                continue;
                            
                            if (provinceParser.title == null)
                            {
                                blankPlacesToSpreadTo.Add(provinceParser);
                                continue;

                            }

                            if (provinceParser.Title.Liege != null)
                            {
                                if (smallest > provinceParser.Title.Liege.SubTitles.Count)
                                {
                                    smallest = provinceParser.Title.Liege.SubTitles.Count;
                                    smallestDuchy = provinceParser.Title.Liege;
                                }
                            }
                            else
                            {
                                blankPlacesToSteal.Add(provinceParser);
                           
                            }
                           
                        }

                            if (smallestDuchy != null)
                            {
                                var title2 = smallestDuchy;

                                // SimulationManager.instance.characters.Remove(ruler);
                                title2.AddSub(homelessCounty.Title);
                                SimulationManager.instance.AddCharacterForTitle(homelessCounty.Title);
                                homeless.Remove(homelessCounty);
                                index--;
                                break;
                            }
                            else
                            {
                                if (blankPlacesToSpreadTo.Count > 0)
                                {
                                    var province = blankPlacesToSpreadTo[0];

                                    var chr = SimulationManager.instance.AddCharacter(culture, religion);
                                    var title = TitleManager.instance.CreateDukeScriptScope(province, chr);
                                    chr.GiveTitle(title);
                                    var ctitle = province.CreateTitle();
                                    province.title = ctitle.Name;
                                    title.AddSub(ctitle);
                                    title.AddSub(homelessCounty.Title);
                                    chr.GiveTitleAsHolder(province.Title);
                                    chr.GiveTitleAsHolder(homelessCounty.Title);
                                    homeless.Remove(homelessCounty);
                                    index--;

                                } else if (blankPlacesToSteal.Count > 0)
                                {
                                    var province = blankPlacesToSteal[0];

                                    var chr = SimulationManager.instance.AddCharacter(culture, religion);
                                    var title = TitleManager.instance.CreateDukeScriptScope(province, chr);
                                    chr.GiveTitle(title);
                                    if(province.Title.Holder != null)
                                        province.Title.Holder.RemoveTitle(province.Title);
                                    homelessCounty.Title.Holder.RemoveTitle(homelessCounty.Title);
                                    title.AddSub(province.Title);
                                    title.AddSub(homelessCounty.Title);
                                    chr.GiveTitleAsHolder(province.Title);
                                    chr.GiveTitleAsHolder(homelessCounty.Title);
                                    homeless.Remove(homelessCounty);
                                    index--;
                                }
                                else
                                {
                                    
                                }
                           }


                        
                    }

                }

            }

            if (homeless.Count > 0)
            {
                
            }
            for (int index = 0; index < Titles.Count; index++)
            {

                var titleParser = Titles[index];
                if (titleParser.Liege == null)
                    titleParser.Liege = PrimaryTitle;
                if (titleParser.Rank == 1)
                {

                    //  if (titleParser.Liege.Rank==2)
                    //       GiveTitle(titleParser.Liege);
                }

            }
            /*
            // now if we're an empire, create some kings! Eep...

            if (PrimaryTitle.Rank == 4)
            {
                timeout = 10;
                while (this.NumberofDukeVassals > 1 && timeout > 0)
                {
                    int nc = this.NumberofDukeVassals;
                    if (last == nc)
                        timeout--;
                    else if (timeout <= 0)
                        break;
                    else
                        timeout = 10;
                    if (timeout == 1)
                    {

                    }
                    last = nc;
                    int kingdomSize = 16 + rand.Next(16);
                    var results = this.GetProvinceGroupSameRealm(kingdomSize, this);
                    for (int index = 0; index < results.Count; index++)
                    {
                        var provinceParser = results[index];
                        if (provinceParser.Title.Liege == null)
                        {
                            results.Remove(provinceParser);
                            index--;
                        }
                    }
                  
                    CharacterManager.instance.Prune();
                    if (results.Count <= 16)
                        continue;

                    ProvinceParser capital = results[rand.Next(results.Count)];
                    var title = TitleManager.instance.CreateKingScriptScope(capital);
                    var chr = SimulationManager.instance.AddCharacterForTitle(title);
                    chr.GiveTitle(title);

                    //    results.RemoveAt(0);
                    foreach (var provinceParser in results)
                    {
                        if (!provinceParser.land)
                            continue;
                        if (provinceParser.title == null)
                            continue;

                        var ruler = provinceParser.Title.Liege;
                        if (provinceParser.Title.Liege != null && provinceParser.Title.Liege.Rank == 2)
                        {
                            if (provinceParser.Title.Liege != title)
                            {
                                if(ruler.Liege != null)
                                    ruler.Liege.RemoveVassal(ruler);
                                title.AddSub(ruler);
                                
                            }
                        }
                        //  this.PrimaryTitle.RemoveVassal(provinceParser.Title);
                        if (Rand.Next(10) == 0 && NumberofCountTitles < 6)
                        {
                            GiveTitleAsHolder(title);
                                            
                        }
                    }
                    PrimaryTitle.AddSub(title);
                    CharacterManager.instance.Prune();
                }

            }
            */
            /*
                        // Now we're at duke level, do the same for kings (but getting entire duchies into the results list...
                        last = 0;
                        timeout = 30; 
                        while (this.NumberofDukeVassals > 0)
                        {
                
                            if (last == NumberofDukeTitles)
                                timeout--;
                            if (timeout <= 0)
                                break;
                            last = NumberofDukeTitles;
                             int kingdomSize = 16 + rand.Next(32);
                            var results = this.GetProvinceGroup(kingdomSize, this);
                            for (int index = 0; index < results.Count; index++)
                            {
                                var provinceParser = results[index];
                                if (provinceParser.Title.Liege != null)
                                {
                                    foreach (var titleParser in provinceParser.Title.Liege.SubTitles)
                                    {
                                        if (titleParser.Value.Owns.Count > 0 && !results.Contains(titleParser.Value.Owns[0]))
                                        {
                                            results.Add(titleParser.Value.Owns[0]);
                                        }
                                    }
                                }
                            }
                            ProvinceParser capital = results[rand.Next(results.Count)];
                            var title = TitleManager.instance.CreateKingScriptScope(capital);
                            var chr = SimulationManager.instance.AddCharacterForTitle(title);
                          //   chr.GiveTitle(title);

                            //    results.RemoveAt(0);
                            foreach (var provinceParser in results)
                            {
                               // var ruler = SimulationManager.instance.AddCharacterForTitle(provinceParser.Title.Liege);
                               // SimulationManager.instance.characters.Remove(ruler);
                                title.AddSub(provinceParser.Title.Liege);
                                this.PrimaryTitle.RemoveVassal(provinceParser.Title.Liege);
                            }
                            PrimaryTitle.AddSub(title);
     
                        }*/
            return;
            if (Titles.Count == 0)
                return;
            var t = Titles[0].GetAllProvinces()[0];
            GiveTitle(t.Title);
            GiveTitle(t.Title.Liege);
            {

            }
            while (this.NumberofDukeTitles > 0)
            {
                int duchySize = 2 + rand.Next(3);
                var results = this.GetProvinceGroup(duchySize, this);
                ProvinceParser capital = results[rand.Next(results.Count)];
                var title = TitleManager.instance.CreateDukeScriptScope(capital);
                var chr = SimulationManager.instance.AddCharacterForTitle(title);
                chr.GiveTitle(title);

                //    results.RemoveAt(0);
                foreach (var provinceParser in results)
                {
                    var ruler = TitleManager.instance.PromoteNewRuler(provinceParser.ProvinceOwner);
                    SimulationManager.instance.characters.Remove(ruler);
                    title.AddSub(provinceParser.Title);
                }

            }
            if (Titles.Count == 0)
                return;
            /*
            
                     int numCounts = this.NumberofCountTitles;
                     int DukeSize = 3;
                     int KingSize = 16;
                     int EmperorSize = 60;
                     if (numCounts > EmperorSize && PrimaryTitle.Liege == null)
                     {
                         var capital = PrimaryTitle.Owns[0];
                         var title = TitleManager.instance.CreateEmperor(capital);
                         GiveTitle(title);

                         while (numCounts > 1)
                         {
                            // Make me an emperor, create new vassals with collections of counts in them > 16, then vassalize them
                            List<ProvinceParser> results = GetProvinceGroup(rand.Next((KingSize / 2), KingSize * 4));
                             results.Remove(capital);
                            if (results.Count > 0)
                            {
                                var king = results[0].Title;// TitleManager.instance.CreateKingScriptScope(results[0]);
                                var chr = SimulationManager.instance.AddCharacterForTitle(king);
                            //    results.RemoveAt(0);
                                foreach (var provinceParser in results)
                                {
                                    chr.GiveTitle(provinceParser.Title);
                                }
                                //king.Liege = title;
                              //  chr.ConvertCountTitlesToDuchies();
                                chr.PrimaryTitle.Liege = title;
                                foreach (var provinceParser in results)
                                {
                                    chr.PrimaryTitle.AddSub(provinceParser.Title);
                                } 

                            }
                            numCounts = this.NumberofCountTitles;
                         }
                         return;
                     }
                     if (numCounts > KingSize && (PrimaryTitle.Liege == null || Liege.PrimaryTitle.Rank == 4))
                     {
                         var capital = PrimaryTitle.Owns[0];
                         var title = TitleManager.instance.CreateKingScriptScope(capital);
                         GiveTitle(title);

                         while (numCounts > 1)
                         {
                             List<ProvinceParser> results = GetProvinceGroup(rand.Next((DukeSize), (DukeSize * 2)-1));
                             results.Remove(capital); 
                             if (results.Count > 0)
                             {
                                 var duke = results[0].Title;// TitleManager.instance.CreateKingScriptScope(results[0]);
                                 chr = CharacterManager.instance.GetNewCharacter();
                                 chr.GiveTitle(title);
          
                        
                                 //    results.RemoveAt(0);
                                 foreach (var provinceParser in results)
                                 {
                                     chr.GiveTitle(provinceParser.Title);

                                 }
                             //    duke.Liege = title;
                                 chr.PrimaryTitle = null;
                                 chr.ConvertCountTitlesToDuchies();
                                 foreach (var provinceParser in results)
                                 {
                                     chr.PrimaryTitle.AddSub(provinceParser.Title);
                                 }
                             }
                             numCounts = this.NumberofCountTitles;
                         }
                         return;
                         // Make me a king, create new vassals with collectin of counts in them >= 3 then feudalize them
                     }
                     if (Liege == null)
                     {
                
                     }
                     if (numCounts >= DukeSize && (PrimaryTitle.Liege == null || Liege.PrimaryTitle.Rank >= 3))
                     {
                         var capital = PrimaryTitle.Owns[0];
                         var title = TitleManager.instance.CreateDukeScriptScope(capital);
                         GiveTitle(title);

                         while (numCounts > 1)
                         {
                             List<ProvinceParser> results = GetProvinceGroup(Math.Min(numCounts-1, rand.Next((DukeSize), (DukeSize * 2) - 1)));
                             results.Remove(capital);
                             if (results.Count > 0)
                             {
                        
                        
                                  //    results.RemoveAt(0);
                                 foreach (var provinceParser in results)
                                 {
                                     TitleManager.instance.PromoteNewRuler(provinceParser.Title);
                           
                                 }
                      
                                // chr.ConvertCountTitlesToDuchies();
                             }
                             numCounts = this.NumberofCountTitles;
                         }

                         return;
                         // Make me a duke, create new count vassals.
                     }
                     PrimaryTitle = null;
                  /*   int numCounts = this.NumberofCountTitles;
                     int nGiveup = 10;
                     while (numCounts > MaxDeminse && nGiveup > 0)
                     {
                         var duke= CreateDuke();
                         numCounts = this.NumberofCountTitles;
                         nGiveup--;
                         if (PrimaryTitle.Rank == 3)
                         {
                             if (duke != null)
                             {
                                 TitleManager.instance.PromoteNewRuler(duke);
                             }
                         }
                     }
                     int numDukes = this.NumberofDukeTitles;
                     nGiveup = 10;
                     while (numDukes > PreferedKingdomSize && numDukes > 0)
                     {
                         bool bGive = false;
                         if (PrimaryTitle.Rank == 3)
                         {
                             bGive = true;
                         }

                         var king = CreateKing();
                         if (bGive)
                         {
                             if (king != null)
                             {
                                 TitleManager.instance.PromoteNewRuler(king);
                             }

                         }
                         numCounts = this.NumberofDukeTitles;
                         nGiveup--;
                     }
                     int numKings = this.NumberofKingTitles;
                     nGiveup = 10;
                     while (numKings > 1)
                     {
                         PrimaryTitle = null;
                         var prim = PrimaryTitle;
                         for (int index = 0; index < Titles.Count; index++)
                         {
                             var titleParser = Titles[index];
                             if (titleParser != prim && titleParser.Rank == 3)
                             {
                                 TitleManager.instance.PromoteNewRuler(titleParser);
                                 break;
                             }
                         }
                         numKings = this.NumberofKingTitles;
                     }

                     */
        }

        private void RemoveTitle(TitleParser title)
        {
            Titles.Remove(title);
            title.Holder = null;
        }

        public void GiveTitleAsHolder(TitleParser title)
        {
            title.CurrentHolder = this;
        }


        public int NumberofCountTitles
        {
            get
            {
                int c = 0;
                foreach (var titleParser in Titles)
                {
                    if (titleParser.Rank == 1)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public int NumberofDukeVassals
        {
            get
            {
                int c = 0;
                foreach (var titleParser in Titles)
                {
                    foreach (var sub in titleParser.SubTitles.Values)
                    {
                        if (sub.Rank == 2)
                        {
                            c++;
                        }
                    }

                }

                return c;
            }
        }
        public int NumberofKingTitles
        {
            get
            {
                int c = 0;
                foreach (var titleParser in Titles)
                {
                    if (titleParser.Rank == 3)
                    {
                        c++;
                    }
                }

                return c;
            }
        }
        public int NumberofDukeTitles
        {
            get
            {
                int c = 0;
                foreach (var titleParser in Titles)
                {
                    if (titleParser.Rank == 2)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public bool TickDisable { get; set; }

        public override ScriptScope CreateScope()
        {
            return null;
        }
        private void RemoveDeathDates(ScriptScope scope)
        {
            //     foreach (var child in scope.Children)
            {

                if (scope.Children.Count > 0)
                {
                    ScriptScope c = scope;
                    if (c.Children[0] is ScriptCommand && (c.Children[0] as ScriptCommand).Name == "death")
                    {
                        c.Parent.Children.Remove(c);
                        return;
                    }
                }
            }
        }
        public void MakeAlive()
        {
            var arr = Scope.Children.ToArray();
            //   Scope.Delete("father");
            foreach (var child in arr)
            {
                if (child is ScriptScope)
                {
                    ScriptScope scope = child as ScriptScope;
                    RemoveDeathDates(scope);
                }
            }
        }

        public void KillTitles()
        {
            foreach (var titleParser in Titles)
            {
                titleParser.Kill();
            }
        }

        public void MakeEmperor()
        {
            var emp = TitleManager.instance.CreateEmpireScriptScope(Titles[rand.Next(Titles.Count)].CapitalProvince);

            GiveTitle(emp);
            TitleParser tit = emp.CapitalProvince.Title;
            while (tit.Liege != null && !Titles.Contains(tit) && tit.Liege.Rank > tit.Rank)
                GiveTitleAsHolder(tit);
        }

        public void MakeKing()
        {
            var emp = TitleManager.instance.CreateKingScriptScope(Titles[rand.Next(Titles.Count)].CapitalProvince);

            GiveTitle(emp);
            GiveTitleAsHolder(emp.CapitalProvince.Title);
            TitleParser tit = emp.CapitalProvince.Title;
            while (tit.Liege != null && !Titles.Contains(tit) && tit.Liege.Rank > tit.Rank)
                GiveTitleAsHolder(tit);
        }

        public CharacterParser Father { get; set; }
        public CharacterParser Mother { get; set; }

        public List<CharacterParser> Kids = new List<CharacterParser>();

        public List<CharacterParser> Spouses = new List<CharacterParser>();
        public List<CharacterParser> Concubines = new List<CharacterParser>();

        public void AddDateEvent(int year, int month, int day, ScriptCommand command)
        {
            int indexForDate = 0;
            string date = year.ToString() + "." + month.ToString() + "." + day.ToString();
            int index = 0;
            bool found = false;
            ScriptScope foundScope = null;
            foreach (var child in Scope.Children)
            {
                if (child is ScriptScope)
                {
                    var scope = ((ScriptScope)child);
                    var split = scope.Name.Split('.');
                    if (split.Length == 3)
                    {
                        int y = Convert.ToInt32(split[0]);
                        int m = Convert.ToInt32(split[1]);
                        int d = Convert.ToInt32(split[2]);
                        if ((y == year && m == month && d == day))
                        {
                            found = true;
                            foundScope = child as ScriptScope;
                            break;
                        }
                        if (y > year || (y == year && m > month) || (y == year && m == month && d > day))
                        {
                            break;
                        }

                    }
                }
                index++;
            }

            indexForDate = index;
            ScriptScope s = null;
            if (found)
            {
                s = new ScriptScope(date);
            }
            else
            {
                s = new ScriptScope(date);
                Scope.Insert(indexForDate, s);
            }

            s.Add(command);
            command.Parent = s;
        }
        private CharacterParser GetSuitableMotherForChild(string religion, string culture, int min, int max, int year)
        {
            CharacterParser mother = null;

            if (Rand.Next(3) == 0)
            {
                mother = CharacterManager.instance.FindUnmarriedChildbearingAgeWomen(year, religion);
                if (mother == null)
                {
                    int nn = year - min;
                    int snn = year - max;
                    max -= 16 - snn;
                    if (max < min)
                        return null;
                    return CharacterManager.instance.CreateNewHistoricCharacter(DynastyManager.instance.GetDynasty(Culture), true, religion, culture, Rand.Next(min, max));
                }
            }
            else
            {
                int nn = year - min;
                int snn = year - max;
                max -= 16 - snn;
                if (max < min)
                    return null;
                return CharacterManager.instance.CreateNewHistoricCharacter(DynastyManager.instance.GetDynasty(Culture), true, religion, culture, Rand.Next(min, max));
            }

            return mother;
        }

        public void CreateFamily(int depth = 0, int maxdepth = 4, int minYearForHeirs = -1)
        {
     

            if (depth > maxdepth)
                return;

            int deathdate = -1;
            if (depth == 0)
            {
                deathdate = 768 - 1;
            }
            if (deathdate <= YearOfBirth)
                deathdate = YearOfBirth + 1;

            Father = CharacterManager.instance.CreateNewHistoricCharacter(Dynasty, false, religion, culture, this.YearOfBirth - (16 + Rand.Next(30)), deathdate);
            int max = YearOfBirth - 19;
            int min = Father.YearOfBirth - 5;

            Mother = GetSuitableMotherForChild(religion, culture, min, max, YearOfBirth);
            if (Mother.ID == 1002182)
            {

            }
            Father.Spouses.Add(Mother);
            Mother.Spouses.Add(Father);
            if (Age > 16 && Spouses.Count < Religion.max_wives && !isFemale)
            {
                int numWives = 1;
                if (Age > 30)
                {
                    numWives++;
                }
                if (Age > 40)
                {
                    numWives++;
                }

                numWives = 1;
                min = YearOfBirth - 3;
                max = YearOfBirth + 16;
                max = DateFunctions.MakeDOBAtLeastAdult(max);
                numWives = Math.Min(numWives, Religion.max_wives - Spouses.Count);

                int dateWhenSixteen = YearOfBirth + 16;
                int startAdulthood = dateWhenSixteen;
                if (minYearForHeirs > startAdulthood)
                    startAdulthood = minYearForHeirs;

            }

            if (Father.YearOfDeath < YearOfBirth)
                Father.YearOfDeath = YearOfBirth + 1;
            if (Mother.YearOfDeath < YearOfBirth)
                Mother.YearOfDeath = YearOfBirth + 1;

            Father.CreateFamily(depth + 1, maxdepth);
            Father.Kids.Add(this);
            Mother.Kids.Add(this);
            if (depth < 4)
                Mother.CreateFamily(depth + 1, maxdepth);

            Father.UpdateCultural();
            Mother.UpdateCultural();
            //  Father.SetupExistingDynasty();
            //   Mother.SetupExistingDynasty();
        }

        public CharacterParser CreateKidWith(CharacterParser otherParent, int dateOfBirth)
        {
            var kid = CharacterManager.instance.CreateNewHistoricCharacter(Dynasty, Rand.Next(2) == 0, religion, culture, dateOfBirth, -1, false);
            if (!isFemale)
            {
                kid.Father = this;
                kid.Mother = otherParent;
            }
            else
            {
                kid.Mother = this;
                kid.Father = otherParent;
            }
          
            kid.SetupExistingDynasty();
            Kids.Add(kid);
            otherParent.Kids.Add(kid);

            return kid;
        }

        public ReligionParser Religion
        {
            get { return ReligionManager.instance.ReligionMap[religion]; }
        }

        public int Age
        {
            get { return 769 - YearOfBirth; }
        }

        public int YearOfBirth { get; set; }
        public int YearOfDeath { get; set; }

        public CultureParser Culture
        {
            get { return CultureManager.instance.CultureMap[culture]; }
        }

        public TitleParser UsurpCountTitle()
        {
            var list = PrimaryTitle.GetAllProvinces();

            var options = new List<TitleParser>();
            foreach (var provinceParser in list)
            {
                if (!provinceParser.Title.AnyHolder() || provinceParser.GetCurrentHolder().PrimaryTitle.Rank == 1)
                {
                    if(!options.Contains(provinceParser.Title) && !Titles.Contains(provinceParser.Title))
                        options.Add(provinceParser.Title);
                }
            }

            if (options.Count > 0)
            {
                var r = options[Rand.Next(options.Count)];
                GiveTitleAsHolder(r);
                return r;

            }
            return null;
        }

        public void DoFamilyDatesOfBirth()
        {
            AddSiblings();
            CharacterManager.instance.SetAllDates(YearOfBirth, YearOfDeath, Scope);
              
            int birthYear = YearOfBirth - 1;
            int deathYear = YearOfBirth + 1;
            if (this.Father != null)
            {
                foreach (var characterParser in this.Father.Kids)
                {
                    if (characterParser.YearOfBirth - 1 < birthYear)
                        birthYear = characterParser.YearOfBirth - 1;
                    if (characterParser.YearOfBirth + 1 > deathYear)
                        deathYear = characterParser.YearOfBirth + 1;
                    if (characterParser != this)
                        CharacterManager.instance.SetAllDates(characterParser.YearOfBirth, characterParser.YearOfDeath, characterParser.Scope);
                }
            }
          
            // now we have a range that definitely covers all the kids.
            int parentMarriage = birthYear - 1;
            // now make the parents definitely 16 years old before marriage...
            int parentBirthYear = parentMarriage - 16;
            int parentDeathYear = deathYear + 1;
            if (Father != null)
            {
                Father.YearOfBirth = parentBirthYear - Rand.Next(10);
                Father.YearOfDeath = parentDeathYear + Rand.Next(50);
                
                Father.DoFamilyDatesOfBirth(); 
            }

            if (Mother != null)
            {
                Mother.YearOfBirth = parentBirthYear - Rand.Next(5);
                Mother.YearOfDeath = parentDeathYear + Rand.Next(50);
                Mother.DoFamilyDatesOfBirth(); 
            }

            if (Father != null && Mother != null)
            {
                Mother.AddDateEvent(parentMarriage, 1, 1, new ScriptCommand("add_spouse", Father.ID, null));
                Father.AddDateEvent(parentMarriage, 1, 1, new ScriptCommand("add_spouse", Mother.ID, null));
            }
        }

        public void AddSiblings()
        {
            if (Mother == null || Father == null)
                return;

            int num = Rand.Next(8);

            int yearOfBirth = YearOfBirth + 1;

            for (int n = 0; n < num; n++)
            {
                yearOfBirth += Rand.Next(3)+1;

                var kid = Father.CreateKidWith(Mother, yearOfBirth);

                kid.YearOfDeath = yearOfBirth + Rand.Next(60);
            }
        }
    }
}