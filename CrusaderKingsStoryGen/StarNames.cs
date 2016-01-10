using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    public static class StarNames
    {
        static string[] vowels = new string[] { "a", "e", "i", "o", "u", "ae", "y" };

        private static string[] namePartsStart = new string[]
        {
            "a", "aba", "aca", "ada", "ara", "ala", "ama", "ana", "arc", "ard", "arg", "arm", "arn", "art", "ass", "ast", "alb",
            "ald", "alm", "all", "ant", "and", "ar", "an", "am", "as", "at", "al", "att", "bab", "bad", "ban", "bann", "bar", "bra", "can", "car", "cra", "cart", "cars", "cann",
            "dan", "dar", "dart", "darn", "da", "ra", "ran", "ras", "rag", "tar", "tan", "xan", "xar", "pa", "pas", "past", "par", "pra", "part", "pat", "patt", "pac",
            "pad", "pan", "pam", "pag", "pra", "sa", "sad", "san", "sam", "sat", "sap", "sar", "sca", "tra"
        };

        private static string[] namePartsEnd = new string[] { "a", "i", "ae", "ia", "ai", "ar", "as", "air", "ain", "ris", "ran", "ras", "ron", "ros", "us", "os", "ous", "on", "uon", "ion", "ios", "iak", "ious", "fi", "ami", "kat", "la", "las", "lon" };

        private static string[] nameAdditional = new string[] { "System", "Cluster", "Majoris", "Minoris", "Major", "Minor", "Star", "Primus" };
       
        public static string Generate(int seed)
        {
            string name = "";
            var rand = Rand.Get();
           
            int syllabuls = rand.Next(2) + 1;

            for (int n = 0; n < syllabuls; n++)
            {
                string wordPart = namePartsStart[rand.Next(namePartsStart.Length)];
                foreach (char c in wordPart)
                {
                    if (c == 'a')
                    {
                        string v = vowels[rand.Next(vowels.Length)];
                        if (v == "y")
                            v = vowels[rand.Next(vowels.Length)];

                        name += vowels[rand.Next(vowels.Length)];
                    }
                    else
                    {
                        name += c;
                    }
                }
            }

            name += namePartsEnd[rand.Next(namePartsEnd.Length)];

            char firstLetter = Char.ToUpper(name[0]);
            name = firstLetter + name.Substring(1);


            return name;
        }

        public static string SafeName(string name)
        {
            name = name.ToLower();
            name = name.Replace(" ", "");
            name = name.Replace("-", "");
            // name += (Rand.Next(10000) + 1).ToString();
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            name = rgx.Replace(name, "");

            return name;
        }

        public static string Generate(string culture)
        {
            return CultureManager.instance.CultureMap[culture].dna.GetPlaceName();
        }
    }
}
