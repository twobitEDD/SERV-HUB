using Architecture.Data.Structs;

namespace Architecture.Statistics.Interfaces
{
    // Layer for data generator
    public interface IGraphicInfo
    {
        int DefaultStep(); // Get default step of pages of calendar
        void ReloadData(HomeDay homeDay); // Reload data in generator by day
        GraphDataStruct GetInfo(int step); // Get page data by page number
        // Additional method that invokes when new page set in center
        void SendStepToOther(int current); 
    }
}
