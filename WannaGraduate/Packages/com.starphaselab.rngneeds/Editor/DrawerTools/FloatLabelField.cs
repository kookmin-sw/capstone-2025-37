using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    public class FloatLabelField
    {
        private float m_OriginalValue;
        private float m_StoredValue;
        private const string ControlIdPrefix = "FloatLabelField";
        private int PreviousSelectedIndex { get; set; } = -1;
        public int SelectedIndex { get; set; } = -1;
        
        public (float Value, bool Finished) DrawAndHandleInput(Rect rect, int index, float currentValue, bool locked, string percentage, GUIStyle style)
        {
            var newValue = currentValue;
            var finishedEditing = false;

            if (SelectedIndex == index)
            {
                var controllID = $"{ControlIdPrefix}_{index.ToString()}";
                EditorGUI.FocusTextInControl(controllID);
                GUI.SetNextControlName(controllID);

                if (SelectedIndex != PreviousSelectedIndex)
                {
                    m_StoredValue = currentValue;
                    PreviousSelectedIndex = SelectedIndex;
                }

                m_StoredValue = EditorGUI.FloatField(rect, m_StoredValue);

                if (Event.current.type == EventType.KeyUp)
                {
                    if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                    {
                        SelectedIndex = -1;
                        GUIUtility.keyboardControl = 0;
                        newValue = m_StoredValue;
                        finishedEditing = true;
                        Event.current.Use();
                    }
                    else if (Event.current.keyCode == KeyCode.Escape)
                    {
                        SelectedIndex = -1;
                        GUIUtility.keyboardControl = 0;
                        newValue = m_OriginalValue;
                        Event.current.Use();
                    }
                    else if (Event.current.keyCode == KeyCode.Tab)
                    {
                        SelectedIndex = index + 1;
                        newValue = m_StoredValue;
                        finishedEditing = true;
                        Event.current.Use();
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(rect, percentage, style);
                
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition) && !locked)
                {
                    SelectedIndex = index;
                    m_OriginalValue = currentValue;
                    Event.current.Use();
                }
            }
            
            return (newValue, finishedEditing);
        }
    }
}