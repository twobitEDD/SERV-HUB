using System.Linq;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.Data.FlowTrackInfo
{
    // Convert Minutes type task progress 
    public static class FlowInfoTimeMinutes
    {
        // Convert line number progress in tracker to real number
        public static int CountByLine(int lineNumber) => lineNumber;
        
        // Convert line number to time info
        private static (int hours, int minutes) ConvertLineToText(int lineNumber)
        {
            var hours = lineNumber / 60;
            var minutes = lineNumber - hours * 60;
            return (hours, minutes);
        }

        // Convert line number to time text view for csv file
        public static string ConvertToViewCsv(int lineNumber)
        {
            var (hours, minutes) = ConvertLineToText(lineNumber);

            var result = hours > 0 ? 
                $"{hours}{FlowSymbols.Hours} {minutes:00}{FlowSymbols.Minutes}" : 
                $"{minutes}{FlowSymbols.Minutes}";
            
            return result;
        }
        
        // Convert line number to time text view
        public static string ConvertToView(int lineNumber, int fontSize, bool boldSecondary = true)
        {
            var (hours, minutes) = ConvertLineToText(lineNumber);
            string result;

            var fontSmallSize = (int) (fontSize * 0.8f);
            if (hours > 0)
            {
                result = boldSecondary ? $"{hours}{FlowSymbols.Hours} <b><size={fontSmallSize}>{minutes:00}{FlowSymbols.Minutes}</size></b>" : 
                    $"{hours}{FlowSymbols.Hours} <size={fontSmallSize}>{minutes:00}{FlowSymbols.Minutes}</size>";
            }
            else
            {
                result = $"{minutes}{FlowSymbols.Minutes}";
            }

            return result;
        }
        
        // Convert line number to time text view
        public static string ConvertToViewGoalWithName(int lineNumber, int fontSize)
        {
            var (hours, minutes) = ConvertLineToText(lineNumber);
            string result;
            
            var fontSmallSize = (int) (fontSize * 0.8f);
            
            var titleHours = $"{FlowSymbols.HoursFull}.";
            var titleMinutes = $"{FlowSymbols.MinutesFull}.";
            
            if (hours > 0)
            {
                if (minutes != 0)
                    result = $"{hours} {titleHours} <b><size={fontSmallSize}>{minutes:00} {titleMinutes}</size></b>";
                else
                    result = $"{hours} {titleHours}";
            }
            else
            {
                result = $"{minutes} {titleMinutes}";
            }

            return result;
        }

        // Convert line number to time info
        private static (int days, int hours) ConvertLineToTextGoal(int lineNumber)
        {
            var hours = lineNumber;
            var minutes = 0;
            return (hours, minutes);
        }

        // Convert line goal number to time info for goal view
        public static string ConvertToViewGoal(int lineNumber)
        {
            lineNumber++;
            lineNumber *= 5;
            
            var (hours, minutes) = ConvertLineToTextGoal(lineNumber);
            
            return $"{hours}";
        }
        
        // Convert line goal number to line number
        public static int ConvertToViewGoalInt(int lineNumber)
        {
            lineNumber++;
            lineNumber *= 5;
            return lineNumber;
        }
    }
}
