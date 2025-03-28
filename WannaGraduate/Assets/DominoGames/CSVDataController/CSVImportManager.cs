using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;


#if UNITY_EDITOR
#if UNITY_5_3_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif
using UnityEditor;
#endif
#if UNITY_EDITOR
namespace BBB.CSVData{
    public class CSVImportManager : Editor{
        public static IEnumerator DownloadAndImport<T>(string url, string assetfile) where T : ScriptableObject, new()
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            while (www.isDone == false)
            {
                yield return new WaitForEndOfFrame();
            }

            if (www.error != null)
            {
                Debug.Log("UnityWebRequest.error:" + www.error);
            }
            else if (www.downloadHandler.text == "" || www.downloadHandler.text.IndexOf("<!DOCTYPE") != -1)
            {
                Debug.Log("Uknown Format:" + www.downloadHandler.text);
            }
            else
            {
                ImportData<T>(www.downloadHandler.text, assetfile);
    #if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Imported Asset: " + assetfile);
    #endif
            }
        }

        static void ImportData<T>(string text, string assetfile) where T : ScriptableObject, new ()
        {
            List<string[]> rows = CSVSerializer.ParseCSV(text);
            if (rows != null)
            {
                T gm = AssetDatabase.LoadAssetAtPath<T>(assetfile);
                if (gm == null)
                {
                    gm = ScriptableObject.CreateInstance(typeof(T)) as T;
                    AssetDatabase.CreateAsset(gm, assetfile);
                }
                Type _tempItemType = gm.GetType().GetField("m_Items").FieldType.GetElementType();
                MethodInfo _tempDeserializeMethod = typeof(CSVSerializer).GetMethod("Deserialize", new Type[] {typeof(List<string[]>)});
                _tempDeserializeMethod = _tempDeserializeMethod.MakeGenericMethod(_tempItemType);

                gm.GetType().GetField("m_Items").SetValue(gm, _tempDeserializeMethod.Invoke(null, new object[]{rows}));

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
            }
        }

        // coroutine for unity editor
        public static void StartCorountine(IEnumerator routine)
        {
            _coroutine.Add(routine);
            if (_coroutine.Count == 1)
                EditorApplication.update += ExecuteCoroutine;
        }
        static List<IEnumerator> _coroutine = new List<IEnumerator>();
        static void ExecuteCoroutine()
        {
            for (int i = 0; i < _coroutine.Count;)
            {
                if (_coroutine[i] == null || !_coroutine[i].MoveNext())
                    _coroutine.RemoveAt(i);
                else
                    i++;
            }
            if (_coroutine.Count == 0)
                EditorApplication.update -= ExecuteCoroutine;
        }
    }
}
#endif