using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UI_Base : SerializedMonoBehaviour
{
    [Button]
    private void AttachAllUIElements()
    {
        Dictionary<string, FieldInfo> targetFields = GetUIAutoAttachFields();
        ClearAllList(targetFields);
        FindAndSetChildComponentsInChildren(targetFields);
    }

    private Dictionary<string, FieldInfo> GetUIAutoAttachFields()
    {
        var t = GetType();
        // 자기 클래스 내에 있는 변수 다 불러오기
        FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        // UIAutoAttachField 라는 Attribute가 붙어있는 변수만 선별
        return fields.Where(x => x.GetCustomAttribute<UIAutoAttachField>() != null).ToDictionary(field => field.Name.ToLower(), field => field);
    }

    private void ClearAllList(Dictionary<string, FieldInfo> fields)
    {
        foreach(var field in fields.Values)
        {
            if (IsList(field))
            {
                (field.GetValue(this) as IList).Clear();
            }
        }
    }
    
    private bool IsList(FieldInfo field)
    {
        return field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>);
    }

    private void FindAndSetChildComponentsInChildren(Dictionary<string, FieldInfo> fields)
    {
        List<FieldInfo> notFoundFields = fields.Values.ToList();

        // Queue 로 BFS children search
        Queue<Transform> searchQueue = new();
        searchQueue.Enqueue(transform);

        while(searchQueue.Count > 0)
        {
            var target = searchQueue.Dequeue();
            var key = string.Join("", target.name.Split(" ")).ToLower();
            if (fields.ContainsKey(key))
            {
                var field = fields[key];

                if(IsList(field))
                {
                    // 변수가 List라면
                    var list = field.GetValue(this) as IList;
                    list.Add(GetObjectByType(field.FieldType.GenericTypeArguments[0], target));
                }
                else
                {
                    field.SetValue(this, GetObjectByType(field.FieldType, target));
                }
                
                notFoundFields.Remove(fields[key]);
            }

            for(int i = 0; i < target.childCount; i++)
            {
                searchQueue.Enqueue(target.GetChild(i));
            }
        }

        for(int i = 0; i < notFoundFields.Count; i++)
        {
            Debug.LogError(notFoundFields[i].FieldType + " " + notFoundFields[i].Name + " is not found!");
        }
    }

    private object GetObjectByType(Type type, Transform target)
    {
        if(type == typeof(GameObject))
        {
            return target.gameObject;
        }
        else
        {
            return target.GetComponent(type);
        }
    }
}


public class UIAutoAttachField : PropertyAttribute
{

}