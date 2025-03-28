using System;

namespace RNGNeeds
{
    [Obsolete("Used only by RNGNeeds PLCollection Drawer")]
    public interface IPLCollectionEditorActions
    {
        void AddList();
        void RemoveList(int index);
        void ClearCollection();
        bool IsListEmpty(int index);
        Type ItemType();
        
        IProbabilityList GetList(int index);
    }
}