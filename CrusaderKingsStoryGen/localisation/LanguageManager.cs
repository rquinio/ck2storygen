using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class LanguageManager
    {
        public static LanguageManager instance = new LanguageManager();
        Dictionary<String, String> english = new Dictionary<string, string>();

        public LanguageManager()
        {
            
            
        }
        public String Add(String key, String english)
        {
          
            this.english[key] = english;
            return english;
        }

        public void Save()
        {
            String filename = "localisation\\aaa_genLanguage.csv";

            filename = Globals.ModDir + filename;

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
            {
                foreach (var entry in english)
                {
                    file.Write(entry.Key + ";" + entry.Value + ";;;;;;;;;;;;;\n");
                }

                file.Close();
            }

            //thing;eng;;;;;;;;;;;;;
        }

        public void Remove(string name)
        {
            this.english.Remove(name);
        }

        public string Get(string name)
        {
            if (!english.ContainsKey(name))
                return null;
            return english[name];
        }

        public string AddSafe(string name)
        {
            String safe = StarNames.SafeName(name);
            Add(safe, name);
            return safe;
        }
    }
}
