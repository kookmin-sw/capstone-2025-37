using System;

namespace StarphaseTools.Core
{
    [Flags]
    public enum LogMessageType
    {
        None = 0,
        Info = 1 << 0,
        Hint = 1 << 1,
        Warning = 1 << 2,
        Debug = 1 << 3,
        All = Info | Hint | Warning | Debug
    }
}