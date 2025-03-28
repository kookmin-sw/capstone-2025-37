using BayatGames.SaveGameFree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace DominoGames.DominoRx.DataModel
{
    public class RxDataModel<T> where T : class, IRxDataModelInitializable, new()
    {
        public static T data = CreateInstance();

        private static T CreateInstance()
        {
            T instance = new();

            if (instance.AutoLoad)
            {
                Load(ref instance);
            }
            
            return instance;
        }

        public static void Save()
        {
            Save(data);
        }
        public static void Save(T dataContainer)
        {
            SaveGame.Save<T>("data_" + dataContainer.GetType().FullName, dataContainer);
        }
        public static void Load()
        {
            Load(ref data);
        }
        public static void Load(ref T dataContainer)
        {
            dataContainer = SaveGame.Load<T>("data_" + dataContainer.GetType().FullName, dataContainer);
            dataContainer.InitializeData();
        }
    }

    public interface IRxDataModelInitializable {
        public void InitializeData();

        public bool AutoLoad => true;
    }
}