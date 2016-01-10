using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class TraitManager
    {
        public static TraitManager instance = new TraitManager();

        public void Init()
        {
            if (!Directory.Exists(Globals.ModDir + "gfx\\traits"))
                Directory.CreateDirectory(Globals.ModDir + "gfx\\traits");
            var files = Directory.GetFiles(Globals.ModDir + "gfx\\traits");
            foreach (var file in files)
            {
                File.Delete(file);
            }


        }
        public void AddTrait(String name, String srcFilename)
        {
            String srcend = srcFilename.Substring(srcFilename.LastIndexOf('.'));
            File.Copy(Globals.SrcTraitIconDir+srcFilename, Globals.ModDir + "gfx\\traits\\" + name + srcend);
            SpriteManager.instance.AddTraitSprite(name, "gfx/traits/" + name + srcend);
        }

        public void AddTrait(string safeName)
        {
             var files = Directory.GetFiles(Globals.SrcTraitIconDir);


            String s = files[Rand.Next(files.Length)];

            String srcend = s.Substring(s.LastIndexOf('\\')+1);
            AddTrait(safeName, srcend);
      
        }
    }
}
