namespace RNGNeeds
{
    /// <summary>
    /// Enum defining methods for handling repeat prevention when selecting items from a list.
    /// If successive unique picks are important, you can use these options to eliminate or reduce repeats.
    /// Each method offers a trade-off between speed, bias (change in resulting probability), and the degree of repeat prevention.
    /// </summary>
    public enum PreventRepeatMethod
    {
        /// <summary>
        /// No prevention of repeats. Successive picks may be identical. This is the default value in <see cref="ProbabilityList{T}"/>.
        /// </summary>
        Off,
        
        /// <summary>
        /// Eliminates all repeats. Fast repeat prevention with noticeable bias, especially for small lists or low number of rolls. Repeats are replaced with nearest enabled items during the selection process.
        /// </summary>
        Spread,
        
        /// <summary>
        /// Eliminates all repeats. Moderate speed, low bias, which may slightly favor lower probability items and penalize higher probability items. Repeats are prevented by re-rolling during the selection process.
        /// </summary>
        Repick,
        
        /// <summary>
        /// Greatly reduces repeats based on <see cref="ProbabilityList{T}.ShuffleIterations"/> while preserving probabilities. Potentially slow for large lists, minimal to no bias. The order of picked items will be randomized after the selection process.
        /// </summary>
        Shuffle
    }
}