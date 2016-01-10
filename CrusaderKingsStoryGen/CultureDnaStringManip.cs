using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    partial class CulturalDna
    {

     
        public string changeFirstLetter(string input)
        {
            String orig = input;
            String[] sub = input.Split(' ');
            //     if (bVowel)

            String output = "";
            foreach (var s in sub)
            {

                input = s;
                if (input.Trim().Length == 0)
                    return "";
                {
                    bool bVowel = false;
                    if (StartsWithVowel(input))
                        bVowel = true;
                    input = input.Substring(1);

                    String newOne = firstLetters[Rand.Next(firstLetters.Count)];
                    if (!EndsWithVowel(newOne))
                        input = stripConsinentsFromStart(input);

                    while (input.Length > 8)
                    {
                        input = stripVowelsFromStart(input);
                        input = stripConsinentsFromStart(input);
                    }

                    if (bVowel)
                    {
                        input = vowels[Rand.Next(vowels.Count)] + input;
                    }
                    else
                    {
                        if (Rand.Next(2) == 0)
                            input = cons[Rand.Next(cons.Count)] + input;
                    }
                    if (input.Length == 0)
                        return orig;

                    char firstLetter = Char.ToUpper(input[0]);
                    input = firstLetter + input.Substring(1);

                }

                output += input + " ";
            }

            return output.Trim();
        }

        private bool EndsWithVowel(string input)
        {
            for (int n = 0; n < vowels.Count; n++)
            {
                var vowel = vowels[n];
                if (input.EndsWith(vowel))
                {
                    return true;
                }
            }

            return false;
        }
    public string ModifyName(string input)
        {
            String[] str = input.Split(' ');
            if (str.Length > 1)
                input = str[str.Length - 1];

            switch (Rand.Next(2))
            {
                case 0:
                    input = changeRandomVowel(input);
                    break;
                case 1:
                    input = changeFirstLetter(input);
                    break;

            }

            return Capitalize(input);
        }
    
      static List<String> choices = new List<string>();
        
        private string changeRandomVowel(string input)
        {
            choices.Clear();
            for (int n = 0; n < vowels.Count; n++)
            {
                var vowel = vowels[n];
                if (input.Contains(vowel))
                {
                    choices.Add(vowel);
                }
            }

            if (choices.Count == 0)
                return input;

            var vowelc = choices[Rand.Next(choices.Count)];
            //foreach (var vowel in choices)
            {
                input = input.Replace(vowelc, vowels[Rand.Next(vowelc.Length)]);
                return input;

            }

            return input;
        }
        public string Capitalize(string toCap)
        {
            if (string.IsNullOrEmpty(toCap))
                return "";
            toCap = toCap.ToLower().Trim();

            int cons = 0;
            int vow = 0;
            string f = firstLetters[Rand.Next(firstLetters.Count)];
            
            if(vowels.Contains(f))
                toCap = f + stripVowelsFromStart(toCap);
            else
                toCap = f + stripConsinentsFromStart(toCap);
            
            for (int n = 0; n < toCap.Length; n++)
            {
                if (toCap[n] == ' ' || toCap[n] == '-')
                    continue;

                if (vowels.Contains(toCap[n].ToString()) && toCap[n].ToString() != "y")
                {
                    vow++;
                    cons = 0;
                }
                else
                {
                    cons++;
                    vow = 0;
                }
             
                if (cons > 2)
                {
                    toCap = toCap.Substring(0, n) + toCap.Substring(n + 1);
                    cons--;
                }
                if (vow > 2)
                {
                    toCap = toCap.Substring(0, n) + toCap.Substring(n + 1);
                    vow--;
                }
            }

            String[] split = toCap.Split(new char[] {' ', '-'});

            String outp = "";

            if (split.Length == 1)
            {
                String ss = split[0].Trim();
                char firstLetter = Char.ToUpper(ss[0]);
                ss = firstLetter + ss.Substring(1);

                return ss;
            }
            foreach (var s in split)
            {
                String ss = s;
                if (ss.Length == 0)
                    continue;
                char firstLetter = Char.ToUpper(ss[0]);
                ss = firstLetter + ss.Substring(1);

                outp += ss;
                if (outp.Length < toCap.Length)
                {
                    if (toCap[outp.Length] == ' ')
                        outp += " ";
                    else
                        outp += "-";
                    
                }

            }
          
            return outp.Trim();
        }
      
        private string changeRandomLetter(string input)
        {
            {

                int n = Rand.Next(input.Length);

                input = input.Substring(0, n) + CommonStartNames[Rand.Next(CommonStartNames.Count)] + input.Substring(n + 1);


            }

            return input;
        }
        private bool StartsWithVowel(string input)
        {
            for (int n = 0; n < vowels.Count; n++)
            {
                var vowel = vowels[n];
                if (input.StartsWith(vowel.ToUpper()) || input.StartsWith(vowel))
                {
                    return true;
                }
            }

            return false;
        }
        public string stripConsinantsFromEnd(String input)
        {
            for (int x = 0; x < input.Length; x++)
            {
                bool bDoIt = true;
                for (int n = 0; n < vowels.Count; n++)
                {
                    var vowel = vowels[n];
                    if (input.EndsWith(vowel))
                    {
                        n = 100;
                        bDoIt = false;
                        continue;
                    }
                }

                if (bDoIt)
                {
                    input = input.Substring(0, input.Length - 1);
                    return input;
                }
                else
                {
                    break;
                }
            }


            return input;
        }

        public string stripVowelsFromEnd(String input)
        {
            for (int n = 0; n < vowels.Count; n++)
            {
                var vowel = vowels[n];
                if (input.EndsWith(vowel))
                {
                    input = input.Substring(0, input.Length - vowel.Length);
                    n = -1;
                }
            }

            return input;
        }
        public string stripVowelsFromStart(String input)
        {
            for (int n = 0; n < vowels.Count; n++)
            {
                var vowel = vowels[n];
                if (input.StartsWith(vowel.ToUpper()) || input.StartsWith(vowel))
                {
                    input = input.Substring(1);
                    n = -1;
                }
            }

            return input;
        }
        public string stripConsinentsFromStart(String input)
        {
            for (int x = 0; x < input.Length; x++)
            {
                bool bDoIt = true;
                for (int n = 0; n < vowels.Count; n++)
                {
                    var vowel = vowels[n];
                    if (input.StartsWith(vowel.ToUpper()) || input.StartsWith(vowel))
                    {
                        n = 100;
                        bDoIt = false;
                        continue;
                    }
                }

                if (bDoIt)
                {
                    input = input.Substring(1);
                    x = -1;

                }
                else
                {
                    break;
                }



            }

            return input;
        }

        public string GetStart(string str, int min)
        {
            bool bDone = false;
            while (!bDone)
            {
                string last = str;

                if (EndsWithVowel(str))
                {
                    str = stripVowelsFromEnd(str);
                }
                else
                {
                    str = stripConsinantsFromEnd(str);
                    str = stripVowelsFromEnd(str);
                }
                if (str.Length < min)
                {
                    str = last;
                    bDone = true;
                }
            }

            return str;
        }

        public string GetEnd(string str, int min)
        {
            String orig = str;
            bool bDone = false;
            while (!bDone)
            {
                string last = str;

                if (StartsWithVowel(str))
                {
                    str = stripVowelsFromStart(str);
                    str = stripConsinentsFromStart(str);
                }
                else
                {
                    str = stripConsinentsFromStart(str);
                }
                if (str.Length < min)
                {
                    str = last;
                    bDone = true;
                }
            }

            if (str == orig)
                return "";

            return str;
        }

        public string GetMiddle(string str, int min)
        {
            bool bDone = false;
            while (!bDone)
            {
                string last = str;

                if (EndsWithVowel(str))
                {
                    str = stripVowelsFromEnd(str);
                }
                else
                {
                    str = stripConsinantsFromEnd(str);
                }
                if (StartsWithVowel(str))
                {
                    str = stripVowelsFromStart(str);
                }
                else
                {
                    str = stripConsinentsFromStart(str);
                }
                if (str.Length < min)
                {
                    str = last;
                    bDone = true;
                }
            }

            return str;
        }
        public void Cannibalize(string str)
        {
            int min = (str.Length/2)-1;
            if (min < 3)
                min = 3;

            String start = GetStart(str, min-1);
            if (start.Trim().Length > 0 && !CommonStartNames.Contains(start))
                CommonStartNames.Add(start);
            String end = GetEnd(str, min);

            if (end.Length > 5)
            {
                String mid = end;
                mid = GetStart(mid, end.Length/3);
       //         CommonMiddleNames.Add(mid);
                end = end.Substring(mid.Length);
            }

            if (end.Trim().Length > 0 && !CommonEndNames.Contains(end))
                CommonEndNames.Add(end);
        }

        public String GetMaleCharacterName()
        {
            String word = ConstructWord(5, 10);
            //return Capitalize(GetCommonStartName() + GetCommonMiddleName() + GetCommonEndName());
            return word;
        }

        public string ConstructWord(float min, float max)
        {
            String str = "";
            do
            {
                String start = GetCommonStartName();
                String mid = GetCommonMiddleName();
                String end = GetCommonEndName();

                if (EndsWithVowel(start) && StartsWithVowel(mid))
                {
                    mid = stripVowelsFromStart(mid);
                }
                if (!EndsWithVowel(start) && !StartsWithVowel(mid))
                {
                    mid = stripConsinentsFromStart(mid);
                }
                int tot = start.Length + mid.Length + end.Length;

                if (tot > max)
                {
                    if (start.Length + end.Length < max && start.Length + end.Length > min)
                    {
                        if (EndsWithVowel(start) && StartsWithVowel(end))
                        {
                            end = stripVowelsFromStart(end);
                        }
                        if (!EndsWithVowel(start) && !StartsWithVowel(end))
                        {
                            end = stripConsinentsFromStart(end);
                        }
                    }
                    else while (!(start.Length + end.Length < max && start.Length + end.Length >= min) && start.Length > 0 && end.Length > 0)
                    {
                        if (start.Length > end.Length)
                        {
                            if (EndsWithVowel(start))
                            {
                                start = stripVowelsFromEnd(start);
                            }
                            else
                            {
                                start = stripConsinantsFromEnd(start);
                            }
                        }                      
                        else                     
                        {
                            if (StartsWithVowel(end))
                            {
                                end = stripVowelsFromStart(end);
                            }
                            else
                            {
                                end = stripConsinentsFromStart(end);
                            }
                        } 
                    }
                    str = start  + end;
                }
                else
                {
                    if (EndsWithVowel(mid) && StartsWithVowel(end))
                    {
                        end = stripVowelsFromStart(end);
                    }
                    if (!EndsWithVowel(mid) && !StartsWithVowel(end))
                    {
                        end = stripConsinentsFromStart(end);
                    }

                    str = start + mid + end;
                }
              
            } while (str.Length < min || str.Length >= max);

            str = str.Replace('"'.ToString(), "");

            if (str.Length > min)
            {
                int rem = Rand.Next(str.Length - (int)min);

                while (rem > 0)
                {
                    if (Rand.Next(2) == 0)
                    {
                        if (StartsWithVowel(str))
                            str = stripVowelsFromStart(str);
                        else
                            str = stripConsinentsFromStart(str);
                    }
                    else
                    {
                        if (StartsWithVowel(str))
                            str = stripVowelsFromEnd(str);
                        else
                            str = stripConsinantsFromEnd(str);
                    }

                    rem--;
                }
          
            }

            if (str.Trim().Length == 1)
                return ConstructWord(min, max);

            return Capitalize(str);
        }

        private String GetCommonStartName()
        {
            return CommonStartNames[Rand.Next(CommonStartNames.Count)];
        }

        private String GetCommonMiddleName()
        {
            if (CommonMiddleNames.Count == 0)
                return "";
            return CommonMiddleNames[Rand.Next(CommonMiddleNames.Count)];
        }
        private String GetCommonEndName()
        {
            return CommonEndNames[Rand.Next(CommonEndNames.Count)];
        }

        public String GetPlaceName()
        {
            if (WordFormats.Count == 0)
            {
                for (int n = 0; n < 1; n++)
                {
                    WordFormats.Add(CommonWordFormats[Rand.Next(CommonWordFormats.Count)]);
                }            
            }
            if (wordsForLand.Count==0)
            {
                string s = ConstructWord(2*wordLengthBias, 4*wordLengthBias);
                wordsForLand.Add(s);
             
              
            }

            if (placeFormat == null)
            {
                placeFormat = placeFormatOptions[Rand.Next(placeFormatOptions.Count())];
            }

            string format = this.WordFormats[Rand.Next(WordFormats.Count)];

            int nWords = 1;

            if (Rand.Next(3) == 0)
            {
                nWords++;
            }

            if (Rand.Next(3) == 0)
            {
                nWords++;
            }

            int index = 0;

            if (nWords == 1)
            {
                index = format.IndexOf("{0}") + 3;
                format = format.Substring(0, index);
            }
            if (nWords == 2)
            {
                index = format.IndexOf("{1}") + 3;
                format = format.Substring(0, index);
            }
            String[] strs = new String[nWords];
            for (int n = 0; n < nWords; n++)
            {
                if(nWords==1)
                    strs[n] = ConstructWord(5*wordLengthBias, 8*wordLengthBias);
                else if (nWords == 2)
                {
                    strs[n] = ConstructWord(3 * wordLengthBias, 5 * wordLengthBias);
                } else if (nWords == 3)
                {
                    strs[n] = ConstructWord(2 * wordLengthBias, 4 * wordLengthBias);
                }

            }
            if(nWords == 1)
                return Capitalize(String.Format(format, strs[0]));
            else if(nWords == 2)
                return Capitalize(String.Format(format, strs[0], strs[1]));
            else
                return Capitalize(String.Format(format, strs[0], strs[1], strs[2]));

            if (Rand.Next(2) == 0)
                return
                    Capitalize(String.Format(placeFormat, wordsForLand[Rand.Next(wordsForLand.Count)],
                        ConstructWord(3*wordLengthBias, 7*wordLengthBias)));
            else
            {
                if (Rand.Next(4) == 0)
                {
                    return ConstructWord(6 * wordLengthBias, 8 * wordLengthBias);
                }
                else
                {
                    return Capitalize(ConstructWord(3 * wordLengthBias, 5 * wordLengthBias) + " " + ConstructWord(3 * wordLengthBias, 4 * wordLengthBias));
                }
            }
        }

        public string GetMaleName()
        {
            var name = ConstructWord(3 * wordLengthBias, 8 * wordLengthBias);

            name = stripVowelsFromEnd(name);

            return name;
        }
        public string GetDynastyName()
        {
            var name = ConstructWord(6 * wordLengthBias, 8 * wordLengthBias);
            if(Rand.Next(3)==0)
                ConstructWord(6 * wordLengthBias, 10 * wordLengthBias);
                   
            return name;
        }

        public string GetFemaleName()
        {
            var name = ConstructWord(3 * wordLengthBias, 8 * wordLengthBias);

            name = stripConsinantsFromEnd(name);

            return name;
        }

        public string GetGodName()
        {
            var name = ConstructWord(3 * wordLengthBias, 5 * wordLengthBias);

            if(Rand.Next(3)==0)
            name = stripConsinantsFromEnd(name);

            return name;
        }

        public void ShortenWordCounts()
        {
            int targ = CommonStartNames.Count/4;
            int targend = CommonEndNames.Count/4;

            targ = Math.Min(20, targ);
            targend = Math.Min(20, targend);

            while (CommonStartNames.Count > targ)
                CommonStartNames.RemoveAt(Rand.Next(CommonStartNames.Count));
            while (CommonEndNames.Count > targend)
                CommonEndNames.RemoveAt(Rand.Next(CommonEndNames.Count));
        }
    }
}
