using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    internal class SpriteManager
    {
        public static SpriteManager instance = new SpriteManager();
        public Script script = new Script();

        public ScriptScope spriteTypes = new ScriptScope();

        public void Init()
        {
            if (!Directory.Exists(Globals.ModDir + "interface"))
                Directory.CreateDirectory(Globals.ModDir + "interface");
            var files = Directory.GetFiles(Globals.ModDir + "interface");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            script.Root = new ScriptScope();
            var s = new ScriptScope();
            s.Name = "spriteTypes";
            spriteTypes = s;
            script.Root.Add(s);
            script.Name = Globals.ModDir + "interface\\genGraphics.gfx";
        }

        public void AddTraitSprite(String name, String relFilename)
        {
            var scope = new ScriptScope();

            scope.Name = "spriteType";

            scope.Do(@"
                	        name = ""GFX_trait_" + name + @"
		                    texturefile = " + relFilename + @"
		                    noOfFrames = 1
		                    norefcount = yes
		                    effectFile = ""gfx/FX/buttonstate.lua""");

            spriteTypes.Add(scope);
        }

    public void Save()
        {
            script.Save();
        }

        public void AddGovernment(Government government)
        {
            var scope = new ScriptScope();

            scope.Name = "spriteType";

            scope.Do(@"
                	        name = ""GFX_icon_" + government.name + @"
		                    texturefile = gfx\\interface\\government_icon_" + government.name.Replace("_government", "")+".dds" );

            spriteTypes.Add(scope);
        }
    }
}
