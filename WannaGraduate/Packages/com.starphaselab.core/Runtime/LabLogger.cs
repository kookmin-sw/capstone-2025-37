using UnityEditor;
using UnityEngine;

namespace StarphaseTools.Core
{
    public class LabLogger
    {
        public LogMessageType ShowLogLevel { get; set; }
        public bool AllowColors { get; set; }
        
        #if UNITY_EDITOR
        private readonly Color m_InfoColor = EditorGUIUtility.isProSkin ? new Color(.8f, .8f, .8f) : Color.black;
        private readonly Color m_HintColor = EditorGUIUtility.isProSkin ? new Color(.6f, .8f, 1f) : new Color(.1f, .3f, 1f);
        private readonly Color m_DebugColor = EditorGUIUtility.isProSkin ? Color.yellow : new Color(.7f, .3f, .2f);
        #endif
        
        public LabLogger(LogMessageType logMessageLevel = LogMessageType.Info)
        {
            ShowLogLevel = logMessageLevel;
        }
        
        public void Log(string message, LogMessageType messageType, Color? color = null)
        {
            if (ShowLogLevel.HasAny(messageType) == false) return;
            
            #if UNITY_EDITOR
            if (AllowColors)
            {
                if (color.HasValue == false)
                {
                    switch (messageType)
                    {
                        case LogMessageType.None:
                        case LogMessageType.Info:
                            color = m_InfoColor;
                            break;
                        case LogMessageType.Hint:
                            color = m_HintColor;
                            break;
                        case LogMessageType.Debug:
                        case LogMessageType.Warning:
                            color = m_DebugColor;
                            break;
                    }
                }
                message = $"<color=#{ColorUtility.ToHtmlStringRGBA(color.Value)}>[RNGNeeds]</color> {message}";
            } else message = $"[RNGNeeds] {message}";
            #else
            message = $"[RNGNeeds] {message}";
            #endif
            
            if (messageType == LogMessageType.Warning) Debug.LogWarning(message);
            else Debug.Log(message);
        }
    }
}