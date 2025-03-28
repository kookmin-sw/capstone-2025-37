using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.RxField;

namespace DominoGames.DominoRx.UI
{
    [RequireComponent(typeof(RxDataModelBinder))]
    public class RxImage : Image, IRxBindData
    {
        [SerializeField] private string basePath = "Sprites/";
        [RxBindField] IRxField targetValue;

        public void OnDataChanged()
        {
            sprite = Resources.Load<Sprite>(basePath + targetValue.GetValue().ToString());
        }

        public void OnSubscribed()
        {

        }
    }
}