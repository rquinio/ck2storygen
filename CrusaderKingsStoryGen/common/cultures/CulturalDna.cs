using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace CrusaderKingsStoryGen
{
    partial class CulturalDna
    {
        private List<String> vowels = new List<string>() { "a", "e", "i", "o", "u", "ae", "y" };
        public CultureParser culture;

        public static string[] placeFormatOptions = new[] { "{1}{0}"};
        public string placeFormat = null;
        public float wordLengthBias = 1.0f;

        public String empTitle = "";
        public String kingTitle = "";
        public String dukeTitle = "";
        public String countTitle = "";
        public String baronTitle = "";
        public String mayorTitle = "";
        public String femaleEnd = "";
        private List<String> wordsForLand = new List<string>()
        {
            
        };

        public static List<String> CommonWordFormats = new List<string>()
        {
            "{0}-{1}-{2}",
            "{0} {1}-{2}",
            "{0} {1} {2}",
            "{0}-{1} {2}",
            "{0}'{1}'{2}",
            "{0}-{1}'{2}",
            "{0}{1}'{2}",
            "{0}'{1}{2}",
            "{0}'{1}-{2}",
        };

        public List<String> WordFormats = new List<string>()
        {
            
        };

        public CulturalDna()
        {
            wordLengthBias = 1.0f;
        }
        private List<String> firstLetters = new List<string>()
        {
            "a", "e", "d", "k", "q", "w", "r", "t", "r", "s", "t", "l", "n", "k", "b",
        };

        List<String> cons = new List<String> { "q", "w", "r", "t", "r", "s", "t", "l", "n", "k", "b" };

        private List<String> CommonStartNames = new List<string>();
        private List<String> CommonMiddleNames = new List<string>();
        private List<String> CommonEndNames = new List<string>(); 
        public List<string> portraitPool = new List<string>();
        public bool dukes_called_kings = false;
        public bool baron_titles_hidden =false;
        public bool count_titles_hidden = false;
        public bool allow_looting = false;
        public bool seafarer = false;
        public string male_patronym = "son";
        public string female_patronym = "daughter";
        public bool patronym_prefix = false;
        public bool founder_named_dynasties = false;
        public bool dynasty_title_names = false;
        public string Name { get; set; }
        public string from_dynasty_prefix = "af ";
        public bool horde = false;
        public CultureParser dna;
        private bool tribal;

        public void CreateFromCulture(CultureParser c)
        {
            dna = c;
        }

        public CulturalDna MutateSmall(int numChanges)
        {
            var c = new CulturalDna();

            c.empTitle = empTitle;
            c.kingTitle = kingTitle;
            c.dukeTitle = dukeTitle;
            c.countTitle = countTitle;
            c.baronTitle = baronTitle;
            c.mayorTitle = mayorTitle;
 
            c.tribal = tribal;
            c.wordsForLand.AddRange(wordsForLand);
            //c.cons.AddRange(cons);
            c.WordFormats.AddRange(CommonWordFormats);
          //  c.vowels.Clear();
            //c.vowels.AddRange(vowels);
            c.CommonStartNames.AddRange(CommonStartNames);
            c.CommonMiddleNames.AddRange(CommonMiddleNames);
            c.CommonEndNames.AddRange(CommonEndNames);
            c.portraitPool.AddRange(portraitPool);
            c.placeFormat = placeFormat;
            c.firstLetters.AddRange(firstLetters);
            c.wordLengthBias = wordLengthBias;
            c.from_dynasty_prefix = from_dynasty_prefix;
            c.count_titles_hidden = count_titles_hidden;
            c.baron_titles_hidden = baron_titles_hidden;
            c.allow_looting = allow_looting;
            c.female_patronym = female_patronym;
            c.male_patronym = male_patronym;
            c.founder_named_dynasties = founder_named_dynasties;
            c.dynasty_title_names = dynasty_title_names;
            c.horde = horde;
            c.culture = culture;
            for (int n = 0; n < numChanges; n++)
            {
                c.DoRandomSmallChange();
            }

            return c;
        }

        public CulturalDna Mutate(int numChanges)
        {
            var c = new CulturalDna();
            c.empTitle = empTitle;
            c.kingTitle = kingTitle;
            c.dukeTitle = dukeTitle;
            c.countTitle = countTitle;
            c.baronTitle = baronTitle;
            c.mayorTitle = mayorTitle;
            
            c.tribal = tribal;
            c.wordsForLand.AddRange(wordsForLand);
            //c.cons.AddRange(cons);
         //   c.vowels.AddRange(vowels);
            c.CommonStartNames.AddRange(CommonStartNames);
            c.CommonMiddleNames.AddRange(CommonMiddleNames);
            c.CommonEndNames.AddRange(CommonEndNames);
            c.WordFormats.AddRange(CommonWordFormats);
            c.portraitPool.AddRange(portraitPool);
            c.placeFormat = placeFormat;
            c.firstLetters.AddRange(firstLetters);
            c.wordLengthBias = wordLengthBias;
            c.from_dynasty_prefix = from_dynasty_prefix;
            c.count_titles_hidden = count_titles_hidden;
            c.baron_titles_hidden = baron_titles_hidden;
            c.allow_looting = allow_looting;
            c.female_patronym = female_patronym;
            c.male_patronym = male_patronym;
            c.founder_named_dynasties = founder_named_dynasties;
            c.dynasty_title_names = dynasty_title_names;
            c.horde = horde;
            c.culture = culture;
            for (int n = 0; n < numChanges; n++)
            {
                c.DoRandomChange();
            }

            return c;
        }

        public void DoRandom()
        {
            from_dynasty_prefix = ConstructWord(2, 4).ToLower() + " ";
            this.female_patronym = ConstructWord(3, 4).ToLower();
            this.male_patronym = ConstructWord(3, 4).ToLower();
                             
            allow_looting = Rand.Next(2) == 0;
            if (allow_looting && GovernmentManager.instance.numNomadic < 10)
                horde = Rand.Next(2) == 0;
            else
            {
                horde = false;
            }

            empTitle = ConstructWord(2, 5);
            kingTitle = ConstructWord(2, 5);
            dukeTitle = ConstructWord(2, 5);
            countTitle = ConstructWord(2, 5);
            baronTitle = ConstructWord(2, 5);
            mayorTitle = ConstructWord(2, 5);

            empTitle = LanguageManager.instance.AddSafe(empTitle);
            kingTitle = LanguageManager.instance.AddSafe(kingTitle);
            dukeTitle = LanguageManager.instance.AddSafe(dukeTitle);
            countTitle = LanguageManager.instance.AddSafe(countTitle);
            baronTitle = LanguageManager.instance.AddSafe(baronTitle);
            mayorTitle = LanguageManager.instance.AddSafe(mayorTitle);
 
        }
        private void DoRandomChange()
        {
            switch (Rand.Next(4))
            {
                case 0:
                    // replace 1/3 of the Start words and replace with random dna culture....
                    {
                        int count = CommonStartNames.Count / 2;
                        WordFormats.Clear();
                         ReplaceStartNames(count);                        
                        {
                            wordsForLand.Clear();
                            int c = wordsForLand.Count / 2;
                            int create = 3;
                       
                            for (int n = 0; n < create; n++)
                            {
                                String a = "";
                                a = ConstructWord(2 * wordLengthBias, 4 * wordLengthBias);

                                if (!wordsForLand.Contains(a))
                                {
                                    wordsForLand.Add(a);
                                    continue;
                                }

                            }

                        }

                    }
                    break;

                case 1:
                    {

                        int count = CommonEndNames.Count / 2;
                        ReplaceEndNames(count);
                        WordFormats.Clear();
                        {
                            wordsForLand.Clear();
                            int c = wordsForLand.Count / 2;
                            int create = 3;

                            for (int n = 0; n < create; n++)
                            {
                                String a = "";
                                a = ConstructWord(2 * wordLengthBias, 4 * wordLengthBias);
                               
                                if (!wordsForLand.Contains(a))
                                {
                                    wordsForLand.Add(a);
                                    continue;
                                }

                            }

                        }
                    }
                    break;

                case 2:

                    // replace 1/3 of the words for land
                    {
                        WordFormats.Clear();
                        if (portraitPool.Count == 0)
                        {
                            return;
                        }

                        int x = Rand.Next(portraitPool.Count);

                        var por = portraitPool[x];
                        portraitPool.RemoveAt(x);
                        portraitPool.Add(culture.GetRelatedCultureGfx(por));

                        if (Rand.Next(6) == 0)
                        {
                            if (portraitPool.Count == 1)
                            {
                                portraitPool.Add(culture.GetRelatedCultureGfx(por));

                            }
                            else if (portraitPool.Count == 2)
                            {
                                portraitPool.RemoveAt(Rand.Next(2));
                            }

                        }

                        if (Rand.Next(6) == 0)
                        {
                            portraitPool.RemoveAt(0);
                            portraitPool.Add(CultureParser.GetRandomCultureGraphics());

                        }
                    }

                    break;

                case 3:

                 //   for(int n=0;n<3;n++)
                    {
                       // switch (Rand.Next(6))
                        {
                        //    case 0:
                                empTitle = ConstructWord(2, 5);
                                empTitle = LanguageManager.instance.AddSafe(empTitle);

                         //       break;
                        //    case 1:
                                kingTitle = ConstructWord(2, 5);
                                kingTitle = LanguageManager.instance.AddSafe(kingTitle);

                          //      break;
                          //  case 2:
                                dukeTitle = ConstructWord(2, 5);
                                dukeTitle = LanguageManager.instance.AddSafe(dukeTitle);

                         //       break;
                         //   case 3:
                                countTitle = ConstructWord(2, 5);
                                countTitle = LanguageManager.instance.AddSafe(countTitle);

                           //     break;
                        //    case 4:
                                baronTitle = ConstructWord(2, 5);
                                baronTitle = LanguageManager.instance.AddSafe(baronTitle);

                        //        break;
                        //    case 5:
                                mayorTitle = ConstructWord(2, 5);
                                mayorTitle = LanguageManager.instance.AddSafe(mayorTitle);

                         //       break;
                        }
                    }

                    break;



            }

            if (culture != null)
                culture.DoDetailsForCulture();
        }
        private void DoRandomSmallChange()
        {
          
            {
                switch (Rand.Next(11))
                {
                    case 0:
                        founder_named_dynasties = !founder_named_dynasties;
                        break;
                    case 1:
                        dynasty_title_names = !dynasty_title_names;
                        break;
                     case 2:
                        baron_titles_hidden = !baron_titles_hidden;
                        break;
                    case 3:
                        count_titles_hidden = !count_titles_hidden;
                        break;
                    case 4:
                        dukes_called_kings = !dukes_called_kings;
                        break;
                    case 5:
                        from_dynasty_prefix = ConstructWord(2, 3);
                        break;
                    case 6:
                        this.female_patronym = ConstructWord(3, 4).ToLower();
                        this.male_patronym = ConstructWord(3, 4).ToLower();
                        if (Rand.Next(10) == 0)
                        {
                            this.female_patronym = ConstructWord(3, 4);
                            this.male_patronym = ConstructWord(3, 4);

                        }
                        break;
                    case 7:
                        // Change place format
                        {
                            this.placeFormat = null;
                        }
                        break;
                    case 8:
                        WordFormats.RemoveAt(Rand.Next(WordFormats.Count));
                        WordFormats.Add(CommonWordFormats[Rand.Next(CommonWordFormats.Count)]);
                        break;
                    case 9:
                        tribal = !tribal;
                        break;
                    case 10:

                    //    for (int n = 0; n < 3; n++)
                        {
                            switch (Rand.Next(6))
                            {
                                case 0:
                                    empTitle = ConstructWord(2, 5);
                                    empTitle = LanguageManager.instance.AddSafe(empTitle);

                                    break;
                                case 1:
                                    kingTitle = ConstructWord(2, 5);
                                    kingTitle = LanguageManager.instance.AddSafe(kingTitle);

                                    break;
                                case 2:
                                    dukeTitle = ConstructWord(2, 5);
                                    dukeTitle = LanguageManager.instance.AddSafe(dukeTitle);

                                    break;
                                case 3:
                                    countTitle = ConstructWord(2, 5);
                                    countTitle = LanguageManager.instance.AddSafe(countTitle);

                                    break;
                                case 4:
                                    baronTitle = ConstructWord(2, 5);
                                    baronTitle = LanguageManager.instance.AddSafe(baronTitle);

                                    break;
                                case 5:
                                    mayorTitle = ConstructWord(2, 8);
                                    mayorTitle = LanguageManager.instance.AddSafe(mayorTitle);

                                    break;
                            }
                        }

                        break;
                }

                  



            }

            if (culture != null)
                culture.DoDetailsForCulture();
        }

        private void ReplaceStartNames(int count)
        {
            int i = CommonStartNames.Count;
            int c = count;
            int removed = c;
            for (int n = 0; n < c; n++)
                CommonStartNames.RemoveAt(Rand.Next(CommonStartNames.Count));

            while (CommonStartNames.Count < i)
                AddRandomStartNames(i-CommonStartNames.Count);

        }

        private void ReplaceMiddleNames(int count)
        {
            return;

            while (CommonMiddleNames.Count < count)
            {
                AddRandomMiddleNames(count-CommonMiddleNames.Count);
            }
            int c = count;
            int removed = c;
            for (int n = 0; n < c; n++)
            {
                
                CommonMiddleNames.RemoveAt(Rand.Next(CommonMiddleNames.Count));
            }

            while (CommonMiddleNames.Count < c)
                AddRandomMiddleNames(c-CommonMiddleNames.Count);

        }

        private void ReplaceEndNames(int count)
        {
            int i = CommonEndNames.Count;
            int c = count;
            int removed = c;
            for (int n = 0; n < c; n++)
                CommonEndNames.RemoveAt(Rand.Next(CommonEndNames.Count));

            while (CommonEndNames.Count < i)
                AddRandomEndNames(i-CommonEndNames.Count);

        }

        private int AddRandomStartNames(int count)
        {
            int c = count;
            CulturalDna dna = CulturalDnaManager.instance.GetVanillaCulture((string)null);
            List<String> choices = new List<string>();
            int added = 0;
            for (int n = 0; n < c; n++)
            {
                String str = dna.CommonStartNames[Rand.Next(dna.CommonStartNames.Count)];
                if (CommonStartNames.Contains(str))
                {
                }
                else
                {
                    choices.Add(str);
                }
            }


            if (choices.Count > 0)
            {
                c = Math.Min(choices.Count, c);
                for (int n = 0; n < c; n++)
                {
                    var cc = choices[Rand.Next(choices.Count)];
                    CommonStartNames.Add(cc);
                    choices.Remove(cc);
                    n--;
                    c = Math.Min(choices.Count, c);
                    added++;
                }
            }


            return added;

        }
        private int AddRandomMiddleNames(int count)
        {
            int c = count;
            CulturalDna dna = CulturalDnaManager.instance.GetVanillaCulture((string)null);
            if (dna.CommonMiddleNames.Count == 0)
                return 0;
            List<String> choices = new List<string>();
            int added = 0;
            for (int n = 0; n < c; n++)
            {
                String str = dna.CommonMiddleNames[Rand.Next(dna.CommonMiddleNames.Count)];
                if (CommonMiddleNames.Contains(str))
                {
                }
                else
                {
                    choices.Add(str);
                }
            }


            if (choices.Count > 0)
            {
                c = Math.Min(choices.Count, c);
                for (int n = 0; n < c; n++)
                {
                    var cc = choices[Rand.Next(choices.Count)];
                    CommonMiddleNames.Add(cc);
                    choices.Remove(cc);
                    n--;
                    c = Math.Min(choices.Count, c);
                    added++;
                }
            }


            return added;

        }

        private int AddRandomEndNames(int count)
        {
            int c = count;
            CulturalDna dna = CulturalDnaManager.instance.GetVanillaCulture((string)null);
            List<String> choices = new List<string>();
            int added = 0;
            for (int n = 0; n < c; n++)
            {
                String str = dna.CommonEndNames[Rand.Next(dna.CommonEndNames.Count)];
                if (CommonEndNames.Contains(str))
                {
                }
                else
                {
                    choices.Add(str);
                }
            }


            if (choices.Count > 0)
            {
                c = Math.Min(choices.Count, c);
                for (int n = 0; n < c; n++)
                {
                    var cc = choices[Rand.Next(choices.Count)];
                    CommonEndNames.Add(cc);
                    choices.Remove(cc);
                    n--;
                    c = Math.Min(choices.Count, c);
                    added++;
                }
            }


            return added;

        }

        public string GetMaleNameBlock()
        {
            StringBuilder b = new StringBuilder();
            for (int n = 0; n < 200; n++)
            {
                b.Append(GetMaleName() + " ");
            }
            return b.ToString();
        }

        public string GetFemaleNameBlock()
        {
            StringBuilder b = new StringBuilder();
            for (int n = 0; n < 200; n++)
            {
                b.Append(GetFemaleName() + " ");
            }
            return b.ToString();
        }
    }
}