using UnityEngine;
using UnityEditor;

namespace BBB.CSVData
{
#if UNITY_EDITOR
    public class CSVImport_ExampleData : Editor, ICSVImportable
    {
        [MenuItem("Domino/CSV Serializer/ExampleData")]
        public static void Init()
        {
            string url = "SPREAD_SHEET_CSV_URL";
            string assetfile = "Assets/Resources/CSVData/ExampleData.asset";

            CSVImportManager.StartCorountine(CSVImportManager.DownloadAndImport<CSVDataContainer_ExampleData>(url, assetfile));
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
            data = Resources.Load<CSVDataContainer_ExampleData>("CSVData/ExampleData");
        }
    }

    [System.Serializable]
    public class CSVDataRow_ExampleData
    {
        public int Id;
        public string Content;
    }
}
