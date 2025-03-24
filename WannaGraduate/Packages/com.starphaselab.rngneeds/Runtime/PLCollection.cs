using System;
using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds
{
#pragma warning disable 0618
    /// <summary>
    /// A collection class for organizing multiple ProbabilityLists into an array.
    /// </summary>
    [Serializable]
    public class PLCollection<T>
        #if UNITY_EDITOR
            : IPLCollectionEditorActions
        #endif
    {
        [SerializeField] private List<ProbabilityList<T>> pl_collection = new List<ProbabilityList<T>>();
        
        /// <summary>
        /// Gets the ProbabilityList at the specified index.
        /// </summary>
        /// <param name="index">The index of the ProbabilityList.</param>
        /// <returns>The ProbabilityList at the specified index.</returns>
        public IProbabilityList GetList(int index)
        {
            return pl_collection[index];
        }
        
        /// <summary>
        /// Adds a provided ProbabilityList to the collection.
        /// </summary>
        /// <param name="list">The ProbabilityList to add.</param>
        public void AddList(ProbabilityList<T> list)
        {
            pl_collection.Add(list);
        }
        
        /// <summary>
        /// Adds a new empty ProbabilityList to the collection.
        /// </summary>
        public void AddList()
        {
            AddList(new ProbabilityList<T>());
        }

        /// <summary>
        /// Removes the ProbabilityList at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index of the ProbabilityList to remove.</param>
        public void RemoveList(int index)
        {
            pl_collection.RemoveAt(index);
        }

        /// <summary>
        /// Clears the entire collection of ProbabilityLists.
        /// </summary>
        public void ClearCollection()
        {
            pl_collection.Clear();
        }

        /// <summary>
        /// Checks if the ProbabilityList at the specified index is empty.
        /// </summary>
        /// <param name="index">The index of the ProbabilityList to check.</param>
        /// <returns>True if the ProbabilityList is empty, false otherwise.</returns>
        public bool IsListEmpty(int index)
        {
            return pl_collection[index].ItemCount == 0;
        }

        /// <summary>
        /// Picks values from all ProbabilityLists in the collection.
        /// </summary>
        /// <returns>A list of picked values from all ProbabilityLists.</returns>
        public List<T> PickFromAll()
        {
            var pickedValues = new List<T>();
            foreach (var probabilityList in pl_collection)
            {
                pickedValues.AddRange(probabilityList.PickValues());
            }

            return pickedValues;
        }

        /// <summary>
        /// Gets the type of the items stored in the ProbabilityLists.
        /// </summary>
        /// <returns>The type of the items stored in the ProbabilityLists.</returns>
        public Type ItemType() => typeof(T);
    }

#pragma warning restore 0618
}