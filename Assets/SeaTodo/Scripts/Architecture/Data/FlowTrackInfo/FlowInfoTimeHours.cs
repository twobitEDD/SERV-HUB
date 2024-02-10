using System;
using System.Linq;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.Data.FlowTrackInfo
{
    // Convert Hours type task progress 
    public static class FlowInfoTimeHours
    {
        // Convert line number progress in tracker to real number
        public static int CountByLine(int lineNumber) => lineNumber;

        // Convert line number to time info
        private static (int days, int hours) ConvertLineToText(int lineNumber)
        {
            var days = lineNumber / 24;
            var hours = lineNumber - days * 24;
            return (days, hours);
        }
        
        // Convert line number to time text view
        public static string ConvertToView(int lineNumber, int fontSize, bool boldSecondary = true)
        {
            var (days, hours) = ConvertLineToText(lineNumber);
            string result;

            var fontSmallSize = (int)(fontSize * 0.8f);
            if (days > 0)
            {
                result = boldSecondary ? $"{days}{FlowSymbols.Days} <b><size={fontSmallSize}>{hours}{FlowSymbols.Hours}</size></b>" :
                    $"{days}{FlowSymbols.Days} <size={fontSmallSize}>{hours}{FlowSymbols.Hours}</size>";
            }
            else
            {
                result = $"{hours}{FlowSymbols.Hours}";
            }

            return result;
        }
        
        // Convert line number to time text view for csv file
        public static string ConvertToViewCsv(int lineNumber)
        {
            var (days, hours) = ConvertLineToText(lineNumber);
            return days > 0 ? $"{days}{FlowSymbols.Days} {hours}{FlowSymbols.Hours}" : $"{hours}{FlowSymbols.Hours}";
        }
        
        // Convert line number to time text view
        public static string ConvertToViewGoalWithName(int lineNumber, int fontSize)
        {
            var (days, hours) = ConvertLineToText(lineNumber);
            string result;

            var fontSmallSize = (int)(fontSize * 0.8f);

            var titleDays = $"{FlowSymbols.DaysFull}.";
            var titleHours = $"{FlowSymbols.HoursFull}.";
            
            if (days > 0)
            {
                if (hours != 0)
                    result = $"{days} {titleDays} <b><size={fontSmallSize}>{hours} {titleHours}</size></b>";
                else
                    result = $"{days} {titleDays}";
            }
            else
            {
                result = $"{hours} {titleHours}";
            }

            return result;
        }
        
        // Convert line number to time info
        private static (int days, int hours) ConvertLineToTextGoal(int lineNumber)
        {
            var days = lineNumber;
            var hours = 0;
            return (days, hours);
        }

        // Convert line goal number to time info for goal view
        public static string ConvertToViewGoalWithName(int lineNumber)
        {
            lineNumber++;
            lineNumber *= 5;
            
            var (days, hours) = ConvertLineToTextGoal(lineNumber);
            return $"{days}";
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
