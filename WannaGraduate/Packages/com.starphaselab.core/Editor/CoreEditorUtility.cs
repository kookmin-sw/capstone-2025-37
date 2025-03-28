using System.Collections;
using System.Reflection;
using UnityEditor;

namespace StarphaseTools.Core.Editor
{
    public static class CoreEditorUtility
    {
        public static object GetTargetObjectOfProperty(this SerializedProperty property)
        {
            var propertyPath = property.propertyPath.Replace(".Array.data[", "[");
            object targetObject = property.serializedObject.targetObject;
            var pathSegments = propertyPath.Split('.');
            
            foreach (var pathSegment in pathSegments)
            {
                if (pathSegment.Contains("["))
                {
                    var memberName = pathSegment.Substring(0, pathSegment.IndexOf("["));
                    targetObject = GetValueFromSource(targetObject, memberName, int.Parse(pathSegment.Substring(pathSegment.IndexOf("["), pathSegment.Length - pathSegment.IndexOf("[")).Trim('[', ']')));
                } else targetObject = GetValueFromSource(targetObject, pathSegment);
            }
            return targetObject;
        }

        private static object GetValueFromSource(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();

            while (type != null)
            {
                var fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo != null) return fieldInfo.GetValue(source);

                var propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propertyInfo != null) return propertyInfo.GetValue(source);
                type = type.BaseType;
            }
            return null;
        }

        private static object GetValueFromSource(object source, string name, int index)
        {
            if (!(GetValueFromSource(source, name) is IEnumerable enumerable)) return null;

            var enm = enumerable.GetEnumerator();
            for (var i = 0; i <= index; i++) if (!enm.MoveNext()) return null;
            return enm.Current;
        }
    }
}