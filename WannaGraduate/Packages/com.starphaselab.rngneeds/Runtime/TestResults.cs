using System.Collections.Generic;
using StarphaseTools.Core;

namespace RNGNeeds
{
    public class TestResults
    {
        public string selectionMethodName;
        public PreventRepeatMethod preventRepeat;
        public uint seed;
        public readonly Dictionary<int, int> indexPicks = new Dictionary<int, int>();
        public int pickCount;
        public int pickCountMin;
        public int pickCountMax;
        
        public int actualPickCount;
        public int repeatCount;
        public double testDuration;

        public float RepeatsPercentage
        {
            get
            {
                var repeatsPercentage = (float)repeatCount / actualPickCount;
                return float.IsNaN(repeatsPercentage) ? 0f : repeatsPercentage;
            }
        }
        
        public string SelectionMethodInfo => $"[{selectionMethodName} | Prevent Repeat - {preventRepeat}] [Seed {seed}]";
        public string PickCountInfo => $"Pick Count: {pickCount:N0} [From Range {pickCountMin:N0} to {pickCountMax:N0}].";
        public string ActualPickCountInfo => $"Items Picked: {actualPickCount:N0} with {RepeatsPercentage:P4} Repeats ({repeatCount:N0})";
        public string TestDurationInfo => $"Test Duration: {CoreUtils.FormatElapsedTime(testDuration)}";

        public string GetFormattedPickDetails()
        {
            var pickDetailsString = "----==== Details ====----\n";
            foreach (var picks in indexPicks)
            {
                pickDetailsString += $"[{picks.Key}] {picks.Value:N0} | {(picks.Value/(float)pickCount):P5}\n";
            }
            return pickDetailsString;
        }

        public string GetFormattedResultsWithDetails()
        {
            return $"{GetFormattedResults()}\n{GetFormattedPickDetails()}";
        }
        
        public string GetFormattedResults()
        {
            return $"{SelectionMethodInfo}\n{PickCountInfo}\n{ActualPickCountInfo}\n{TestDurationInfo}";
        }

        public void Print()
        {
            RLogger.Log($"<b>{SelectionMethodInfo}</b>", LogMessageType.Info);
            RLogger.Log(PickCountInfo, LogMessageType.Info);
            RLogger.Log(ActualPickCountInfo, LogMessageType.Info);
            RLogger.Log(TestDurationInfo, LogMessageType.Info);
        }

        public void Clear()
        {
            indexPicks.Clear();
            pickCount = 0;
            testDuration = 0;
        }
    }
}