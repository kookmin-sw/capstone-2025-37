using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DominoGames.DominoRx.DataModel;

public class RxDataModelSearch : MonoBehaviour
{
    public static List<string> GetModelNames()
    {
        return JsonUtility.FromJson<CacheModelNames>(System.IO.File.ReadAllText(Application.dataPath + "/Resources/Data/RxData/ModelNames.json")).modelNames;
    }

    private class CacheModelNames
    {
        public List<string> modelNames;
    }

    // Ư�� ���׸� �θ� Ŭ������ ��ӹ޴� Ŭ�������� �̸��� �������� �޼���
    public static List<string> GetDerivedClassNames(Type baseType)
    {
        // Assembly-CSharp ������� �˻�
        var assemblyCSharp = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");

        if (assemblyCSharp == null)
        {
            return new List<string> { "(None)" }; // Assembly-CSharp�� ������ �� ����Ʈ ��ȯ
        }

        return assemblyCSharp.GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                IsSubclassOfGeneric(type, baseType) &&
                !type.IsGenericTypeDefinition) // ���׸� ���� Ÿ�� ����
            .Select(type => type.FullName) // Ŭ���� ��ü �̸� ��ȯ
            .ToList();
    }

    public static List<Type> GetDerivedClassTypes(Type baseType)
    {
        // Assembly-CSharp ������� �˻�
        var assemblyCSharp = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");

        if (assemblyCSharp == null)
        {
            return new List<Type> { null }; // Assembly-CSharp�� ������ �� ����Ʈ ��ȯ
        }

        return assemblyCSharp.GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                IsSubclassOfGeneric(type, baseType) &&
                !type.IsGenericTypeDefinition) // ���׸� ���� Ÿ�� ����
            .ToList();
    }

    // Ư�� ���׸� Ÿ���� �θ�� �������� Ȯ���ϴ� �޼���
    private static bool IsSubclassOfGeneric(Type type, Type genericBaseType)
    {
        while (type != null && type != typeof(object))
        {
            var currentType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (currentType == genericBaseType)
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }
}
