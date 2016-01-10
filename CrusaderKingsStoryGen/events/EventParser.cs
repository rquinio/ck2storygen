using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    class EventParser : Parser
    {
        public List<EventParser> LinkedEvents = new List<EventParser>();
        public List<String> LinkedEventIDs = new List<String>();
        public EventParser(ScriptScope scope)
            : base(scope)
        {
            Trigger = scope.Find("trigger") as ScriptScope;
            MeanTime = scope.Find("mean_time_to_happen") as ScriptScope;

        
        }

        public void FindLinks()
        {
            ExamineEvent(Scope);
        }
        private void ExamineEvent(ScriptScope node)
        {
            for (int index = 0; index < node.Children.Count; index++)
            {
                var child = node.Children[index];
                if (child is ScriptScope)
                    ExamineEvent(child as ScriptScope);
                if (child is ScriptCommand)
                {
                    ScriptCommand c = child as ScriptCommand;
                    if(c.Name == "id" && node.Name == "character_event")
                    {
                        if (!LinkedEventIDs.Contains(c.Value.ToString()))
                        {
                            var e = (EventManager.instance.GetEvent(c.Value.ToString()));
                            if (e != null)
                            {
                                LinkedEvents.Add(e);
                                LinkedEventIDs.Add(c.Value.ToString());                                
                            }
                        }
                    }

                }
            }
        }

        public ScriptScope Trigger { get; set; }
        public ScriptScope MeanTime { get; set; }

        public override ScriptScope CreateScope()
        {
            return null;
        }
    }
}
