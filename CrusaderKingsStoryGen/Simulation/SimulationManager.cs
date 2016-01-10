using System;
using System.Collections.Generic;

namespace CrusaderKingsStoryGen.Simulation
{
    class SimulationManager
    {
        public static SimulationManager instance = new SimulationManager();
        public List<CharacterParser> characters = new List<CharacterParser>();
        public int Year = 0;
        public bool Active { get; set; }
        public void Init()
        {
        //    ModularFunctionalityManager.instance.Save();
            DynastyManager.instance.Init();
            CharacterManager.instance.Init();
            CulturalDnaManager.instance.Init();
            CultureManager.instance.Init();
            EventManager.instance.Load();
            DecisionManager.instance.Load();
            TraitManager.instance.Init();
            SpriteManager.instance.Init();
            foreach (var titleParser in TitleManager.instance.Titles)
            {
                titleParser.DoCapital();
            }
            ReligionManager.instance.Init();
            CharacterParser chr = CharacterManager.instance.GetNewCharacter();
            characters.Add(chr);

            Random rand = new Random(); 
            for (int n = 0; n < 1; n++)
            {
                
                ScriptScope s = new ScriptScope();
                string name = chr.Culture.dna.GetPlaceName();

            
                s.Name = StarNames.SafeName(name);
                LanguageManager.instance.Add(s.Name, name);
                //872 vanilla

                MapManager.instance.ProvinceIDMap[872].RenameForCulture(chr.Culture);
                var tit = MapManager.instance.ProvinceIDMap[872].CreateTitle();
            //    if (chr.Culture.dna.horde)
            //        tit.Scope.Do("historical_nomad = yes");
                chr.GiveTitle(tit);
                MapManager.instance.ProvinceIDMap[872].CreateProvinceDetails(chr.Culture);
              
             
            }
        }

        public CharacterParser AddCharacterForTitle(TitleParser title, String culture, String religion)
        {
            CharacterParser chr = CharacterManager.instance.CreateNewCharacter(culture, religion, Rand.Next(8) == 0);
            chr.GiveTitle(title);
            if (title.Rank > 0)
                characters.Add(chr);

            return chr;
        }
        public CharacterParser AddCharacter(String culture, String religion)
        {
            CharacterParser chr = CharacterManager.instance.CreateNewCharacter(culture, religion, Rand.Next(8) == 0);
           
            return chr;
        }

        public CharacterParser AddCharacterForTitle(TitleParser title, bool adult = false)
        {
            CharacterParser chr = CharacterManager.instance.GetNewCharacter();
            chr.GiveTitle(title);
            if (title.Rank > 0)
                characters.Add(chr);

            return chr;
        }
        public void Tick()
        {

            if (!Active)
                return;

            CharacterManager.instance.Prune();
            MapManager.instance.LockRenderBitmap();
            foreach (var religionParser in ReligionManager.instance.AllReligions)
            {
                religionParser.TryFillHolySites();
            }
            for (int n = 0; n < 100; n++)
            {

                for(int x=0;x<characters.Count;x++)
                {
               
                    CharacterParser character = characters[x];
                    if (character.Liege != null)
                        continue;
                    if (character.PrimaryTitle == null)
                        continue;

                    if (character.PrimaryTitle.Government != null)
                    {
                     
                        if (!character.PrimaryTitle.Government.cultureAllow.Contains(character.culture))
                        {
                            if (character.culture == "norse")
                            {
                                
                            }
                            character.PrimaryTitle.Government.cultureAllow.Add(character.culture);
                            //Government.cultureDone.Add(character.culture + "_" + character.PrimaryTitle.Government.type);
      
                        }
                   
                    }
                   
                    character.UpdateCultural();
                    if (character.PrimaryTitle != null && character.PrimaryTitle.Liege == null)
                    {
                        if(!character.TickDisable)
                            character.Tick();
                       
                    }
                    float chanceOfRevolt = 1.0f;

                    chanceOfRevolt *= Globals.BaseChanceOfRevolt;
                    int i = character.NumberofCountTitles;
                    if (i == 0)
                        i++;
                    chanceOfRevolt /= i;
                    int c = character.NumberofCountTitles;
                    if (c == 0)
                        continue;

                    if (c < 10)
                        continue;
                    if (c > 150)
                        chanceOfRevolt /= 100;

                    if (Rand.Next((int)chanceOfRevolt) == 0)
                    {
                        HandleRevolt(character);
                        character.TickDisable = true;
                    }
                    if (character.Titles.Count == 0 || character.bKill)
                    {
                        characters.Remove(character);
                        character.KillTitles();
                        x--;
                    }
                }
                Year++;
                if (Year == 5000)
                {
              //      Form1.instance.Export();
                }
            }
            
            MapManager.instance.UnlockRenderBitmap();
        }

        private void HandleRevolt(CharacterParser character)
        {
            if (character.NumberofCountTitles > 0)
            {
                foreach (var titleParser in character.Titles)
                {
                    if (titleParser.Owns.Count > 0)
                    {
                        if (
                            !ReligionManager.instance.ReligionMap[character.religion].Believers.Contains(
                                titleParser.CapitalProvince))
                        {
                            ReligionManager.instance.ReligionMap[character.religion].Believers.Add(titleParser.CapitalProvince);
                        }
                    }
                }
            }    
            var pr = new List<ProvinceParser>();

            foreach (var title in character.Titles)
            {
                title.AddChildProvinces(pr);
                if (title.Owns.Count > 0)
                {
                    bool bDo = false;
                    foreach (var provinceParser in title.Owns[0].Adjacent)
                    {
                        if (provinceParser.land && (provinceParser.title == null || provinceParser.Title.Holder == null))
                        {
                            bDo = true;
                            break;
                        }
                    }
                    if(bDo)
                        pr.Add(title.Owns[0]);
                }
            }

            pr.Remove(character.PrimaryTitle.CapitalProvince);
            for (int index = 0; index < pr.Count; index++)
            {
                var provinceParser = pr[index];
                bool keep = false;
                foreach (var parser in provinceParser.Adjacent)
                {
                    if(parser.title == null)
                        continue;
                    ;
                    if (parser.Title.Holder == null)
                    {
                        keep = true;
                        break;
                    }
                }
                if (!keep)
                {
                    pr.Remove(provinceParser);
                    index--;
                }
              
            }
            if (pr.Count == 0)
                return;

            var p = pr[Rand.Next(pr.Count)];
            List<ProvinceParser> provinces = new List<ProvinceParser>();
            provinces.Add(p);
            while (provinces.Count < character.Titles.Count / 2)
            {
                MapManager.instance.FindAdjacent(provinces, provinces.Count/2);
            }
            var ch = CharacterManager.instance.GetNewCharacter();
            characters.Add(ch);
            foreach (var provinceParser in provinces)
            {
                if(provinceParser.title != null)
                    ch.GiveTitle(TitleManager.instance.TitleMap[provinceParser.title]);
            }

            int nn = Globals.OneInChanceOfReligionSplinter;
            if (ReligionManager.instance.ReligionMap[character.religion].Believers.Count > 100)
                nn--;
            if (ReligionManager.instance.ReligionMap[character.religion].Believers.Count > 300)
                nn--;
            if (character.religion == "pagan")
                nn = 0;
            else
                nn += ReligionManager.instance.ReligionMap[character.religion].Resilience;


         
            if (Rand.Next(nn) == 0)
            {
                ReligionParser r = ReligionManager.instance.BranchReligion(character.religion, pr[Rand.Next(pr.Count)]);
                ch.religion = r.Name;
            }
            if (Rand.Next(Globals.OneInChanceOfCultureSplinter) == 0)
            {
                ch.culture = CultureManager.instance.BranchCulture(character.culture).Name;
                if (character.PrimaryTitle.Government != null)
                {
                    ch.PrimaryTitle.Government = GovernmentManager.instance.BranchGovernment(character.PrimaryTitle.Government, ch.Culture);
                    if (ch.PrimaryTitle.Government.uses_decadence)
                        ch.Religion.uses_decadence = true;
                }
            }
        }

       
    }
}
