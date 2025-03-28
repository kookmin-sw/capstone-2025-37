using System;
using UnityEngine;

namespace StarphaseTools.Core
{
    public static class JsonUtils
    {
        public static bool SaveObjectToFile<T>(T objectToSave, string filePath, bool prettyPrint = false)
        {
            try
            {
                var jsonString = JsonUtility.ToJson(objectToSave, prettyPrint);
                System.IO.File.WriteAllText(filePath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save object to file: {filePath}. Error: {ex.Message}");
            }

            return false;
        }

        public static T LoadObjectFromFile<T>(string filePath) where T : class
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    Debug.LogError($"File does not exist: {filePath}");
                    return null;
                }

                var jsonString = System.IO.File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(jsonString);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load object from file: {filePath}. Error: {ex.Message}");
                return null;
            }
        }
    }

}