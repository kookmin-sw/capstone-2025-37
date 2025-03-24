using System;

namespace StarphaseTools.Core
{
    [Serializable]
    public struct SerializableDateTime
    {
        public long Ticks;

        public SerializableDateTime(DateTime dateTime)
        {
            Ticks = dateTime.Ticks;
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Ticks);
        }

        public static implicit operator DateTime(SerializableDateTime sdt)
        {
            return sdt.ToDateTime();
        }

        public static implicit operator SerializableDateTime(DateTime dt)
        {
            return new SerializableDateTime(dt);
        }
    }
}