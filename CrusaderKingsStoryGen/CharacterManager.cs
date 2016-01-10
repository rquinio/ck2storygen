using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrusaderKingsStoryGen.Simulation;

namespace CrusaderKingsStoryGen
{
    class CharacterManager
    {
        public static CharacterManager instance = new CharacterManager();
        public List<CharacterParser> Characters = new List<CharacterParser>();
        public List<CharacterParser> AddedSinceLastPrune = new List<CharacterParser>();
        public Dictionary<int, CharacterParser> CharacterMap = new Dictionary<int, CharacterParser>();
        private bool bInit;
        
        public void Init()
        {
            bInit = true;
            CharactersScript = new Script();
            CharactersScript.Name = Globals.ModDir + "history\\characters\\characters.txt";
            CharactersScript.Root = new ScriptScope();
            CharactersScript.Root.AllowDuplicates = false;
            CharactersScript.Root.Clear();
            int d = 768 - 18;
            foreach (var child in CharactersScript.Root.Children)
            {
                if (child is ScriptScope)
                {
                    
                    Unpicked.Add((ScriptScope)child);
                }    
            }
            
       //     CharactersScript.Save();
        }

        public void Save()
        {
            for (int index = 0; index < Characters.Count; index++)
            {
                var characterParser = Characters[index];
               
                characterParser.UpdateCultural();
            }
            List<CharacterParser> withoutFamilies = new List<CharacterParser>(Characters);

            foreach (var characterParser in withoutFamilies)
            {
                if (characterParser.PrimaryTitle == null)
                    continue;
                if(characterParser.PrimaryTitle.Rank == 0)
                    continue;
                if (characterParser.PrimaryTitle.SubTitles.Count == 0 && characterParser.PrimaryTitle.Owns.Count == 0)
                    continue;

                if (characterParser.PrimaryTitle.Rank == 1)
                    characterParser.CreateFamily(0, 0);
                else if (characterParser.PrimaryTitle.Rank == 2)
                    characterParser.CreateFamily(0, 0);
                else if (characterParser.PrimaryTitle.Rank == 3)
                    characterParser.CreateFamily(0, 4);
                else if (characterParser.PrimaryTitle.Rank == 4)
                    characterParser.CreateFamily(0, 4);
                characterParser.UpdateCultural();
            }
            CharacterManager.instance.CalculateAllDates();
        
            CharactersScript.Save();
        }

        public void Prune()
        {      
            AddedSinceLastPrune.Clear();
        }

        private void RemoveDeathDates(ScriptScope scope)
        {
            foreach (var child in scope.Children)
            {
      
                if (child is ScriptScope)
                {
                    ScriptScope c = (ScriptScope)child;
                    if (c.Children[0] is ScriptCommand && (c.Children[0] as ScriptCommand).Name == "death")
                    {
                        c.Parent.Remove(c);
                        return;
                    }
                }
            }
        }

        public void SetAllDates(int birth, int death, ScriptScope scope)
        {
            foreach (var child in scope.Children)
            {
                if (child is ScriptCommand)
                {
                    ScriptCommand c = (ScriptCommand)child;
                    if (c.Name == "birth")
                    {
                       // if (c.Value.ToString().Split('.').Length == 3)
                        {
                            c.Value = birth + ".1.1";
                            scope.Name = c.Value.ToString();
                        }
                    }
                    if (c.Name == "death")
                    {
                      //  if (c.Value.ToString().Split('.').Length == 3)
                        {
                            c.Value = death + ".2.1";
                            scope.Name = c.Value.ToString();
                        }
                    }

                 
                } 
                if (child is ScriptScope)
                {
                    ScriptScope c = (ScriptScope)child;
                   
                    SetAllDates(birth, death, c);
                }
            }
        }

        public Script CharactersScript { get; set; }
        public List<ScriptScope> Unpicked = new List<ScriptScope>(); 
        public CharacterParser GetNewCharacter(bool adult = false)
        {
            if (!bInit)
                Init();
            
         //   var scope = new ScriptScope();
          //  scope.Name = CharacterParser.IDMax.ToString();
          //  scope.SetChild(CharactersScript.Root);
            var chr = new CharacterParser();
            
            //   chr.SetProperty("dynasty", Rand.Next(1235)+1);
        //    chr.SetProperty("culture", new ScriptReference("norse"));
       //     chr.SetProperty("religion", new ScriptReference("pagan"));
            //  chr.DeleteProperty("name");
            Characters.Add(chr);
            this.CharactersScript.Root.SetChild(chr.Scope);
            AddedSinceLastPrune.Add(chr);
            CharacterMap[chr.ID] = chr;
            return chr;
        }

        public ScriptScope GetNewCreatedCharacter()
        {
            var scope = new ScriptScope();
            scope.Name = CharacterParser.IDMax.ToString();
           
            scope.Add("name", "Bob");
            scope.Add("culture", "norse");
            scope.Add("religion", "pagan");
            var born = scope.AddScope("730.1.1");
            
            var died = scope.AddScope("790.1.1");
            born.Add("birth", "730.1.1");
            died.Add("death", "790.1.1");
            //scope.SetChild(CharactersScript.Root);
            return scope;
        }

        public CharacterParser CreateNewCharacter(Dynasty dynasty, bool bFemale, int dateOfBirth, string religion, String culture)
        {
            if (!bInit)
                Init();

            //   var scope = new ScriptScope();
            //  scope.Name = CharacterParser.IDMax.ToString();
            //  scope.SetChild(CharactersScript.Root);
            var chr = new CharacterParser();
            //   chr.SetProperty("dynasty", Rand.Next(1235)+1);
            //    chr.SetProperty("culture", new ScriptReference("norse"));
            //     chr.SetProperty("religion", new ScriptReference("pagan"));
            //  chr.DeleteProperty("name");
            Characters.Add(chr);
            chr.YearOfBirth = dateOfBirth;
            chr.Dynasty = dynasty;
            chr.religion = religion;
            chr.isFemale = bFemale;
            chr.culture = culture;
            chr.YearOfDeath = 769 + Rand.Next(30);
            this.CharactersScript.Root.SetChild(chr.Scope);
            AddedSinceLastPrune.Add(chr);
            CharacterMap[chr.ID] = chr;
            chr.SetupExistingDynasty();
            chr.UpdateCultural();
            return chr;
        }

        public CharacterParser CreateNewCharacter(String culture, String religion, bool bFemale)
        {
            if (!bInit)
                Init();

            //   var scope = new ScriptScope();
            //  scope.Name = CharacterParser.IDMax.ToString();
            //  scope.SetChild(CharactersScript.Root);
            var chr = new CharacterParser();
            //   chr.SetProperty("dynasty", Rand.Next(1235)+1);
            //    chr.SetProperty("culture", new ScriptReference("norse"));
            //     chr.SetProperty("religion", new ScriptReference("pagan"));
            //  chr.DeleteProperty("name");
            Characters.Add(chr);
            chr.YearOfBirth = 769 - Rand.Next(60);
            chr.Dynasty = DynastyManager.instance.GetDynasty(CultureManager.instance.CultureMap[culture]);
            chr.religion = religion;
            chr.isFemale = bFemale;
            chr.culture = culture;
            chr.YearOfDeath = 769 + Rand.Next(30);
            this.CharactersScript.Root.SetChild(chr.Scope);
            AddedSinceLastPrune.Add(chr);
            CharacterMap[chr.ID] = chr;
            chr.SetupExistingDynasty();
            chr.UpdateCultural();
            return chr;
        }

        public CharacterParser CreateNewHistoricCharacter(Dynasty dynasty, bool bFemale, string religion, String culture, int dateOfBirth, int dateOfDeath = -1, bool adult = true)
        {
            if (!bInit)
                Init();

            //   var scope = new ScriptScope();
            //  scope.Name = CharacterParser.IDMax.ToString();
            //  scope.SetChild(CharactersScript.Root);
            var chr = new CharacterParser();
            //   chr.SetProperty("dynasty", Rand.Next(1235)+1);
            //    chr.SetProperty("culture", new ScriptReference("norse"));
            //     chr.SetProperty("religion", new ScriptReference("pagan"));
            //  chr.DeleteProperty("name");
            Characters.Add(chr);
            chr.YearOfBirth = dateOfBirth;
            chr.isFemale = bFemale;
            chr.culture = culture;
            chr.religion = religion;
            if (dateOfDeath != -1)
            {
                chr.YearOfDeath = dateOfDeath;
            }
            else
            {
                chr.YearOfDeath = dateOfBirth + Rand.Next(40);
                if(Rand.Next(4)==0)
                    chr.YearOfDeath = dateOfBirth + Rand.Next(80);
                
                if (adult)
                    chr.YearOfDeath = dateOfBirth + 16 + Rand.Next(80 - 16);
  
            }
           
            this.CharactersScript.Root.SetChild(chr.Scope);
            AddedSinceLastPrune.Add(chr);
            CharacterMap[chr.ID] = chr;
            chr.Dynasty = dynasty;
            chr.SetupExistingDynasty();
            chr.UpdateCultural();
        //    CharacterManager.instance.SetAllDates(chr.YearOfBirth, chr.YearOfDeath,  chr.Scope);
            return chr;
        }

        public CharacterParser FindUnmarriedChildbearingAgeWomen(int year, string religion)
        {
            foreach (var characterParser in Characters)
            {
                if (!characterParser.isFemale)
                    continue;

                if (year < characterParser.YearOfBirth + 16)
                    continue;
                if (year > characterParser.YearOfBirth + 35)
                    continue;

                if (characterParser.Spouses.Count > 0)
                    continue;

                return characterParser;
            }

            return null;
        }

        public void RemoveCharacter(CharacterParser characterParser)
        {
            Characters.Remove(characterParser);
            CharacterMap.Remove(characterParser.ID);
            CharactersScript.Root.Remove(characterParser.Scope);
        }

        public void CalculateAllDates()
        {
            for (int index = 0; index < Characters.Count; index++)
            {
                var characterParser = Characters[index];
                if (characterParser.Titles.Count > 0)
                {
                    characterParser.YearOfBirth -= 2;

                    SetAllDates(characterParser.YearOfBirth, characterParser.YearOfDeath, characterParser.Scope);
                    
                     characterParser.DoFamilyDatesOfBirth();
                }
            }
        }
    }
}
