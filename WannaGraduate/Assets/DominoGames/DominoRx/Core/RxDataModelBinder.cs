using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Sirenix.OdinInspector;
using System.Linq;
using DominoGames.DominoRx.RxField;
using Unity.VisualScripting;
using DominoGames.DominoRx.Util;

namespace DominoGames.DominoRx.DataModel
{
    [Serializable]
    public class RxDataModelBinderTarget
    {
        [BoxGroup("Model")]
        [ValueDropdown("GetOptions"), OnValueChanged("ClearFieldPath")]
        public string className = "";

        [BoxGroup("Model")]
        [ValueDropdown("GetFieldPathSuggestion")]
        [SerializeField] private string fieldPath = "";
        public string FieldPath { 
            get
            {
                return fieldPath.Split(" ")[0];
            } 
        }


        [BoxGroup("Component")]
        [ValidateInput("ValidateRxBindTarget", "the field Type is not castable to bind type!"), Delayed]
        [ValueDropdown("GetRxBindTargets")]
        [SerializeField] private string rxBindTarget = "";
        public string RxBindTarget
        {
            get
            {
                return rxBindTarget.Split("/")[1].Split(" ")[0];
            }
        }


        public IRxField bindedRxField;

        [HideInInspector]
        public RxDataModelBinder parentBinder;

        private List<string> GetOptions()
        {
            // RxDataModel<>를 상속받는 클래스 목록 가져오기
            return RxDataModelSearch.GetDerivedClassNames(typeof(RxDataModel<>));
        }

        private List<string> GetRxBindTargets()
        {
            List<string> result = new();
            var comps = parentBinder.GetComponents<IRxBindData>();
            for(int i = 0; i < comps.Length; i++)
            {
                var targetComp = comps[i];

                var compType = comps[i].GetType();
                var fields = GetAllFieldsIncludeParent(compType);
                foreach (FieldInfo field in fields)
                {
                    if (field.HasAttribute(typeof(RxBindFieldAttribute)))
                    {
                        result.Add($"{compType.Name}/{field.Name} ({GetTypeName(field.FieldType)})");
                    }
                }
            }
            return result;
        }
        IEnumerable<FieldInfo> GetAllFieldsIncludeParent(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            List<FieldInfo> fieldInfos = new List<FieldInfo>();

            while (type != null)
            {
                fieldInfos.AddRange(type.GetFields(flags));
                type = type.BaseType; // 부모 타입으로 이동
            }

            return fieldInfos;
        }


        private void ClearFieldPath()
        {
            fieldPath = "";
        }

        private FieldInfo GetFieldIncludeParent(Type type, string fieldName)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo result = null;

            while (type != null)
            {
                result = type.GetField(fieldName, flags);
                if(result != null)
                {
                    return result;
                }
                type = type.BaseType; // 부모 타입으로 이동
            }

            return result;
        }

        private bool ValidateRxBindTarget()
        {
            if(fieldPath == "" || rxBindTarget == "")
            {
                return true;
            }

            var field = RxDataAccessor.GetRxField(className, FieldPath);

            var target = rxBindTarget.Split("/");
            var targetComp = parentBinder.GetComponent(target[0]);
            var fieldInfo = GetFieldIncludeParent(targetComp.GetType(), target[1].Split(" ")[0]);

            if(fieldInfo == null)
            {
                Debug.LogError("field not found : " + target[1].Split(" ")[0]);
                return false;
            }
            var targetType = fieldInfo.FieldType;

            try
            {
                if (targetType.IsAssignableFrom(field.GetType()))
                {
                    return true;
                }

                System.Convert.ChangeType(field, targetType);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private static string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return GetFriendlyTypeName(type);
            }

            // 제네릭 타입 이름 (RxProperty)
            string typeName = type.GetGenericTypeDefinition().Name;

            // '1 등의 숫자 접미사 제거
            if (typeName.Contains('`'))
            {
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
            }

            // 제네릭 타입 매개변수 이름 (int, string 등)
            Type[] genericArgs = type.GetGenericArguments();
            string genericArgsString = string.Join(", ", genericArgs.Select(GetFriendlyTypeName));

            return $"{typeName}<{genericArgsString}>";
        }

        private static string GetFriendlyTypeName(Type type)
        {
            // 기본 C# 타입 이름으로 변환 (Int32 -> int, String -> string 등)
            return type == typeof(int) ? "int" :
                   type == typeof(float) ? "float" :
                   type == typeof(double) ? "double" :
                   type == typeof(string) ? "string" :
                   type == typeof(bool) ? "bool" :
                   type.Name; // 기본 타입 이름
        }


        private List<string> GetFieldPathSuggestion()
        {
            var classType = Type.GetType(className + ", Assembly-CSharp");
            List<string> suggestions = new();

            GetAllFieldPathRecursively("", classType, suggestions);

            return suggestions;
        }
        private void GetAllFieldPathRecursively(string path, Type classType, List<string> result)
        {
            if(classType == null)
            {
                return;
            }

            FieldInfo[] fields = classType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                if (typeof(IRxField).IsAssignableFrom(fields[i].FieldType))
                {
                    result.Add($"{path + fields[i].Name} ({GetTypeName(fields[i].FieldType)})");
                }
                else if (fields[i].FieldType.IsClass)
                {
                    GetAllFieldPathRecursively(path + fields[i].Name + "/", fields[i].FieldType, result);
                }
            }
        }
    }




    public class RxDataModelBinder : MonoBehaviour
    {
        [OnCollectionChanged("UpdateParentBinders")]
        public List<RxDataModelBinderTarget> binderTargets = new();
        public List<RxDataModelBinder> forwardOnDataChanged = new();

        private void UpdateParentBinders()
        {
            foreach (var target in binderTargets)
            {
                target.parentBinder = this;
            }
        }

        [Button]
        public void BindFields()
        {
            Dispose();

            for (int i = 0; i < binderTargets.Count; i++)
            {
                SubscribeToReactiveProperty(binderTargets[i]);
            }
        }
        public void Dispose()
        {
            disposes.ForEach(x => x.Dispose());
            disposes.Clear();
        }





        private static Dictionary<string, Type> classTypeCache = new();

        bool flushOnSubscribedEvent = false, flushOnDataChangedEvent = false;

        List<IDisposable> disposes = new();
        private void SubscribeToReactiveProperty(RxDataModelBinderTarget binderTarget)
        {
            Type classType;
            if (classTypeCache.ContainsKey(binderTarget.className))
            {
                classType = classTypeCache[binderTarget.className];
            }
            else
            {
                classType = Type.GetType(binderTarget.className + ", Assembly-CSharp");
                classTypeCache[binderTarget.className] = classType;
            }

            var fieldNames = binderTarget.FieldPath.Split(".").ToList();

            if (classType == null)
            {
                Debug.LogError($"Type '{binderTarget.className}' not found");
                return;
            }

            Type currentType = classType;

            FieldInfo fieldInfo = classType.BaseType.GetField("data", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            object currentObject = fieldInfo.GetValue(null);
            currentType = fieldInfo.FieldType;
            object modelObject = currentObject;

            var instanceFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var fieldPart in fieldNames)
            {
                if (currentType == null)
                {
                    Debug.LogError($"Field '{fieldPart}' not found in the type.");
                    return;
                }

                if (fieldPart.Contains("[") && fieldPart.Contains("]")) // 배열 또는 리스트 접근
                {
                    string fieldName = fieldPart.Substring(0, fieldPart.IndexOf("["));

                    while (currentType.BaseType != null && currentType.GetField(fieldName, instanceFlag) == null)
                    {
                        currentType = currentType.BaseType;
                    }

                    fieldInfo = currentType.GetField(fieldName, instanceFlag);

                    if (fieldInfo == null)
                    {
                        Debug.LogError($"Field '{fieldName}' not found in type '{currentType.Name}'.");
                        return;
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
                            return;
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
                            return;
                        }
                    }
                }
                else // 일반 필드 접근
                {
                    while (currentType.BaseType != null && currentType.GetField(fieldPart, instanceFlag) == null)
                    {
                        currentType = currentType.BaseType;
                    }

                    fieldInfo = currentType.GetField(fieldPart, instanceFlag);
                    if (fieldInfo == null)
                    {
                        Debug.LogError($"Field '{fieldPart}' not found in type '{currentType.Name}'.");
                        return;
                    }

                    currentObject = fieldInfo.GetValue(currentObject);
                    currentType = fieldInfo.FieldType;
                }
            }

            // 최종 필드가 IRxField 인지 확인
            if (currentObject != null && IsRxField(currentObject.GetType()))
            {
                binderTarget.bindedRxField = (IRxField)currentObject;

                // Subscribe 호출
                var subscribeMethod = currentObject.GetType().GetMethod("Subscribe");

                if (subscribeMethod != null)
                {
                    // 이벤트 등록 시점에 호출
                    flushOnSubscribedEvent = true;

                    // Subscribe 메서드에 콜백 등록
                    disposes.Add(subscribeMethod.Invoke(currentObject, new object[] { new Action<IRxField>(x => {
                        flushOnDataChangedEvent = true;
                    }) }) as IDisposable);
                }
            }
            else
            {
                Debug.LogError($"The field '{binderTarget.FieldPath}' is not a ReactiveProperty.");
            }
        }

        private void Update()
        {
            if (flushOnSubscribedEvent)
            {
                OnSubscribed();
                flushOnSubscribedEvent = false;
            }

            if (flushOnDataChangedEvent)
            {
                for(int i = 0; i < forwardOnDataChanged.Count; i++)
                {
                    forwardOnDataChanged[i].flushOnDataChangedEvent = true;
                }
            }
        }

        private void LateUpdate()
        {
            if (flushOnSubscribedEvent)
            {
                OnSubscribed();
                flushOnSubscribedEvent = false;
            }

            if (flushOnDataChangedEvent)
            {
                OnDataChanged();
                flushOnDataChangedEvent = false;
            }
        }


        [SerializeField] bool bindFieldsOnEnable = true;

        private void OnEnable()
        {
            if (bindFieldsOnEnable)
            {
                BindFields();
            }
        }

        private void OnDisable()
        {
            Dispose();
        }

        // ReactiveProperty<>인지 확인하는 헬퍼 메서드
        private static bool IsRxField(Type type)
        {
            return typeof(IRxField).IsAssignableFrom(type);
        }

        private FieldInfo GetFieldIncludeParent(Type type, string fieldName)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo result = null;

            while (type != null)
            {
                result = type.GetField(fieldName, flags);
                if (result != null)
                {
                    return result;
                }
                type = type.BaseType; // 부모 타입으로 이동
            }

            return result;
        }

        public virtual void OnDataChanged()
        {
            var comps = GetComponents<IRxBindData>();
            foreach (var comp in comps)
            {
                comp.OnDataChanged();
            }
        }

        public virtual void OnSubscribed()
        {
            var comps = GetComponents<IRxBindData>();
            foreach (var comp in comps)
            {
                for (int i = 0; i < binderTargets.Count; i++)
                {
                    var field = GetFieldIncludeParent(comp.GetType(), binderTargets[i].RxBindTarget);
                    if (field == null)
                    {
                        return;
                    }

                    try
                    {
                        if (field.FieldType.IsAssignableFrom(binderTargets[i].bindedRxField))
                        {
                            field.SetValue(comp, binderTargets[i].bindedRxField);
                        }
                        else
                        {
                            field.SetValue(comp, System.Convert.ChangeType(binderTargets[i].bindedRxField, field.FieldType));
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(gameObject.name + "\n" + e);
                    }
                }

                comp.OnSubscribed();
            }
        }
    }


    public interface IRxBindData
    {
        public void OnSubscribed();
        public void OnDataChanged();
    }



    [AttributeUsage(AttributeTargets.Field)]
    public class RxBindFieldAttribute : System.Attribute
    {

    }
}