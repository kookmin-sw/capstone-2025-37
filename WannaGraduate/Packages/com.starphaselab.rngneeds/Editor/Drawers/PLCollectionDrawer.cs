using StarphaseTools.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    #pragma warning disable 0618
    [CustomPropertyDrawer(typeof(PLCollection<>))]
    public class PLCollectionDrawer : LabDrawerBase
    {
        private IPLCollectionEditorActions PlCollectionInterface;
        private SerializedProperty p_Lists;
        private float m_TotalHeight;
        private Event m_CurrentEvent;
        private const float m_InnerRectPadding = 8f;
        private bool m_AllowRemove;
        private Rect m_InnerRect;
        
        protected override bool Initialize(SerializedProperty property)
        {
            p_Lists = property.FindPropertyRelative("pl_collection");
            PlCollectionInterface = property.GetTargetObjectOfProperty() as IPLCollectionEditorActions;
            return true;
        }

        protected override void BaseDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            m_CurrentEvent = Event.current;
            if (m_CurrentEvent.type == EventType.Layout) return;
            var arraySize = p_Lists.arraySize;
            
            m_InnerRect = new Rect(position.x + m_InnerRectPadding, position.y + m_InnerRectPadding + 5f, position.width - m_InnerRectPadding * 2, position.height - m_InnerRectPadding * 2);
            var headerRectWidths = m_InnerRect.width / 3f;
            
            var propertyNameRect = new Rect(m_InnerRect.x, m_InnerRect.y - 2f, headerRectWidths, 28f);
            var addButtonRect = new Rect(propertyNameRect.xMax + 4f, m_InnerRect.y, headerRectWidths, 24f);
            var clearButtonRect = new Rect(addButtonRect.xMax + 4f, m_InnerRect.y, headerRectWidths - 38f, 24);
            var lockIconRect = new Rect(clearButtonRect.xMax + 8f, m_InnerRect.y + 4f, 16f, 16f);
            var lockButtonRect = new Rect(clearButtonRect.xMax + 4f, m_InnerRect.y, 24f, 24f);

            if (m_CurrentEvent.type == EventType.Repaint)
            {
                var headerRect = new Rect(position.x, position.y + 5f, position.width, 40f);
                
                if (arraySize > 0)
                {
                    var contentRect = new Rect(headerRect.x, headerRect.yMax, headerRect.width,
                        position.height - headerRect.height - 20f);
                    PLDrawerTheme.CollectionFrameImage.Draw(contentRect, false, false, false, false);    
                }
                PLDrawerTheme.CollectionHeaderImage.Draw(headerRect, false, false, false, false);
            }
            
            EditorGUI.LabelField(propertyNameRect, $"{property.displayName} ({arraySize})", PLDrawerTheme.CollectionPropertyNameStyle);
            
            if (GUI.Button(addButtonRect, "Add List to Collection", PLDrawerTheme.OptionButtonOn))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Add List to Collection");
                PlCollectionInterface.AddList();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            if (GUI.Button(lockButtonRect, string.Empty, m_AllowRemove ? PLDrawerTheme.OptionButtonOff : PLDrawerTheme.OptionButtonOn)) m_AllowRemove = !m_AllowRemove;

            if (m_CurrentEvent.type == EventType.Repaint)
            {
                if(!m_AllowRemove) PLDrawerTheme.LockIconEnabled.Draw(lockIconRect, false, false, false, false);
                else PLDrawerTheme.LockIconDisabled.Draw(lockIconRect, false, false, false, false);
            }

            EditorGUI.BeginDisabledGroup(m_AllowRemove == false);
            if (GUI.Button(clearButtonRect, "Clear Collection", PLDrawerTheme.OptionButtonOn))
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Clear Collection");
                PlCollectionInterface.ClearCollection();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                m_AllowRemove = false;
                return;
            }
            EditorGUI.EndDisabledGroup();

            var height = arraySize > 0 ? m_InnerRect.y + 22f : m_InnerRect.y + 32f;
            m_TotalHeight = height;
            
            for (var i = 0; i < arraySize; i++)
            {
                var list = p_Lists.GetArrayElementAtIndex(i);
                var listHeight = EditorGUI.GetPropertyHeight(list);
                
                var rect = new Rect(m_InnerRect.x, height + 20f, m_InnerRect.width, listHeight);
                EditorGUI.PropertyField(rect, list, true);
                var isListEmpty = PlCollectionInterface.IsListEmpty(i);
                var removeButtonOffset = isListEmpty ? 4f : 20f;
                var removeListButtonRect = new Rect(m_InnerRect.x + 4f, rect.yMax - removeButtonOffset, m_InnerRect.width - 12f, 20f);
                
                EditorGUI.BeginDisabledGroup(!m_AllowRemove && !isListEmpty);
                if (GUI.Button(removeListButtonRect, "Remove List from Collection", PLDrawerTheme.OptionButtonOn))
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Remove List");
                    PlCollectionInterface.RemoveList(i);
                    list.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                    return;
                }
                EditorGUI.EndDisabledGroup();
                
                if(i < arraySize - 1) EditorGUI.DrawRect(new Rect(removeListButtonRect.x + 80f, removeListButtonRect.yMax + 15f, removeListButtonRect.width - 160f, 1f), PLDrawerTheme.CollectionSeparatorColor);
                height = removeListButtonRect.yMax + 8f;
            }
            
            m_TotalHeight = height - position.y;
        }

        protected override float OnGetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_TotalHeight + 20f;
        }
    }
    #pragma warning restore 0618
}