using System;
using System.Collections.Generic;
using System.Linq;

namespace CrusaderKingsStoryGen
{
    class ReligionGroupParser : Parser
    {
        public List<ReligionParser> Religions = new List<ReligionParser>();

        public ReligionGroupParser(ScriptScope scope)
            : base(scope)
        {
            foreach (var scriptScope in scope.Scopes)
            {
                if (ReligionManager.instance.ReligionMap.ContainsKey(scriptScope.Name))
                    Religions.Add(ReligionManager.instance.ReligionMap[scriptScope.Name]);
            }
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

        public void RemoveReligion(String name)
        {
            var r = ReligionManager.instance.ReligionMap[name];
            Religions.Remove(r);
            Scope.Remove(r);
        }
        public void AddReligion(ReligionParser r)
        {
            if (r.Group != null)
            {
                r.Group.Scope.Remove(r.Scope);
            }
            Scope.Add(r.Scope);
            Religions.Add(r);            
        }
        public ReligionParser AddReligion(String name, String orig=null)
        {
            if (name != "pagan")
            {
                String oname = name;
                name = StarNames.SafeName(name);
                LanguageManager.instance.Add(name, oname);
                orig = oname;
            }

       
            
            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            Scope.Add(scope);
            ReligionParser r = new ReligionParser(scope);
            ReligionManager.instance.AllReligions.Add(r);
            if (orig != null)
            {
                r.LanguageName = orig;
            }
            Religions.Add(r);
            ReligionManager.instance.ReligionMap[name] = r;
            return r;
        }
        static string[] gfx = new string[]
            {
                "muslimgfx",
                "westerngfx",
                "norsegfx",
                "mongolgfx",
                "mesoamericangfx",
                "africangfx",
                "persiangfx",
                "jewishgfx",
                "indiangfx",
                "hindugfx",
                "buddhistgfx",
                "jaingfx",
            };

        public bool hostile_within_group = false;
        public void Init()
        {

            hostile_within_group = Rand.Next(2) == 0;
            String g = gfx[Rand.Next(gfx.Count())];
            Scope.Clear();
            Scope.Do(@"

	            has_coa_on_barony_only = yes
	            graphical_culture = " + g + @"
	            playable = yes
	            hostile_within_group = " + (hostile_within_group ? "yes" : "no") + @"
	
	            # Names given only to Pagan characters (base names)
	            male_names = {
		            Anund Asbjörn Aslak Audun Bagge Balder Brage Egil Emund Frej Gnupa Gorm Gudmund Gudröd Hardeknud Helge Odd Orm 
		            Orvar Ottar Rikulfr Rurik Sigbjörn Styrbjörn Starkad Styrkar Sämund Sölve Sörkver Thorolf Tjudmund Toke Tolir 
		            Torbjörn Torbrand Torfinn Torgeir Toste Tyke
	            }
	            female_names = {
		            Aslaug Bothild Björg Freja Grima Gytha Kráka Malmfrid Thora Thordis Thyra Ragnfrid Ragnhild Svanhild Ulvhilde
	            }

");
         

        }
		
		

     

        
    }
}