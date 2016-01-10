using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class Globals
    {
        public static string SrcTraitIconDir =
             "C:\\Users\\LEMMYMAIN\\Documents\\Paradox Interactive\\Crusader Kings II\\mod\\storygen\\_data\\";
        public static string GameDir =
             "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Crusader Kings II\\";
        //     public static string MapDir =
          //   "C:\\Users\\LEMMYMAIN\\Documents\\Paradox Interactive\\Crusader Kings II\\mod\\A Game of Thrones\\";
          //   public static string MapDir =
        //     "C:\\Users\\LEMMYMAIN\\Documents\\Paradox Interactive\\Crusader Kings II\\mod\\Historical Immersion Project\\";
        // C:\Users\LEMMYMAIN\Documents\Paradox Interactive\Crusader Kings II\mod\Historical Immersion Project
          public static string MapDir =
            "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Crusader Kings II\\";
          public static string ModDir = "C:\\Users\\LEMMYMAIN\\Documents\\Paradox Interactive\\Crusader Kings II\\mod\\storygen\\";
          public static string OModDir = "C:\\Users\\LEMMYMAIN\\Documents\\Paradox Interactive\\Crusader Kings II\\mod\\storygen\\";
          public static string UserDir = "C:\\Users\\LEMMYMAIN\\Documents\\Paradox Interactive\\Crusader Kings II\\storygen\\";
        public static int OneInChanceOfReligionSplinter = 1;
       // public static int OneInChanceOfReligionReformed = 2;
      //  public static int OneInChanceOfReligionModern = 2;
        public static float BaseChanceOfRevolt = 1000.0f;
        //     public static float BaseChanceOfRevolt = 70000.0f;

        public static int OneInChanceOfCultureSplinter = 1;
        public static string ModRoot { get; set; }
        public static string ModName { get; set; }
    }
}
