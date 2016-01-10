using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace CrusaderKingsStoryGen
{
    class Module
    {
        public String name;
        public Dictionary<String, ScriptScope> Events = new Dictionary<String, ScriptScope>();
        public Dictionary<String, ScriptScope> Decisions = new Dictionary<String, ScriptScope>();
        public Dictionary<String, ScriptScope> Traits = new Dictionary<String, ScriptScope>();
        public string nameSpace = null;

        public void Save()
        {
            Script s = new Script();
            s.Name = Globals.ModDir + "events\\gen_" + name + "_events.txt";

            s.Root = new ScriptScope();
            s.Root.Add(new ScriptCommand("namespace", nameSpace, s.Root));
       
            foreach (var scriptScope in Events)
            {
                scriptScope.Value.Name = "character_event";
                s.Root.Add(scriptScope.Value);
            }

            s.Save();
            s = new Script();
            s.Name = Globals.ModDir + "decisions\\gen_" + name + "_decisions.txt";

            s.Root = new ScriptScope();
            ScriptScope ss = new ScriptScope();
            s.Root.Add(ss);
            ss.Name = "decisions";
          //  s.Root.Add(new ScriptCommand("namespace", nameSpace, s.Root));
       
             foreach (var scriptScope in Decisions)
            {
                ScriptScope e = new ScriptScope();
                scriptScope.Value.Name = scriptScope.Key;
                ss.Add(scriptScope.Value);
            }

            s.Save();

            s = new Script();
            s.Name = Globals.ModDir + "common\\traits\\03_zz_gen_" + name + "_traits.txt";

            s.Root = new ScriptScope();
     //       s.Root.Add(new ScriptCommand("namespace", nameSpace, s.Root));
            foreach (var scriptScope in Traits)
            {
                scriptScope.Value.Name = scriptScope.Key;
                s.Root.Add(scriptScope.Value);
            }

            s.Save();
        }
    }
    class ModularFunctionalityManager
    {
        public static ModularFunctionalityManager instance = new ModularFunctionalityManager();
        public Dictionary<String, Module> Modules = new Dictionary<string, Module>();

        public class SchoolInfo
        {
            public String Name { get; set; }
        }

        public struct SchoolTypes
        {
            public String principles;
            public String effectString;
            public float ZealousMod;
            public float CynicalMod;

            public SchoolTypes(String principles, String doEffects, float cyn, float zea)
            {
                this.principles = principles;
                this.effectString = doEffects;
                CynicalMod = cyn;
                ZealousMod = zea;
            }
        }

        public SchoolTypes[] Schools = 
        {
            new SchoolTypes("peace and tradition", @"
                            monthly_character_piety = 0.5                            
                     ",
                        0.5f,
                        2.0f),
            new SchoolTypes("peace and learning", @"
                            learning = 2                  

                  ",
                        2.0f,
                        0.5f),
            new SchoolTypes("power and wealth", @"
                            stewardship = 2
                            monthly_character_prestige = 0.5

                    ",
                        2.0f,
                        0.5f),
            new SchoolTypes("war and tradition", @"
                            martial = 2
                            monthly_character_piety = 0.5
                         	combat_rating = 1
	                        infidel_opinion = -50
                   ",
                        0.5f,
                        2.0f),
        };

        public Module CreateReligionPhilosophy(String nameSpace, String religion, List<SchoolInfo> schools)
        {      
            Module m = new Module();
            m.name = religion + "_philosophies";
            m.nameSpace = nameSpace;
            String eventID = "";
            int n = 1;
            var schoolss = new List<SchoolTypes>(Schools);
            foreach (var school in schools)
            {
                ScriptScope decision = new ScriptScope();
                ScriptScope trait = new ScriptScope();

                int nn = Rand.Next(schoolss.Count);
                var schooldata = schoolss[nn];
                schoolss.RemoveAt(nn);
                String otherSchools = "";
                foreach (var schoolInfo in schools)
                {
                    if (schoolInfo.Name == school.Name)
                        continue;

                    otherSchools += StarNames.SafeName(schoolInfo.Name) + "\n\r";
                }
                String rname = LanguageManager.instance.Get(religion);
                ScriptScope ev = new ScriptScope();
                eventID = nameSpace + "." + n;
                String eventDesc = eventID + "desc";
                
                TraitManager.instance.AddTrait(StarNames.SafeName(school.Name));
                LanguageManager.instance.Add(StarNames.SafeName(school.Name), school.Name);
                LanguageManager.instance.Add("embrace_" + StarNames.SafeName(school.Name), "Embrace " + school.Name);
                LanguageManager.instance.Add("embrace_" + StarNames.SafeName(school.Name) + "_desc", "You embrace " + school.Name + ", a philosophy of " + rname + " that promotes " + schooldata.principles + @". Members of the opposing philosophies will disapprove.");
                LanguageManager.instance.Add(StarNames.SafeName(school.Name) + "_desc", "Follows the school of " + school.Name + ", a philosophy of " + rname + " that promotes " + schooldata.principles + @".");
                LanguageManager.instance.Add(eventDesc, "You have embraced the " + school.Name + " philosophy of " + rname);
                LanguageManager.instance.Add("embrace_" + StarNames.SafeName(school.Name) + "_desc", "Praise " + ReligionManager.instance.ReligionMap[religion].high_god_name + ", " + school.Name + " is the way!");
                LanguageManager.instance.Add("embrace_" + StarNames.SafeName(school.Name) + "_event_desc", "You embrace " + school.Name + ", a philosophy of " + rname + " that promotes " + schooldata.principles + @". Members of the opposing philosophies will disapprove.");
                ev.Do(@"

                    id = " + eventID + @"
	                desc = " + eventDesc + @"
	                picture = GFX_evt_kaaba
	                border = GFX_event_normal_frame_religion
	                is_triggered_only = yes
	                hide_from = yes
	                option = {
		                name = " + "embrace_" + StarNames.SafeName(school.Name) + "_desc" + @"
		                add_trait = " + StarNames.SafeName(school.Name) + @"		                
	                }
                ");

                decision.Do(@" 
		        potential = {
			        has_dlc = ""Sons of Abraham""
			        religion = " + religion + @"
			        NOT = { 
				        OR = {
					        
				        }
			        }
			        is_ruler = yes
			        age = 16
			        prisoner = no
		        }
		
		        allow = {
			        piety = 50
		        }
		
		        effect = {
			        add_trait = " + StarNames.SafeName(school.Name) + @"
			        character_event = { id = " + eventID + @" tooltip = " + "embrace_" + StarNames.SafeName(school.Name) + "_event_desc" + @" }
		        }
		
		        revoke_allowed = {
			        always = no
		        }
		
		        ai_will_do = {
			        factor = 1
			
			        modifier = {
				        factor = " + schooldata.ZealousMod + @"
				        trait = zealous
			        }
			        modifier = {
				        factor = " + schooldata.CynicalMod + @"
				        trait = cynical
			        }
			        modifier = {
				        factor = 1.2
				        liege = { trait =  " + StarNames.SafeName(school.Name) + @" }
			        }
			        modifier = {
				        factor = 0.05 # Slow it down
			        }
		        }

            ");

                trait.Do(@" 
	
                opposites = {
	                "+ otherSchools + @"

                }

                " + schooldata.effectString + @"	

	            customizer = no
	            random = no
                opposite_opinion = -20
   	            same_opinion = 20
                religious = yes
      ");
                n++;
                m.Decisions["embrace_" + StarNames.SafeName(school.Name)] = decision;
                m.Events["embrace_" + StarNames.SafeName(school.Name)] = ev;
                m.Traits[StarNames.SafeName(school.Name)] = trait;
            }

            return m;
        }

        public void Save()
        {
            if (!Directory.Exists(Globals.ModDir + "decisions\\"))
                Directory.CreateDirectory(Globals.ModDir + "decisions\\");
            var files = Directory.GetFiles(Globals.ModDir + "decisions\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            if (!Directory.Exists(Globals.ModDir + "events\\"))
                Directory.CreateDirectory(Globals.ModDir + "events\\");
            files = Directory.GetFiles(Globals.ModDir + "events\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            if (!Directory.Exists(Globals.ModDir + "common\\traits\\"))
                Directory.CreateDirectory(Globals.ModDir + "common\\traits\\");
            files = Directory.GetFiles(Globals.ModDir + "common\\traits\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var religionParser in ReligionManager.instance.AllReligions)
            {
                if (religionParser.Believers.Count == 0)
                    continue;
                if (Rand.Next(4) != 0)
                    continue;
/*
                if (Rand.Next(4) != 0)
                {
                    var module = CreateReligionPhilosophy(religionParser.Name, religionParser.Name, new List<SchoolInfo>()
                    {
                        new SchoolInfo() {Name = religionParser.Believers[0].GetCurrentHolder().Culture.dna.GetPlaceName()},
                        new SchoolInfo() {Name = religionParser.Believers[0].GetCurrentHolder().Culture.dna.GetPlaceName()},
                    });
                    module.Save();
                }
                else
                {
                    var module = CreateReligionPhilosophy(religionParser.Name, religionParser.Name, new List<SchoolInfo>()
                    {
                        new SchoolInfo() {Name = religionParser.Believers[0].GetCurrentHolder().Culture.dna.GetPlaceName()},
                        new SchoolInfo() {Name = religionParser.Believers[0].GetCurrentHolder().Culture.dna.GetPlaceName()},
                        new SchoolInfo() {Name = religionParser.Believers[0].GetCurrentHolder().Culture.dna.GetPlaceName()},
                     });
                    module.Save();    
                }
                */
                
            }
            
        }
    }
}
