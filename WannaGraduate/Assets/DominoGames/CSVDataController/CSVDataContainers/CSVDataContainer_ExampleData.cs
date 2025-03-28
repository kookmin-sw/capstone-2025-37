using RNGNeeds;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BBB.CSVData
{
#if UNITY_EDITOR
    public class CSVImport_WeaponData : Editor, ICSVImportable
    {
        [MenuItem("Domino/CSV Serializer/ExampleData")]
        public static void Init()
        {
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSWqIC1sdz4qEC_p4yCWn_MVHZeQZ5fGt7T_q95VWRhzvUWJ2mPtYAptr5PEZd3DjbWVIgWgHJblmeN/pub?gid=640039239&single=true&output=csv";
            string assetfile = "Assets/Resources/CSVData/WeaponData.asset";

            //CSVImportManager.StartCorountine(CSVImportManager.DownloadAndImport<CSVDataContainer_ExampleData>(url, assetfile));
        }
    }
#endif

    public class CSVDataContainer_ExampleData : ScriptableObject
    {
        public CSVDataRow_ExampleData[] m_Items;

        public static CSVDataContainer_ExampleData data;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void InitData()
        {
            data = Resources.Load<CSVDataContainer_ExampleData>("CSVData/WeaponData");
        }
    }

    [System.Serializable]
    public class CSVDataRow_ExampleData
    {
        public int exampleValue;
    }
}
