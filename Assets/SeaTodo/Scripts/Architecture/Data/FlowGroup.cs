namespace Architecture.Data
{
    // Task package and additional information (Global package)
    public class FlowGroup
    {
        public string Name; // Title name
        public int Id; // Id of global package
        public int Order; // Order of global package
        public int Icon; // Icon id of title
        public bool Reminders; // Activity of reminders
        public int DateStarted; // Started day of global package 
        
        public Flow[] Flows { get; set; } // Tasks
        public Flow[] ArchivedFlows { get; set; } // Archived tasks
    }
}
