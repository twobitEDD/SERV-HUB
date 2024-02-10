namespace Architecture.Data.FlowTrackInfo
{
    // Convert Count type task progress 
    public static class FlowInfoCount
    {
        // Convert line number progress in tracker to real number
        public static int CountByLine(int lineNumber)
        {
            var count = 0;
            
            var to100 = 100;
            while (to100 > 0 && lineNumber > 0)
            {
                to100--;
                lineNumber--;
                count++;
            }

            if (lineNumber <= 0)
                return count;

            var to1000 = 180;
            while (to1000 > 0 && lineNumber > 0)
            {
                to1000--;
                lineNumber--;
                count += 5;
            }

            if (lineNumber <= 0)
                return count;

            while (lineNumber > 0)
            {
                lineNumber--;
                count += 10;
            }
            
            return count;
        }
        
        // Convert line number goal in tracker to real number
        public static int CountGoalByLine(int lineNumber)
        {
            var count = 10;
            
            var to100 = 9;
            while (to100 > 0 && lineNumber > 0)
            {
                to100--;
                lineNumber--;
                count += 10;
            }

            if (lineNumber <= 0)
                return count;

            var to10000 = 99;
            while (to10000 > 0 && lineNumber > 0)
            {
                to10000--;
                lineNumber--;
                count += 100;
            }

            if (lineNumber <= 0)
                return count;

            while (lineNumber > 0)
            {
                lineNumber--;
                count += 500;
            }
            
            return count;
        }
    }
}
