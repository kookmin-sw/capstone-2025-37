using UnityEditor;
using UnityEngine;

namespace StarphaseTools.Core.Editor
{
    [CustomPropertyDrawer(typeof(LabInterfaceAttribute), true)]
    public class LabInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference) return;
            var interfaceAttribute = attribute as LabInterfaceAttribute;

            EditorGUI.BeginProperty(position, label, property);
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, interfaceAttribute.interfaceType, true);
            EditorGUI.EndProperty();
        }
    }
}