using System.Collections.Generic;

namespace StarphaseTools.Core
{
    public static class LabPool<T> where T : ICleanable, new()
    {
        private static readonly Stack<T> Pool = new Stack<T>();

        public static void WarmUp(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var item = new T();
                Pool.Push(item);
            }
        }
        
        public static T Claim()
        {
            return Pool.Count > 0 ? Pool.Pop() : new T();
        }

        public static void Release(T item)
        {
            item.Clean();
            Pool.Push(item);
        }
    }
}