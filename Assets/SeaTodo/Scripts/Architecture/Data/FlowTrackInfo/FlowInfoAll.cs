using System;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.Data.FlowTrackInfo
{
    // Convert task progress to visual text
    public static class FlowInfoAll
    {
        // Construct to the text of the converted task progress with the specified parameters
        public static string GetWorkProgressFlow(FlowType flowType, int flowResult, int fontSize, bool boldSecondary = true)
        {
            string result;

            switch (flowType)
            {
                case FlowType.count:
                    result = $"{OtherHTools.ConvertToShortText(flowResult)}{GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.done:
                    result = $"{OtherHTools.ConvertToShortText(flowResult)} {GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.stars:
                    result = $"{OtherHTools.ConvertToShortText(flowResult)}{GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.symbol:
                    result = $"{Mathf.Abs(flowResult)} " + (flowResult >= 0 ? "ʾ" : "ˀ"); // up
                    break;
                case FlowType.timeH:
                    result = FlowInfoTimeHours.ConvertToView(flowResult, fontSize, boldSecondary);
                    break;
                case FlowType.timeM:
                    result = FlowInfoTimeMinutes.ConvertToView(flowResult, fontSize, boldSecondary);
                    break;
                case FlowType.timeS:
                    result = FlowInfoTimeSeconds.ConvertToView(flowResult, fontSize, boldSecondary);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }

            return result;
        }
        
        // Convert progress id to user progress
        public static int GetGoalByOriginFlowInt(FlowType flowType, int flowResult)
        {
            int result;

            switch (flowType)
            {
                case FlowType.count:
                    result = FlowInfoCount.CountGoalByLine(flowResult);
                    break;
                case FlowType.done:
                    result = FlowInfoDone.CountGoalByLine(flowResult);
                    break;
                case FlowType.stars:
                    result = FlowInfoStars.CountGoalByLine(flowResult);
                    break;
                case FlowType.symbol:
                    result = FlowInfoSymbol.CountGoalByLine(flowResult);
                    break;
                case FlowType.timeH:
                    result = FlowInfoTimeHours.ConvertToViewGoalInt(flowResult) * 24;
                    break;
                case FlowType.timeM:
                    result = FlowInfoTimeMinutes.ConvertToViewGoalInt(flowResult) * 60;
                    break;
                case FlowType.timeS:
                    result = FlowInfoTimeSeconds.ConvertToViewGoalInt(flowResult) * 60;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }

            return result;
        }
        
        // Construct to the text of the converted task goal with the specified parameters
        public static string GetViewGoalByOriginFlow(FlowType flowType, int flowResult, int fontSize)
        {
            string result;

            switch (flowType)
            {
                case FlowType.count:
                    result = $"{flowResult}{GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.done:
                    result = $"{flowResult} {GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.stars:
                    result = $"{flowResult}{GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.symbol:
                    result = $"{Mathf.Abs(flowResult)} " + (flowResult >= 0 ? "ʾ" : "ˀ"); // up
                    break;
                case FlowType.timeH:
                    result = FlowInfoTimeHours.ConvertToViewGoalWithName(flowResult, fontSize);
                    break;
                case FlowType.timeM:
                    result = FlowInfoTimeMinutes.ConvertToViewGoalWithName(flowResult, fontSize);
                    break;
                case FlowType.timeS:
                    result = FlowInfoTimeSeconds.ConvertToViewGoalWithName(flowResult, fontSize);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }

            return result;
        }
        
        // Construct to the short text of the converted task goal with the specified parameters
        public static string GetShortViewGoalByOriginFlow(FlowType flowType, int flowResult, int fontSize)
        {
            string result;

            switch (flowType)
            {
                case FlowType.count:
                    result = $"{flowResult}{GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.done:
                    result = $"{flowResult} {GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.stars:
                    result = $"{flowResult}{GetGoalSymbolByResultFlow(flowType, flowResult)}";
                    break;
                case FlowType.symbol:
                    result = $"{Mathf.Abs(flowResult)} " + (flowResult >= 0 ? "ʾ" : "ˀ"); // up
                    break;
                case FlowType.timeH:
                    result = FlowInfoTimeHours.ConvertToViewGoalWithName(flowResult, fontSize);
                    break;
                case FlowType.timeM:
                    result = FlowInfoTimeMinutes.ConvertToViewGoalWithName(flowResult, fontSize);
                    break;
                case FlowType.timeS:
                    result = FlowInfoTimeSeconds.ConvertToViewGoalWithName(flowResult, fontSize);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }

            return result;
        }
        
        // Construct to the text of the converted task progress for csv file
        public static string GetWorkProgressFlowCsv(FlowType flowType, int flowResult)
        {
            string result;

            switch (flowType)
            {
                case FlowType.count:
                    result = $"{OtherHTools.ConvertToShortText(flowResult)}+";
                    break;
                case FlowType.done:
                    result = $"+";
                    break;
                case FlowType.stars:
                    result = $"{flowResult} (stars)";
                    break;
                case FlowType.symbol:
                    result = $"{Mathf.Abs(flowResult)}";
                    break;
                case FlowType.timeH:
                    result = $"{FlowInfoTimeHours.ConvertToViewCsv(flowResult)}";
                    break;
                case FlowType.timeM:
                    result = $"{FlowInfoTimeMinutes.ConvertToViewCsv(flowResult)}";
                    break;
                case FlowType.timeS:
                    result = $"{FlowInfoTimeSeconds.ConvertToViewCsv(flowResult)}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }

            return result;
        }

        // Return symbol of task type for font
        private static string GetGoalSymbolByResultFlow(FlowType flowType, int flowResult)
        {
            string result;

            switch (flowType)
            {
                case FlowType.count:
                    result = string.Empty;
                    break;
                case FlowType.done:
                    result = "ʿ";
                    break;
                case FlowType.stars:
                    result = "ʽ";
                    break;
                case FlowType.symbol:
                    result = flowResult >= 0 ? "ʾ" : "ˀ"; // up
                    break;
                case FlowType.timeH:
                    result = string.Empty;// $"{FlowSymbols.Hours}";
                    break;
                case FlowType.timeM:
                    result = string.Empty;// $"{FlowSymbols.Minutes}";
                    break;
                case FlowType.timeS:
                    result = string.Empty;// $"{FlowSymbols.Seconds}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }

            return result;
        }
    }
}
