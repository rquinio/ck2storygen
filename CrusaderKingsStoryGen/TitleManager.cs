using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using CrusaderKingsStoryGen.Simulation;

namespace CrusaderKingsStoryGen
{
    class TitleManager
    {
        public static TitleManager instance = new TitleManager();

        public Dictionary<string, TitleParser> TieredTitles = new Dictionary<string, TitleParser>();
        public Dictionary<string, TitleParser> TitleMap = new Dictionary<string, TitleParser>();
        public List<TitleParser> Titles = new List<TitleParser>();
        public Script LandedTitlesScript { get; set; }
        public void Load()
        {
            LandedTitlesScript = ScriptLoader.instance.Load(
             Globals.GameDir + "common\\landed_titles\\landed_titles.txt");

            LandedTitlesScript.Root.Clear();
            /*
            foreach (var child in LandedTitlesScript.Root.Children)
            {
                var empire = new TitleParser(child as ScriptScope);
                TieredTitles[empire.Name] = empire;
                TitleMap[empire.Name] = empire;
             //   Titles.Add(empire);
            }

            PromoteAllToTop();
            RemoveKingdomAndAbove();
       //     RemoveDukeAndAbove();
            Save();*/
        }

        private void RemoveDukeAndAbove()
        {
            var array = TieredTitles.Values.ToArray();
        }

        private void RemoveKingdomAndAbove()
        {
            var array = TieredTitles.Values.ToArray();
            toTop.Clear();
            List<string> list = new List<string>();
            bool bActive = false;
            foreach (var titleParser in array)
            {
                
                {
                //    if (bActive)
                    {
                        if (titleParser.Rank==4)
                            list.Add(titleParser.Name);
                        else if (titleParser.Rank == 3)
                            list.Add(titleParser.Name);
                        else if (titleParser.Rank == 2)
                            list.Add(titleParser.Name);
                        else if(titleParser.Rank == 1)
                            titleParser.Liege = null;
                        else
                            list.Add(titleParser.Name);
                    }
                }
                
            }
            
            foreach (var item in list)
            {
                TieredTitles[item].Remove();
                TitleMap.Remove(TieredTitles[item].Name);
                Titles.Remove(TieredTitles[item]);
                TieredTitles.Remove(item);
                
            }
            
        }

        List<TitleParser> toTop = new List<TitleParser>(); 
        private void PromoteAllToTop()
        {
            toTop.Clear();
            var array = TieredTitles.Values.ToArray();

            foreach (var titleParser in array)
            {
                foreach (var subTitle in titleParser.SubTitles)
                {
                    if (subTitle.Value.Rank == 0)
                        continue;
                    PromoteToTop(subTitle.Value);
                }

            }

            foreach (var titleParser in toTop)
            {
                LandedTitlesScript.Root.Add(titleParser.Scope);
                titleParser.Scope.Parent = LandedTitlesScript.Root;
                TieredTitles[titleParser.Name] = titleParser;
            }
        }
        private bool PromoteToTop(TitleParser title)
        {
            var array = title.SubTitles.ToArray();
            if (title.Rank == 0)
                return false;
            foreach (var titleParser in array)
            {
                if(PromoteToTop(titleParser.Value))
                    titleParser.Value.SubTitles.Clear();
            }
            title.Scope.Parent.Remove(title.Scope);            
            title.SubTitles.Clear();
            toTop.Add(title);
            return true;
        }

        public void Save()
        {
            LandedTitlesScript.Root.Strip(new string[]
            {
                "layer",
                "pagan_coa"
            });

            CreateMercs();

            LandedTitlesScript.Save();
        }

        private static String[] compositions = new[]
        {
            "muslim_turkic_company_composition",
            "bedouin_company_composition",
            "berber_company_composition",
            "muslim_cuman_company_composition",
            "zun_warriors_composition",
            "bulls_rishabha_composition",
            "white_company_composition",
            "great_company_composition",
            "company_of_st_george_composition",
            "star_company_composition",
            "little_hat_company_composition",
            "rose_company_composition",
            "catalan_company_composition",
            "navarrese_company_composition",
            "swiss_company_composition",
            "breton_company_composition",
           "victual_brothers_composition",
           "varangian_guard_composition",
           "cuman_company_composition",
           "rus_company_composition",
           "pecheneg_company_composition",
           "bulgarian_company_composition",
           "lombard_band_composition",
           "breton_band_composition",
           "catalan_band_composition",
           "saxon_band_composition",
           "cuman_band_composition",
           "rus_band_composition",
           "finnish_band_composition",
           "lithuanian_band_composition",
           "abyssinian_band_composition",
           "nubian_band_composition",
           "scottish_band_composition",
           "irish_band_composition",
           "alan_band_composition",
          "pecheneg_band_composition",
          "bulgarian_band_composition",
          "turkic_band_composition",
          "mamluks_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
          "naval_merc_composition",
       
        };
        public void CreateMercs()
        {
            // do mercs...
            foreach (var cultureParser in CultureManager.instance.AllCultures)
            {
                String name = cultureParser.dna.GetMaleName();
                String namesafe = "d_" + StarNames.SafeName(name);
                LanguageManager.instance.Add(namesafe, name);
                String composition = compositions[Rand.Next(compositions.Length)];
                LandedTitlesScript.Root.Do(@"

                    " + namesafe + @" = {

	                   	color = { 135 170 60 }
	                    color2 = { 255 255 255 }

	                    capital = " + Rand.Next(1300) + @" 
	
	                    # Parent Religion 
	                    culture =  " + cultureParser.Name + @" 
	
	                    mercenary = yes

	                    title = ""CAPTAIN""
	                    foa = ""CAPTAIN_FOA""

	                    # Always exists
	                    landless = yes
	
	                    # Cannot be held as a secondary title
	                    primary = yes
	
	                    # Cannot be vassalized
	                    independent = yes
	
	                    strength_growth_per_century = 1.00
	
	                    mercenary_type = " + composition + @"
                    }


                ");
            }
        }


        public CharacterParser PromoteNewRuler(TitleParser title)
        {
    
            {
                var chara = CharacterManager.instance.GetNewCharacter();

                chara.GiveTitle(title);
                SimulationManager.instance.characters.Add(chara);
                return chara;
            }


        }

        public TitleParser CreateDukeScriptScope(ProvinceParser capital, String name = null)
        {
            var rand = Rand.Get();
            Color col = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            ScriptScope scope = new ScriptScope();
            scope.Parent = LandedTitlesScript.Root;

            if (name == null)
            {
                String place = capital.Title.Holder.Culture.dna.GetPlaceName();
                String text = place;
                place = StarNames.SafeName(place);
                LanguageManager.instance.Add(place, text);
                scope.Name = "d_" + place;
                LanguageManager.instance.Add(scope.Name, text);
            }
            else
                scope.Name = "d_" + name;

            //  scope.Kids.Add(new ScriptCommand() { Name = "rebel", Value = false });
            scope.Add(new ScriptCommand() { Name = "color", Value = col });
            scope.Add(new ScriptCommand() { Name = "color2", Value = col });
            
            scope.Add(new ScriptCommand() { Name = "capital", Value = capital.id });
          
            TitleParser title = new TitleParser(scope);
            
  //          if (capital.Title.Culture.dna.horde)
//                title.Scope.Do("historical_nomad = yes");
            
            AddTitle(title);
            if (title.capital != 0)
                title.CapitalProvince = MapManager.instance.ProvinceIDMap[title.capital];
            // now place the counties into it...

            return title;
        }

        public TitleParser CreateDukeScriptScope(ProvinceParser capital, CharacterParser chr)
        {
            var rand = Rand.Get();
            Color col = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            ScriptScope scope = new ScriptScope();
            scope.Parent = LandedTitlesScript.Root;

            
            {
                String place = chr.Culture.dna.GetPlaceName();
                String text = place;
                place = StarNames.SafeName(place);
                LanguageManager.instance.Add(place, text);
                scope.Name = "d_" + place;
                LanguageManager.instance.Add(scope.Name, text);
            }
           
            //  scope.Kids.Add(new ScriptCommand() { Name = "rebel", Value = false });
            scope.Add(new ScriptCommand() { Name = "color", Value = col });
            scope.Add(new ScriptCommand() { Name = "color2", Value = col });

            scope.Add(new ScriptCommand() { Name = "capital", Value = capital.id });

            TitleParser title = new TitleParser(scope);
  //          if (chr.Culture.dna.horde)
//                title.Scope.Do("historical_nomad = yes");
            AddTitle(title);
            if (title.capital != 0)
                title.CapitalProvince = MapManager.instance.ProvinceIDMap[title.capital];
            // now place the counties into it...

            return title;
        }
        public TitleParser CreateBaronyScriptScope(ProvinceParser capital, CultureParser culture)
        {
            var place = culture.dna.GetPlaceName();
            var text = place;
            place = StarNames.SafeName(text);
            if (TitleManager.instance.TieredTitles.ContainsKey("b_" + place))
            {
                return CreateBaronyScriptScope(capital, culture);
            }
            var rand = Rand.Get();
            Color col = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            ScriptScope scope = new ScriptScope();
            scope.Parent = capital.Title.Scope;

            
                scope.Name = "b_" + place;
                LanguageManager.instance.Add("b_" + place, text);
            //  scope.Kids.Add(new ScriptCommand() { Name = "rebel", Value = false });
         
            TitleParser title = new TitleParser(scope);
            TieredTitles[title.Name] = title;
            capital.Title.Scope.Add(title.Scope);

  //          if (culture.dna.horde)
//                title.Scope.Do("historical_nomad = yes");
         //   AddTitle(title);
         
            return title;
        }
        public TitleParser CreateEmpireScriptScope(ProvinceParser capital, String name = null)
        {
            var rand = Rand.Get();
            Color col = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            ScriptScope scope = new ScriptScope();
            scope.Parent = LandedTitlesScript.Root;

            if (name == null)
                scope.Name = "e_" + capital.Title.Name.Substring(2);
            else
                scope.Name = "e_" + name;
            //  scope.Kids.Add(new ScriptCommand() { Name = "rebel", Value = false });
            scope.Add(new ScriptCommand() { Name = "color", Value = col });
            scope.Add(new ScriptCommand() { Name = "color2", Value = col });

            scope.Add(new ScriptCommand() { Name = "capital", Value = capital.id });

            TitleParser title = new TitleParser(scope);
            AddTitle(title);
            if (title.capital != 0)
                title.CapitalProvince = MapManager.instance.ProvinceIDMap[title.capital];
            // now place the counties into it...

            return title;
        }
        public TitleParser CreateKingScriptScope(ProvinceParser capital, CharacterParser chr)
        {
            var rand = Rand.Get();
            Color col = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            ScriptScope scope = new ScriptScope();
            scope.Parent = LandedTitlesScript.Root;



            {
                String place = chr.Culture.dna.GetPlaceName();
                String text = place;
                place = StarNames.SafeName(place);
                LanguageManager.instance.Add(place, text);
                scope.Name = "k_" + place;
                LanguageManager.instance.Add(scope.Name, text);
            }


            //scope.Kids.Add(new ScriptCommand() { Name = "rebel", Value = false });
            scope.Add(new ScriptCommand() { Name = "color", Value = col });
            scope.Add(new ScriptCommand() { Name = "color2", Value = col });

            scope.Add(new ScriptCommand() { Name = "capital", Value = capital.id });

            TitleParser title = new TitleParser(scope);
            AddTitle(title);

            // now place the counties into it...
            if (title.capital != 0)
                title.CapitalProvince = MapManager.instance.ProvinceIDMap[title.capital];

            return title;
        }
        public TitleParser CreateEmpireScriptScope(ProvinceParser capital, CharacterParser chr)
        {
            var rand = Rand.Get();
            Color col = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            ScriptScope scope = new ScriptScope();
            scope.Parent = LandedTitlesScript.Root;



            {
                String place = chr.Culture.dna.GetPlaceName();
                String text = place;
                place = StarNames.SafeName(place);
                LanguageManager.instance.Add(place, text);
                scope.Name = "e_" + place;
                LanguageManager.instance.Add(scope.Name, text);
            }


            //scope.Kids.Add(new ScriptCommand() { Name = "rebel", Value = false });
            scope.Add(new ScriptCommand() { Name = "color", Value = col });
            scope.Add(new ScriptCommand() { Name = "color2", Value = col });

            scope.Add(new ScriptCommand() { Name = "capital", Value = capital.id });

            TitleParser title = new TitleParser(scope);
            AddTitle(title);

            // now place the counties into it...
            if (title.capital != 0)
                title.CapitalProvince = MapManager.instance.ProvinceIDMap[title.capital];

            return title;
        }
        
        public TitleParser CreateKingScriptScope(ProvinceParser capital, String name = null)
        {
            var rand = Rand.Get();
            Color col = Color.FromArgb(255, rand.Next(200) + 55, rand.Next(200) + 55, rand.Next(200) + 55);
            ScriptScope scope = new ScriptScope();
            scope.Parent = LandedTitlesScript.Root;


            if (name == null)
                scope.Name = "k_" + capital.Title.Name.Substring(2);
            else
                scope.Name = "k_" + name;


            //scope.Kids.Add(new ScriptCommand() { Name = "rebel", Value = false });
            scope.Add(new ScriptCommand() { Name = "color", Value = col });
            scope.Add(new ScriptCommand() { Name = "color2", Value = col });
        
            scope.Add(new ScriptCommand() { Name = "capital", Value = capital.id });

            TitleParser title = new TitleParser(scope);
            AddTitle(title);

            // now place the counties into it...
            if (title.capital != 0)
                title.CapitalProvince = MapManager.instance.ProvinceIDMap[title.capital];

            return title;
        }

        public void AddVassalsToTitle(TitleParser title, List<TitleParser> vassals)
        {
            foreach (var titleParser in vassals)
            {
                title.Scope.SetChild(titleParser.Scope);
                if (titleParser.Liege != null)
                {
                    titleParser.Liege.RemoveVassal(titleParser);
                }
                title.SubTitles[titleParser.Name] = titleParser;
                if (title.Liege == titleParser || title.Liege == title)
                    title.Liege = null;
                titleParser.Liege = title;                
            }
            
        }

     
        public void AddTitle(TitleParser title)
        {
         //   Titles.Add(title);
            TieredTitles[title.Name] = title;
            LandedTitlesScript.Root.Add(title.Scope);
            title.Scope.Parent = LandedTitlesScript.Root;
        }

        public void SaveTitles()
        {

            foreach (var titleParser in Titles)
            {
                if (titleParser.culture == null)
                    continue;

                String tit = titleParser.Culture.dna.kingTitle;
           
                switch (titleParser.Rank)
                {
                    case 0:
                        tit = titleParser.Culture.dna.baronTitle;
                        break;
                    case 1:
                        tit = titleParser.Culture.dna.countTitle;
                        break;
                    case 2:
                        tit = titleParser.Culture.dna.dukeTitle;
                        break;
                    case 3:
                        tit = titleParser.Culture.dna.kingTitle;
                        break;
                    case 4:
                        tit = titleParser.Culture.dna.empTitle;
                        break;
                }

                titleParser.Scope.Add(new ScriptCommand() { Name = "culture", Value = titleParser.culture });

                titleParser.Scope.Do(
                    @"
                        title=" + tit + @"
                        title_female=" + tit + @"
"
                    );
            }

            foreach (var religionParser in ReligionManager.instance.AllReligions)
            {
                if (religionParser.Believers.Count > 0 && religionParser.hasLeader)
                {
                    religionParser.DoLeader(religionParser.Believers[Rand.Next(religionParser.Believers.Count)]);
                }
            }
             if(!Directory.Exists(Globals.ModDir + "history\\titles\\"))
                Directory.CreateDirectory(Globals.ModDir + "history\\titles\\");
            var files = Directory.GetFiles(Globals.ModDir + "history\\titles\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }
             foreach (var title in Titles)
            {
                if (!title.Active)
                    continue;
                if (title.Religious)
                {
                    
                }
                Script titleScript = ScriptLoader.instance.Load(Globals.GameDir + "history\\titles\\" + title + ".txt");
                
                titleScript.Root.Clear();
               
               

                {
                   if (titleScript.Root.HasNamed("767.1.1"))
                    {
                        titleScript.Root.Delete("767.1.1");
                        titleScript.Root.Delete("768.1.1");
                    }
                    {
                        ScriptScope thing = new ScriptScope();
                        thing.Name = "767.1.1";
                        titleScript.Root.SetChild(thing);
                        if (title.Culture.dna.horde)
                            thing.Add(new ScriptCommand() { Name = "historical_nomad", Value = true });
                           
                        if (title.Liege != null)
                        {
                            thing.Add(new ScriptCommand() { Name = "liege", Value = title.Liege.Name });
                            
                        }

                         if (title.Holder != null)
                        {
                            thing.Add(new ScriptCommand() { Name = "holder", Value = title.Holder.ID });    
                        
                       //     title.Holder.MakeAlive();
                        }else if (title.SubTitles.Count > 0 && title.Rank >= 2 && title.Holder == null)
                        {

                            thing.Add(new ScriptCommand() { Name = "holder", Value = title.SubTitles.Values.ToArray()[0].Holder.ID });
                        //    title.SubTitles.Values.ToArray()[0].Holder.MakeAlive();
                        }
                         thing = new ScriptScope();
                         thing.Name = "768.1.1";
                         titleScript.Root.SetChild(thing);
                         if (title.CurrentHolder != null)
                         {
                             thing.Add(new ScriptCommand() { Name = "holder", Value = title.CurrentHolder.ID });

                         }
                      
                    
                    }


                }
              
                titleScript.Save();
            }
        }

        public TitleParser CreateEmperor(ProvinceParser capital)
        {
            TitleParser s = CreateEmpireScriptScope(capital);

            return s;
        }

        public void RemoveTitle(TitleParser titleParser)
        {
            Titles.Remove(titleParser);
            TitleMap.Remove(titleParser.Name);
            LandedTitlesScript.Root.Remove(titleParser.Scope);
        }

       
    }
}
