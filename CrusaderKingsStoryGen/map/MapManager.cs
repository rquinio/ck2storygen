using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrusaderKingsStoryGen.Simulation;

namespace CrusaderKingsStoryGen
{
    class MapManager
    {
        public class ProvinceMapBitmap 
        {
            public Bitmap Bitmap { get; set; }
            public Point MapPoint { get; set; }
        }
        public Dictionary<int, ProvinceMapBitmap> ProvinceBitmaps = new Dictionary<int, ProvinceMapBitmap>();
        private ProvinceParser[,] ProvincePixelMap;
        public Bitmap ProvinceRenderBitmap;
        public float SizeMod = 0.5f;
        public float RenderMod = 1.0f;
        public static MapManager instance = new MapManager();
        public List<ProvinceParser> Provinces = new List<ProvinceParser>();
        public Dictionary<String, ProvinceParser> ProvinceMap = new Dictionary<string, ProvinceParser>();
        public Dictionary<int, ProvinceParser> ProvinceIDMap = new Dictionary<int, ProvinceParser>();
        public Dictionary<Color, ProvinceParser> ProvinceColorMap = new Dictionary<Color, ProvinceParser>();

        public void Save()
        {
            foreach (var provinceScript in provinceScripts)
            {
                provinceScript.Save();
            }
        }

        public void FindAdjacent(List<ProvinceParser> provinces, int c)
        {
            if (c == 0)
                c++;
            List<ProvinceParser> choices = new List<ProvinceParser>();
            foreach (var provinceParser in provinces)
            {
                foreach (var parser in provinceParser.Adjacent)
                {
                    if (!provinces.Contains(parser))
                        choices.Add(parser);

                }
            }
            for (int i = 0; i < c; i++)
            {
                if (choices.Count > 0)
                {
                    int cc = Rand.Next(choices.Count);

                    provinces.Add(choices[cc]);
                    choices.RemoveAt(cc);
                }
            }
        }

        public void FindAdjacent(List<ProvinceParser> provinces, int c, CharacterParser head)
        {
            List<ProvinceParser> choices = new List<ProvinceParser>();
            foreach (var provinceParser in provinces)
            {
                foreach (var parser in provinceParser.Adjacent)
                {
                    var t = parser.Title;
                    if (!provinces.Contains(parser) && (head == null || (head == t.Holder || (head.PrimaryTitle.Rank == 2 && parser.Title.TopmostTitle == head.PrimaryTitle.TopmostTitle)) && (parser.Title.Liege == null || parser.Title.Rank == 2)))
                        choices.Add(parser);

                }
            }
            for (int i = 0; i < c; i++)
            {
                if (choices.Count > 0)
                {
                    int cc = Rand.Next(choices.Count);

                    provinces.Add(choices[cc]);
                    choices.RemoveAt(cc);
                }
            }
        }

        public void FindAdjacentSameRealm(List<ProvinceParser> provinces, int c, CharacterParser head)
        {
            List<ProvinceParser> choices = new List<ProvinceParser>();
            foreach (var provinceParser in provinces)
            {
                foreach (var parser in provinceParser.Adjacent)
                {
                    if (!parser.land)
                        continue;
                    if (parser.title == null)
                        continue;

                    var t = parser.Title;
                    if (!provinces.Contains(parser) && (head == parser.Title.Holder || parser.Title.Holder == null || (parser.Title.Liege != null && head == parser.TotalLeader)))
                        choices.Add(parser);

                }
            }
            for (int i = 0; i < c; i++)
            {
                if (choices.Count > 0)
                {
                    int cc = Rand.Next(choices.Count);

                    provinces.Add(choices[cc]);
                    choices.RemoveAt(cc);
                }
            }
        }

        List<Script>  provinceScripts = new List<Script>();
        public void Load()
        {
            String provincesDir = Globals.GameDir + "history\\provinces\\";
            foreach (var file in Directory.GetFiles(provincesDir))
            {
                String name = file.Substring(file.LastIndexOf('\\') + 1);
                int id = Convert.ToInt32(name.Split('-')[0].Trim());


            }
            TitleManager.instance.Load();

        //    foreach (var provinceParser in Provinces)
            {
        //        provinceParser.DoTitleOwnership();
            }

            for (int n = 0; n < 2000; n++)
            {
                ProvinceParser parser = new ProvinceParser(new ScriptScope());
                parser.Name = "c_unnamed" + n;
                parser.id = n + 1;
                ProvinceIDMap[parser.id] = parser;
           //     TitleManager.instance.CreateDukeScriptScope();
                Provinces.Add(parser);
            }
            provincesDir = Globals.GameDir + "history\\provinces\\";
            foreach (var file in Directory.GetFiles(provincesDir))
            {
                String name = file.Substring(file.LastIndexOf('\\') + 1);
                int id = Convert.ToInt32(name.Split('-')[0].Trim());

                {
                    Script s = ScriptLoader.instance.Load(file);

                    ScriptCommand terrain = (ScriptCommand)s.Root.Find("terrain");

                    if (terrain != null)
                    {
                        var ter = terrain.Value.ToString();
                        LoadedTerrain[id] = ter;
                    }
                    else
                    {
                        
                    }

                }
                ProvinceIDMap[id].land = true;

            }
            //s
            ProvinceBitmap = new Bitmap(Globals.MapDir + "map\\provinces.bmp");
            ProvinceBitmap = ResizeBitmap(ProvinceBitmap, (int)(ProvinceBitmap.Width * SizeMod), (int)(ProvinceBitmap.Height * SizeMod));
            ProvinceRenderBitmap = new Bitmap(ProvinceBitmap.Width, ProvinceBitmap.Height);
            LoadDefinitions();
          
            ProvincePixelMap = new ProvinceParser[ProvinceBitmap.Width, ProvinceBitmap.Height];

            CreateLandscape();

            LockBitmap lockBitmap = new LockBitmap(ProvinceBitmap);
            lockBitmap.LockBits();
            for (int x = 0; x < lockBitmap.Width; x++)
                for (int y = 0; y < lockBitmap.Height; y++)
                {
                    Color col = lockBitmap.GetPixel(x, y);
                    // col2 = Color.FromArgb(255, col.R, col.G, col.B);
                    if (ProvinceColorMap.ContainsKey(col))
                    {
                        ProvincePixelMap[x, y] = ProvinceColorMap[col];
                        ProvinceColorMap[col].Points.Add(new Point(x, y));
         
                    }

                    int minX = x - 1;
                    int minY = y - 1;
                    if (minX < 0)
                        minX = 0;
                    if (minY < 0)
                        minY = 0;

                    for (int xx = minX; xx <= x; xx++)
                    {
                        for (int yy = minY; yy <= y; yy++)
                        {
                            if (xx == x && yy == y)
                                continue;

                            Color col2 = lockBitmap.GetPixel(xx, yy);
                            if (col2 != col && ProvinceColorMap.ContainsKey(col2) && ProvinceColorMap.ContainsKey(col))
                            {
                                ProvinceColorMap[col].AddAdjacent(ProvinceColorMap[col2]);
                            }
                        }
                    }



                }

            for (int y = lockBitmap.Height - 1; y >= 0; y--)
                for (int x = lockBitmap.Width - 1; x >= 0; x--)
                {
                    Color col = lockBitmap.GetPixel(x, y);
                    // col2 = Color.FromArgb(255, col.R, col.G, col.B);
                   
                    int maxX = x+1;
                    int maxY = y+1;

                    maxX = Math.Min(lockBitmap.Width - 1, maxX);
                    maxY = Math.Min(lockBitmap.Height - 1, maxY);

                    for (int xx = x; xx <= maxX; xx++)
                    {
                        for (int yy = y; yy <= maxY; yy++)
                        {
                            if (xx == x && yy == y)
                                continue;

                            Color col2 = lockBitmap.GetPixel(xx, yy);
                            if (col2 != col && ProvinceColorMap.ContainsKey(col2) && ProvinceColorMap.ContainsKey(col))
                            {
                                ProvinceColorMap[col].AddAdjacent(ProvinceColorMap[col2]);
                            }
                        }
                    }



                }
            lockBitmap.UnlockBits();

            foreach (var provinceParser in Provinces)
            {
                int maxX = -1000000;
                int maxY = -1000000;
                int minX = 1000000;
                int minY = 1000000;
                if (provinceParser.Points.Count == 0)
                    continue;

                foreach (var point in provinceParser.Points)
                {
                    if (point.X > maxX)
                        maxX = point.X;
                    if (point.Y > maxY)
                        maxY = point.Y;
                    if (point.X < minX)
                        minX = point.X;
                    if (point.Y < minY)
                        minY = point.Y;


                }

                Bitmap bmp = new Bitmap(maxX - minX + 1, maxY - minY + 1);
                ProvinceMapBitmap b = new ProvinceMapBitmap() {Bitmap = bmp, MapPoint = new Point(minX, minY)};
                LockBitmap lb = new LockBitmap(b.Bitmap);

                lb.LockBits();
                for (int x = 0; x < lb.Width; x++)
                {
                    for (int y = 0; y < lb.Height; y++)
                    {
                        lb.SetPixel(x, y, Color.Transparent);
                    }
                }
                foreach (var point in provinceParser.Points)
                {
                    lb.SetPixel(point.X-minX, point.Y-minY, Color.White);
                }
                lb.UnlockBits();

                ProvinceBitmaps[provinceParser.id] = b;
            }

            LoadAdjacencies();

            Simulation.SimulationManager.instance.Init();
        }
        private void LoadAdjacencies()
        {
            String filename = Globals.MapDir + "map\\adjacencies.csv";
            using (System.IO.StreamReader file =
                new System.IO.StreamReader(filename, Encoding.GetEncoding(1252)))
            {
                string line = "";
                int count = 0;
                while ((line = file.ReadLine()) != null)
                {
                    if (count > 0)
                    {
                        if (line.Trim().Length == 0)
                            continue;
                        String[] s = line.Split(';');
                        if (s[0].Trim().Length == 0)
                            continue;

                        int id = Convert.ToInt32(s[0]);
                        int id2 = Convert.ToInt32(s[1]);

                        if (ProvinceIDMap.ContainsKey(id) && ProvinceIDMap.ContainsKey(id2))
                        {
                            ProvinceIDMap[id].AddAdjacent(ProvinceIDMap[id2]);

                        }
                    }
                    count++;
                }

                ProvinceIDMap[73].AddAdjacent(ProvinceIDMap[75]);

                file.Close();
            }
          
        }

        private void CreateLandscape()
        {
        /*    LandscapeLibrary.Landscape l = new Landscape(ProvinceBitmap.Width/3, ProvinceBitmap.Height/3, Rand.Next(100000));

            l.Generate(200);

            LockBitmap b = new LockBitmap(ProvinceBitmap);

            b.LockBits();
            for (int x = 0; x < l.Width; x++)
            {
                for (int y = 0; y < l.Height; y++)
                {
                    var col = l.TileMap[(y*l.Width) + x].Tile.DisplayColour;
                    for (int xx = 0; xx < 3; xx++)
                    {
                        for (int yy = 0; yy < 3; yy++)
                        {
                            b.SetPixel((x * 3) + xx, (y * 3) + yy, Color.FromArgb(255, col.R, col.G, col.B));        
                        }
                    }
                    
                }
            }
            b.UnlockBits();*/
        }

        private Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            }
            return result;
        }
        public void SetColour(int province, Color col)
        {
            ProvinceIDMap[province].Color = col;
        }

        public void LockRenderBitmap()
        {
        //    globalLockBitmap = new LockBitmap(ProvinceRenderBitmap);
       //     globalLockBitmap.LockBits();
        }

        public void UnlockRenderBitmap()
        {
       //     globalLockBitmap.UnlockBits();
        }
        public void SetColour(LockBitmap bmp, int province, Color col)
        {
           // this.ProvinceColorMap = col;
          //  ProvinceParser provinceParser = ProvinceIDMap[province];
          //  foreach (var point in provinceParser.Points)
            {
         //       bmp.SetPixel(point.X, point.Y, col);
            }

        //    bmp.UnlockBits();
        }
        private void LoadDefinitions()
        {
            String filename = Globals.MapDir + "map\\definition.csv";
            using (System.IO.StreamReader file =
                new System.IO.StreamReader(filename, Encoding.GetEncoding(1252)))
            {
                string line = "";
                int count = 0;
                while ((line = file.ReadLine()) != null)
                {
                    if (count > 0)
                    {
                        if (line.Trim().Length == 0)
                            continue;
                        String[] s = line.Split(';');
                        if (s[0].Trim().Length == 0)
                            continue;

                        int id = Convert.ToInt32(s[0]);
                        int r = Convert.ToInt32(s[1]);
                        int g = Convert.ToInt32(s[2]);
                        int b = Convert.ToInt32(s[3]);
                        String name = s[4];
                        if (ProvinceIDMap.ContainsKey(id))
                        {
                            ProvinceIDMap[id].provinceRCode = r;
                            ProvinceIDMap[id].provinceGCode = g;
                            ProvinceIDMap[id].provinceBCode = b;
                            ProvinceColorMap[Color.FromArgb(255, r, g, b)] = ProvinceIDMap[id];
                           
                            if (!ProvinceIDMap[id].land)
                            {
                                
                            }

                        }
                    }
                    count++;
                }
                file.Close();
            }
           
        
        }

        public void SaveDefinitions()
        {
            String filename = Globals.ModDir + "map\\definition.csv";
            List<String> defs = new List<string>();
            using (System.IO.StreamReader filein =
                 new System.IO.StreamReader(Globals.MapDir + "map\\definition.csv", Encoding.GetEncoding(1252)))
            {
                string line = "";
                int count = 0;
                while ((line = filein.ReadLine()) != null)
                {
                  //  if (count > 0)
                    {
                 
                        defs.Add(line);
                    }
                }

                filein.Close();
            }

            int n = 0;
          //  File.Mutate(filename, filename);
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
            {

                foreach (var def in defs)
                {
                    String[] split = def.Split(';');
                    if (split.Length < 5)
                    {
                        file.Write(def + Environment.NewLine);
                        continue;                        
                    }
                    if (split[0] == "province")
                    {
                        file.Write(def + Environment.NewLine);
                        continue;
                    
                    }

                    int i = -1;
                    try
                    {
                        i = Convert.ToInt32(split[0]);
                    }
                    catch (Exception)
                    {
                        
                     //   throw;
                    }
                    if (!ProvinceIDMap.ContainsKey(i))
                    {
                        String outStr = split[0] + ";" + split[1] + ";" + split[2] + ";" + split[3] + ";;" + split[5].Trim();
                        file.Write(outStr + Environment.NewLine);
                        continue;
                    }
                       
                    if (split[4].Trim().Length > 0 && split[4].Trim() != "x" && ProvinceIDMap.ContainsKey(i) && ProvinceIDMap[i].land && ProvinceIDMap[i].title != null)
                    {
                        String outStr = split[0] + ";" + split[1] + ";" + split[2] + ";" + split[3] + ";" +  ProvinceIDMap[Convert.ToInt32(split[0])].title.Replace("c_", "") +
                                        ";" + split[5].Trim();
                        file.Write(outStr + Environment.NewLine);
                        continue;
                    }
                    String ooo = split[0] + ";" + split[1] + ";" + split[2] + ";" + split[3] + ";;" + split[5].Trim();
                    file.Write(ooo + Environment.NewLine);
               
                }

                file.Close();
            }


        }
    
        public Bitmap ProvinceBitmap { get; set; }
        public Dictionary<int, String> LoadedTerrain= new Dictionary<int, string>();

        public LockBitmap globalLockBitmap;

        public void InitRandom()
        {
        /*   LockBitmap bmp = new LockBitmap(ProvinceRenderBitmap);

           bmp.LockBits();
           foreach (var provinceParser in ProvinceIDMap)
           {
               if (provinceParser.Value.ProvinceOwner != null)                
                    SetColour(bmp, provinceParser.Key, provinceParser.Value.TotalLeader.color);

            }
           bmp.UnlockBits();*/
        }

        public void RandomChange()
        {
        //    this.SetColour(Provinces[rand.Next(Provinces.Count)].id, Color.FromArgb(255, rand.Next(255), rand.Next(255), rand.Next(255)));
        }
        ColorMatrix cm = new ColorMatrix();
        ImageAttributes ia = new ImageAttributes();
        public Dictionary<String, ProvinceParser.Barony> Temples = new Dictionary<string, ProvinceParser.Barony>();

        public void Draw(Graphics graphics, float w, float h)
        {
            float xrat = w/1456.0f;
            float yrat = h/1027.0f;
            try
            {
                foreach (var provinceParser in Provinces)
                {
                    if (provinceParser.title == null)
                        continue;
                    if (!provinceParser.land)
                        continue;

                    if (!TitleManager.instance.TitleMap[provinceParser.title].Active)
                        continue;
                    if (TitleManager.instance.TitleMap[provinceParser.title].Holder == null && TitleManager.instance.TitleMap[provinceParser.title].CurrentHolder == null)
                        continue;

                    if (this.ProvinceBitmaps.ContainsKey(provinceParser.id))
                    {
                        // Get a picture box's Graphics object
                        Graphics gra = graphics;
                        if (provinceParser == null)
                            continue;

                        if (provinceParser.ProvinceOwner == null)
                            continue;
                        var hh = provinceParser.ProvinceOwner.Holder;
                        if (TitleManager.instance.TitleMap[provinceParser.title].CurrentHolder != null)
                            hh = TitleManager.instance.TitleMap[provinceParser.title].CurrentHolder;
                        Color col = hh.Color;

                        //    col = Color.FromArgb(255, rand.Next(255), rand.Next(255), rand.Next(255));
                        // Create a new color matrix and set the alpha value to 0.5
                        cm.Matrix00 = col.R / 255.0f;
                        cm.Matrix11 = col.G / 255.0f;
                        cm.Matrix22 = col.B / 255.0f;
                        cm.Matrix33 = 1.0f;
                        cm.Matrix44 = 1.0f;

                        TitleParser tit = TitleManager.instance.TitleMap[provinceParser.title];
                        if (tit.Liege == null)
                            cm.Matrix33 = 0.3f;
                        while (tit.Liege != null && tit.Liege.Rank > tit.Rank)
                            tit = tit.Liege;
                    
                        // Create a new image attribute object and set the color matrix to
                        // the one just created
                        ia.SetColorMatrix(cm);

                        if (!ProvinceBitmaps.ContainsKey(provinceParser.id))
                        {

                        }
                        Bitmap bmp = this.ProvinceBitmaps[provinceParser.id].Bitmap;
                        var p = this.ProvinceBitmaps[provinceParser.id].MapPoint;
                        graphics.DrawImage(bmp, new Rectangle((int)(p.X * RenderMod * xrat), (int)(p.Y * RenderMod * yrat), (int)(bmp.Width * RenderMod * xrat), (int)(bmp.Height * RenderMod * yrat)), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
                    }
                }
            }
            catch (Exception ex)
            {
                

            }
        

       //     graphics.DrawImage(this.ProvinceBitmap, new Rectangle(0, 0, ProvinceBitmap.Width, ProvinceBitmap.Height));
        }

        public void UpdateOwnership()
        {
        }

        public void RegisterBarony(string name, TitleParser subTitle)
        {
            if (this.Temples.ContainsKey(name))
            {
                Temples[name].titleParser = subTitle;
            }
        }
    }
}
