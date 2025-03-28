using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DominoGames.DominoRx.DataModel
{
    public class RxDataModelBinderInitializer_Deprecated : MonoBehaviour
    {
        /*[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitBinders()
        {
            // 씬 내의 모든 RxDataModelBinder 검색
            RxDataModelBinder[] binders = FindObjectsOfType<RxDataModelBinder>();

            // 각 DataModelBinder의 SubscribeToReactiveProperty 호출
            foreach (var binder in binders)
            {
                binder.SubscribeToReactiveProperty();
            }

            Debug.Log($"Subscribed to {binders.Length} DataModelBinder(s).");
        }*/
    }
}