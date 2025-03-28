using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using Sirenix.OdinInspector;
using DominoGames.DominoRx.RxField;

namespace DominoGames.DominoRx.Util
{
    public class RxDataAccessor : MonoBehaviour
    {
        public static IRxField GetRxField(string className, string fieldPath)
        {
            var result = GetValueProcess(className, fieldPath);

            if (IsRxField(result.type))
            {
                return result.instance as IRxField;
            }

            return null;
        }

        public static object GetValue(string className, string fieldPath)
        {
            var result = GetValueProcess(className, fieldPath);

            if (IsRxField(result.type))
            {
                return (result.instance as IRxField).GetValue();
            }

            return result.instance;
        }

        private static ValueResult GetValueProcess(string className, string fieldPath)
        {
            Type classType = Type.GetType(className + ", Assembly-CSharp");
            Type currentType = classType;

            var fieldNames = fieldPath.Split(".").ToList();

            FieldInfo fieldInfo = classType.BaseType.GetField("data", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            object currentObject = fieldInfo.GetValue(null);
            currentType = fieldInfo.FieldType;
            object modelObject = currentObject;

            foreach (var fieldPart in fieldNames)
            {
                if (currentType == null)
                {
                    Debug.LogError($"Field '{fieldPart}' not found in the type.");
                    return new()
                    {
                        instance = null,
                        type = null
                    };
                }

                if (fieldPart.Contains("[") && fieldPart.Contains("]")) // 배열 또는 리스트 접근
                {

                    string fieldName = fieldPart.Substring(0, fieldPart.IndexOf("["));
                    fieldInfo = currentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (fieldInfo == null)
                    {
                        Debug.LogError($"Field '{fieldName}' not found in type '{currentType.Name}'.");
                        return new()
                        {
                            instance = null,
                            type = null
                        };
                    }

                    currentObject = fieldInfo.GetValue(currentObject);

                    if (currentObject is IDictionary)
                    {
                        string key = fieldPart.Substring(fieldPart.IndexOf("[") + 1, fieldPart.IndexOf("]") - fieldPart.IndexOf("[") - 1);

                        if (currentObject is IDictionary dict)
                        {
                            currentObject = dict[key];
                            currentType = currentObject.GetType();
                        }
                        else
                        {
                            Debug.LogError($"Field '{fieldName}' is not a list or array.");
                            return new()
                            {
                                instance = null,
                                type = null
                            };
                        }
                    }
                    else if (currentObject is IList)
                    {
                        int index = int.Parse(fieldPart.Substring(fieldPart.IndexOf("[") + 1, fieldPart.IndexOf("]") - fieldPart.IndexOf("[") - 1));

                        if (currentObject is IList list)
                        {
                            currentObject = list[index];
                            currentType = currentObject.GetType();
                        }
                        else
                        {
                            Debug.LogError($"Field '{fieldName}' is not a list or array.");
                            return new()
                            {
                                instance = null,
                                type = null
                            };
                        }
                    }
                }
                else // 일반 필드 접근
                {
                    fieldInfo = currentType.GetField(fieldPart, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (fieldInfo == null)
                    {
                        Debug.LogError($"Field '{fieldPart}' not found in type '{currentType.Name}'.");
                        return new()
                        {
                            instance = null,
                            type = null
                        };
                    }

                    currentObject = fieldInfo.GetValue(currentObject);
                    currentType = fieldInfo.FieldType;
                }
            }

            return new ValueResult() { instance = currentObject, type = currentType };
        }

        private static bool IsRxField(Type type)
        {
            return typeof(IRxField).IsAssignableFrom(type);
        }

        private struct ValueResult {
            public object instance;
            public Type type;
        }

    }
}