using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using DominoGames.Core.DataStructure;
using BBB.DataStructure;
using System;

namespace DominoGames.Core.EventSystem
{
    public class DominoEventSystem : SerializedMonoBehaviour
    {
        public static DoubleDictionary<EEventTypes, object> eventMap = new();

        public static void Pub<T>(EEventTypes eventName, T args = default)
        {
            if (!eventMap.ContainsKey(eventName))
            {
                return;
            }

            foreach (var action in eventMap.GetValues(eventName))
            {
                ((System.Action<T>)action)?.Invoke(args);
            }
        }
        public static void Pub<T>(string eventName, T args = default)
        {
            Enum.TryParse(eventName, out EEventTypes eventType);
            Pub<T>(eventType, args);
        }

        public static void Pub(EEventTypes eventName)
        {
            if (!eventMap.ContainsKey(eventName))
            {
                return;
            }

            foreach (var action in eventMap.GetValues(eventName))
            {
                ((System.Action)action)?.Invoke();
            }
        }



        public static int Sub<T>(string eventName, System.Action<T> action)
        {
            Enum.TryParse(eventName, out EEventTypes eventType);
            return Sub(eventType, action);
        }
        public static int Sub<T>(EEventTypes eventName, System.Action<T> action)
        {
            int idx = eventMap.AddItem(eventName, action);
            return idx;
        }
        public static int Sub(EEventTypes eventName, System.Action action)
        {
            int idx = eventMap.AddItem(eventName, action);
            return idx;
        }
        public static void UnSub(EEventTypes eventName, int idx)
        {
            eventMap.RemoveItem(eventName, idx);
        }
    }
}
