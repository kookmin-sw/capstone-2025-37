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
            // �� ���� ��� RxDataModelBinder �˻�
            RxDataModelBinder[] binders = FindObjectsOfType<RxDataModelBinder>();

            // �� DataModelBinder�� SubscribeToReactiveProperty ȣ��
            foreach (var binder in binders)
            {
                binder.SubscribeToReactiveProperty();
            }

            Debug.Log($"Subscribed to {binders.Length} DataModelBinder(s).");
        }*/
    }
}