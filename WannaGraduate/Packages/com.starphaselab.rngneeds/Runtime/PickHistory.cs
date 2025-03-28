using System;
using System.Collections.Generic;
using StarphaseTools.Core;
using Unity.Collections;
using UnityEngine;

namespace RNGNeeds
{
    /// <summary>
    /// Maintains a record of recently picked items in a <see cref="ProbabilityList{T}"/>.
    /// Each record, or history entry, contains the index of the item that was picked and the time the pick was made.
    /// The number of entries stored is adjustable through the <see cref="Capacity"/> property.
    /// Entries can be accessed directly or via various helper methods that return lists of recent entries or picks.
    /// History entries are reused to minimize GC impact via a pooling mechanism.
    /// </summary>
    [Serializable]
    public class PickHistory
    {
        [SerializeField] private List<HistoryEntry> m_History;
        [SerializeField] private int m_Capacity;
        private List<int> m_RequestedPicks;
        private List<HistoryEntry> m_RequestedHistoryEntries;
        
        public List<HistoryEntry> History => m_History;
        
        public PickHistory(int capacity = 1000)
        {
            m_Capacity = capacity;
            m_History = new List<HistoryEntry>(m_Capacity * 2);
            m_RequestedPicks = new List<int>(100);
            m_RequestedHistoryEntries = new List<HistoryEntry>(100);
        }

        /// <summary>
        /// Populates and returns a shared list of the latest <see cref="HistoryEntry"/> objects, up to the specified count.
        /// </summary>
        /// <param name="count">The number of history entries to retrieve.</param>
        /// <returns>A shared list of the latest <see cref="HistoryEntry"/> objects.</returns>
        /// <remarks>
        /// This method reuses a private list to avoid unnecessary memory allocation. The returned list is shared and may change upon subsequent calls to this method. If you need a stable list, consider using <see cref="GetLatestEntries(System.Collections.Generic.List{HistoryEntry}, int)"/> with your own list.
        /// </remarks>
        public List<HistoryEntry> GetLatestEntries(int count)
        {
            m_RequestedHistoryEntries.Clear();
            count = Mathf.Min(count, History.Count);
            for (var i = 0; i < count; i++)
            {
                m_RequestedHistoryEntries.Add(History[i]);
            }

            return m_RequestedHistoryEntries;
        }

        /// <summary>
        /// Populates the provided list with the latest <see cref="HistoryEntry"/> objects, up to the specified count.
        /// </summary>
        /// <param name="list">The list to populate with the latest history entries.</param>
        /// <param name="count">The number of history entries to retrieve.</param>
        public void GetLatestEntries(List<HistoryEntry> list, int count)
        {
            list.AddRange(GetLatestEntries(count));
        }

        /// <summary>
        /// Returns a shared list of indices from the latest history entries, up to the specified count.
        /// </summary>
        /// <param name="count">The number of history entry indices to retrieve.</param>
        /// <returns>A shared list of indices from the latest history entries.</returns>
        /// <remarks>
        /// This method reuses a private list to avoid unnecessary memory allocation. The returned list is shared and may change upon subsequent calls to this method. If you need a stable list, consider using <see cref="GetLatestPicks(System.Collections.Generic.List{int}, int)"/> with your own list.
        /// </remarks>
        public List<int> GetLatestPicks(int count)
        {
            var entries = GetLatestEntries(count);
            m_RequestedPicks.Clear();
            foreach (var entry in entries)
            {
                m_RequestedPicks.Add(entry.Index);
            }
            return m_RequestedPicks;
        }

        /// <summary>
        /// Populates the provided list with indices from the latest history entries, up to the specified count.
        /// </summary>
        /// <param name="list">The list to populate with indices from the latest history entries.</param>
        /// <param name="count">The number of history entry indices to retrieve.</param>
        public void GetLatestPicks(List<int> list, int count)
        {
            var entries = GetLatestEntries(count);
            foreach (var entry in entries)
            {
                list.Add(entry.Index);
            }
        }
        
        /// <summary>
        /// Gets or sets the maximum number of entries that the pick history can contain.
        /// If the set capacity is less than the current count of entries, older entries are removed.
        /// The minimum allowed capacity is 1.
        /// </summary>
        public int Capacity
        {
            get => m_Capacity;
            set
            {
                m_Capacity = Mathf.Max(1, value);
                History.Capacity = m_Capacity * 2;
                Trim();
            }
        }

        /// <summary>
        /// Gets the index of the most recent pick. 
        /// If no picks have been made, returns -1.
        /// </summary>
        public int LatestIndex => History.Count > 0 ? History[0].Index : -1;
        
        /// <summary>
        /// Gets the latest history entry in the selection process.
        /// </summary>
        /// <returns>The most recent <see cref="HistoryEntry"/> or null if the history is empty.</returns>
        public HistoryEntry LatestEntry => History.Count > 0 ? History[0] : null;

        /// <summary>
        /// Clears all entries from the pick history.
        /// </summary>
        /// <remarks>
        /// This method removes all entries from the History list and returns them to the pool. Use this method when you want to reset the pick history.
        /// </remarks>
        public void ClearHistory()
        {
            while (History.Count > 0)
            {
                var item = History[0];
                History.RemoveAt(0);
                LabPool<HistoryEntry>.Release(item);
            }
        }
        
        /// <summary>
        /// Adds an entry to the pick history.
        /// </summary>
        /// <param name="index">The index of the picked item.</param>
        /// <remarks>
        /// This method claims a <see cref="HistoryEntry"/> from the pool and inserts it at the beginning of the History list. The entry is populated with the index of the picked item and the current DateTime.
        /// </remarks>
        public void AddEntry(int index)
        {
            var newEntry = LabPool<HistoryEntry>.Claim();
            newEntry.SetEntry(index, DateTime.Now);
            History.Insert(0, newEntry);
            Trim();
        }
        
        /// <summary>
        /// Adds multiple entries to the pick history.
        /// </summary>
        /// <param name="picks">The list of picked item indices.</param>
        /// <remarks>
        /// This method claims multiple <see cref="HistoryEntry"/> objects from the pool and inserts them at the beginning of the History list. Each entry is populated with the index of the picked item and the current DateTime. If the total number of entries exceeds the capacity of the History, the oldest entries are removed.
        /// </remarks>
        public void AddEntries(NativeList<int> picks)
        {
            if(picks.Length == 0) return;
            
            var startIndex = Mathf.Max(0, picks.Length - m_Capacity);

            for (var i = startIndex; i < picks.Length; i++)
            {
                var newEntry = LabPool<HistoryEntry>.Claim();
                newEntry.SetEntry(picks[i], DateTime.Now);
                History.Insert(0, newEntry);
            }

            Trim();
        }

        private void Trim()
        {
            if (History.Count <= Capacity) return;
            for (var i = Capacity; i < History.Count; i++)
            {
                LabPool<HistoryEntry>.Release(History[i]);
            }
            History.RemoveRange(Capacity, History.Count - Capacity);
            // History.TrimExcess();
        }
    }
}