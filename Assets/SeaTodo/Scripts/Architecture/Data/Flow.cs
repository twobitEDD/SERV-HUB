using System.Collections.Generic;
using Architecture.Data.Structs;

namespace Architecture.Data
{
    // Information package about task
    public class Flow
    {
        public int Id { get; set; } // flow global id
        public byte Icon { get; set; } // id of flow icon
        public byte Order{ get; set; } // order in group
        public string Name { get; set; } // name of flow
        public int GoalInt { get; set; } // goal of flow in origin format
        public byte Color { get; set; } // color id
        
        public FlowType Type; // type of flow

        public Dictionary<int, HomeTime> Reminders; // reminders of flow 

        public bool Finished; // local finished flag
        
        public int DateStarted; // date started
        public int DateFinished; // date finished
        public Dictionary<int, int> GoalData; // tracked data
    }
}
