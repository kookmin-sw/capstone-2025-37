using System;
using StarphaseTools.Core;
using UnityEngine;

namespace RNGNeeds
{
    /// <summary>
    /// Represents a historical record in the selection process.
    /// It contains the index of the selected item and the timestamp of the selection.
    /// Implements the <see cref="ICleanable"/> interface for object pooling purposes.
    /// </summary>
    [Serializable]
    public class HistoryEntry : ICleanable
    {
        [SerializeField] private int m_Index;
        [SerializeField] private SerializableDateTime m_Time;

        public int Index => m_Index;
        public DateTime Time => m_Time.ToDateTime();

        public void SetEntry(int index, DateTime time)
        {
            m_Index = index;
            m_Time = time;
        }
        
        /// <summary>
        /// Cleans the history entry by resetting the Index to -1 and the Time to default.
        /// This method is called when the object is returned to the pool to prepare it for reuse.
        /// </summary>
        public void Clean()
        {
            m_Index = -1;
            m_Time = default;
        }
    }
}