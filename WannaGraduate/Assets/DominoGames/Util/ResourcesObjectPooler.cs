using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DominoGames.Util
{
    public class ResourcesObjectPooler : MonoBehaviour {
        const string BASE_RESOURCE_PATH = "Prefabs/";

        private static Dictionary<string, GameObject> prefabs = new();
        private static Dictionary<string, Queue<GameObject>> pooledObjects = new();
        private static GameObject rootPooledObject = null;
        private static ResourcesObjectPooler Instance;

        /// <summary>
        /// 오브젝트 풀링 된 인스턴스를 반환합니다
        /// </summary>
        /// <param name="path">Prefabs/ 이하의 경로를 쓰세요</param>
        /// <returns></returns>
        public static GameObject Instantiate(string path)
        {
            GameObject obj = GetPooledObjectOrInstantiate(path);
            obj.SetActive(true);

            var comps = obj.GetComponents<IPooledObjectEvent>();

            foreach (var item in comps)
            {
                item.OnPooledObjectInit();
            }

            obj.transform.SetParent(null);

            return obj;
        }

        public static void Destroy(GameObject obj, int addPoolAfterFrame = 0)
        {
            var targetProperty = obj.GetComponent<PooledObjectProperties>();
            if (targetProperty == null || !prefabs.ContainsKey(targetProperty.pooledPath))
            {
                // 관리되는 오브젝트가 아님
                GameObject.Destroy(obj);
                return;
            }

            var comps = obj.GetComponents<IPooledObjectEvent>();

            foreach (var item in comps)
            {
                item.OnPooledObjectDestroy();
            }

            obj.SetActive(false);

            obj.transform.SetParent(GetRootPooledObject().transform);

            if (addPoolAfterFrame == 0)
            {
                AddPooledObject(targetProperty.pooledPath, obj);
            }
            else
            {
                Instance.StartCoroutine(AddPoolAfterFrame(targetProperty.pooledPath, obj, addPoolAfterFrame));
            }
        }
        private static IEnumerator AddPoolAfterFrame(string pooledPath, GameObject obj, int frame)
        {
            for(int i = 0; i < frame; i++)
            {
                yield return null;
            }
            AddPooledObject(pooledPath, obj);
        }

        public static void FreePool(string path)
        {
            if (!pooledObjects.ContainsKey(path))
            {
                return;
            }

            while (pooledObjects[path].Count > 0)
            {
                GameObject.Destroy(pooledObjects[path].Dequeue());
            }
        }


        [Command("System_FreePoolAll")]
        public static void FreePoolAll()
        {
            foreach(string path in pooledObjects.Keys)
            {
                while (pooledObjects[path].Count > 0)
                {
                    GameObject.Destroy(pooledObjects[path].Dequeue());
                }
            }

            pooledObjects.Clear();
        }



        
        private static GameObject GetPooledObjectOrInstantiate(string path)
        {
            if (!pooledObjects.ContainsKey(path))
            {
                pooledObjects[path] = new();
            }

            if (pooledObjects[path].Count <= 0)
            {
                var instance = GameObject.Instantiate(GetPrefab(path));
                if(instance.GetComponent<PooledObjectProperties>() == null)
                {
                    instance.AddComponent<PooledObjectProperties>().pooledPath = path;
                }
                return instance;
            }

            return pooledObjects[path].Dequeue();
        }
        private static GameObject GetPrefab(string path)
        {
            if (!prefabs.ContainsKey(path))
            {
                prefabs[path] = Resources.Load<GameObject>(BASE_RESOURCE_PATH + path);
            }

            return prefabs[path];
        }
        private static GameObject GetRootPooledObject()
        {
            if(rootPooledObject == null)
            {
                rootPooledObject = new GameObject("ResourceObjectPooler_Root");
                Instance = rootPooledObject.AddComponent<ResourcesObjectPooler>();
            }

            return rootPooledObject;
        }
        private static void AddPooledObject(string path, GameObject obj)
        {
            if (!pooledObjects.ContainsKey(path))
            {
                pooledObjects[path] = new();
            }

            pooledObjects[path].Enqueue(obj);
        }
    }


    public interface IPooledObjectEvent
    {
        public void OnPooledObjectInit();
        public void OnPooledObjectDestroy();
    }

    public class PooledObjectProperties : MonoBehaviour {
        public string pooledPath;
    }
}