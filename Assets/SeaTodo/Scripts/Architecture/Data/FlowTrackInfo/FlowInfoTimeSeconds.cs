using System.Linq;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.Data.FlowTrackInfo
{
    // Convert Seconds type task progress 
    public static class FlowInfoTimeSeconds
    {
        // Convert line number progress in tracker to real number
        public static int CountByLine(int lineNumber) => lineNumber;
        
        // Convert line number to time info
        public static (int minutes, int seconds) ConvertLineToText(int lineNumber)
        {
            var minutes = lineNumber / 60;
            var seconds = lineNumber - minutes * 60;
            return (minutes, seconds);
        }
        
        // Convert line number to time text view
        public static string ConvertToView(int lineNumber, int fontSize, bool boldSecondary = true)
        {
            var (minutes, seconds) = ConvertLineToText(lineNumber);
            string result;

            var fontSmallSize = (int)(fontSize * 0.8f);
            if (minutes > 0)
            {
                result = boldSecondary ? $"{minutes}{FlowSymbols.Minutes} <b><size={fontSmallSize}>{seconds:00}{FlowSymbols.Seconds}</size></b>" : 
                   $"{minutes}{FlowSymbols.Minutes} <size={fontSmallSize}>{seconds:00}{FlowSymbols.Seconds}</size>";
            }
            else
            {
                result = $"{seconds}{FlowSymbols.Seconds}";
            }

            return result;
        }
        
        // Convert line number to time text view for csv file
        public static string ConvertToViewCsv(int lineNumber)
        {
            var (minutes, seconds) = ConvertLineToText(lineNumber);

            var result = minutes > 0 ? 
                $"{minutes}{FlowSymbols.Minutes} {seconds:00}{FlowSymbols.Seconds}" : 
                $"{seconds}{FlowSymbols.Seconds}";
            
            return result;
        }
        
        // Convert line number to time text view
        public static string ConvertToViewGoalWithName(int lineNumber, int fontSize)
        {
            var (minutes, seconds) = ConvertLineToText(lineNumber);
            string result;

            var fontSmallSize = (int)(fontSize * 0.8f);
            
            var titleMinutes = $"{FlowSymbols.MinutesFull}.";
            var titleSeconds = $"{FlowSymbols.SecondsFull}.";
            
            if (minutes > 0)
            {
                if (seconds != 0)
                    result = $"{minutes} {titleMinutes} <size={fontSmallSize}>{seconds:00} {titleSeconds}</size>";
                else
                    result = $"{minutes} {titleMinutes}";
            }
            else
            {
                result = $"{seconds} {titleSeconds}";
            }

            return result;
        }

        // Convert line number to time info
        private static (int days, int hours) ConvertLineToTextGoal(int lineNumber)
        {
            var minutes = lineNumber;
            var seconds = 0;
            return (minutes, seconds);
        }

        // Convert line goal number to time info for goal view
        public static string ConvertToViewGoal(int lineNumber)
        {
            lineNumber++;
            lineNumber *= 5;
            
            var (minutes, t) = ConvertLineToTextGoal(lineNumber);
            return $"{minutes}";
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
