using StarphaseTools.Core;
using UnityEngine;

namespace RNGNeeds
{
    public static class RLogger
    {
        private static readonly LabLogger m_Logger;

        static RLogger()
        {
            m_Logger = new LabLogger();
        }

        internal static void SetLogLevel(LogMessageType logLevel)
        {
            m_Logger.ShowLogLevel = logLevel;
        }

        internal static void SetAllowColors(bool allowColors)
        {
            m_Logger.AllowColors = allowColors;
        }

        public static void Log(string message, LogMessageType messageType, Color? color = null)
        {
            m_Logger.Log(message, messageType, color);
        }
    }
}