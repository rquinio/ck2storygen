using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class DecisionManager
    {
       public static DecisionManager instance = new DecisionManager();
        public List<Script>  Scripts = new List<Script>();

        private string[] exclude = new[]
        {
            "convert_to_swedish",
            "convert_to_norwegian",
            "convert_to_danish",
            "convert_to_norman",
             "convert_to_spouse_catholic",
             "convert_to_spouse_cathar",
             "convert_to_spouse_fraticelli",
            "convert_to_spouse_waldensian",
            "convert_to_spouse_lollard",
            "convert_to_spouse_bogomilist",
            "convert_to_spouse_nestorian",
            "convert_to_spouse_messalian",
            "convert_to_spouse_monothelite",
            "convert_to_spouse_iconoclast",
            "convert_to_spouse_orthodox",
            "convert_to_spouse_paulician",
            "convert_to_spouse_sunni",
            "convert_to_spouse_zikri",
            "convert_to_spouse_yazidi",
            "convert_to_spouse_ibadi",
             "convert_to_spouse_kharijite",
             "convert_to_spouse_shiite",
             "convert_to_spouse_druze",
             "convert_to_spouse_hurufi",
             "convert_to_spouse_zoroastrian",
             "convert_to_spouse_mazdaki",
             "convert_to_spouse_manichean",
             "convert_to_spouse_miaphysite",
             "convert_to_spouse_monophysite",
             "convert_to_spouse_jewish",
             "convert_to_spouse_samaritan",
             "convert_to_spouse_karaite",
             "convert_to_hinduism",
             "convert_to_buddhism",
             "convert_to_hinduism",
             "convert_to_jainism",
             "convert_to_hinduism",
             "convert_indian_branch",
             "convert_to_spouse_hindu",
             "convert_to_spouse_buddhist",
             "convert_to_spouse_jain",
             "convert_to_french",
             "convert_to_scottish",
             "convert_to_andalusian",
             "convert_to_castillan",
             "convert_to_catalan",
             "convert_to_portuguese",
             "convert_to_dutch",
             "convert_to_italian",
             "convert_to_dutch",
             "convert_to_occitan",
             "convert_to_russian",
             "renounce_iconoclasm",                      
             "convert_to_reformed",
        };
        public bool IncludeDecision(String str)
        {
            if (exclude.Contains(str))
                return false;

            return true;
        }
        public void Load()
        {
            var files = Directory.GetFiles(Globals.GameDir + "decisions");

            foreach (var file in files)
            {
                Script s = ScriptLoader.instance.Load(file);
                Scripts.Add(s);
            }
        }

        public void Save()
        {
            foreach (var script in Scripts)
            {
                ExporeDecisions(script.Root.Children[0] as ScriptScope);
        //        script.Save();
            }
        }

        private void ExporeDecisions(ScriptScope node)
        {
            for (int index = 0; index < node.Children.Count; index++)
            {
                var child = node.Children[index];
                
                if (child is ScriptScope)
                {
                    if(!IncludeDecision((child as ScriptScope).Name))
                    {
                        node.Remove(child);
                        index--;
                        continue;
                    }
                  //  RemoveAllReligionTests(child as ScriptScope);
                }
            }
        }

        private void RemoveAllReligionTests(ScriptScope node)
        {
            for (int index = 0; index < node.Children.Count; index++)
            {
                var child = node.Children[index];

                if (child is ScriptScope)
                {

                    RemoveAllReligionTests(child as ScriptScope);
                }
                if (child is ScriptCommand)
                {
                    ScriptCommand c = (ScriptCommand) child;
                    if (c.Name == "religion" || c.Name == "religion_group")
                    {
                        if (c.Value.ToString().ToUpper() == "FROM" || c.Value.ToString().ToUpper() == "ROOT" || c.Value.ToString().ToUpper().Contains("PREV"))
                        {

                        }
                        else
                        {
                            node.Remove(child);
                            index--;

                        }
                        continue;
                    }
                    if (c.Name == "culture" || c.Name == "culture_group")
                    {
                        if (c.Value.ToString().ToUpper() == "FROM" || c.Value.ToString().ToUpper() == "ROOT" || c.Value.ToString().ToUpper().Contains("PREV"))
                        {

                        }
                        else
                        {
                            node.Remove(child);
                            index--;

                        }
                        continue;
                    }
                }
            }
        }
    }
}
