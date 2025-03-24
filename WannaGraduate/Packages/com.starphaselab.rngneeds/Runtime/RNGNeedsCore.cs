using System.Collections.Generic;
using StarphaseTools.Core;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds
{
    /// <summary>
    /// The RNGNeedsCore class provides static functionalities and properties that cater to random number generation and selection methods.
    /// </summary>
    public static class RNGNeedsCore
    {
        private static bool m_Initialized { get; set; }
        private static ISeedProvider m_DefaultSeedProvider;
        private static readonly object _lockObject = new object();
        
        private static ISelectionMethod SelectionMethodFallback { get; set; }
        private static ISeedProvider m_SeedProvider { get; set; }
        private static Dictionary<string, ISelectionMethod> SelectionMethods { get; set; }
        private static List<IProbabilityItem> m_EnabledItems;

        #region Internal

        [RuntimeInitializeOnLoadMethod]
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        private static void Initialize()
        {
            if (m_Initialized) return;

            m_EnabledItems = new List<IProbabilityItem>(100);
            m_DefaultSeedProvider = new DefaultSeedProvider();
            ResetSeedProvider();
            
            SelectionMethods = new Dictionary<string, ISelectionMethod>();
            var linearSearchMethod = new LinearSearchMethod();
            SelectionMethodFallback = linearSearchMethod;
            
            SelectionMethods.Add(linearSearchMethod.Identifier, linearSearchMethod);
            
            m_Initialized = true;
            
            RegisterSelectionMethod(new LinearSearchMethodBurst());
            RegisterSelectionMethod(new CumulativeProbability());
            RegisterSelectionMethod(new CumulativeProbabilityBurst());
            RegisterSelectionMethod(new RandomSelectionMethod());
        }

        public static void SetLogLevel(LogMessageType logMessageLevel)
        {
            RLogger.SetLogLevel(logMessageLevel);
        }

        public static void SetLogAllowColors(bool allowColors)
        {
            RLogger.SetAllowColors(allowColors);
        }
        
        // JAW - this is a theory-craft for handling package updates if one method was changed to another
        // and users still have serialized the old method identifier which now does not exist
        // this code would be updated before each package update to handle saved changes
        // otherwise the serialized method infos would fallback to default
        private static bool TryGetMethodChange(string oldIdentifier, out string newIdentifier)
        {
            switch (oldIdentifier)
            {
                case "OldID":
                    newIdentifier = "CorrectID";
                    return true;
            }

            newIdentifier = string.Empty;
            return false;
        }

        internal static void SelectItemsInternal(this IProbabilityList list, int pickCount, ISelectionMethod selectionMethod, out NativeList<int> result)
        {
            lock (_lockObject)
            {
                m_EnabledItems.Clear();

                var itemCount = list.ItemCount;
                for (var i = 0; i < itemCount; i++)
                {
                    var item = list.Item(i);
                    if (item.Enabled) m_EnabledItems.Add(item);
                }

                if (m_EnabledItems.Count == 0)
                {
                    result = new NativeList<int>(Allocator.TempJob);
                    return;
                }

                // Edge Cases - if only one item is enabled, we can skip the selection process
                if (m_EnabledItems.Count == 1 && list.MaintainPickCountIfDisabled)
                {
                    RLogger.Log("Edge Case - single enabled item with Maintain Pick Count.", LogMessageType.Debug);
                    result = new NativeList<int>(Allocator.TempJob);

                    if (m_EnabledItems[0].Probability > 0f == false) return;

                    for (var index = 0; index < itemCount; index++)
                    {
                        var probabilityItem = list.Item(index);
                        if (!probabilityItem.Enabled) continue;
                        for (var i = 0; i < pickCount; i++) result.Add(index);
                        return;
                    }

                    RLogger.Log("Should not reach this point!", LogMessageType.Warning);
                }

                var totalEnabledProbability = 0f;
                foreach (var enabledItem in m_EnabledItems) totalEnabledProbability += enabledItem.Probability;

                if (list.MaintainPickCountIfDisabled && ((totalEnabledProbability * 10000000) / pickCount) < 0.1f)
                {
                    RLogger.Log(
                        "Total enabled probability of the list is too low to Maintain selected Pick Count. This could result in a very long selection process or hang. No items were picked. Enable more items, lower the pick count, or turn off 'Maintain Pick Count'. This can also happen in influenced list - make sure -1 influence spread of at least one item is above 0%",
                        LogMessageType.Hint);
                    result = new NativeList<int>(Allocator.TempJob);
                    return;
                }

                result = selectionMethod.SelectItems(list, pickCount);
            }
        }

        #endregion
        
        /// <summary>
        /// Generates a new seed using the registered Seed Provider.
        /// </summary>
        public static uint NewSeed => m_SeedProvider.NewSeed;

        /// <summary>
        /// A collection of all registered selection methods that are available. Each method has a unique key 
        /// which is the identifier of the selection method. This makes it easy to look up and use 
        /// different methods for selecting items from a probability list.
        /// </summary>
        /// <remarks>You can use <see cref="RegisterSelectionMethod"/> to register your own selection process.</remarks>
        public static List<(string Identifier, string Name)> RegisteredSelectionMethods
        {
            get
            {
                var selectionMethodsInfo = new List<(string Identifier, string Name)>();
                foreach (var selectionMethod in SelectionMethods)
                {
                    selectionMethodsInfo.Add((selectionMethod.Value.Identifier, selectionMethod.Value.Name));
                }
                return selectionMethodsInfo;
            }
        }

        /// <summary>
        /// The identifier of the default selection method. If no other method is specified, this one 
        /// will be used. It's always a good idea to have a reliable fallback!
        /// </summary>
        public static string DefaultSelectionMethodID
        {
            get
            {
                if (m_Initialized == false) Initialize();
                return SelectionMethodFallback.Identifier;
            }
        }

        /// <summary>
        /// Sets the seed provider for the random number generator. The seed provider determines 
        /// the initial state of the random number generator and can be used to create reproducible 
        /// random results.
        /// </summary>
        /// <param name="seedProvider">The seed provider to use for the random number generator.</param>
        public static void SetSeedProvider(ISeedProvider seedProvider)
        {
            m_SeedProvider = seedProvider;
        }
        
        /// <summary>
        /// Resets the seed provider to its default state. This can be useful if you want to return 
        /// to the original state of the random number generator after using a custom seed provider.
        /// </summary>
        public static void ResetSeedProvider()
        {
            m_SeedProvider = m_DefaultSeedProvider;
        }
        
        /// <summary>
        /// Registers a custom selection method to the RNGNeeds library. This allows for the use of new 
        /// selection strategies beyond the built-in ones. The selection method must have a unique identifier.
        /// </summary>
        /// <param name="selectionMethod">The selection method to register.</param>
        public static void RegisterSelectionMethod(ISelectionMethod selectionMethod)
        {
            lock (_lockObject)
            {
                if (m_Initialized == false) Initialize();
                
                if (string.IsNullOrEmpty(selectionMethod.Identifier))
                {
                    RLogger.Log($"Provided selection method '{selectionMethod.GetType().Name}' has no identifier and was not registered. Assets using this method will fall back to {SelectionMethodFallback.Name}", LogMessageType.Warning);
                    return;
                }

                if (SelectionMethods.ContainsKey(selectionMethod.Identifier))
                {
                    RLogger.Log($"A selection method with Identifier '{selectionMethod.Identifier}' is already registered.", LogMessageType.Warning);
                    return;
                }

                SelectionMethods.Add(selectionMethod.Identifier, selectionMethod);
            }
        }
        
        /// <summary>
        /// Retrieves a registered selection method by its identifier. If the identifier is not found,
        /// the fallback selection method is returned.
        /// </summary>
        /// <param name="identifier">The identifier of the selection method to retrieve.</param>
        /// <returns>The selection method associated with the identifier, or the fallback selection method 
        /// if the identifier is not found.</returns>
        public static ISelectionMethod GetSelectionMethod(string identifier)
        {
            lock (_lockObject)
            {
                if (m_Initialized == false) Initialize();
                if (SelectionMethods.TryGetValue(identifier, out var selectionMethod)) return selectionMethod;
                if (TryGetMethodChange(identifier, out var newIdentifier)) return GetSelectionMethod(newIdentifier);
                return SelectionMethodFallback;
            }
        }

        /// <summary>
        /// Pre-allocates a specified number of HistoryEntry objects in the global pool for future use by all ProbabilityList instances. 
        /// This can be useful to reduce allocations during runtime if you know you'll need a certain number of entries.
        /// </summary>
        /// <param name="count">The number of HistoryEntry objects to pre-allocate in the global pool.</param>
        public static void WarmupHistoryEntries(int count)
        {
            LabPool<HistoryEntry>.WarmUp(count);
        }
    }
}