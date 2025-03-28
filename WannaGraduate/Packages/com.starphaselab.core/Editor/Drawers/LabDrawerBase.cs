using UnityEditor;
using UnityEngine;

namespace StarphaseTools.Core.Editor
{
    public abstract class LabDrawerBase : PropertyDrawer
    {
        protected readonly int baseID;
        private bool m_Initialized;
        private bool m_SelectionMightHaveChanged;
        
        protected virtual bool m_KeepAliveOnLockedInspectorSelectionChange => false;

        protected LabDrawerBase()
        {
            baseID = Random.Range(11111, 99999);
            Enable();
        }
        
        ~LabDrawerBase()
        {
            Disable();
        }
        
        private void Enable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            OnEnable();
        }

        private void Disable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            OnDisable();
        }

        private void OnSelectionChanged()
        {
            Disable();
            m_SelectionMightHaveChanged = true;
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode) Disable();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_Initialized == false) if (TryInitialize(property) == false) return;
            if (m_Initialized == false) return;

            if (m_KeepAliveOnLockedInspectorSelectionChange && m_SelectionMightHaveChanged)
            {
                Enable();
                m_SelectionMightHaveChanged = false;
            }
            
            BaseDraw(position, property, label);
        }

        private bool TryInitialize(SerializedProperty property)
        {
            if (Event.current.type == EventType.Layout || m_Initialized) return m_Initialized;
            m_Initialized = Initialize(property);
            return m_Initialized;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (m_Initialized) return OnGetPropertyHeight(property, label);
            return TryInitialize(property) ? OnGetPropertyHeight(property, label) : EditorGUIUtility.singleLineHeight;
        }

        protected virtual float OnGetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        
        protected virtual void OnEnable() {}
        protected virtual void OnDisable() {}
        protected abstract bool Initialize(SerializedProperty property);
        protected abstract void BaseDraw(Rect position, SerializedProperty property, GUIContent label);
    }
}