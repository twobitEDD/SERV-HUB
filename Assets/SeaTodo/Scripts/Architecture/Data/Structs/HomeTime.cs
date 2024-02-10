namespace Architecture.Data.Structs
{
    // Structure for storing and working with a time
    public struct HomeTime
    {
        public int Hour;
        public int Minute;

        // Create
        public HomeTime(int hour, int minute)
        {
            this.Hour = hour;
            this.Minute = minute;
        }

        // Get time in 12 hours format
        public (int hours, int minutes, bool am) GetTime(bool english = false)
        {
            if (!english)
                return (Hour, Minute, false);
            
            var hours = Hour;

            if (hours < 12 && hours != 0) 
                return (hours, Minute, true);
            
            if (hours > 12 && hours != 12)
                return (hours - 12, Minute, false);
            
            if (hours == 12)
                return (hours, Minute, false);

            return (hours + 12, Minute, true);
        }

        // Set time
        public void SetTime(int hours, int minutes, bool am, bool english)
        {
            if (english)
                SetTimeLocal(hours, minutes, am);
            else
                SetTimeLocal(hours, minutes);
        }

        // Set time in 24 hours format
        private void SetTimeLocal(int hours, int minutes)
        {
            Hour = hours;
            Minute = minutes;
        }
        
        // Set time in 12 hours format
        private void SetTimeLocal(int hours, int minutes, bool am)
        {
            Hour = hours;
            
            if (!am && hours < 12)
                Hour = hours + 12;

            if (hours == 12 && am)
                Hour = hours - 12;
            
            Minute = minutes;
        }
    }
}
