namespace RNGNeeds
{
    internal struct SelectJobData
    {
        public int pickCount;
        public int itemCount;
        public bool maintainPickCountWhenDisabled;
        public PreventRepeatMethod preventRepeatMethod;
        public int shuffleIterations;
        public int lastPickedIndex;
        public uint seed;
    }
    
    internal static class SelectJobUtility
    {
        public static SelectJobData GetJobData(IProbabilityList probabilityList, int pickCount)
        {
            var jobData = new SelectJobData()
            {
                pickCount = pickCount,
                itemCount = probabilityList.ItemCount,
                maintainPickCountWhenDisabled = probabilityList.MaintainPickCountIfDisabled,
                preventRepeatMethod = probabilityList.PreventRepeat,
                shuffleIterations = probabilityList.ShuffleIterations,
                lastPickedIndex = probabilityList.LastPickedIndex,
                seed = probabilityList.CurrentSeed
            };
            
            return jobData;
        }
    }
}