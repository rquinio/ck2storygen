using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class GovernmentManager
    {
        public static GovernmentManager instance = new GovernmentManager();
        public List<Government> governments = new List<Government>();
        public void Init()
        {
           
        }

        public Government BranchGovernment(Government gov, CultureParser culture)
        {
            var newGov = gov.Mutate(10);
           
            {
                if (newGov.type == "nomadic")
                {
                    newGov.type = "tribal";
                    newGov.SetType(newGov.type);
          
                }
            }

            do
            {
                newGov.name = culture.dna.GetMaleName();
            } while (LanguageManager.instance.Get(StarNames.SafeName(newGov.name) + "_government") != null);

            string s = newGov.name;
            newGov.name = StarNames.SafeName(newGov.name) + "_government";
            LanguageManager.instance.Add(newGov.name, s);
            culture.Governments.Add(newGov);
            if (!newGov.cultureAllow.Contains(culture.Name))
                newGov.cultureAllow.Add(culture.Name);
            return newGov;
        }
        public void Save()
        {

            foreach (var cultureParser in CultureManager.instance.AllCultures)
            {
                if (cultureParser.Governments.Count == 0)
                {
                    var gov = GovernmentManager.instance.CreateNewGovernment(cultureParser);

                }
                foreach (var government in cultureParser.Governments)
                {
                    if (government.cultureAllow.Count == 0)
                    {
                        government.cultureAllow.Add(cultureParser.Name);
                    }
                }

                if (!Government.cultureDone.Contains(cultureParser.Name))
                {
                    cultureParser.Governments.Add(GovernmentManager.instance.CreateNewGovernment(cultureParser));      
                }
            }

            if (!Directory.Exists(Globals.ModDir + "gfx\\interface\\"))
                Directory.CreateDirectory(Globals.ModDir + "gfx\\interface\\");
            var files = Directory.GetFiles(Globals.ModDir + "gfx\\interface\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            

            foreach (var government in governments)
            {
                switch (government.type)
                {
                    case "nomadic":
                        try
                        {
                            File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_nomadic.dds", Globals.ModDir + "gfx\\interface\\government_icon_" + government.name.Replace("government_", "") + ".dds");
                        }
                        catch (Exception)
                        {
                            
                            
                        }
                        
                        SpriteManager.instance.AddGovernment(government);
                        break;
                    case "tribal":
                       try
                        {
                            File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_tribal.dds", Globals.ModDir + "gfx\\interface\\government_icon_" + government.name.Replace("government_", "") + ".dds");
                        }
                        catch (Exception)
                        {
                            
                            
                        }
                        SpriteManager.instance.AddGovernment(government);
                    
                        break;
                    case "feudal":
                        try
                        {
                            File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_feudal.dds", Globals.ModDir + "gfx\\interface\\government_icon_" + government.name.Replace("government_", "") + ".dds");

                        }
                        catch (Exception)
                        {
                            
                            
                        }
                        SpriteManager.instance.AddGovernment(government);
                    
                        break;
                    case "theocracy":
                        try
                        {
                            File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_theocracy.dds", Globals.ModDir + "gfx\\interface\\government_icon_" + government.name.Replace("government_", "") + ".dds");
                        }
                        catch (Exception)
                        {
                            
                        }
                        
                        SpriteManager.instance.AddGovernment(government);
                    
                        break;
                    case "republic":
                        try
                        {
                            File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_republic.dds", Globals.ModDir + "gfx\\interface\\government_icon_" + government.name.Replace("government_", "") + ".dds");

                        }
                        catch (Exception)
                        {
                            
                        }
                        SpriteManager.instance.AddGovernment(government);
                    
                        break;      
                }
                
            }


            Script s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\nomadic_governments.txt";

            s.Root = new ScriptScope();

            var scope = new ScriptScope();
            scope.Name = "nomadic_governments";
            s.Root.Add(scope);
            foreach (var government in governments)
            {
                if (government.type == "nomadic" && government.cultureAllow.Count > 0)
                {
                    var g = new ScriptScope();
                    g.Name = government.name;
                    government.Save(g);
                    scope.Add(g);
                }
            }

            s.Save();

            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\feudal_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "feudal_governments";
            s.Root.Add(scope);
            foreach (var government in governments)
            {
                if (government.type == "feudal" && government.cultureAllow.Count > 0)
                {
                    var g = new ScriptScope();
                    g.Name = government.name;
                    government.Save(g);
                    scope.Add(g);
                }
            }

            s.Save();
            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\theocracy_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "theocracy_governments";
            s.Root.Add(scope);
            foreach (var government in governments)
            {
                if (government.type == "theocracy" && government.cultureAllow.Count > 0)
                {
                    var g = new ScriptScope();
                    g.Name = government.name;
                    government.Save(g);
                    scope.Add(g);
                }
            }

            s.Save();
            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\republic_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "republic_governments";
            s.Root.Add(scope);
            foreach (var government in governments)
            {
                if (government.type == "republic" && government.cultureAllow.Count > 0)
                {
                    var g = new ScriptScope();
                    g.Name = government.name;
                    government.Save(g);
                    scope.Add(g);
                }
            }

            s.Save();
            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\tribal_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "tribal_governments";
            s.Root.Add(scope);
            foreach (var government in governments)
            {
                if (government.type == "tribal" && government.cultureAllow.Count > 0)
                {
                    var g = new ScriptScope();
                    g.Name = government.name;
                    government.Save(g);
                    scope.Add(g);
                }
            }

            s.Save();
        }

        public int numNomadic = 0;
        public int numTribal = 0;

        public Government CreateNewGovernment(CultureParser culture)
        {
            Government g = new Government();
            g.type = "tribal";
            Government r = g.Mutate(8);
            r.name = culture.dna.GetMaleName();
            string s = r.name;
            r.name = StarNames.SafeName(r.name) + "_government";
            LanguageManager.instance.Add(r.name, s);
            culture.Governments.Add(r);
            r.SetType(r.type);
            if (!r.cultureAllow.Contains(culture.Name))
                r.cultureAllow.Add(culture.Name);   //    governments.Add(r);
            return r;
        }
    }
}
