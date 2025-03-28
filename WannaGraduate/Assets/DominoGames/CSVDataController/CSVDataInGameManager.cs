using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using BBB.CSVData;
using System.Reflection;
using Unity.VisualScripting;

namespace BBB.InGame{
    public class CSVDataInGameManager : MonoBehaviour
    {
        #region Informations
        /*
            CSVData들을 로드하고, 인게임에 맞게 가공하는 매니저입니다.
        */
        #endregion

#if UNITY_EDITOR
        [MenuItem("Domino/CSV Serializer/Import All CSV Data")]
        public static void ImportAll()
        {
            // 현재 어셈블리에서 모든 타입 가져오기
            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in types)
            {
                // IInitializable 인터페이스를 구현하는 클래스인지 확인
                if (typeof(ICSVImportable).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    // Init 메서드 확인
                    var method = type.GetMethod("Init", BindingFlags.Static | BindingFlags.Public);

                    if (method != null)
                    {
                        // Init 메서드 호출
                        method.Invoke(null, null);
                        Debug.Log($"{type.Name}.Init() executed.");
                    }
                    else
                    {
                        Debug.LogWarning($"{type.Name} does not have a static Init() method.");
                    }
                }
            }
        }
#endif
    }
}

public interface ICSVImportable
{

}

public enum ECalcType {
    Add,
    Multiply
}
