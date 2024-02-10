using System.Collections.Generic;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.EditTaskArea;
using UnityEngine;

namespace Modules.CSVCreator
{
    // Component for create and setup csv table
    public static class CsvFormer
    {
        // 
        public static List<string> CreateCsvResources()
        {
            var data = GenerateData();
            var result = new List<string>();

            foreach (var line in data)
                result.Add(line.FormLine());

            return result;
        }

        // Generate data lines
        private static List<CsvLine> GenerateData()
        {
            var result = new List<CsvLine>
            {
                FirstLine()
            };

            var mainPart = GenerateBaseList();
            AddFlowsToLine(mainPart);
            result.AddRange(mainPart);
            
            return result;
        }

        // Create first line for csv file with cells titles
        private static CsvLine FirstLine()
        {
            var result = CsvLineCreator.CreateNewLine();
            
            result.SetCell(0, "DATE");
            result.SetCell(1, "DAY");
            result.SetCell(2, "SEA CALENDAR");
            result.SetCell(3, "TASKS");
            result.SetCell(6, "ACTIVE TASKS /(TYPE)");
            result.SetCell(7, "COMPLETED/GOAL");
            result.SetCell(8, "ARCHIVED TASKS /(TYPE)");
            result.SetCell(9, "COMPLETED/GOAL");
            
            return result;
        }
        
        // Generate main data lines
        private static List<CsvLine> GenerateBaseList()
        {
            var result = new List<CsvLine>();

            // Generate days for csv
            var days = CsvDaysGenerator.GenerateDaysList();
            var ellipsis = false;

            // For each day
            for (var i = 0; i < days.Length; i++)
            {
                // Check if has info and if has not than add skip points 
                if (!days[i].HasInformation)
                {
                    if (!ellipsis)
                    {
                        var line = CsvLineCreator.CreateNewLine();
                        AddEllipsis(line);
                        result.Add(line);
                        ellipsis = true;
                    }
                    
                    continue;
                }
                
                // Create line with info
                var mainLine = CsvLineCreator.CreateNewLine();
                result.Add(mainLine);
                mainLine.SetCell(0, days[i].Date);
                mainLine.SetCell(1, days[i].Day);
                mainLine.SetCell(2, days[i].SeaCalendar);

                ellipsis = false;
                
                // Check if has tracked progress in this day
                if (days[i].Tasks.Count == 0)
                    continue;
                
                mainLine.SetCell(3, days[i].Tasks[0]);
                mainLine.SetCell(4, days[i].Results[0]);

                // Add additional line for each additional task progress
                for (var j = 1; j < days[i].Tasks.Count; j++)
                {
                    var additionalLine = CsvLineCreator.CreateNewLine();
                    additionalLine.SetCell(3, days[i].Tasks[j]);
                    additionalLine.SetCell(4, days[i].Results[j]);
                    result.Add(additionalLine);
                }
            }
            
            return result;
        }

        // Setup points to line
        private static void AddEllipsis(CsvLine line)
        {
            line.SetCell(0, "...");
        }

        // Add tasks info part
        private static void AddFlowsToLine(List<CsvLine> line)
        {
            // Get all tasks
            var activeFlows = AppData.GetCurrentGroup().Flows;
            var archivedFlows = AppData.GetCurrentGroup().ArchivedFlows;
            // Get max count of tasks (active or archived)
            var flowsLinesCount = Mathf.Max(activeFlows.Length, archivedFlows.Length);
            
            // Create line for each task
            while (line.Count < flowsLinesCount)
                line.Add(CsvLineCreator.CreateNewLine());

            // Setup active tasks and progress to lines
            for (var i = 0; i < activeFlows.Length; i++)
            {
                var flow = activeFlows[i];
                line[i].SetCell(6, $"{flow.Name} / ({ChooseGoalPartJob.GetFlowNameByType(flow.Type).ToLower()})");
                line[i].SetCell(7, 
                    $" {activeFlows[i].GetIntProgress()}/" +
                    $"{FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt)}");
            }
            
            // Setup archived tasks and progress to lines
            for (var i = 0; i < archivedFlows.Length; i++)
            {
                var flow = archivedFlows[i];
                line[i].SetCell(8, $"{flow.Name} / ({ChooseGoalPartJob.GetFlowNameByType(flow.Type).ToLower()})");
                line[i].SetCell(9, 
                    $" {archivedFlows[i].GetIntProgress()}/" +
                    $"{FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt)}");
            }
        }
    }
}
