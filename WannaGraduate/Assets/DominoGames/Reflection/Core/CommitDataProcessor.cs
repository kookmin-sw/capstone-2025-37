using System;
using Defective.JSON;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using DominoGames.DominoRx.RxField;

public class CommitDataProcessor
{
    public static void ApplyData(JSONObject data)
    {
        if (!data.HasField("__commitData"))
        {
            return;
        }

        // data.__commitData의 DataModel들을 바탕으로
        // RxDataModel 들을 불러와서 값을 적용합니다
        foreach (string modelName in data["__commitData"].keys)
        {
            UpdateChangeFromJson(GetModelData(modelName), data["__commitData"][modelName]);
        }
    }

    private static object GetModelData(string modelName)
    {
        Type modelType = Type.GetType(modelName + ", Assembly-CSharp");
        return modelType.GetField("data", BindingFlags.Public | BindingFlags.Static).GetValue(null);
    }







    private static void ApplyListChangeFromJson(object valueObject, JSONObject jsonData)
    {
        Type valueType = valueObject.GetType().GetGenericArguments()[0];

        object structureValue;

        for (int j = 0; j < jsonData.count; j++)
        {
            ICollection valueCount = valueObject as ICollection;
            while (valueCount.Count <= System.Convert.ToInt32(jsonData.keys[j].ToString()))
            {
                if (valueType == typeof(string))
                {
                    string temp = "";
                    valueObject.GetType().GetMethod("Add", new[] { valueType }).Invoke(valueObject, new object[] { temp });
                }
                else if (valueType == typeof(ObscuredString))
                {
                    ObscuredString temp = "";
                    valueObject.GetType().GetMethod("Add", new[] { valueType }).Invoke(valueObject, new object[] { temp });
                }
                else
                {
                    valueObject.GetType().GetMethod("Add", new[] { valueType }).Invoke(valueObject, new object[] { Activator.CreateInstance(valueType) });
                }
            }

            System.Reflection.PropertyInfo info = valueObject.GetType().GetProperty("Item");
            if (IsDictionary(valueType))
            {
                structureValue = info.GetValue(valueObject, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
                ApplyDictionaryChangeFromJson(structureValue, jsonData[jsonData.keys[j]]);
            }
            else if (IsList(valueType))
            {
                structureValue = info.GetValue(valueObject, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
                ApplyListChangeFromJson(structureValue, jsonData[jsonData.keys[j]]);
            }
            else if (IsClass(valueType))
            {
                structureValue = info.GetValue(valueObject, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
                ApplyChangeFromJson(structureValue, jsonData[jsonData.keys[j]]);
            }
            else
            {
                structureValue = Convert.ChangeType(jsonData[jsonData.keys[j]].ToString(), valueType);
            }

            info.SetValue(valueObject, structureValue, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
        }
    }

    private static void ApplyDictionaryChangeFromJson(object valueObject, JSONObject jsonData)
    {
        Type keyType = valueObject.GetType().GetGenericArguments()[0];
        Type valueType = valueObject.GetType().GetGenericArguments()[1];

        object structureValue;
        IDictionary castedObject = valueObject as IDictionary;
        object castedKeyValue;

        for (int j = 0; j < jsonData.count; j++)
        {
            castedKeyValue = Convert.ChangeType(jsonData.keys[j].ToString(), keyType);

            if (IsDictionary(valueType))
            {
                structureValue = castedObject[castedKeyValue];
                //structureValue = valueObject.GetType().GetField(jsonData.keys[j]);
                if (structureValue == null)
                {
                    structureValue = Activator.CreateInstance(valueType);
                }
                ApplyDictionaryChangeFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
            }
            else if (IsList(valueType))
            {
                structureValue = castedObject[castedKeyValue];
                if (structureValue == null)
                {
                    structureValue = Activator.CreateInstance(valueType);
                }
                ApplyListChangeFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
            }
            else if (IsClass(valueType))
            {
                structureValue = castedObject[castedKeyValue];
                if (structureValue == null)
                {
                    structureValue = Activator.CreateInstance(valueType);
                }
                ApplyChangeFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
            }
            else
            {
                structureValue = Convert.ChangeType(jsonData.GetField(jsonData.keys[j]).ToString(), valueType);
            }

            if ((bool)valueObject.GetType().GetMethod("ContainsKey").Invoke(valueObject, new object[] { castedKeyValue }))
            {
                // 키가 존재하면 수정
                IDictionary castingDictionary = valueObject as IDictionary;
                castingDictionary[castedKeyValue] = structureValue;
                //valueObject.GetType().GetField(castedKeyValue).SetValue(valueObject, structureValue);
            }
            else
            {
                // 존재하지 않으면 추가
                valueObject.GetType().GetMethod("Add", new[] { keyType, valueType }).Invoke(valueObject,
                new object[] { castedKeyValue, structureValue });
            }
        }
    }

    public static void UpdateChangeFromJson(object classObject, JSONObject jsonData)
    {
        // 이중 reflection update 콜 되면 꼬임...
        //NetworkingLoadingManager.instance.ShowLoadingScreen();

        if (jsonData["deleted"].ToString() != "{}")
        {
            ApplyChangeFromJson(classObject, jsonData["changed"]);
            DeleteClassFromJson(classObject, jsonData["deleted"]);
        }
        else
        {
            ApplyChangeFromJson(classObject, jsonData["changed"]);
        }

        //NetworkingLoadingManager.instance.HideLoadingScreen();
    }

    private static void DeleteListFromJson(object valueObject, JSONObject jsonData)
    {
        Type valueType = valueObject.GetType().GetGenericArguments()[0];

        object structureValue;

        if (jsonData.count <= 0)
        {
            return;
        }

        List<JSONObject> temp = new List<JSONObject>();
        List<int> indexes = new List<int>();


        for (int i = 0; i < jsonData.count; i++)
        {
            jsonData[indexes[i].ToString()] = temp[i];
        }

        for (int j = 0; j < jsonData.count; j++)
        {
            if (jsonData[j].isObject || jsonData[j].isArray)
            {
                if (IsDictionary(valueType))
                {
                    System.Reflection.PropertyInfo info = valueObject.GetType().GetProperty("Item");
                    structureValue = info.GetValue(valueObject, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
                    DeleteDictionaryFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
                }
                else if (IsList(valueType))
                {
                    System.Reflection.PropertyInfo info = valueObject.GetType().GetProperty("Item");
                    structureValue = info.GetValue(valueObject, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
                    DeleteListFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
                }
                else if (IsClass(valueType))
                {
                    System.Reflection.PropertyInfo info = valueObject.GetType().GetProperty("Item");
                    structureValue = info.GetValue(valueObject, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
                    DeleteClassFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
                }
            }
            else
            {
                // true면 해당 인덱스 객체를 삭제
                int length = (int)valueObject.GetType().GetProperty("Count").GetValue(valueObject);
                if (System.Convert.ToInt32(jsonData.keys[j]) >= length)
                {
                    return;
                }
                valueObject.GetType().GetMethod("RemoveAt").Invoke(valueObject, new object[] { System.Convert.ToInt32(jsonData.keys[j].ToString()) });
            }
        }
    }

    private static void DeleteDictionaryFromJson(object valueObject, JSONObject jsonData)
    {
        Type keyType = valueObject.GetType().GetGenericArguments()[0];
        Type valueType = valueObject.GetType().GetGenericArguments()[1];

        object structureValue;

        for (int j = 0; j < jsonData.count; j++)
        {
            if (jsonData[jsonData.keys[j]].isObject || jsonData[jsonData.keys[j]].isArray)
            {
                if (IsDictionary(valueType))
                {
                    structureValue = valueObject.GetType().GetField(jsonData.keys[j]);
                    DeleteDictionaryFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
                }
                else if (IsList(valueType))
                {
                    structureValue = valueObject.GetType().GetField(jsonData.keys[j]);
                    DeleteListFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
                }
                else if (IsClass(valueType))
                {
                    structureValue = valueObject.GetType().GetField(jsonData.keys[j]);
                    DeleteClassFromJson(structureValue, jsonData.GetField(jsonData.keys[j]));
                }
            }
            else
            {
                // true 가 들어있으면 해당 dictionary를 삭제
                //if((bool)valueObject.GetType().GetMethod("ContainsKey", new[]{keyType}).Invoke(valueObject, new object[] {jsonData.keys[j]})){
                valueObject.GetType().GetMethod("Remove", new[] { keyType }).Invoke(valueObject, new object[] { jsonData.keys[j] });
                //}
            }
        }
    }

    private static void DeleteClassFromJson(object classObject, JSONObject jsonData)
    {
        string tempName;
        JSONObject tempValue;
        System.Reflection.FieldInfo tempInfo;

        for (int i = 0; i < jsonData.count; i++)
        {
            tempName = jsonData.keys[i];
            tempValue = jsonData.GetField(tempName);
            tempInfo = classObject.GetType().GetField(tempName);

            try
            {
                if (tempInfo != null)
                {
                    // 똑같은 이름의 property가 존재하고, 그 값이 true 라면 해당 필드를 삭제하자
                    if (tempValue.isObject || tempValue.isArray)
                    {
                        object valueObject = tempInfo.GetValue(classObject);

                        if (valueObject == null)
                        {
                            // 없다면 스톱
                            return;
                        }

                        if (IsDictionary(valueObject))
                        {
                            DeleteDictionaryFromJson(valueObject, tempValue);
                        }
                        else if (IsList(valueObject))
                        {
                            DeleteListFromJson(valueObject, tempValue);
                        }
                        else
                        {
                            // class type
                            DeleteClassFromJson(valueObject, tempValue); // 2차 오브젝트까지 다 ui update 콜하면 곤란함
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("ERROR : " + tempName + "\n" + e.ToString());
            }
        }
    }

    private static void ApplyChangeFromJson(object classObject, JSONObject jsonData)
    {
        string tempName;
        JSONObject tempValue;
        System.Reflection.FieldInfo tempInfo;

        for (int i = 0; i < jsonData.count; i++)
        {
            tempName = jsonData.keys[i];
            tempValue = jsonData.GetField(tempName);
            tempInfo = classObject.GetType().GetField(tempName);

            try
            {
                if (tempInfo != null)
                {
                    // 똑같은 이름의 property가 존재하면 거기에 값을 넣어주자.
                    if (tempValue.isObject || tempValue.isArray)
                    {
                        // class or dictioanry object value
                        // 기존에 valueObject가 있다면 그 값을 사용하고.
                        object valueObject = tempInfo.GetValue(classObject);
                        if (valueObject == null)
                        {
                            // 없다면 새로 만들어서 붙힘
                            valueObject = Activator.CreateInstance(tempInfo.FieldType);
                        }

                        if (IsDictionary(valueObject))
                        {
                            ApplyDictionaryChangeFromJson(valueObject, tempValue);

                            tempInfo.SetValue(classObject, valueObject);
                        }
                        else if (IsList(valueObject))
                        {
                            ApplyListChangeFromJson(valueObject, tempValue);

                            tempInfo.SetValue(classObject, valueObject);
                        }
                        else
                        {
                            // class type
                            ApplyChangeFromJson(valueObject, tempValue); // 2차 오브젝트까지 다 ui update 콜하면 곤란함
                            tempInfo.SetValue(classObject, valueObject);
                        }
                    }
                    else
                    {
                        if(tempInfo.FieldType == typeof(RxProperty<>))
                        {
                            Type genericType = tempInfo.FieldType.GenericTypeArguments[0];

                            tempInfo.GetType().GetMethod("SetValue", BindingFlags.Instance | BindingFlags.Public)
                                .Invoke(classObject, new object[] { Convert.ChangeType(tempValue, genericType) });
                        }
                        else
                        {
                            tempInfo.SetValue(classObject, System.Convert.ChangeType(tempValue, tempInfo.FieldType));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("ERROR : " + tempName + "\n" + e.ToString());
                UnityEngine.Debug.LogError("value : " + tempValue.ToString());
            }
        }

        //NetworkingLoadingManager.instance.HideLoadingScreen();
    }

    private static Type[] basicTypes = { typeof(int), typeof(double), typeof(float), typeof(short), typeof(string), typeof(bool), typeof(ObscuredInt), typeof(ObscuredDouble), typeof(ObscuredFloat), typeof(ObscuredShort), typeof(ObscuredString), typeof(ObscuredBool) };
    private static bool IsClass(Type type)
    {
        for (int i = 0; i < basicTypes.Length; i++)
        {
            if (type == basicTypes[i])
            {
                return false;
            }
        }

        return true;
    }
    private static bool IsDictionary(object obj)
    {
        return obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }
    private static bool IsDictionary(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }
    private static bool IsList(object obj)
    {
        return obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(List<>);
    }
    private static bool IsList(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
}