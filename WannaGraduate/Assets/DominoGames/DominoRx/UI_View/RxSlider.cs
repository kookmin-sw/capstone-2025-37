using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.RxField;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RxDataModelBinder))]
[RequireComponent(typeof(Slider))]
public class RxSlider : MonoBehaviour, IRxBindData
{
    [RxBindField] IRxField valueField;
    [RxBindField] IRxField maxField;

    public void OnDataChanged()
    {
        GetComponent<Slider>().value = float.Parse(valueField.GetValue().ToString()) / float.Parse(maxField.GetValue().ToString());
    }

    public void OnSubscribed()
    {

    }
}
