using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrusaderKingsStoryGen.Simulation;

namespace CrusaderKingsStoryGen
{
    class Dynasty
    {
        public int ID;
        public List<CharacterParser> Members = new List<CharacterParser>();
        public ScriptScope Scope;
    }
    class DynastyManager
    {
        public static DynastyManager instance = new DynastyManager();
        public int ID = 1;
        public void Init()
        {
            Script s = new Script();
            script = s;
            s.Name = Globals.ModDir + "common\\dynasties\\dynasties.txt";
            s.Root = new ScriptScope();
     
        }
        public Dictionary<int, Dynasty> DynastyMap = new Dictionary<int, Dynasty>();
        public void Save()
        {
            script.Save();
        }
        public Dynasty GetDynasty(CultureParser culture)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = ID.ToString();
            ID++;
            scope.Add(new ScriptCommand("name", culture.dna.GetDynastyName(), scope));
            scope.Add(new ScriptCommand("culture", culture.Name, scope));
            script.Root.Add(scope);
            var d = new Dynasty() {ID = ID - 1, Scope = scope};
            DynastyMap[ID - 1] = d;
            culture.Dynasties.Add(d);
            return d;
        }
        public Script script { get; set; }
    }
}
