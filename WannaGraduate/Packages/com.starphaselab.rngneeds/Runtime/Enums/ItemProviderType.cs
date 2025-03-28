using System;

namespace RNGNeeds
{
    /// <summary>
    /// Flags enumeration representing different types of providers that a ProbabilityItem can have.
    /// These providers allow for extended functionality and customization of each item.
    /// </summary>
    /// <list type="bullet">
    /// <item><description>None - No provider is assigned.</description></item>
    /// <item><description>InfluenceProvider - Item has a provider that can influence its base probability.</description></item>
    /// <item><description>ModProvider - Item has a provider that can modify it in an arbitrary way. Not yet implemented (NYI).</description></item>
    /// <item><description>InfoProvider - Item has a provider that can supply additional information about it, mainly for inspector display.</description></item>
    /// <item><description>ColorProvider - Item has a provider that can assign a specific color to it, useful for visualization in the inspector and potentially at runtime.</description></item>
    /// </list>
    [Flags]
    public enum ItemProviderType
    {
        None = 0,
        InfluenceProvider = 1 << 0,
        ModProvider = 1 << 1,       // NYI
        InfoProvider = 1 << 2,
        ColorProvider = 1 << 3
    }
}