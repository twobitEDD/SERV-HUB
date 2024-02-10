namespace Architecture.Data.FlowTrackInfo
{
    // Convert Stars type task progress 
    public static class FlowInfoStars
    {
        // Convert line number progress in tracker to real number
        public static int CountByLine(int lineNumber) => lineNumber;
        
        // Convert line number goal in tracker to real number
        public static int CountGoalByLine(int lineNumber) 
        {
            var count = 5;
            
            var to100 = 19;
            while (to100 > 0 && lineNumber > 0)
            {
                to100--;
                lineNumber--;
                count += 5;
            }

            if (lineNumber <= 0)
                return count;

            var to10000 = 90;
            while (to10000 > 0 && lineNumber > 0)
            {
                to10000--;
                lineNumber--;
                count += 10;
            }

            if (lineNumber <= 0)
                return count;

            while (lineNumber > 0)
            {
                lineNumber--;
                count += 20;
            }
            
            return count;
        }
    }
}
