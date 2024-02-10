using System;
using Architecture.Other;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.Data.Structs
{
    // Structure for storing and working with a date
    public struct HomeDay : IEquatable<HomeDay>
    {
        public short Year { get; private set; } // Year
        public byte Month  { get; private set; } // Month
        public byte Day { get; private set; } // Day

        // Create
        public HomeDay(int year, int month, int day)
        {
            Year = (short)year;
            Month = (byte)month;
            Day = (byte)day;
        }
        
        // Create
        public HomeDay(short year, byte month, byte day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        // Check equals to other day
        public bool Equals(HomeDay other) => Year == other.Year && Month == other.Month && Day == other.Day;

        public static bool operator >(HomeDay firstDay, HomeDay secondDay) =>
            OtherHTools.DaysDistance(firstDay, secondDay) > 0;

        public static bool operator <(HomeDay firstDay, HomeDay secondDay) => 
            OtherHTools.DaysDistance(firstDay, secondDay) < 0;
        
        public static bool operator ==(HomeDay firstDay, HomeDay secondDay) => 
            OtherHTools.DaysDistance(firstDay, secondDay) == 0;
        
        public static bool operator !=(HomeDay firstDay, HomeDay secondDay) => 
            OtherHTools.DaysDistance(firstDay, secondDay) != 0;
        
        public static int operator -(HomeDay firstDay, HomeDay secondDay) => 
            OtherHTools.DaysDistance(firstDay, secondDay);
        
        public static bool operator >=(HomeDay firstDay, HomeDay secondDay) =>
            OtherHTools.DaysDistance(firstDay, secondDay) > 0 || firstDay == secondDay;

        public static bool operator <=(HomeDay firstDay, HomeDay secondDay) => 
            OtherHTools.DaysDistance(firstDay, secondDay) < 0  || firstDay == secondDay;

        public static HomeDay operator ++(HomeDay firstDay)
        {
            firstDay.AddDays(1);
            return firstDay;
        }
        
        public static HomeDay operator --(HomeDay firstDay)
        {
            firstDay.AddDays(-1);
            return firstDay;
        }

        // Get day of week by date
        public WeekInfo.DayOfWeek DayOfWeek => (WeekInfo.DayOfWeek) new DateTime(Year, Month, Day).DayOfWeek;

        // Add days count to this date
        public void AddDays(int days)
        {
            var newDate = GetSystemDay().AddDays(days);
            Year = (short)newDate.Year;
            Month = (byte)newDate.Month;
            Day = (byte)newDate.Day;
        }
        
        // Add month count to this date
        public void AddMonths(int months)
        {
            var newDate = GetSystemDay().AddMonths(months);
            Year = (short)newDate.Year;
            Month = (byte)newDate.Month;
            Day = (byte)newDate.Day;
        }

        // Convert to DateTime
        public DateTime GetSystemDay() => new DateTime(Year, Month, Day);
    }
}
