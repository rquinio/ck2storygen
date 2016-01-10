using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class CultureGroupParser : Parser
    {
        public List<CultureParser> Cultures = new List<CultureParser>();
        internal string chosenGfx;
        public int r;
        public int b;
        public int g;

        public CultureGroupParser(ScriptScope scope)
            : base(scope)
        {
            if (scope.UnsavedData.ContainsKey("color"))
            {
                var col = ( Color)Scope.UnsavedData["color"];

                r = col.R;
                g = col.G;
                b = col.B;
            }
            foreach (var scriptScope in scope.Scopes)
            {
                
                if (CultureManager.instance.CultureMap.ContainsKey(scriptScope.Name))
                    Cultures.Add(CultureManager.instance.CultureMap[scriptScope.Name]);
            }
        }

        public void RemoveCulture(String name)
        {
            var r = CultureManager.instance.CultureMap[name];
            Cultures.Remove(r);
            Scope.Remove(r);
        }

        public void AddCulture(CultureParser r)
        {
            if (r.Group != null)
            {
                r.Group.Scope.Remove(r.Scope);
            }
            Scope.Add(r.Scope);
            Cultures.Add(r);
        }

        public CultureParser AddCulture(String name)
        {
            if (name != "norse")
            {
                String oname = name;
                name = StarNames.SafeName(name);
              
                LanguageManager.instance.Add(name, oname);

            }


            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            Name = name;

            Scope.Add(scope);
            CultureParser r = new CultureParser(scope);
            CultureManager.instance.AllCultures.Add(r);
            Cultures.Add(r);
            CultureManager.instance.CultureMap[name] = r;
            r.Name = Name;
            r.Init();
           
            Scope.SetChild(r.Scope);
            return r;
            
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

        public void Init()
        {
            Scope.Clear();
            chosenGfx = CultureParser.GetRandomCultureGraphics();
            Scope.Do(@"
                
	            graphical_cultures = { 
                  " + chosenGfx + @" 
                }
");

            r = Rand.Next(255);
            g = Rand.Next(255);
            b = Rand.Next(255);

            Scope.UnsavedData["color"] = Color.FromArgb(255, r, g, b);
        }
    }
    class CultureParser : Parser
    {
           public CultureGroupParser Group
        {
            get
            {
                return new CultureGroupParser(Scope.Parent);
            }
        }

        public CulturalDna dna { get; set; }
        public List<Dynasty> Dynasties = new List<Dynasty>();

        public List<Government> Governments = new List<Government>();
        public CultureParser(ScriptScope scope) : base(scope)
        {
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

      
        public void Init()
        {
            
    //        Scope.Clear();

            String fx = Group.chosenGfx;
            if (Group.chosenGfx == null)
            {
                fx = Group.Scope.Scopes[0].Data;
            }
            int r = Rand.Next(255);
            int g = Rand.Next(255);
            int b = Rand.Next(255);

            r = Group.r;
            g = Group.g;
            b = Group.b;
            switch (Rand.Next(3))
            {
                case 0:
                    r += Rand.Next(-45, 45);
                    g += Rand.Next(-25, 25);
                    b += Rand.Next(-15, 15);

                    break;
                case 1:
                    g += Rand.Next(-45, 45);
                    r += Rand.Next(-25, 25);
                    b += Rand.Next(-15, 15);

                    break;
                case 2:
                    b += Rand.Next(-45, 45);
                    g += Rand.Next(-25, 25);
                    r += Rand.Next(-15, 15);

                    break;
            }
            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;

           Scope.Do(@"
                
		         color = { " + r + " " + g + " " + b + @" }
        ");

        }

        public void DoDetailsForCulture()
        {
            Scope.Clear();
            dna.culture = this;
            if (dna.portraitPool.Count == 0)
            {
                int c = Rand.Next(2)+1;
                String cul = GetRandomCultureGraphics();
                for (int i = 0; i < c; i++)
                {
                    dna.portraitPool.Add(cul);
                    cul = GetRelatedCultureGfx(cul);
                }

            }

            String portrait = "";

            foreach (var p in dna.portraitPool)
            {
                portrait += p + " ";
            }
            int r = Rand.Next(255);
            int g = Rand.Next(255);
            int b = Rand.Next(255);

            r = Group.r;
            g = Group.g;
            b = Group.b;
            switch (Rand.Next(3))
            {
                case 0:
                    r += Rand.Next(-45, 45);
                    g += Rand.Next(-25, 25);
                    b += Rand.Next(-15, 15);

                    break;
                case 1:
                    g += Rand.Next(-45, 45);
                    r += Rand.Next(-25, 25);
                    b += Rand.Next(-15, 15);

                    break;
                case 2:
                    b += Rand.Next(-45, 45);
                    g += Rand.Next(-25, 25);
                    r += Rand.Next(-15, 15);

                    break;
            }
            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;


            Scope.Do(@"
            
               color = { " + r + " " + g + " " + b + @" }    

               graphical_cultures = { 
                    " + portrait + @" 
                }
		
		        male_names = {
			        " + dna.GetMaleNameBlock() +  @"
		        }
		        female_names = {
			        " + dna.GetFemaleNameBlock() + @"
		        }
		
		        dukes_called_kings =  " + (dna.dukes_called_kings ? "yes" : "no") + @"
		        baron_titles_hidden =  " + (dna.baron_titles_hidden ? "yes" : "no") + @"
		        count_titles_hidden =  " + (dna.count_titles_hidden ? "yes" : "no") + @"
		        horde = " + (dna.horde ? "yes" : "no") + @"
                founder_named_dynasties = " + (dna.founder_named_dynasties ? "yes" : "no") + @"
                dynasty_title_names = " + (dna.dynasty_title_names ? "yes" : "no") + @"

		        from_dynasty_prefix = [" + '"' + dna.from_dynasty_prefix + '"' + @"
		     
		        male_patronym = " + (dna.male_patronym) + @"
		        female_patronym =  " + (dna.female_patronym) + @"
		        prefix =  " + (dna.patronym_prefix ? "yes" : "no") + @" # The patronym is added as a suffix
		        # Chance of male children being named after their paternal or maternal grandfather, or their father. Sum must not exceed 100.
		        pat_grf_name_chance = 25
		        mat_grf_name_chance = 0
		        father_name_chance = 25
		
		        # Chance of female children being named after their paternal or maternal grandmother, or their mother. Sum must not exceed 100.
		        pat_grm_name_chance = 10
		        mat_grm_name_chance = 25
		        mother_name_chance = 25

		        modifier = default_culture_modifier
		
		        allow_looting =  " + (dna.allow_looting ? "yes" : "no") + @"
		        seafarer =  " + (dna.seafarer ? "yes" : "no") + @"
        ");

        }

        public string GetRelatedCultureGfx(string cul)
        {
            if (wh.Contains(cul))
            {
                return wh[Rand.Next(wh.Count)];
            }
            else if(!wh.Contains(cul))
            {
                return bl[Rand.Next(bl.Count)];
            }

            return null;
        }

        public List<String> male_names = new List<string>();
        public List<String> female_names = new List<string>(); 

        private static String[] gfx =
        {
            "norsegfx", "germangfx", "frankishgfx", "westerngfx", "saxongfx", "italiangfx", "southerngfx", "occitangfx",
            "easterngfx", "byzantinegfx", "easternslavicgfx", "westernslavicgfx",
            "celticgfx", "ugricgfx", "turkishgfx", "mongolgfx", "muslimgfx", "persiangfx", "cumangfx", "arabicgfx",
            "andalusiangfx", "africangfx", "mesoamericangfx", "indiangfx"
        };

        private static List<String> wh = new List<string>()
        {
            "norsegfx", "germangfx", "frankishgfx", "westerngfx", "saxongfx", "italiangfx", "southerngfx", "occitangfx",
            "easterngfx", "byzantinegfx", "easternslavicgfx", "westernslavicgfx",
            "celticgfx"
        };

        private static List<String> bl = new List<string>()
        {
            "ugricgfx", "turkishgfx", "mongolgfx", "muslimgfx", "persiangfx", "cumangfx", "arabicgfx",
            "andalusiangfx", "africangfx", "mesoamericangfx", "indiangfx"
        };

     
        internal static string GetRandomCultureGraphics(CultureGroupParser group = null)
        {
            if (group != null)
            {
                if (Rand.Next(3) == 0)
                {
                    switch (group.chosenGfx)
                    {
                        case "norsegfx":
                        case "germangfx":
                        case "frankishgfx":
                        case "westerngfx":
                        case "saxongfx":
                        case "italiangfx":
                        case "celticgfx":
                        case "mongolgfx":
                            return wh[Rand.Next(gfx.Count())];
                            break;
                        case "ugricgfx":
                        case "turkishgfx":
                        case "muslimgfx":
                        case "persiangfx":
                        case "cumangfx":
                        case "arabicgfx":
                        case "andalusiangfx":
                        case "africangfx":
                        case "mesoamericangfx":
                        case "indiangfx":
                            return bl[Rand.Next(gfx.Count())];
                            break;
                    }
                }
                else
                {
                    switch (group.chosenGfx)
                    {
                        case "norsegfx":
                        case "germangfx":
                        case "frankishgfx":
                        case "westerngfx":
                        case "saxongfx":
                        case "italiangfx":
                        case "celticgfx":
                        case "mongolgfx":
                            return bl[Rand.Next(gfx.Count())];
                            break;
                        case "ugricgfx":
                        case "turkishgfx":
                        case "muslimgfx":
                        case "persiangfx":
                        case "cumangfx":
                        case "arabicgfx":
                        case "andalusiangfx":
                        case "africangfx":
                        case "mesoamericangfx":
                        case "indiangfx":
                            return wh[Rand.Next(gfx.Count())];
                            break;
                    }
                }
            }

            return gfx[Rand.Next(gfx.Count())];
        }

        public String PickCharacterName()
        {
            return dna.GetMaleName();
        }

        public String PickCharacterName(bool isFemale)
        {
            String str = "";
            do
            {
                str = DoPickCharacterName(isFemale);
            } while (str.Trim().Length==0);

            return str;
        }

        public String DoPickCharacterName(bool isFemale)
        {
            if (isFemale)
                return dna.GetFemaleName();
            return dna.GetMaleName();
        }

    }
    class CultureManager
    {
        public static CultureManager instance = new CultureManager();
        
        private Script script;
        public List<CultureParser> AllCultures = new List<CultureParser>();
        public Dictionary<String, CultureParser> CultureMap = new Dictionary<String, CultureParser>();
        public List<CultureGroupParser> AllCultureGroups = new List<CultureGroupParser>();
        public CultureManager()
        {
        
        }

        public Script Script
        {
            get { return script; }
            set { script = value; }
        }

        public Dictionary<String, CultureGroupParser> GroupMap = new Dictionary<string, CultureGroupParser>();

        public CultureGroupParser AddCultureGroup(string name, CultureGroupParser group = null)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            
            script.Root.Add(scope);
            
            CultureGroupParser r = new CultureGroupParser(scope);
            r.Init();
            if (group != null)
            {
                r.chosenGfx = GetRelatedCultureGfx(group);
            }
            GroupMap[name] = r;
            AllCultureGroups.Add(r);
         
            r.chosenGfx = scope.Scopes[0].Data;
            return r;
        }

        private string GetRelatedCultureGfx(CultureGroupParser group)
        {
            return CultureParser.GetRandomCultureGraphics(group);
            
        }

        public void Save()
        {
            script.Save();
        }

        public void Init()
        {
            LanguageManager.instance.Add("norse", StarNames.Generate(Rand.Next(1000000)));
            LanguageManager.instance.Add("pagan", StarNames.Generate(Rand.Next(1000000)));
            
            Script s = new Script();
            script = s; 
            s.Name = Globals.ModDir + "common\\cultures\\00_cultures.txt";
            s.Root = new ScriptScope();
            CultureGroupParser r = AddCultureGroup("barbarian");

          //  r.Init();
            AllCultureGroups.Add(r);

            var cul = r.AddCulture("norse");

            cul.dna = CulturalDnaManager.instance.GetNewFromVanillaCulture();
            cul.dna.horde = false;
         //   cul.dna.horde = false;
            cul.DoDetailsForCulture(); 
            
            //    cul.Init();
            cul.Name = cul.Scope.Name;
            CultureMap[cul.Scope.Name] = cul;
            AllCultures.Add(cul);
            s.Save();
           
        }

        public CultureParser BranchCulture(string Culture)
        {
            var rel = this.CultureMap[Culture];
            var group = rel.Group;

            var rel2 = group.AddCulture(rel.dna.GetPlaceName());

            if (Rand.Next(10) == 0 || rel.Group.Cultures.Count > 5)
            {
                var group2 = AddCultureGroup(StarNames.SafeName(rel.dna.GetPlaceName()), group);
                group2.AddCulture(rel2);
                rel2.Init();
                rel2.dna = rel.dna.Mutate(6);
                rel2.dna.DoRandom();
     
            }
            else
            {
                rel2.Init();
                rel2.dna = rel.dna.MutateSmall(5);
                
            }
     
            rel2.DoDetailsForCulture();
            return rel2;
        }
   
    }
}
