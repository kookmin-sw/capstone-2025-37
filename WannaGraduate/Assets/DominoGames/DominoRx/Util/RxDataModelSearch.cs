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

    // 특정 제네릭 부모 클래스를 상속받는 클래스들의 이름을 가져오는 메서드
    public static List<string> GetDerivedClassNames(Type baseType)
    {
        // Assembly-CSharp 어셈블리만 검색
        var assemblyCSharp = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");

        if (assemblyCSharp == null)
        {
            return new List<string> { "(None)" }; // Assembly-CSharp이 없으면 빈 리스트 반환
        }

        return assemblyCSharp.GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                IsSubclassOfGeneric(type, baseType) &&
                !type.IsGenericTypeDefinition) // 제네릭 정의 타입 제외
            .Select(type => type.FullName) // 클래스 전체 이름 반환
            .ToList();
    }

    public static List<Type> GetDerivedClassTypes(Type baseType)
    {
        // Assembly-CSharp 어셈블리만 검색
        var assemblyCSharp = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");

        if (assemblyCSharp == null)
        {
            return new List<Type> { null }; // Assembly-CSharp이 없으면 빈 리스트 반환
        }

        return assemblyCSharp.GetTypes()
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                IsSubclassOfGeneric(type, baseType) &&
                !type.IsGenericTypeDefinition) // 제네릭 정의 타입 제외
            .ToList();
    }

    // 특정 제네릭 타입을 부모로 가지는지 확인하는 메서드
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
