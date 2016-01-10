using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrusaderKingsStoryGen.Simulation;
using Microsoft.Win32;

namespace CrusaderKingsStoryGen
{
    public partial class Form1 : Form
    {
        private SolidBrush brush;
        public static bool loaded = false;
        public static bool autoload = true;
        public Form1()
        {

            String filename = ".\\settings.txt";


            brush = new SolidBrush(Color.White);
          //  Rand.SetSeed();
            InitializeComponent();
         
            string mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            mydocs += "\\Paradox Interactive\\Crusader Kings II\\mod\\";

            Globals.ModRoot = mydocs;

            Globals.ModDir = Globals.ModRoot + modname.Text + "\\";
        
            if (File.Exists(filename))
            {
                using (System.IO.StreamReader file =
                    new System.IO.StreamReader(filename, Encoding.GetEncoding(1252)))
                {
                    string line = "";
                    int count = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        Globals.GameDir = line;
                        Globals.MapDir = line;
                    }
                }
                if (Directory.Exists(Globals.MapDir) && File.Exists(Globals.GameDir + "ck2game.exe"))
                {
                    ck2dir.Text = Globals.GameDir;
                }
                else
                {
                    autoload = false;
                    Globals.GameDir = "";
                    Globals.MapDir = "";
                    exportButton.Enabled = false;
                    start.Enabled = false;
                    stop.Enabled = false;
                    reset.Enabled = false;
                }
                
            }
            else
            {
                string userRoot = "HKEY_LOCAL_MACHINE";
                string subkey = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 203770";
                string keyName = userRoot + "\\" + subkey;
                 string noSuch = (string)Registry.GetValue(keyName,
                 "InstallLocation",
                 null);
                // Console.WriteLine("\r\nNoSuchName: {0}", noSuch);

                if (noSuch != null)
                {
                    ck2dir.Text = noSuch;

                    Globals.GameDir = ck2dir.Text;
                    if (!Globals.GameDir.EndsWith("\\"))
                        Globals.GameDir += "\\";


                }
                else
                {
                    ck2dir.Text = "";

                }
              
                if (Globals.GameDir == null || !Directory.Exists(Globals.GameDir))
                {
                    // doesn't exist, so don't load everything yet...
                    autoload = false;
                    exportButton.Enabled = false;
                    start.Enabled = false;
                    stop.Enabled = false;
                    reset.Enabled = false;
                }
            }

            if (autoload)
            {

                filename = ".\\settings.txt";
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
                {

                    file.Write(Globals.GameDir);

                    file.Close();
                }
            }
         
            Globals.MapDir = Globals.GameDir;
            numericUpDown3.Value = Globals.OneInChanceOfCultureSplinter;
            ReligionStability.Value = Globals.OneInChanceOfReligionSplinter;
            GovernmentStability.Value = (decimal) (Globals.BaseChanceOfRevolt / 1000);

            if (autoload)
            {
                LoadFiles();
            }
            instance = this;
        }

        private void LoadFiles()
        {
            if (loaded)
                return;

            MapManager.instance.Load();
            //  TitleManager.instance.Load();

            MapManager.instance.InitRandom();
            loaded = true;
        }

        private void renderPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
           // e.Graphics.DrawImage(MapManager.instance.ProvinceRenderBitmap, new Rectangle(0, 0, Width, Height));
            MapManager.instance.Draw(e.Graphics, renderPanel.Width, renderPanel.Height);
            e.Graphics.DrawString(SimulationManager.instance.Year.ToString(), DefaultFont, brush, new PointF(10, 10));
        }

        private void renderPanel_Resize(object sender, EventArgs e)
        {
            renderPanel.Invalidate();
        }

        private int ticks = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            SimulationManager.instance.Tick();
            MapManager.instance.RandomChange();
            renderPanel.Invalidate();
            ticks++;
           
        }

        public static Form1 instance;
        private void renderPanel_Click(object sender, EventArgs e)
        {
            
        }

        public void Export()
        {
         
            // for (int x = 0; x < 30; x++)
            {
                CharacterParser[] a = CharacterManager.instance.Characters.ToArray();
                for (int index = 0; index < a.Length; index++)
                {
                    var characterParser = a[index];


                    if (characterParser.Titles.Count == 0)
                    {
                        // index--;
                        continue;
                    }
                
                    if (characterParser.PrimaryTitle.Rank < 1)
                        continue;
                 
                    characterParser.ConvertCountTitlesToDuchies();
                }
            }

            FixupDuchies();
            CreateKingdomsFromDuchies();
//            CreateEmpiresFromKingdoms();
            FixupTitles();
          
            MapManager.instance.UpdateOwnership();
            TitleManager.instance.CreateMercs();

            if (!Globals.ModDir.Contains("\\mod"))
            {
                if (Globals.ModDir.EndsWith("\\"))
                    Globals.ModDir += "mod\\" + Globals.ModName + "\\";
                else
                {
                    Globals.ModDir += "\\mod\\" + Globals.ModName + "\\";
                }
                
            }
      
            TitleManager.instance.SaveTitles();

            TitleManager.instance.LandedTitlesScript.Save();
            ReligionManager.instance.Save();
            CharacterManager.instance.Save();
            MapManager.instance.Save();
            GovernmentManager.instance.Save();
            CultureManager.instance.Save();
            FlagManager.instance.AssignAndSave();
            EventManager.instance.Save();
            DecisionManager.instance.Save();
            MapManager.instance.SaveDefinitions();
            RegionManager.instance.Save();
            if (CultureManager.instance.CultureMap.Count > 0)
                foreach (var titleParser in TitleManager.instance.Titles)
                {
                 

                    if (titleParser.Holder != null && CultureManager.instance.CultureMap.ContainsKey(titleParser.Holder.culture) &&
                        CultureManager.instance.CultureMap[titleParser.Holder.culture].dna != null)
                    {
                        if (titleParser.Rank == 4 )
                            LanguageManager.instance.Add(titleParser.Name,
                                LanguageManager.instance.Get(titleParser.Name) + " Empire");
                        else if (LanguageManager.instance.Get(titleParser.Name)==null)
                            titleParser.RenameForCulture(titleParser.Holder.Culture);

                    }

                }
            DynastyManager.instance.Save();

            ModularFunctionalityManager.instance.Save();
            SpriteManager.instance.Save();
            LanguageManager.instance.Save();

            if (!Directory.Exists(Globals.ModDir + "history\\provinces\\"))
                Directory.CreateDirectory(Globals.ModDir + "history\\provinces\\");
            var files = Directory.GetFiles(Globals.ModDir + "history\\provinces\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            
            foreach (var provinceParser in MapManager.instance.Provinces)
            {
                if (provinceParser.land && provinceParser.title != null)
                {
                    provinceParser.Save();
                }
            }

            String filename = Globals.ModRoot + modname.Text + ".mod";
            using (System.IO.StreamWriter file =
             new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
            {

               file.Write(@"name="""+ modname.Text +@"""
path=""mod/" + modname.Text + @"""
user_dir = """ + modname.Text + @"""
replace_path=""history/titles""
replace_path=""history/characters""
replace_path=""history/wars""
replace_path=""history/provinces""
replace_path=""gfx/flags""
replace_path=""common/landed_titles""
replace_path=""common/dynasties""
replace_path=""common/cultures""
replace_path=""common/religions""
replace_path=""common/religious_titles""
");

                file.Close();
            }

       //     this.Close();
        }

        private void CreateKingdomsFromDuchies()
        {
            List<TitleParser> duchies = new List<TitleParser>();
            List<TitleParser> done = new List<TitleParser>();

            for (int index = 0; index < TitleManager.instance.Titles.Count; index++)
            {
                var titleParser = TitleManager.instance.Titles[index];

                if (titleParser.Rank == 2)
                {
                    duchies.Add(titleParser);
                }
            }
            for (int index = 0; index < duchies.Count; index++)
            {
                var duchie = duchies[index];
                int kingSize = Rand.Next(4 * 4, 16 * 4);
                var results = duchie.Holder.GetProvinceGroup(kingSize, null);
                if (results.Count < 4*4)
                    continue;
                List<TitleParser> kingdomDuchies = new List<TitleParser>();
                foreach (var provinceParser in results)
                {
                    if (!provinceParser.land)
                        continue;
                    if (provinceParser.title == null)
                        continue;

                    if (provinceParser.Title.Liege != null)
                    {
                        var liege = provinceParser.Title.Liege;

                        if (liege.Holder != null &&  liege.Rank==2 &&liege.Holder.PrimaryTitle.Rank == 2)
                        {
                            if (!kingdomDuchies.Contains(liege) && !done.Contains(liege))
                                kingdomDuchies.Add(liege);
                        }
                    }
                }

                if(kingdomDuchies.Count < 3)
                    continue;

                foreach (var kingdomDuchy in kingdomDuchies)
                {
                    int i = duchies.IndexOf(kingdomDuchy);                    
                    duchies.Remove(kingdomDuchy);
                    if (i < index)
                        index--;
                }
                done.AddRange(kingdomDuchies);
                var chr = SimulationManager.instance.AddCharacter(duchie.Holder.culture, duchie.Holder.religion);
                var title = TitleManager.instance.CreateKingScriptScope(duchie.CapitalProvince, chr);
                chr.GiveTitle(title);

                foreach (var kingdomDuchy in kingdomDuchies)
                {
                    title.AddSub(kingdomDuchy);
                }

                var list = title.GetAllProvinces();

                int num = Math.Min(5, list.Count/2);
                chr.GiveTitleAsHolder(list[0].Title.Liege);
                foreach (var value in list[0].Title.Liege.SubTitles.Values)
                {
                    chr.GiveTitleAsHolder(value);
                }

                var count = chr.UsurpCountTitle();
                if (count == null)
                    continue;
                while (count.Liege != null && count.Liege.Rank > count.Rank)
                {
                    count = count.Liege;
                    chr.GiveTitleAsHolder(count);
                }
            }
        }

        private void CreateEmpiresFromKingdoms()
        {
            List<TitleParser> kingdoms = new List<TitleParser>();
            List<TitleParser> done = new List<TitleParser>();

            for (int index = 0; index < TitleManager.instance.Titles.Count; index++)
            {
                var titleParser = TitleManager.instance.Titles[index];

                if (titleParser.Rank == 3)
                {
                    kingdoms.Add(titleParser);
                }
            }

           for (int index = 0; index < kingdoms.Count; index++)
            {
                var kingdom = kingdoms[index];
                if (kingdom.Holder == null)
                    continue;
                if (kingdom.Liege != null)
                    continue;

                int kingSize = Rand.Next(4 * 4 * 4 * 4, 8 * 8 * 8 * 8);
                var results = kingdom.Holder.GetProvinceGroup(kingSize, null);
            
                List<TitleParser> kingdomDuchies = new List<TitleParser>();
           
                foreach (var provinceParser in results)
                {
                    if (!provinceParser.land)
                        continue;
                    if (provinceParser.title == null)
                        continue;

                    if (provinceParser.Title.Liege != null)
                    {
                        var liege = provinceParser.Title.Liege;

                        if (liege.Rank == 2)
                        {
                            liege = liege.Liege;
                        }
                        if (liege != null && liege.Rank == 3)
                        {
                            if (!kingdomDuchies.Contains(liege) && liege.LandedTitlesCount > 8 && !done.Contains(liege))
                            {
                                kingdomDuchies.Add(liege);
                                done.Add(liege);
                            }
                        }
                    }
                    if (kingdomDuchies.Count > 6)
                        break;

                }
                if (kingdomDuchies.Count < 4)
                    continue;

               if(done.Count > kingdomDuchies.Count)
                    return;

                foreach (var kingdomDuchy in kingdomDuchies)
                {
                    int i = kingdoms.IndexOf(kingdomDuchy);
                    kingdoms.Remove(kingdomDuchy);
                    if (i < index)
                        index--;
                }
                kingdoms.AddRange(kingdomDuchies); 
  
                var chr = SimulationManager.instance.AddCharacter(kingdom.Holder.culture, kingdom.Holder.religion);
                var title = TitleManager.instance.CreateEmpireScriptScope(kingdom.CapitalProvince, chr);
                chr.GiveTitle(title);

                foreach (var kingdomDuchy in kingdomDuchies)
                {
                    title.AddSub(kingdomDuchy);
                }

                var count = chr.UsurpCountTitle();
                if (count == null)
                    continue;
             
                while (count.Liege != null && count.Liege.Rank > count.Rank)
                {
                    count = count.Liege;
                    chr.GiveTitleAsHolder(count);
                }

               
            }
        }


        private static void FixupTitles()
        {
            //   List<TitleParser> outOfRank = new List<TitleParser>();
            for (int index = 0; index < TitleManager.instance.Titles.Count; index++)
            {
                var titleParser = TitleManager.instance.Titles[index];

                if (titleParser.SubTitles.Count == 0)
                {
                    TitleManager.instance.RemoveTitle(titleParser);
                }
            }

            for (int index = 0; index < CharacterManager.instance.Characters.Count; index++)
            {
                var characterParser = CharacterManager.instance.Characters[index];
                if (characterParser.Titles.Count == 0)
                {
                    CharacterManager.instance.RemoveCharacter(characterParser);
                }
            }
        }

        private static void FixupDuchies()
        {
            List<TitleParser> orphans = new List<TitleParser>();
            foreach (var titleParser in TitleManager.instance.Titles)
            {
                if (titleParser.Rank == 1 && titleParser.Liege == null)
                {
                    orphans.Add(titleParser);
                }

               
            }

            for (int index = 0; index < orphans.Count; index++)
            {
                var titleParser = orphans[index];
                int smallest = 10000000;
                TitleParser title = null;
                foreach (var provinceParser in titleParser.Owns[0].Adjacent)
                {
                    if (provinceParser.title == null)
                        continue;

                    if (provinceParser.Title.Liege != null)
                    {
                        var liege = provinceParser.Title.Liege;

                        int c = liege.SubTitles.Count;
                        if (smallest > c)
                        {
                            smallest = c;
                            title = liege;
                        }
                    }
                }

                if (title != null)
                {
                    title.AddSub(titleParser);
                    orphans.Remove(titleParser);
                    index--;
                }
            }
            orphans.Clear();
            foreach (var titleParser in TitleManager.instance.Titles)
            {
                if (titleParser.Rank == 2)
                {
                    List<TitleParser> titles = new List<TitleParser>(titleParser.SubTitles.Values);
                    for (int index = 0; index < titles.Count; index++)
                    {
                        var value = titles[index];
                        for (int i = index + 1; i < titles.Count; i++)
                        {
                            var value2 = titles[i];
                            if (value.Owns.Count == 0)
                                continue;

                            if (!value.Adjacent(value2))
                            {
                                if (!orphans.Contains(value2))
                                    orphans.Add(value2);
                                continue;
                            }
                        }
                    }
                }
            }
            for (int index = 0; index < orphans.Count; index++)
            {
                var titleParser = orphans[index];
                int smallest = 10000000;
                TitleParser title = null;
                foreach (var provinceParser in titleParser.Owns[0].Adjacent)
                {
                    if (provinceParser.title == null)
                        continue;

                    if (provinceParser.Title.Liege != null)
                    {
                        var liege = provinceParser.Title.Liege;

                        int c = liege.SubTitles.Count;
                        if (smallest > c)
                        {
                            smallest = c;
                            title = liege;
                        }
                    }
                }

                if (title != null)
                {
                    title.AddSub(titleParser);
                    orphans.Remove(titleParser);
                    index--;
                }
            }
            orphans.Clear();

            foreach (var titleParser in TitleManager.instance.Titles)
            {
                if (titleParser.Rank == 2)
                {
                    if (titleParser.SubTitles.Count > 0)
                    {
                        orphans.Add(titleParser);
                    }
                }
            }

            // now wo do the regions...
            List<TitleParser> forRegion = new List<TitleParser>();
            for (int index = 0; index < orphans.Count; index++)
            {
                var titleParser = orphans[index];
                int kingSize = Rand.Next(10*10);
                var results = titleParser.Holder.GetProvinceGroup(kingSize, null);

                foreach (var provinceParser in results)
                {
                    if (provinceParser.Title != null && provinceParser.Title.Liege != null &&
                        provinceParser.Title.Liege.Rank == 2)
                    {
                        if (!forRegion.Contains(provinceParser.Title.Liege))
                        {
                            forRegion.Add(provinceParser.Title.Liege);
                        }
                    }
                }

                foreach (var parser in forRegion)
                {
                    if (orphans.Contains(parser))
                    {
                        int index2 = orphans.IndexOf(parser);

                        if (index2 <= index)
                            index--;
                        orphans.Remove(parser);                            
                    }
                    
                }

                String name = forRegion[0].Culture.dna.GetPlaceName();
                RegionManager.instance.AddRegion(name, forRegion);
                forRegion.Clear();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            SimulationManager.instance.Active = false;
        }

        private void start_Click(object sender, EventArgs e)
        {
            SimulationManager.instance.Active = true;
        }

        private void reset_Click(object sender, EventArgs e)
        {
            SimulationManager.instance.Active = false;
            SimulationManager.instance = new SimulationManager();
            FlagManager.instance = new FlagManager();
            CulturalDnaManager.instance = new CulturalDnaManager();
            CultureManager.instance = new CultureManager();
            ReligionManager.instance = new ReligionManager();
            CharacterManager.instance = new CharacterManager();
            LanguageManager.instance = new LanguageManager();
            MapManager.instance = new MapManager();
            TitleManager.instance = new TitleManager();
            EventManager.instance = new EventManager();
            DecisionManager.instance = new DecisionManager();
            SpriteManager.instance = new SpriteManager();
            TraitManager.instance = new TraitManager();
            MapManager.instance.Load();
            //  TitleManager.instance.Load();

            MapManager.instance.InitRandom();    
            renderPanel.Invalidate();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            // culture
            Globals.OneInChanceOfCultureSplinter = (int)numericUpDown3.Value;
          
        }

        private void ReligionStability_ValueChanged(object sender, EventArgs e)
        {
            Globals.OneInChanceOfReligionSplinter = (int)ReligionStability.Value;
        }

        private void GovernmentStability_ValueChanged(object sender, EventArgs e)
        {
            Globals.BaseChanceOfRevolt = (int)GovernmentStability.Value*1000;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void selectCK2Dir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();

            if (d.ShowDialog() == DialogResult.OK)
            {
                Globals.GameDir = d.SelectedPath;
                if (Globals.GameDir.Length > 0 && !Globals.GameDir.EndsWith("\\"))
                    Globals.GameDir += "\\";

                Globals.MapDir = Globals.GameDir;
                if (Directory.Exists(Globals.MapDir) && File.Exists(Globals.GameDir + "ck2game.exe"))
                {
                    LoadFiles();
                    exportButton.Enabled = true;
                    start.Enabled = true;
                    stop.Enabled = true;
                    reset.Enabled = true;
                    String filename = ".\\settings.txt";
                    using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
                    {

                        file.Write(Globals.GameDir);

                        file.Close();
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Error: Could not find CK2 base files in specified directory. Make sure you are pointing where CK2 is INSTALLED (for example in C:\\Program Files (x86)\\Steam\\steamapps\\common\\Crusader Kings II) NOT the My Documents folder",
                        "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    exportButton.Enabled = false;
                    start.Enabled = false;
                    stop.Enabled = false;
                    reset.Enabled = false;
                }

                this.ck2dir.Text = Globals.GameDir;

            }
        }

        private void ck2dir_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void modname_TextChanged(object sender, EventArgs e)
        {
            Globals.ModDir = Globals.ModRoot + modname.Text + "\\";
            Globals.ModName = modname.Text;
        }

        private void ck2dir_KeyPress(object sender, KeyPressEventArgs e)
        {
            Globals.GameDir = ck2dir.Text;
            if (Globals.GameDir.Length > 0 && !Globals.GameDir.EndsWith("\\"))
                Globals.GameDir += "\\";

            Globals.MapDir = Globals.GameDir;
            if(Directory.Exists(Globals.MapDir))
                LoadFiles();
        }
    }
}
